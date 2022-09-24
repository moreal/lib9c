using System;
using System.Collections.Generic;
using System.Linq;
using Nekoyume.Model.Elemental;
using Nekoyume.TableData;

#nullable disable
namespace Nekoyume.Model.Skill
{
    [Serializable]
    public abstract class AttackSkill : Skill
    {
        protected AttackSkill(SkillSheet.Row skillRow, int power, int chance) : base(skillRow, power, chance)
        {
        }

        /// <summary>
        /// todo: 캐릭터 스탯에 반영된 버프 효과가 스킬의 순수 데미지에는 영향을 주지 않는 로직.
        /// todo: 타겟의 회피 여부와 상관없이 버프의 확률로 발동되고 있음. 고민이 필요함.
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="simulatorWaveTurn"></param>
        /// <param name="isNormalAttack"></param>
        /// <returns></returns>
        protected IEnumerable<BattleStatus.Skill.SkillInfo> ProcessDamage(CharacterBase caster, int simulatorWaveTurn,
            bool isNormalAttack = false)
        {
            var infos = new List<BattleStatus.Skill.SkillInfo>();
            var targets = SkillRow.SkillTargetType.GetTarget(caster).ToList();
            var elementalType = SkillRow.ElementalType;
            var totalDamage = caster.ATK + Power;
            var multipliers = GetMultiplier(SkillRow.HitCount, 1m);
            for (var i = 0; i < SkillRow.HitCount; i++)
            {
                var multiplier = multipliers[i];

                foreach (var target in targets)
                {
                    var damage = 0;
                    var isCritical = false;
                    // 일반 공격이 아니거나 일반 공격인데 명중한 경우.
                    if (!isNormalAttack ||
                        target.IsHit(caster))
                    {
                        // 방깎 적용.
                        damage = totalDamage - target.DEF;
                        // 멀티 히트 적용.
                        damage = (int) (damage * multiplier);
                        if (damage < 1)
                        {
                            damage = 1;
                        }
                        else
                        {
                            // 모션 배율 적용.
                            damage = caster.GetDamage(damage, isNormalAttack);
                            // 속성 적용.
                            damage = elementalType.GetDamage(target.defElementType, damage);
                            // 치명 적용.
                            isCritical = caster.IsCritical(isNormalAttack);
                            if (isCritical)
                            {
                                damage = (int) (damage * CharacterBase.CriticalMultiplier);
                            }

                            // 연타공격은 항상 연출이 크리티컬로 보이도록 처리.
                            isCritical |= SkillRow.SkillCategory == SkillCategory.DoubleAttack;
                        }

                        target.CurrentHP -= damage;
                    }

                    infos.Add(new BattleStatus.Skill.SkillInfo((CharacterBase) target.Clone(), damage, isCritical,
                        SkillRow.SkillCategory, simulatorWaveTurn, SkillRow.ElementalType,
                        SkillRow.SkillTargetType));
                }
            }

            return infos;
        }

        private static decimal[] GetMultiplier(int hitCount, decimal totalDamage)
        {
            if (hitCount == 1) return new[] {totalDamage};
            var multiplier = new List<decimal>();
            var avg = totalDamage / hitCount;
            var lastDamage = avg * 1.3m;
            var lastHitIndex = hitCount - 1;
            var eachDamage = (totalDamage - lastDamage) / lastHitIndex;
            for (var i = 0; i < hitCount; i++)
            {
                var result = i == lastHitIndex ? lastDamage : eachDamage;
                multiplier.Add(result);
            }

            return multiplier.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using Nekoyume.Model.Elemental;
using Nekoyume.TableData;

#nullable disable
namespace Nekoyume.Model.Skill.Arena
{
    [Serializable]
    public abstract class ArenaAttackSkill : ArenaSkill
    {
        protected ArenaAttackSkill(SkillSheet.Row skillRow, int power, int chance)
            : base(skillRow, power, chance)
        {
        }

         protected IEnumerable<BattleStatus.Arena.ArenaSkill.ArenaSkillInfo> ProcessDamage(
            ArenaCharacter caster,
            ArenaCharacter target,
            int simulatorWaveTurn,
            bool isNormalAttack = false)
        {
            var infos = new List<BattleStatus.Arena.ArenaSkill.ArenaSkillInfo>();

            var multipliers = GetMultiplier(SkillRow.HitCount, 1m);
            var elementalType = isNormalAttack ? caster.OffensiveElementalType : SkillRow.ElementalType;
            for (var i = 0; i < SkillRow.HitCount; i++)
            {
                var multiplier = multipliers[i];
                var damage = 0;
                var isCritical = false;

                if (target.IsHit(caster))
                {
                    damage = caster.ATK + Power;
                    damage = (int) (damage * multiplier);
                    damage = caster.GetDamage(damage, isNormalAttack);
                    damage = elementalType.GetDamage(target.DefenseElementalType, damage);
                    isCritical = caster.IsCritical(isNormalAttack);
                    if (isCritical)
                    {
                        damage = (int) (damage * ArenaCharacter.CriticalMultiplier);
                    }

                    damage = Math.Max(damage - target.DEF, 1);
                    target.CurrentHP -= damage;

                    // double attack must be showed critical attack
                    isCritical |= SkillRow.SkillCategory == SkillCategory.DoubleAttack;
                }

                infos.Add(new BattleStatus.Arena.ArenaSkill.ArenaSkillInfo(
                    (ArenaCharacter)target.Clone(),
                    damage,
                    isCritical,
                    SkillRow.SkillCategory,
                    simulatorWaveTurn,
                    elementalType,
                    SkillRow.SkillTargetType));
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

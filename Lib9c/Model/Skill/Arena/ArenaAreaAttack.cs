using System;
using System.Collections.Generic;
using Nekoyume.TableData;

#nullable disable
namespace Nekoyume.Model.Skill.Arena
{
    [Serializable]
    public class ArenaAreaAttack : ArenaAttackSkill
    {
        public ArenaAreaAttack(SkillSheet.Row skillRow, int power, int chance)
            : base(skillRow, power, chance)
        {
        }

        public override BattleStatus.Arena.ArenaSkill Use(
            ArenaCharacter caster,
            ArenaCharacter target,
            int turn,
            IEnumerable<Buff.Buff> buffs)
        {
            var clone = (ArenaCharacter)caster.Clone();
            var damage = ProcessDamage(caster, target, turn);
            var buff = ProcessBuff(caster, target, turn, buffs);

            return new BattleStatus.Arena.ArenaAreaAttack(clone, damage, buff);
        }

        [Obsolete("Use Use")]
        public override BattleStatus.Arena.ArenaSkill UseV1(
            ArenaCharacter caster,
            ArenaCharacter target,
            int turn,
            IEnumerable<Buff.Buff> buffs)
        {
            var clone = (ArenaCharacter)caster.Clone();
            var damage = ProcessDamage(caster, target, turn);
            var buff = ProcessBuffV1(caster, target, turn, buffs);

            return new BattleStatus.Arena.ArenaAreaAttack(clone, damage, buff);
        }
    }
}

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Nekoyume.Model.Elemental;
using Nekoyume.Model.Skill;

#nullable disable
namespace Nekoyume.Model.BattleStatus.Arena
{
    [Serializable]
    public abstract class ArenaSkill : ArenaEventBase
    {
        [Serializable]
        public class ArenaSkillInfo
        {
            public readonly ArenaCharacter Target;
            public readonly int Effect;
            public readonly bool Critical;
            public readonly SkillCategory SkillCategory;
            public readonly ElementalType ElementalType;
            public readonly SkillTargetType SkillTargetType;
            public readonly int Turn;

            [CanBeNull]
            public readonly Model.Buff.Buff Buff;

            public ArenaSkillInfo(ArenaCharacter character, int effect, bool critical, SkillCategory skillCategory,
                int turn, ElementalType elementalType = ElementalType.Normal,
                SkillTargetType targetType = SkillTargetType.Enemy, [CanBeNull] Model.Buff.Buff buff = null)
            {
                Target = character;
                Effect = effect;
                Critical = critical;
                SkillCategory = skillCategory;
                ElementalType = elementalType;
                SkillTargetType = targetType;
                Buff = buff;
                Turn = turn;
            }
        }

        public readonly IEnumerable<ArenaSkillInfo> SkillInfos;

        [CanBeNull]
        public readonly IEnumerable<ArenaSkillInfo> BuffInfos;

        protected ArenaSkill(
            ArenaCharacter character,
            IEnumerable<ArenaSkillInfo> skillInfos,
            IEnumerable<ArenaSkillInfo> buffInfos)
            : base(character)
        {
            SkillInfos = skillInfos;
            BuffInfos = buffInfos;
        }
    }
}

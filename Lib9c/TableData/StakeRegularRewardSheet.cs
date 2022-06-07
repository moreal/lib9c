using System;
using System.Collections.Generic;
using System.Linq;
using Bencodex.Types;
using Libplanet.Assets;
using Nekoyume.Model.State;
using static Nekoyume.TableData.TableExtensions;
using static Lib9c.SerializeKeys;

namespace Nekoyume.TableData
{
    [Serializable]
    public class StakeRegularRewardSheet : Sheet<int, StakeRegularRewardSheet.Row>, IStakeRewardSheet
    {
        [Serializable]
        public class RewardInfo
        {
            public enum RewardType
            {
                Fixed,
                Arithmetic,
            }

            protected bool Equals(RewardInfo other)
            {
                return ItemId == other.ItemId && RateOrCount == other.RateOrCount && Type == other.Type;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((RewardInfo) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (ItemId * 397) ^ RateOrCount ^ Type.GetHashCode();
                }
            }

            public readonly int ItemId;
            public readonly int RateOrCount;
            public readonly RewardType Type;

            public RewardInfo(params string[] fields)
            {
                ItemId = ParseInt(fields[0]);
                RateOrCount = ParseInt(fields[1]);
                Type = fields.Length >= 3 && Enum.TryParse(fields[2], true, out RewardType type)
                    ? type
                    : RewardType.Arithmetic;
            }

            public RewardInfo(int itemId, int rateOrCount, RewardType type)
            {
                ItemId = itemId;
                RateOrCount = rateOrCount;
                Type = type;
            }

            public RewardInfo(Dictionary dictionary)
            {
                ItemId = dictionary[IdKey].ToInteger();
                RateOrCount = dictionary[RateOrCountKey].ToInteger();
                Type = dictionary.TryGetValue((Text)RewardTypeKey, out IValue value) &&
                       RewardType.TryParse((Text)value, out RewardType result)
                    ? result
                    : RewardType.Arithmetic;
            }
            public IValue Serialize()
            {
                return Dictionary.Empty
                    .Add(IdKey, ItemId.Serialize())
                    .Add(RateOrCountKey, RateOrCount.Serialize())
                    .Add(RewardTypeKey, Type.Serialize());
            }
        }

        [Serializable]
        public class Row : SheetRow<int>, IStakeRewardRow
        {
            public override int Key => Level;

            public int Level { get; private set; }

            public long RequiredGold { get; private set; }

            public List<RewardInfo> Rewards { get; private set; }

            public override void Set(IReadOnlyList<string> fields)
            {
                Level = ParseInt(fields[0]);
                RequiredGold = ParseInt(fields[1]);
                var info = new RewardInfo(fields.Skip(2).ToArray());
                Rewards = new List<RewardInfo> {info};
            }
        }

        public StakeRegularRewardSheet() : base(nameof(StakeRegularRewardSheet))
        {
        }

        protected override void AddRow(int key, Row value)
        {
            if (!TryGetValue(key, out var row))
            {
                Add(key, value);

                return;
            }

            if (!value.Rewards.Any())
            {
                return;
            }

            row.Rewards.Add(value.Rewards[0]);
        }

        public IReadOnlyList<IStakeRewardRow> OrderedRows => OrderedList;
    }
}

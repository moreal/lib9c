using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bencodex.Types;
using Libplanet.Crypto;
using Nekoyume.Action;
using Nekoyume.Model.Item;

namespace Nekoyume.Model.State
{
    public class CombinationSlotState : State
    {
        private const string UnlockBlockIndexKey = "unlockBlockIndex";
        private const string StartBlockIndexKey = "startBlockIndex";
        private const string ResultKey = "result";
        private const string PetIdKey = "petId";
        private const string IndexKey = "index";
        private const string IsUnlockedKey = "isUnlocked";

        public const string DeriveFormat = "combination-slot-{0}";
        public long UnlockBlockIndex { get; private set; }
        public long StartBlockIndex { get; private set; }
        public AttachmentActionResult Result { get; private set; }
        public int? PetId { get; private set; }
        public long RequiredBlockIndex => UnlockBlockIndex - StartBlockIndex;
        /// <summary>
        /// It is a CombinationSlot index. start from 0.
        /// </summary>
        public int Index { get; set; }

        public bool IsUnlocked
        {
            get => Index < AvatarState.DefaultCombinationSlotCount || _isUnlocked;
            private set => _isUnlocked = value;
        }

        private bool _isUnlocked;

        /// <summary>
        /// Is only used for migration legacy state.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="slotIndex"></param>
        /// <returns></returns>
        public static Address DeriveAddress(Address address, int slotIndex) =>
            address.Derive(string.Format(
                CultureInfo.InvariantCulture,
                DeriveFormat,
                slotIndex));

        public CombinationSlotState(Address address, int index = 0) : base(address)
        {
            if (!ValidateSlotIndex(index))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(index),
                    index,
                    "The index of the combination slot must be between 0 and CombinationSlotCapacity.");
            }

            UnlockBlockIndex = 0;
            Index = index;
            IsUnlocked = AvatarState.DefaultCombinationSlotCount > index;
        }

        public CombinationSlotState(Dictionary serialized) : base(serialized)
        {
            UnlockBlockIndex = serialized[UnlockBlockIndexKey].ToLong();

            if (serialized.TryGetValue((Text)IndexKey, out var index))
            {
                Index = (Integer)index;
            }

            if (serialized.TryGetValue((Text)ResultKey, out var result))
            {
                Result = AttachmentActionResult.Deserialize((Dictionary) result);
            }

            if (serialized.TryGetValue((Text)StartBlockIndexKey, out var value))
            {
                StartBlockIndex = value.ToLong();
            }

            if (serialized.TryGetValue((Text)PetIdKey, out var petId))
            {
                PetId = petId.ToNullableInteger();
            }

            if (serialized.TryGetValue((Text)IsUnlockedKey, out var isUnlocked))
            {
                IsUnlocked = isUnlocked.ToBoolean();
            }
        }

        public static bool ValidateSlotIndex(int index)
        {
            return index >= 0 && index < AvatarState.CombinationSlotCapacity;
        }

        [Obsolete("Use ValidateV2")]
        public bool Validate(AvatarState avatarState, long blockIndex)
        {
            if (avatarState is null)
            {
                return false;
            }

            return avatarState.worldInformation != null &&
                   blockIndex >= UnlockBlockIndex;
        }

        public bool ValidateV2(long blockIndex)
        {
            if (!IsUnlocked)
            {
                return false;
            }

            return blockIndex >= UnlockBlockIndex;
        }

        public void Update(AttachmentActionResult result, long blockIndex, long unlockBlockIndex, int? petId = null)
        {
            Result = result;
            StartBlockIndex = blockIndex;
            UnlockBlockIndex = unlockBlockIndex;
            PetId = petId;
        }

        public void Update(long blockIndex)
        {
            UnlockBlockIndex = blockIndex;
            Result.itemUsable.Update(blockIndex);
        }

        public void Update(long blockIndex, Material material, int count)
        {
            Update(blockIndex);
            var result = new RapidCombination0.ResultModel((Dictionary) Result.Serialize())
            {
                cost = new Dictionary<Material, int> {[material] = count}
            };
            Result = result;
        }

        public void UpdateV2(long blockIndex, Material material, int count)
        {
            Update(blockIndex);
            var result = new RapidCombination5.ResultModel((Dictionary) Result.Serialize())
            {
                cost = new Dictionary<Material, int> {[material] = count}
            };
            Result = result;
        }

        public bool Unlock()
        {
            if (IsUnlocked)
            {
                return false;
            }

            IsUnlocked = true;
            return true;
        }

        public override IValue Serialize()
        {
            var values = new Dictionary<IKey, IValue>
            {
                [(Text)UnlockBlockIndexKey] = UnlockBlockIndex.Serialize(),
                [(Text)StartBlockIndexKey] = StartBlockIndex.Serialize(),
                [(Text)IndexKey] = (Integer)Index,
                [(Text)IsUnlockedKey] = IsUnlocked.Serialize(),
            };

            if (Result is not null)
            {
                values.Add((Text)ResultKey, Result.Serialize());
            }

            if (PetId is not null)
            {
                values.Add((Text)PetIdKey, PetId.Serialize());
            }

#pragma warning disable LAA1002
            return new Dictionary(values.Union((Dictionary) base.SerializeBase()));
#pragma warning restore LAA1002
        }
    }
}

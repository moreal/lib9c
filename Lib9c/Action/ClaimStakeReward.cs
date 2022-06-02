using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet;
using Libplanet.Action;
using Nekoyume.BlockChain.Policy;
using Nekoyume.Extensions;
using Nekoyume.Model.Item;
using Nekoyume.Model.State;
using Nekoyume.TableData;
using Serilog;
using static Lib9c.SerializeKeys;

namespace Nekoyume.Action
{
    [ActionType("claim_stake_reward")]
    public class ClaimStakeReward : GameAction
    {
        internal Address AvatarAddress { get; private set; }

        public ClaimStakeReward(Address avatarAddress)
        {
            AvatarAddress = avatarAddress;
        }

        public ClaimStakeReward() : base()
        {
        }

        public override IAccountStateDelta Execute(IActionContext context)
        {
            var states = context.PreviousStates;
            if (!states.TryGetStakeState(context.Signer, out StakeState stakeState))
            {
                throw new FailedLoadStateException(nameof(StakeState));
            }

            var blockPolicySource = new BlockPolicySource(Log.Logger);
            BlockPolicy blockPolicy = (BlockPolicy)(states.GetGoldCurrency().Minters
                .Contains(new Address("340f110b91d0577a9ae0ea69ce15269436f217da"))
                ? blockPolicySource.GetPermanentPolicy()
                : blockPolicySource.GetPolicy());
            long rewardInterval = blockPolicy.GetStakeRewardInterval(context.BlockIndex);


            var sheets = states.GetSheets(sheetTypes: new[]
            {
                typeof(StakeRegularRewardSheet),
                typeof(ConsumableItemSheet),
                typeof(CostumeItemSheet),
                typeof(EquipmentItemSheet),
                typeof(MaterialItemSheet),
            });

            var stakeRegularRewardSheet = sheets.GetSheet<StakeRegularRewardSheet>();

            var currency = states.GetGoldCurrency();
            var stakedAmount = states.GetBalance(stakeState.address, currency);

            if (!stakeState.IsClaimable(context.BlockIndex, rewardInterval))
            {
                throw new RequiredBlockIndexException();
            }

            var avatarState = states.GetAvatarStateV2(AvatarAddress);
            int level = stakeRegularRewardSheet.FindLevelByStakedAmount(stakedAmount);
            var rewards = stakeRegularRewardSheet[level].Rewards;
            ItemSheet itemSheet = sheets.GetItemSheet();
            var accumulatedRewards = stakeState.CalculateAccumulatedRewards(context.BlockIndex);
            foreach (var reward in rewards)
            {
                var (quantity, _) = stakedAmount.DivRem(currency * reward.Rate);
                ItemSheet.Row row = itemSheet[reward.ItemId];
                ItemBase item = row is MaterialItemSheet.Row materialRow
                    ? ItemFactory.CreateTradableMaterial(materialRow)
                    : ItemFactory.CreateItem(row, context.Random);
                avatarState.inventory.AddItem(item, (int) quantity * accumulatedRewards);
            }

            stakeState.Claim(context.BlockIndex);

            return states.SetState(stakeState.address, stakeState.Serialize())
                .SetState(avatarState.address, avatarState.SerializeV2())
                .SetState(
                    avatarState.address.Derive(LegacyInventoryKey),
                    avatarState.inventory.Serialize());
        }

        protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
            ImmutableDictionary<string, IValue>.Empty
                .Add(AvatarAddressKey, AvatarAddress.Serialize());

        protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
        {
            AvatarAddress = plainValue[AvatarAddressKey].ToAddress();
        }
    }
}

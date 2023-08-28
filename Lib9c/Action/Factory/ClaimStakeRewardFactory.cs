using System;
using Libplanet.Crypto;

namespace Nekoyume.Action.Factory
{
    public static class ClaimStakeRewardFactory
    {
        // NOTE: This method does not return a type of `ClaimStakeReward1`.
        //       Because it is not obsoleted yet.
        public static IClaimStakeReward CreateByBlockIndex(
            long blockIndex,
            Address avatarAddress)
        {
            // FIXME: This method should consider the starting block index of
            //        `claim_stake_reward2`. And if the `blockIndex` is less than
            //        the starting block index, it should throw an exception.
            // default: Version 2
            return blockIndex switch
            {
                > ClaimStakeReward7.ObsoleteBlockIndex => new ClaimStakeReward(avatarAddress),
                > ClaimStakeReward6.ObsoleteBlockIndex => new ClaimStakeReward7(avatarAddress),
                > ClaimStakeReward5.ObsoleteBlockIndex => new ClaimStakeReward6(avatarAddress),
                > ClaimStakeReward4.ObsoleteBlockIndex => new ClaimStakeReward5(avatarAddress),
                > ClaimStakeReward3.ObsoleteBlockIndex => new ClaimStakeReward4(avatarAddress),
                > ClaimStakeReward2.ObsoletedIndex => new ClaimStakeReward3(avatarAddress),
                _ => new ClaimStakeReward2(avatarAddress)
            };
        }

        public static IClaimStakeReward CreateByVersion(
            int version,
            Address avatarAddress) => version switch
        {
            1 => new ClaimStakeReward1(avatarAddress),
            2 => new ClaimStakeReward2(avatarAddress),
            3 => new ClaimStakeReward3(avatarAddress),
            4 => new ClaimStakeReward4(avatarAddress),
            5 => new ClaimStakeReward5(avatarAddress),
            6 => new ClaimStakeReward6(avatarAddress),
            7 => new ClaimStakeReward7(avatarAddress),
            8 => new ClaimStakeReward(avatarAddress),
            _ => throw new ArgumentOutOfRangeException(
                $"Invalid version: {version}"),
        };
    }
}

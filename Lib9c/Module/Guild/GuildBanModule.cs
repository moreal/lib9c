using System;
using Libplanet.Action.State;
using Libplanet.Crypto;
using Boolean = Bencodex.Types.Boolean;

namespace Nekoyume.Module.Guild
{
    public static class GuildBanModule
    {
        public static bool IsBanned(this IWorldState worldState, Address guildAddress, Address agentAddress)
        {
            var value = worldState.GetAccountState(Addresses.GetGuildBanAccountAddress(guildAddress))
                .GetState(agentAddress);
            if (value is Boolean boolean)
            {
                if (!boolean)
                {
                    throw new InvalidOperationException();
                }

                return true;
            }

            return false;
        }

        public static IWorld Ban(this IWorld world, Address guildAddress, Address agentAddress)
        {
            var account = world.GetAccount(Addresses.GetGuildBanAccountAddress(guildAddress));
            account = account.SetState(agentAddress, (Boolean)true);
            return world.SetAccount(Addresses.GuildParticipant, account);
        }

        public static IWorld Unban(this IWorld world, Address guildAddress, Address agentAddress)
        {
            var account = world.GetAccount(Addresses.GetGuildBanAccountAddress(guildAddress));
            account = account.RemoveState(agentAddress);
            return world.SetAccount(Addresses.GuildParticipant, account);
        }
    }
}

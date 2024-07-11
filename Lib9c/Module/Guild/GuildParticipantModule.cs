#nullable enable
using System.Diagnostics.CodeAnalysis;
using Bencodex.Types;
using Libplanet.Action.State;
using Libplanet.Crypto;
using Nekoyume.Action;

namespace Nekoyume.Module.Guild
{
    public static class GuildParticipantModule
    {
        public static Model.Guild.GuildParticipant GetGuildParticipant(this IWorldState worldState, Address guildParticipantAddress)
        {
            var value = worldState.GetAccountState(Addresses.GuildParticipant)
                .GetState(guildParticipantAddress);
            if (value is List list)
            {
                return new Model.Guild.GuildParticipant(list);
            }

            throw new FailedLoadStateException("There is no such guild.");
        }

        public static bool TryGetGuildParticipant(this IWorldState worldState,
            Address agentAddress,
            [NotNullWhen(true)] out Model.Guild.GuildParticipant? guildParticipant)
        {
            try
            {
                guildParticipant = GetGuildParticipant(worldState, agentAddress);
                return true;
            }
            catch
            {
                guildParticipant = null;
                return false;
            }
        }

        public static IWorld SetGuildParticipant(this IWorld world, Address agentAddress,
            Model.Guild.GuildParticipant guildParticipant)
        {
            var account = world.GetAccount(Addresses.GuildParticipant);
            account = account.SetState(agentAddress, guildParticipant.Bencoded);
            return world.SetAccount(Addresses.GuildParticipant, account);
        }

        public static IWorld RemoveGuildParticipant(this IWorld world, Address agentAddress)
        {
            var account = world.GetAccount(Addresses.GuildParticipant);
            account = account.RemoveState(agentAddress);
            return world.SetAccount(Addresses.GuildParticipant, account);
        }
    }
}

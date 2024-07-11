#nullable enable
using System.Diagnostics.CodeAnalysis;
using Bencodex.Types;
using Libplanet.Action.State;
using Libplanet.Crypto;
using Nekoyume.Action;

namespace Nekoyume.Module.Guild
{
    public static class GuildModule
    {
        public static Model.Guild.Guild GetGuild(this IWorldState worldState, Address guildAddress)
        {
            var value = worldState.GetAccountState(Addresses.Guild).GetState(guildAddress);
            if (value is List list)
            {
                return new Model.Guild.Guild(list);
            }

            throw new FailedLoadStateException("There is no such guild.");
        }

        public static bool TryGetGuild(this IWorldState worldState,
            Address guildAddress, [NotNullWhen(true)] out Model.Guild.Guild? guild)
        {
            try
            {
                guild = GetGuild(worldState, guildAddress);
                return true;
            }
            catch
            {
                guild = null;
                return false;
            }
        }

        public static IWorld SetGuild(this IWorld world, Address guildAddress, Model.Guild.Guild guild)
        {
            var account = world.GetAccount(Addresses.Guild);
            account = account.SetState(guildAddress, guild.Bencoded);
            return world.SetAccount(Addresses.Guild, account);
        }

        public static IWorld RemoveGuild(this IWorld world, Address guildAddress)
        {
            var account = world.GetAccount(Addresses.Guild);
            account = account.RemoveState(guildAddress);
            return world.SetAccount(Addresses.Guild, account);
        }
    }
}

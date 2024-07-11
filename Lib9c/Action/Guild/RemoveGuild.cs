using System;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Nekoyume.Module.Guild;

namespace Nekoyume.Action.Guild
{
    /// <summary>
    /// An action to remove the guild.
    /// </summary>
    [ActionType("remove_guild")]
    public class RemoveGuild : ActionBase
    {
        public override IValue PlainValue => Null.Value;

        public override void LoadPlainValue(IValue plainValue)
        {
            if (plainValue is not Null)
            {
                throw new InvalidCastException();
            }
        }

        public override IWorld Execute(IActionContext context)
        {
            var world = context.PreviousState;

            // NOTE: GuildMaster address and GuildAddress are the same with signer address.
            var guildAddress = context.Signer;

            if (!world.TryGetGuild(context.Signer, out var guild))
            {
                throw new InvalidOperationException("The signer does not have a guild.");
            }

            if (guild.GuildMaster != guildAddress)
            {
                throw new InvalidOperationException("The signer is not a guild master.");
            }

            // TODO: Check there are remained participants in the guild.
            // TODO: Do something to return 'Power' token;

            return world.RemoveGuild(guildAddress);
        }
    }
}

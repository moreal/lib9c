using System;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using Nekoyume.Module.Guild;

namespace Nekoyume.Action.Guild
{
    [ActionType("ban_guild_member")]
    public class BanGuildMember : ActionBase
    {
        public Address Target { get; private set; }

        public override IValue PlainValue => Dictionary.Empty
            .Add("target", Target.Bencoded);

        public BanGuildMember() {}

        public BanGuildMember(Address target)
        {
            Target = target;
        }

        public override void LoadPlainValue(IValue plainValue)
        {
            if (plainValue is not Dictionary dictionary)
            {
                throw new InvalidCastException();
            }

            Target = new Address(dictionary["target"]);
        }

        public override IWorld Execute(IActionContext context)
        {
            var world = context.PreviousState;

            // NOTE: GuildMaster address and GuildAddress are the same with signer address.
            var guildAddress = context.Signer;

            if (!world.TryGetGuild(guildAddress, out var guild))
            {
                throw new InvalidOperationException("The signer does not have a guild.");
            }

            if (guild.GuildMaster != guildAddress)
            {
                throw new InvalidOperationException("The signer is not a guild master.");
            }

            if (!world.TryGetGuildParticipant(Target, out var guildParticipant))
            {
                throw new InvalidOperationException("The target did not join any guild.");
            }

            if (guildParticipant.GuildAddress != guildAddress)
            {
                throw new InvalidOperationException("The target is not in your guild.");
            }

            return world
                .Ban(guildAddress, Target)
                .RemoveGuildParticipant(Target);
        }
    }
}

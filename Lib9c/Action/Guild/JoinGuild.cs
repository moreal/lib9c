using System;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using Nekoyume.Model.Guild;
using Nekoyume.Module.Guild;

namespace Nekoyume.Action.Guild
{
    [ActionType("join_guild")]
    public class JoinGuild : ActionBase
    {
        public JoinGuild() {}

        public JoinGuild(Address guildAddress)
        {
            GuildAddress = guildAddress;
        }

        public Address GuildAddress { get; private set; }

        public override IValue PlainValue => Dictionary.Empty
            .Add("guild_address", GuildAddress.Bencoded);

        public override void LoadPlainValue(IValue plainValue)
        {
            if (plainValue is not Dictionary dictionary)
            {
                throw new InvalidCastException();
            }

            GuildAddress = new Address(dictionary["guild_address"]);
        }

        public override IWorld Execute(IActionContext context)
        {
            var world = context.PreviousState;
            var guildParticipantAccount = world.GetAccount(Addresses.GuildParticipant);
            var signer = context.Signer;

            if (guildParticipantAccount.GetState(signer) is not null)
            {
                throw new InvalidOperationException("The signer is already joined in a guild.");
            }

            // NOTE: Check there is such guild.
            _ = world.GetGuild(GuildAddress);

            if (world.IsBanned(GuildAddress, signer))
            {
                throw new InvalidOperationException("The signer is banned from the guild.");
            }

            // TODO: Do something related with ConsensusPower delegation.

            guildParticipantAccount = guildParticipantAccount.SetState(
                signer,
                new GuildParticipant(GuildAddress).Bencoded);

            return world.SetAccount(Addresses.GuildParticipant, guildParticipantAccount);
        }
    }
}

using System;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Nekoyume.Module.Guild;

namespace Nekoyume.Action.Guild
{
    [ActionType("quit_guild")]
    public class QuitGuild : ActionBase
    {
        public override IValue PlainValue => Dictionary.Empty;

        public override void LoadPlainValue(IValue plainValue)
        {
            // NOTE: Do nothing.
        }

        public override IWorld Execute(IActionContext context)
        {
            var world = context.PreviousState;
            if (!world.TryGetGuildParticipant(context.Signer, out _))
            {
                throw new InvalidOperationException("The signer did not join any guild.");
            }

            // TODO: Do something to return 'Power' token;

            return world.RemoveGuildParticipant(context.Signer);
        }
    }
}

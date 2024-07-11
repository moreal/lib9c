using System;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Nekoyume.Module.Guild;

namespace Nekoyume.Action.Guild
{
    [ActionType("make_guild")]
    public class MakeGuild : ActionBase
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
            var signer = context.Signer;

            if (world.TryGetGuild(signer, out _))
            {
                throw new InvalidOperationException("The signer already has a guild.");
            }

            return world.SetGuild(signer, new Model.Guild.Guild(signer));
        }
    }
}

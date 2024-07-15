using System;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Nekoyume.Module.Guild;

namespace Nekoyume.Action.Guild
{
    [ActionType(TypeIdentifier)]
    public class MakeGuild : ActionBase
    {
        public const string TypeIdentifier = "make_guild";

        public override IValue PlainValue => Dictionary.Empty
            .Add("type_id", TypeIdentifier);

        public override void LoadPlainValue(IValue plainValue)
        {
            if (plainValue is not Dictionary)
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

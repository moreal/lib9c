using System;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Nekoyume.Extensions;
using Nekoyume.Module.Guild;

namespace Nekoyume.Action.Guild
{
    /// <summary>
    /// An action to remove the guild.
    /// </summary>
    [ActionType(TypeIdentifier)]
    public class RemoveGuild : ActionBase
    {
        public const string TypeIdentifier = "remove_guild";

        public override IValue PlainValue => Dictionary.Empty
            .Add("type_id", TypeIdentifier)
            .Add("values", Null.Value);

        public override void LoadPlainValue(IValue plainValue)
        {
            if (plainValue is not Dictionary root ||
                !root.TryGetValue((Text)"values", out var rawValues) ||
                rawValues is not Null)
            {
                throw new InvalidCastException();
            }
        }

        public override IWorld Execute(IActionContext context)
        {
            context.UseGas(1);

            var world = context.PreviousState;
            var signer = context.GetAgentAddress();

            // TODO: Do something to return 'Power' token;

            return world.RemoveGuild(signer);
        }
    }
}

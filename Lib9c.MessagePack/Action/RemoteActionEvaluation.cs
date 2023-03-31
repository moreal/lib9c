#nullable enable
using System;
using System.Collections.Generic;
using Bencodex.Types;
using Lib9c.Formatters;
using Libplanet.Action;
using MessagePack;

namespace Lib9c.MessagePack.Action
{
    [MessagePackObject]
    public struct RemoteActionEvaluation
    {
        [Key(0)]
        public IValue? Action { get; set; }

        [Key(1)]
        [MessagePackFormatter(typeof(ActionContextFormatter))]
        public IActionContext InputContext { get; set; }

        [Key(2)]
        [MessagePackFormatter(typeof(AccountStateDeltaFormatter))]
        public IAccountStateDelta OutputStates { get; set; }

        [Key(3)]
        [MessagePackFormatter(typeof(ExceptionFormatter<Exception>))]
        public Exception? Exception { get; set; }

        [Key(4)]
        public List<string> Logs { get; set; }

        [SerializationConstructor]
        public RemoteActionEvaluation(
            IAction? action,
            IAccountStateDelta outputStates,
            Exception? exception,
            IActionContext inputContext,
            List<string> logs
        )
        {
            Action = action?.PlainValue;
            OutputStates = outputStates;
            Exception = exception;
            InputContext = inputContext;
            Logs = logs;
        }
    }
}

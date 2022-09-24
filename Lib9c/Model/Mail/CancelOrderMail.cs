using System;
using System.Collections.Generic;
using System.Linq;
using Bencodex.Types;
using Nekoyume.Model.State;
using static Lib9c.SerializeKeys;

#nullable disable
namespace Nekoyume.Model.Mail
{
    [Serializable]
    public class CancelOrderMail : Mail
    {
        public readonly Guid OrderId;
        public CancelOrderMail(long blockIndex, Guid id, long requiredBlockIndex, Guid orderId) : base(blockIndex, id, requiredBlockIndex)
        {
            OrderId = orderId;
        }

        public CancelOrderMail(Dictionary serialized) : base(serialized)
        {
            OrderId = serialized[OrderIdKey].ToGuid();
        }

        public override void Read(IMail mail)
        {
            mail.Read(this);
        }

        public override MailType MailType => MailType.Auction;

        protected override string TypeId => nameof(CancelOrderMail);

        public override IValue Serialize() =>
#pragma warning disable LAA1002
            new Dictionary(new Dictionary<IKey, IValue>
            {
                [(Text)OrderIdKey] = OrderId.Serialize(),
            }.Union((Dictionary)base.Serialize()));
#pragma warning restore LAA1002
    }
}

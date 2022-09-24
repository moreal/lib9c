using System;
using System.Runtime.Serialization;

#nullable disable
namespace Nekoyume.Action
{
    [Serializable]
    public class InvalidPriceException : ArgumentOutOfRangeException
    {
        public InvalidPriceException(string msg) : base(msg)
        {
        }

        protected InvalidPriceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Nekoyume.Action
{
    [Serializable]
    public class InvalidStageException : Exception
    {
        public InvalidStageException()
        {
        }

        public InvalidStageException(string msg) : base(msg)
        {
        }

        protected InvalidStageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

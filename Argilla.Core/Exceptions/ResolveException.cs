using System;
using System.Runtime.Serialization;

namespace Argilla.Core.Exceptions
{
    [Serializable]
    public class ResolveException : Exception
    {
        public ResolveException()
        {
        }

        public ResolveException(string message) : base(message)
        {
        }

        public ResolveException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ResolveException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
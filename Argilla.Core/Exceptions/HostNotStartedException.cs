using System;
using System.Runtime.Serialization;

namespace Argilla.Core.Exceptions
{
    [Serializable]
    public class HostNotStartedException : Exception
    {
        public HostNotStartedException()
        {
        }

        public HostNotStartedException(string message) : base(message)
        {
        }

        public HostNotStartedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HostNotStartedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
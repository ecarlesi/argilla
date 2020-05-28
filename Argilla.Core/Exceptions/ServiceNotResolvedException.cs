using System;
using System.Runtime.Serialization;

namespace Argilla.Core.Exceptions
{
    [Serializable]
    public class ServiceNotResolvedException : Exception
    {
        public ServiceNotResolvedException()
        {
        }

        public ServiceNotResolvedException(string message) : base(message)
        {
        }

        public ServiceNotResolvedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ServiceNotResolvedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
using System;
using System.Runtime.Serialization;

namespace Argilla.Core.Exceptions
{
    [Serializable]
    public class CallbackNullException : Exception
    {
        public CallbackNullException()
        {
        }

        public CallbackNullException(string message) : base(message)
        {
        }

        public CallbackNullException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CallbackNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
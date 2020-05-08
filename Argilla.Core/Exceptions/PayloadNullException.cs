using System;
using System.Runtime.Serialization;

namespace Argilla.Core.Exceptions
{
    [Serializable]
    internal class PayloadNullException : Exception
    {
        public PayloadNullException()
        {
        }

        public PayloadNullException(string message) : base(message)
        {
        }

        public PayloadNullException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PayloadNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
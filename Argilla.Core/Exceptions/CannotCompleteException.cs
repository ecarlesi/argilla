using System;
using System.Runtime.Serialization;

namespace Argilla.Core.Exceptions
{
    [Serializable]
    public class CannotCompleteException : Exception
    {
        public CannotCompleteException()
        {
        }

        public CannotCompleteException(string message) : base(message)
        {
        }

        public CannotCompleteException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotCompleteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
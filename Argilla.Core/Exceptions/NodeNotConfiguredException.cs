using System;
using System.Runtime.Serialization;

namespace Argilla.Core.Exceptions
{
    [Serializable]
    internal class NodeNotConfiguredException : Exception
    {
        public NodeNotConfiguredException()
        {
        }

        public NodeNotConfiguredException(string message) : base(message)
        {
        }

        public NodeNotConfiguredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NodeNotConfiguredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
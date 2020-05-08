using System;

namespace Argilla.Core.Entities
{
    internal class PendingRequest
    {
        internal Action<Object> Action { get; set; }
    }
}

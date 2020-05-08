using System.Collections.Generic;

namespace Argilla.Common.Entities
{
    public class ResolveResponse : ObjectWithProperties
    {
        public ResolveResponse()
        {
            this.Endpoints = new List<Endpoint>();
        }

        public List<Endpoint> Endpoints { get; set; }
    }
}

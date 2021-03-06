﻿using System.Collections.Generic;

namespace Argilla.Core.Common.Entities
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

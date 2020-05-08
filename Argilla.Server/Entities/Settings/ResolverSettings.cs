using System;
using System.Collections.Generic;
using Argilla.Common.Entities;

namespace Argilla.Server.Entities.Settings
{
    public class ResolverSettings
    {
        public Manager SecurityManager { get; set; }
        public Manager StorageManager { get; set; }
    }
}

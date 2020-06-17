using System;
using System.Reflection;

namespace Argilla.Core.Entities.Setting
{
    public class ArgillaSettings
    {
        public static ArgillaSettings Current { get; set; }

        public Resolver Resolver { get; set; }
        public Node Node { get; set; }
        public MethodInfo MessageReceivedHandler { get; set; }
    }
}

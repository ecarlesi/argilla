using System;
namespace Argilla.Core.Entities.Setting
{
    public class ArgillaSettings
    {
        public static ArgillaSettings Current { get; set; }

        public Resolver Resolver { get; set; }
        public Node Node { get; set; }
        public Func<string, string> MessageReceivedHandler { get; set; }
    }
}

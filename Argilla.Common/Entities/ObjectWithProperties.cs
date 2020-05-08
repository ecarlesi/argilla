using System.Collections.Generic;

namespace Argilla.Common.Entities
{
    public class ObjectWithProperties
    {
        public Dictionary<string, string> Properties { get; set; }

        public ObjectWithProperties()
        {
            Properties = new Dictionary<string, string>();
        }
    }
}

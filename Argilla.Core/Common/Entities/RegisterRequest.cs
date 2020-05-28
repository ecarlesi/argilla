namespace Argilla.Core.Common.Entities
{
    public class RegisterRequest
    {
        public string ServiceName { get; set; }
        public string EndpointSync { get; set; }
        public string EndpointAsync { get; set; }
    }
}

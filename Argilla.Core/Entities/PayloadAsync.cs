namespace Argilla.Core.Entities
{
    public class PayloadAsync : PayloadSync
    {
        public string UrlCallback { get; set; }
        public string CorrelationId { get; set; }
    }
}

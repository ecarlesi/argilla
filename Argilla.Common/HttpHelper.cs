using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Argilla.Common
{
    /// <summary>
    /// HTTP helper
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// Execute a POST action with a Json body.
        /// </summary>
        /// <param name="url">The HTTP resource.</param>
        /// <param name="json">The Json body.</param>
        /// <returns></returns>
        public static string Post(string url, string json)
        {
            HttpClient client = new HttpClient();

            Task<HttpResponseMessage> responseMessage = client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

            HttpResponseMessage success = responseMessage.Result.EnsureSuccessStatusCode();

            return success.Content.ReadAsStringAsync().Result;
        }
    }
}

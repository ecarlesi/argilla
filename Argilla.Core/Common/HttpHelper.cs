using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Argilla.Core.Common
{
    /// <summary>
    /// HTTP helper
    /// </summary>
    public static class HttpHelper
    {
        private static NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// Execute a POST action with a Json body.
        /// </summary>
        /// <param name="endpoint">The HTTP resource.</param>
        /// <param name="json">The Json body.</param>
        /// <returns></returns>
        public static string Post(string endpoint, string json)
        {
            logger.Debug("Endpoint: " + endpoint);
            logger.Debug("Json: " + json);

            using (HttpClient client = new HttpClient())
            {
                StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                Task<HttpResponseMessage> responseMessage = client.PostAsync(endpoint, stringContent);

                try
                {
                    HttpResponseMessage success = responseMessage.Result.EnsureSuccessStatusCode();

                    string result = success.Content.ReadAsStringAsync().Result;

                    logger.Debug("Result: " + result);

                    return result;
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);

                    throw;
                }
            }
        }
    }
}

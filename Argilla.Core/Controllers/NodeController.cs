using System.Text.Json;
using System.Threading;
using Argilla.Common;
using Argilla.Core.Entities;
using Argilla.Core.Entities.Setting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Argilla.Core.Controllers
{
    [Route("api/[controller]")]
    public class NodeController : ControllerBase
    {
        private ILogger<NodeController> logger;

        public NodeController(ILogger<NodeController> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        [Route("callback")]
        public ActionResult Callback([FromBody] PayloadSync payload)
        {
            string content = CustomJsonSerializer.Serialize(payload.Payload);

            string result = ArgillaSettings.Current.MessageReceivedHandler.Invoke(content);

            return new ContentResult() { Content = result, ContentType = "application/json", StatusCode = 200 };
        }

        [HttpPost]
        [Route("callbackasync")]
        public ActionResult CallbackAsync([FromBody] PayloadAsync payload)
        {
            new Thread(new ParameterizedThreadStart(BackgroundProcessor.Process)).Start(payload);

            return new JsonResult(new { Success = true });
        }

        [HttpPost]
        [Route("return")]
        public ActionResult Return([FromBody] PayloadAsync payload)
        {
            string json = ((JsonElement)payload.Payload).GetString();

            Client.Complete(payload.CorrelationId, json);

            return new JsonResult(new { Success = true });
        }
    }
}
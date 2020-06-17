using System;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using Argilla.Core.Common;
using Argilla.Core.Entities;
using Argilla.Core.Entities.Setting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Argilla.Core.Controllers
{
    [Route("api/[controller]")]
    public class NodeController : ControllerBase
    {
        // TODO add logger, manage exceptions

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

            MethodInfo methodInfo = ArgillaSettings.Current.MessageReceivedHandler;

            ParameterInfo[] pis = methodInfo.GetParameters();
            ParameterInfo argumentPI = pis[0];
            Type t = pis[0].ParameterType;

            object o = CustomJsonSerializer.Deserialize(content, t);
            object a = ArgillaSettings.Current.MessageReceivedHandler.Invoke(null, new[] { o });

            string result = CustomJsonSerializer.Serialize(a);

            return new ContentResult() { Content = result, ContentType = "application/json", StatusCode = 200 };
        }

        [HttpPost]
        [Route("callbackasync")]
        public ActionResult CallbackAsync([FromBody] PayloadAsync payload)
        {
            ThreadPool.QueueUserWorkItem(BackgroundProcessor.Process, payload);

            return new JsonResult(new { Success = true });
        }

        [HttpPost]
        [Route("return")]
        public ActionResult Return([FromBody] PayloadAsync payload)
        {
            var a = payload.Payload;
            var b = (JsonElement)payload.Payload;
            string json = b.ToString();

            Client.Complete(payload.CorrelationId, json);

            return new JsonResult(new { Success = true });
        }
    }
}
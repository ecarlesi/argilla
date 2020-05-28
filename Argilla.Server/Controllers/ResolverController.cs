using Argilla.Core.Common;
using Argilla.Core.Common.Entities;
using Argilla.Server.Entities;
using Argilla.Server.Entities.Settings;
using Argilla.Server.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Argilla.Server.Controllers
{
    [Route("api/[controller]")]
    public class ResolverController : ControllerBase
    {
        private static NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        private readonly ResolverSettings resolverSettings;
        private readonly ISecurityManager securityManager;
        private readonly IStorageManager storageManager;

        public ResolverController(IOptions<ResolverSettings> resolverSettings)
        {
            this.resolverSettings = resolverSettings.Value;

            securityManager = SecurityProvider.Get(this.resolverSettings.SecurityManager);
            storageManager = StorageProvider.Get(this.resolverSettings.StorageManager);
        }

        [HttpPost]
        [Route("register")]
        public ActionResult Register([FromBody] Endpoint endpoint)
        {
            securityManager.Verify(new SecurityAssertion() { Payload = endpoint });

            logger.Info("Register: " + CustomJsonSerializer.Serialize(endpoint));

            storageManager.Register(endpoint);

            return new JsonResult(new RegisterResponse() { Success = true });
        }

        [HttpPost]
        [Route("unregister")]
        public ActionResult Unregister([FromBody] Endpoint endpoint)
        {
            securityManager.Verify(new SecurityAssertion() { Payload = endpoint });

            logger.Info("Unregister: " + CustomJsonSerializer.Serialize(endpoint));

            storageManager.Unregister(endpoint);

            return new JsonResult(new RegisterResponse() { Success = true });
        }

        [HttpPost]
        [Route("resolve")]
        public ActionResult Resolve([FromBody] ResolveRequest resolveRequest)
        {
            securityManager.Verify(new SecurityAssertion() { Payload = resolveRequest });

            logger.Info("Resolve: " + CustomJsonSerializer.Serialize(resolveRequest));

            ResolveResponse resolveResponse = new ResolveResponse();

            resolveResponse.Endpoints = storageManager.Resolve(resolveRequest);

            return new JsonResult(resolveResponse);
        }
    }
}

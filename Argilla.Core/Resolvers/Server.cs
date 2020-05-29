using System;
using Argilla.Core.Common;
using Argilla.Core.Common.Entities;
using Argilla.Core.Entities.Setting;

namespace Argilla.Core.Resolvers
{
    public class Server : IResolver
    {
        private static NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        public RegisterResponse Register(RegisterRequest registerRequest)
        {
            string resolverBaseAddress = ArgillaSettings.Current.Resolver.BaseAddress.EndsWith("/") ? ArgillaSettings.Current.Resolver.BaseAddress : ArgillaSettings.Current.Resolver.BaseAddress + "/";
            string resolverAddress = resolverBaseAddress + "register";
            string json = CustomJsonSerializer.Serialize(registerRequest);

            logger.Debug(String.Format("Registration request: {0}", json));

            string result = HttpHelper.Post(resolverAddress, json);

            return CustomJsonSerializer.Deserialize<RegisterResponse>(result);
        }

        public ResolveResponse Resolve(string serviceName)
        {
            string resolverBaseAddress = ArgillaSettings.Current.Resolver.BaseAddress.EndsWith("/") ? ArgillaSettings.Current.Resolver.BaseAddress : ArgillaSettings.Current.Resolver.BaseAddress + "/";
            string resolverAddress = resolverBaseAddress + "resolve";

            logger.Info(String.Format("Resolve service {0} on {1}", serviceName, resolverAddress));

            string result = HttpHelper.Post(resolverAddress, CustomJsonSerializer.Serialize(new ResolveRequest() { ServiceName = serviceName }));

            logger.Debug(String.Format("Resove result: {0}", result));

            return CustomJsonSerializer.Deserialize<ResolveResponse>(result);
        }

        public RegisterResponse Unregister(RegisterRequest registerRequest)
        {
            string resolverBaseAddress = ArgillaSettings.Current.Resolver.BaseAddress.EndsWith("/") ? ArgillaSettings.Current.Resolver.BaseAddress : ArgillaSettings.Current.Resolver.BaseAddress + "/";
            string resolverAddress = resolverBaseAddress + "unregister";
            string json = CustomJsonSerializer.Serialize(registerRequest);

            logger.Debug(String.Format("Registration request: {0}", json));

            string result = HttpHelper.Post(resolverAddress, json);

            return CustomJsonSerializer.Deserialize<RegisterResponse>(result);
        }
    }
}

using System;
using Argilla.Core.Common;
using Argilla.Core.Entities;
using Argilla.Core.Entities.Setting;
using Argilla.Core.Exceptions;

namespace Argilla.Core
{
    internal class BackgroundProcessor
    {
        private static NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        internal static void Process(Object o)
        {
            PayloadAsync payload = o as PayloadAsync;

            if (payload == null)
            {
                throw new PayloadNullException();
            }

            if (ArgillaSettings.Current.MessageReceivedHandler == null)
            {
                throw new CallbackNullException();
            }

            string jsonArgument = CustomJsonSerializer.Serialize(payload.Payload);

            logger.Info(String.Format("Json argument: {0}", jsonArgument));

            payload.Payload = ArgillaSettings.Current.MessageReceivedHandler.Invoke(jsonArgument);

            string json = CustomJsonSerializer.Serialize(payload);

            logger.Info(String.Format("Json: {0}", json));

            string result = HttpHelper.Post(payload.UrlCallback, json);

            logger.Info(String.Format("Result: {0}", result));
        }
    }
}

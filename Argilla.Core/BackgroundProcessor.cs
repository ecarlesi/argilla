using System;
using System.Reflection;
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




            MethodInfo methodInfo = ArgillaSettings.Current.MessageReceivedHandler;

            ParameterInfo[] pis = methodInfo.GetParameters();
            ParameterInfo argumentPI = pis[0];
            Type t = pis[0].ParameterType;

            object argument = CustomJsonSerializer.Deserialize(jsonArgument, t);
            object a = ArgillaSettings.Current.MessageReceivedHandler.Invoke(null, new[] { argument });

            payload.Payload = a;

            string json = CustomJsonSerializer.Serialize(payload);

            logger.Info(String.Format("Json: {0}", json));

            string result = HttpHelper.Post(payload.UrlCallback, json);

            logger.Info(String.Format("Result: {0}", result));
        }
    }
}

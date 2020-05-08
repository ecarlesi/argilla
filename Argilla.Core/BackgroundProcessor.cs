using System;
using Argilla.Common;
using Argilla.Core.Entities;
using Argilla.Core.Entities.Setting;
using Argilla.Core.Exceptions;

namespace Argilla.Core
{
    internal class BackgroundProcessor
    {
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

            Logger.Info(String.Format("Json argument: {0}", jsonArgument));

            payload.Payload = ArgillaSettings.Current.MessageReceivedHandler.Invoke(jsonArgument);

            string json = CustomJsonSerializer.Serialize(payload);

            Logger.Info(String.Format("Json: {0}", json));

            string result = HttpHelper.Post(payload.UrlCallback, json);

            Logger.Info(String.Format("Result: {0}", result));
        }
    }
}

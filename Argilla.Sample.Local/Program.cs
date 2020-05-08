using System;
using System.Threading;
using Argilla.Common;
using Argilla.Core;
using Argilla.Core.Entities;

namespace Argilla.Sample.Local
{
    class Program
    {
        private static readonly string REMOTE_SERVICE_NAME = "local service";

        static void Main(string[] args)
        {
            Host.Start(OnIncomingMessage, OnError);

            Thread.Sleep(3000);

            Greeting greetingSync = new Greeting() { Message = "Incoming message from local (sync)" };
            Greeting greetingAsync = new Greeting() { Message = "Incoming message from local (async)" };

            Greeting2 response = Client.Invoke<Greeting, Greeting2>(REMOTE_SERVICE_NAME, greetingSync);

            Logger.Info(string.Format("Response sync: {0}", response == null ? "null" : response.Message));

            Client.Invoke<Greeting>(REMOTE_SERVICE_NAME, greetingAsync, new Action<object>(OnAsyncCompleted));

            Logger.Info(string.Format("Press ENTER to stop the host."));

            Console.ReadLine();

            Host.Stop();
        }

        private static OnClientErrorBehavior OnError(Exception arg)
        {
            return OnClientErrorBehavior.Throw;
        }

        public static void OnAsyncCompleted(Object o)
        {
            Greeting greeting = CustomJsonSerializer.Deserialize<Greeting>((string)o);

            Logger.Info(string.Format("OnAsyncCompleted: {0}", greeting.Message));
        }

        public static string OnIncomingMessage(string json)
        {
            Greeting greeting = CustomJsonSerializer.Deserialize<Greeting>(json);

            Logger.Info(string.Format("OnIncomingMessage: " + greeting.Message));

            return CustomJsonSerializer.Serialize(new Greeting() { Message = "Hello from local" });
        }
    }

    public class Greeting
    {
        public string Message { get; set; }
    }

    public class Greeting2
    {
        public string Message { get; set; }
    }
}

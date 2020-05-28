using System;
using System.Threading;
using Argilla.Core.Common;
using Argilla.Core;

namespace Argilla.Sample.Mary
{
    class Program
    {
        private static readonly string REMOTE_SERVICE_NAME = "bob service";

        static void Main(string[] args)
        {
            Host.Start(OnIncomingMessage);

            Thread.Sleep(10000);

            Greeting greetingSync = new Greeting() { Message = "Incoming message from Mary (sync)" };
            Greeting greetingAsync = new Greeting() { Message = "Incoming message from Mary (async)" };

            Greeting2 response = Client.Invoke<Greeting, Greeting2>(REMOTE_SERVICE_NAME, greetingSync);

            Logger.Info(string.Format("Response sync: {0}", response == null ? "null" : response.Message));

            Client.Invoke<Greeting>(REMOTE_SERVICE_NAME, greetingAsync, new Action<object>(OnAsyncCompleted));

            Logger.Info(string.Format("Press ENTER to stop the host."));

            Console.ReadLine();

            Host.Stop();
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

            return CustomJsonSerializer.Serialize(new Greeting() { Message = "Hello from Mary" });
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

using System;
using System.Threading;
using Argilla.Core.Common;
using Argilla.Core;

namespace Argilla.Sample.Bob
{
    class Program
    {
        private static NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        private static readonly string REMOTE_SERVICE_NAME = "mary service";

        static void Main(string[] args)
        {
            Host.Start<Greeting, Greeting2>(OnIncomingMessage);

            Thread.Sleep(10000);

            Greeting greetingSync = new Greeting() { Message = "Incoming message from Bob (sync)" };
            Greeting greetingAsync = new Greeting() { Message = "Incoming message from Bob (async)" };

            Greeting2 response = Client.Invoke<Greeting, Greeting2>(REMOTE_SERVICE_NAME, greetingSync);
            response = Client.Invoke<Greeting, Greeting2>(REMOTE_SERVICE_NAME, greetingSync);
            response = Client.Invoke<Greeting, Greeting2>(REMOTE_SERVICE_NAME, greetingSync);
            response = Client.Invoke<Greeting, Greeting2>(REMOTE_SERVICE_NAME, greetingSync);

            logger.Info(string.Format("Response sync: {0}", response == null ? "null" : response.Message));

            Client.Invoke<Greeting>(REMOTE_SERVICE_NAME, greetingAsync, new Action<object>(OnAsyncCompleted));

            logger.Info(string.Format("Press ENTER to stop the host."));

            Console.ReadLine();

            Host.Stop();
        }

        public static void OnAsyncCompleted(Object o)
        {
            Greeting greeting = CustomJsonSerializer.Deserialize<Greeting>((string)o);

            logger.Info(string.Format("OnAsyncCompleted: {0}", greeting.Message));
        }

        public static Greeting2 OnIncomingMessage(Greeting greeting)
        {
            logger.Info(string.Format("OnIncomingMessage: " + greeting.Message));

            return new Greeting2() { Message = "Hello from Bob" };
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

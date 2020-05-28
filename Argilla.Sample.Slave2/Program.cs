using System;
using System.Threading;
using Argilla.Core.Common;
using Argilla.Core;

namespace Argilla.Sample.Slave2
{
    class Program
    {
        static void Main(string[] args)
        {
            Host.Start(OnIncomingMessage);

            Thread.Sleep(10000);

            Logger.Info(string.Format("Press ENTER to stop the host."));

            Console.ReadLine();

            Host.Stop();
        }

        public static string OnIncomingMessage(string json)
        {
            Message greeting = CustomJsonSerializer.Deserialize<Message>(json);

            Logger.Info(string.Format("OnIncomingMessage: " + greeting.Text));

            return CustomJsonSerializer.Serialize(new Message() { Text = "Hello from Slave 2" });
        }
    }

    public class Message
    {
        public string Text { get; set; }
    }
}

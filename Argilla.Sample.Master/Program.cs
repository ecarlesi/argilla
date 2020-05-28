using System;
using System.Threading;
using Argilla.Core.Common;
using Argilla.Core;

namespace Argilla.Sample.Master
{
    class Program
    {
        private static readonly string REMOTE_SERVICE_NAME = "slave service";

        static void Main(string[] args)
        {
            for (int i = 0; i < 1000; i++)
            {
                if (!Client.CanResolve(REMOTE_SERVICE_NAME))
                {
                    Logger.Info("Cannot proceed because the resolver is offline.");
                    i--;
                    Thread.Sleep(1000);
                    continue;
                }

                Message syncMessage = new Message() { Text = "sync request " + i };
                AnotherMessage syncResponse = Client.Invoke<Message, AnotherMessage>(REMOTE_SERVICE_NAME, syncMessage);
                Logger.Info(string.Format("Response: {0}", syncResponse == null ? "null" : syncResponse.Text));
                Thread.Sleep(1000);
            }
        }
    }

    public class Message
    {
        public string Text { get; set; }
    }

    public class AnotherMessage
    {
        public string Text { get; set; }
    }
}

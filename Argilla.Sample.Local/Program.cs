using System;
using System.Threading;
using Argilla.Common;
using Argilla.Core;
using Argilla.Core.Entities;

namespace Argilla.Sample.Local
{
    /*****************************************************************************
    * This class is a simple example of using Argilla.
    * 
    * During startup, the exposed service is registered (as indicated in the 
    * configuration file appsettings.json). After this registration, you will 
    * receive calls from clients invoking the exposed service. 
    * 
    * Incoming calls will be passed to the OnIncomingMessage method. 
    * 
    * Note that multiple processes can register to provide the same service. 
    * Argilla does not support any session concept so calls will be distributed 
    * across all available services without any precedence or affinity.
    *****************************************************************************/

    class Program
    {
        private static readonly string REMOTE_SERVICE_NAME = "local service";

        static void Main(string[] args)
        {
            /*****************************************************************************
             * Be aware to start this program AFTER the process Argilla.Server
             *****************************************************************************/

            /*****************************************************************************
             * Initialize Argilla.
             * 
             * The first argument is the method that must be invoked when a message 
             * arrives. Within this method you must implement your application logic.
             * 
             * The second argument (optional) is the method that must be invoked when a 
             * transmission / serialization error occurs.
             *****************************************************************************/
            Host.Start(OnIncomingMessage, OnError);

            /*****************************************************************************
             * This delay allows internal components to start correctly.
             *****************************************************************************/
            Thread.Sleep(3000);

            /*****************************************************************************
             * This is an example of a synchronous call. In this case a service exposed 
             * by this same process is called.
             *****************************************************************************/
            Message syncMessage = new Message() { Text = "sync request" };
            AnotherMessage syncResponse = Client.Invoke<Message, AnotherMessage>(REMOTE_SERVICE_NAME, syncMessage);
            Logger.Info(string.Format("Response: {0}", syncResponse == null ? "null" : syncResponse.Text));

            /*****************************************************************************
             * This is an example of an asynchronous call. In this case a service exposed 
             * by this same process is called.
             *****************************************************************************/
            Message asyncMessage = new Message() { Text = "async request" };
            Client.Invoke<Message>(REMOTE_SERVICE_NAME, asyncMessage, new Action<object>(OnAsyncCompleted));

            /*****************************************************************************
             * Waits for a newline to end the process.
             *****************************************************************************/
            Logger.Info(string.Format("Press ENTER to stop the host."));
            Console.ReadLine();
            Host.Stop();
        }

        /*****************************************************************************
         * This method is invoked when an error occurs internally (typically 
         * communication or serialization). The argument is the exception that has 
         * thrown. Here you can implement your management logic (throw or ignore).
         *****************************************************************************/
        private static OnClientErrorBehavior OnError(Exception arg)
        {
            return OnClientErrorBehavior.Throw;
        }

        /*****************************************************************************
         * This method is invoked on the client side when completing an asynchronous 
         * call. The argument is the return value from the invoked service.
         *****************************************************************************/
        public static void OnAsyncCompleted(Object o)
        {
            Message greeting = CustomJsonSerializer.Deserialize<Message>((string)o);

            Logger.Info(string.Format("OnAsyncCompleted: {0}", greeting.Text));
        }

        /*****************************************************************************
         * This method is invoked on the server side when a new request arrives. The 
         * argument is the Json containing the request message and you must resturn a
         * Json string which will be returned to the client.
         *****************************************************************************/
        public static string OnIncomingMessage(string json)
        {
            Message greeting = CustomJsonSerializer.Deserialize<Message>(json);

            Logger.Info(string.Format("OnIncomingMessage: " + greeting.Text));

            return CustomJsonSerializer.Serialize(new Message() { Text = "Hello from local" });
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

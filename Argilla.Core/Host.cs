using System;
using System.IO;
using System.Reflection;
using Argilla.Core.Entities;
using Argilla.Core.Entities.Setting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Argilla.Core
{
    public class Host
    {
        private static NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        private static IHost host = null;

        public static bool IsStarted { get; private set; }

        static Host()
        {
            IsStarted = false;
        }

        /// <summary>
        /// This method must be invoked as first step, before try to use any Argilla's resource.
        /// </summary>
        /// <param name="messageReceivedHandler">This callback will be invoked every time a message are received</param>
        /// <param name="clientErrorHandler">This callback will be invoked every time an exception will be throwed in the Client component.</param>
        public static void Start<TRequest, TResponse>(Func<TRequest, TResponse> messageReceivedHandler, Func<Exception, OnClientErrorBehavior> clientErrorHandler = null)
        {
            logger.Info("Enter Start");

            if (IsStarted)
            {
                logger.Info("Host already started");

                return;
            }

            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            IConfigurationRoot config = builder.Build();

            ArgillaSettings.Current = config.GetSection("Argilla").Get<ArgillaSettings>();

            ArgillaSettings.Current.MessageReceivedHandler = messageReceivedHandler.GetMethodInfo();

            Client.SetErrorHandler(clientErrorHandler);
            Client.Register();

            host = CreateHostBuilder().Build();
            host.StartAsync();

            IsStarted = true;

            logger.Info("Host sucessfully started");
        }

        /// <summary>
        /// This method stop Argilla host, after this invocation you cannot use also the client component.
        /// </summary>
        public static async void Stop()
        {
            if (!IsStarted)
            {
                return;
            }

            Client.Unregister();

            await host.StopAsync();

            host.Dispose();

            IsStarted = false;
        }

        private static IHostBuilder CreateHostBuilder() =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

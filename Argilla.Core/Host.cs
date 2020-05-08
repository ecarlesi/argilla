using System;
using System.IO;
using Argilla.Core.Entities;
using Argilla.Core.Entities.Setting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Argilla.Core
{
    public class Host
    {
        private static IHost host = null;

        /// <summary>
        /// This method must be invoked as first step, before try to use any Argilla's resource.
        /// </summary>
        /// <param name="messageReceivedHandler">This callback will be invoked every time a message are received</param>
        /// <param name="clientErrorHandler">This callback will be invoked every time an exception will be throwed in the Client component.</param>
        public static void Start(Func<string, string> messageReceivedHandler, Func<Exception, OnClientErrorBehavior> clientErrorHandler = null)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            IConfigurationRoot config = builder.Build();

            ArgillaSettings.Current = config.GetSection("Argilla").Get<ArgillaSettings>();
            ArgillaSettings.Current.MessageReceivedHandler = messageReceivedHandler;

            Client.SetErrorHandler(clientErrorHandler);
            Client.Register();

            host = CreateHostBuilder().Build();
            host.StartAsync();
        }

        /// <summary>
        /// This method stop Argilla host, after this invocation you cannot use also the client component.
        /// </summary>
        public static async void Stop()
        {
            Client.Unregister();

            await host.StopAsync();
            host.Dispose();
        }

        private static IHostBuilder CreateHostBuilder() =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

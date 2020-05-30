# Project description
Argilla is a simple distributed bus for the integration between microservices. The adoption of Argilla allows the elimination of endpoints that refine a microservice so that they can be managed more easily.

At present the project is functional even if it is not performing and unstable. Your help could be very useful to improve it;)

The idea of this project stems from an internal need of my cyber security project developed in .NET Core on Linux. I needed to enable various services to communicate without having to manage a catalog and even worse a distributed configuration. I also had the need to have callbacks from calls that can last for hours.

Creating a Argilla-enabled microservice is very simple, add a reference, a few lines of code and your microservice will be ready.

The architecture of Argilla is very simple, there is a Resolver server and its endpoint is configured in the various microservices. Argilla automatically publishes the catalog of services so that clients can consume the services without previously knowing their locations. If multiple services implement the same service, the client takes turns invoking the services to distribute the load. The client then takes care not to invoke the offline services and turn the requests to those available.

Argilla expects to be able to call a service synchronously and asynchronously. This type of call is essential when you want to invoke a slow service to answer (seconds but also hours or days), just specify the method in which to receive the response and Argilla will manage the return from the service.
This is a simple bus thought for .NET and especially for microservice.

To get an idea of how it works, check out the examples. 
The Argilla.Sample.Local project is documented and illustrates a component that in addition to exhibiting a service, also consumes it.
The Argilla.Sample.Slave1, Argilla.Sample.Slave2 and Argilla.Sample.Master projects, on the other hand, allow you to check the balancing mechanism of calls to multiple services. In fact, try to stop a service and you will see that the requests will move to the other instance. Here you will also find a simple pattern to make a process reliable in case of a Resolver fault. I'm working on making these features transparent in the client component.

## Getting started
This section explains how to create a simple test project consisting of a service provider and a client.  
This project shows how to make a synchronous call from a client to a microservice. Looking at the examples you will get more confidence than the other features.  
Create an empty solution named **ArgillaSample**.  
Add **Argilla.Core** project as reference to **ArgillaSample** solution.  
Add a new *Class Library* project to the solution and with name **ArgillaSample.Entities**. This project will contain entities shared between the service and the client.  
Inside the project **ArgillaSample.Entities** rename the file *Class1.cs* in *Message.cs*.  
Below the code of the *Message.cs* file: 

```c#
namespace ArgillaSample.Entities
{
    public class Message
    {
        public string Text { get; set; }
    }
}
```

### Create the service provider
Add a new *Console Application* project to the solution and with name **ArgillaSample.Service**.
In ArgillaSample.Service add a reference to Argilla.Core.
In ArgillaSample.Service add a reference to ArgillaSample.Entities.

Below the code of the Message.cs file:

using System;
using System.Threading;
using Argilla.Core;
using Argilla.Core.Common;
using ArgillaSample.Entities;

namespace ArgillaSample.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Host.Start(OnIncomingMessage);

            Thread.Sleep(3000);

            Console.WriteLine(string.Format("Press ENTER to stop the host."));

            Console.ReadLine();

            Host.Stop();
        }

        public static string OnIncomingMessage(string json)
        {
            Message message = CustomJsonSerializer.Deserialize<Message>(json);

            return CustomJsonSerializer.Serialize(new Message() { Text = "Echo: " + message.Text });
        }
    }
}



In ArgillaSample.Service add a new file appsettings.json with this contents:

{
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:2000"
      }
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "Argilla": {
    "Resolver": {
      "BaseAddress": "http://localhost:2020/api/resolver"
    },
    "Node": {
      "BaseAddress": "http://localhost:2000/api/node",
      "ServiceName": "Argilla sample service"
    }
  }
}



In ArgillaSample.Service add a new file nlog.config with this contents (specify the rith path): 

<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true">

  <extensions>
    <add assembly="NLog.Web.AspNetCore"/>
  </extensions>

  <targets>
    <target 
            xsi:type="File" 
            name="logger" 
            fileName="/Users/eca/logs/argillasample-service-${shortdate}.log"
            layout="${longdate} ${threadid} ${uppercase:${level}} ${logger} ${message} ${exception:format=tostring}" />
  </targets>

  <rules>
    <logger name="*" minlevel="Debug" writeTo="logger" />
  </rules>
</nlog>



### Create the consumer
Add a new "Console Application" project to the ArgillaSample solution, we call it "ArgillaSample.Client".
In ArgillaSample.Client add a reference to Argilla.Core.
In ArgillaSample.Client add a reference to ArgillaSample.Entities.



using System;
using ArgillaSample.Entities;

namespace ArgillaSample.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Message request = new Message() { Text = "Hi with Argilla" };
            Message response = Argilla.Core.Client.Invoke<Message, Message>("Argilla sample service", request);
            Console.WriteLine("Response message is: " + response.Text);
        }
    }
}






{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "Argilla": {
    "Resolver": {
      "BaseAddress": "http://localhost:2020/api/resolver"
    }
  }
}






<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true">

  <extensions>
    <add assembly="NLog.Web.AspNetCore"/>
  </extensions>

  <targets>
    <target 
            xsi:type="File" 
            name="logger" 
            fileName="/Users/eca/logs/argillasample-client-${shortdate}.log"
            layout="${longdate} ${threadid} ${uppercase:${level}} ${logger} ${message} ${exception:format=tostring}" />
  </targets>

  <rules>
    <logger name="*" minlevel="Debug" writeTo="logger" />
  </rules>
</nlog>



Be aware the files are Content and mcopied tooutput dir




### Test

Start Argilla.Resolver
Start Argilla Sample Service
Start Argilla Sample Client



5/30/2020 4:58:40 PM
Response message is: Echo: Hi with Argilla

## TODO
1) Sample projects.
2) Write documentation.
3) Check authorization on incoming request.
4) Cache support.

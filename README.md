# Project description
**Argilla** is a simple distributed bus for the integration between microservices. The adoption of **Argilla** allows the elimination of microservice endpoint configuration so that they can be managed more easily.

At present the project is functional even if it is not performing and unstable. Your help could be very useful to improve it ;)

The idea of this project starts from an internal need of my cyber security project developed in .NET Core on Linux. I needed to enable various services to communicate without having to manage a catalog and even worse a distributed configuration. I also had the need to have callbacks from calls that can last for hours.

Creating an **Argilla** enabled microservice is very simple, just add a reference, a few lines of code and your microservice are ready.

The architecture of **Argilla** is very simple, there is a *Resolver* server and its endpoint is configured in the various microservices. **Argilla** automatically publishes the catalog of services endpoint so that clients can consume the services without previously knowing their locations. If multiple services implement the same service, the client takes turns invoking the services to distribute the load. The client then takes care not to invoke the offline services and turn the requests to those available.

**Argilla** microservices can be invoked synchronously and asynchronously. The asynchronously invocation is essential when you want to invoke a service slow to answer (seconds but also hours or days), just specify the method in which to receive the response and **Argilla** will manage the return from the service.  

To get an idea of how it works, check out the examples.   
The **Argilla.Sample.Local** project is documented and illustrates a component that in addition to exhibiting a service, also consumes it.  
The **Argilla.Sample.Slave1**, **Argilla.Sample.Slave2** and **Argilla.Sample.Master** projects, on the other hand, allow you to check the balancing mechanism of calls to multiple services. In fact, try to stop a service and you will see that the requests will move to the other instance. Here you will also find a simple pattern to make a process reliable in case of a *Resolver* fault (I'm working on making these features transparent in the client component).

## Getting started
This section explains how to create a simple test project consisting of a service provider and a client.  
This project shows how to make a synchronous call from a client to a microservice. Looking at the examples you will get more confidence than the other features.

Create an empty solution named **ArgillaSample**.  
Within the solution **ArgillaSample** add the existing project **Argilla.Core** (previously cloned from Github).  
Within the solution **ArgillaSample** creates a new project of type *Class Library* with the name **ArgillaSample.Entities**. This project will contain entities shared between the service and the client.  
Within the project **ArgillaSample.Entities** rename the file **Class1.cs** in **Message.cs**.

Replace the contents of the **Message.cs** file with the one shown below
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
Within the solution **ArgillaSample** creates a new project of type *Console Application* with the name **ArgillaSample.Service**.

In **ArgillaSample.Service** 

1. add a reference to **Argilla.Core**
1. add a reference to **ArgillaSample.Entities**
1. add a new file **appsettings.json**, make sure that the build action for this file is "Content" and that it is copied to the output
1. add a new file **nlog.config**, make sure that the build action for this file is "Content" and that it is copied to the output

Replace the contents of the **Program.cs** file with the one shown below
```c#
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
            Host.Start<Message, Message>(OnIncomingMessage);
            Thread.Sleep(3000);
            Console.WriteLine(string.Format("Press ENTER to stop the host."));
            Console.ReadLine();
            Host.Stop();
        }

        public static Message OnIncomingMessage(Message message)
        {
            return new Message() { Text = "Echo: " + message.Text };
        }
    }
}
```

Replace the contents of the **appsettings.json** file with the one shown below
```json
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
```
The value of the *Argilla.Node.ServiceName* setting specifies the name that should be known by clients who wish to invoke this service. The service publishes its endpoint by associating it with the name of the service, the client asks to consume the service knowing its name. If an error occurs during the name resolution phase, check that it is written correctly in the client and in the configuration of the services.

Replace the contents of the **nlog.config** file with the one shown below (verify that the path of the log file is compatible with your environment)
```xml
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
```

### Create the consumer
Within the solution **ArgillaSample** creates a new project of type *Console Application* with the name **ArgillaSample.Client**.

In **ArgillaSample.Client** 

1. add a reference to **Argilla.Core**
1. add a reference to **ArgillaSample.Entities**
1. add a new file **appsettings.json**, make sure that the build action for this file is "Content" and that it is copied to the output
1. add a new file **nlog.config**, make sure that the build action for this file is "Content" and that it is copied to the output

Replace the contents of the **Program.cs** file with the one shown below
```c#
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
```

Replace the contents of the **appsettings.json** file with the one shown below
```json
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
```

Replace the contents of the **nlog.config** file with the one shown below (verify that the path of the log file is compatible with your environment)
```xml
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
```

### Test

1. open the **Argilla** solution, check the address in the **appsettings.json** inside the **Argilla.Server** is correct, then rebuild the solution
1. start the **Argilla.Server** project
1. back to the **ArgillaSample** solution and rebuild all
1. start the **ArgillaSample.Service**
1. start the **ArgillaSample.Client**

If everything went well within the **ArgillaSample.Client** log file you will find this message: *Response message is: Echo: Hi with Argilla*

### Security
**Argilla** currently does not implement any security mechanisms. We are working on implementing an authentication / authorization mechanism.

## TODO
1. improve documentation and sample projects
1. improve async mechanism
1. add security support

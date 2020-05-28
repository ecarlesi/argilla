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

## TODO
1) Sample projects.
2) Write documentation.
3) Check authorization on incoming request.
4) Cache support.

using System;
using System.Collections.Generic;
using System.Timers;
using Argilla.Core.Common;
using Argilla.Core.Entities;
using Argilla.Core.Exceptions;
using Argilla.Core.Entities.Setting;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;
using Argilla.Core.Common.Entities;

namespace Argilla.Core
{
    /// <summary>
    /// This class allow the invocation the Argilla services
    /// </summary>
    public static class Client
    {
        private static NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        private static readonly int DEFAULT_RESOLVER_CACHE_LIFETIME = 60; // seconds

        private static object LOCK = new object();

        private static Timer registerTimer = null;
        private static Func<Exception, OnClientErrorBehavior> errorHandler = null;
        private static Dictionary<string, PendingRequest> pending = null;
        private static List<CachedResolveResponse> resolveResponses = null;
        private static Dictionary<string, int> invocationCounter = new Dictionary<string, int>();

        public static bool WaitForResolver { get; set; }

        static Client()
        {
            pending = new Dictionary<string, PendingRequest>();

            registerTimer = new Timer(10000);
            registerTimer.Elapsed += OnRegisterTimer;
            registerTimer.AutoReset = true;
            registerTimer.Enabled = true;

            resolveResponses = new List<CachedResolveResponse>();

            if (ArgillaSettings.Current == null)
            {
                IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                IConfigurationRoot config = builder.Build();


                ArgillaSettings.Current = config.GetSection("Argilla").Get<ArgillaSettings>();
            }
        }

        #region public methods

        /// <summary>
        /// Invoke a synchronous service with the specified payload. The method call block until the method return.
        /// </summary>
        /// <typeparam name="TI">The payload type.</typeparam>
        /// <typeparam name="TO">The return type.</typeparam>
        /// <param name="serviceName">The name of the service, as configured in Argilla.Node.ServiceName.</param>
        /// <param name="payload">The payload used for invoke the service.</param>
        /// <returns></returns>
        public static TO Invoke<TI, TO>(string serviceName, TI payload)
        {
            TO outputValue = default;

            PayloadSync payloadSync = new PayloadSync() { Payload = payload };

            string json = CustomJsonSerializer.Serialize(payloadSync);

            Exception lastException = null;

            try
            {
                Endpoint endpoint = GetFreeEndpoint(serviceName);

                string jsonResult = HttpHelper.Post(endpoint.EndpointSync, json);

                outputValue = CustomJsonSerializer.Deserialize<TO>(jsonResult);

                lastException = null;
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);

                ResolveResponse resolveResponse = Resolve(serviceName);

                foreach (Endpoint endpoint in resolveResponse.Endpoints)
                {
                    try
                    {
                        string jsonResult = HttpHelper.Post(endpoint.EndpointSync, json);

                        outputValue = CustomJsonSerializer.Deserialize<TO>(jsonResult);

                        lastException = null;

                        break;
                    }
                    catch (Exception e)
                    {
                        lastException = e;
                    }
                }
            }

            if (lastException != null)
            {
                logger.Error(lastException, lastException.Message);

                throw lastException;
            }

            logger.Debug(String.Format("Output value: {0}", outputValue));

            return outputValue;
        }

        /// <summary>
        /// Invoke an asynchronous service with the specified payload. The method call return immediately. When the service return the response this will be used as argumento of the callback.
        /// </summary>
        /// <typeparam name="T">the payload type.</typeparam>
        /// <param name="serviceName">The name of the service.</param>
        /// <param name="payload">The payload used for invoke the service.</param>
        /// <param name="action">The callback invoked when the response arrive from the service.</param>
        public static void Invoke<T>(string serviceName, T payload, Action<Object> action)
        {
            if (!Host.IsStarted)
            {
                throw new HostNotStartedException();
            }

            string correlationId = Guid.NewGuid().ToString();

            pending.Add(correlationId, new PendingRequest() { Action = action });

            logger.Debug(String.Format("Correlation ID: {0}", correlationId));

            PayloadAsync payloadAsync = new PayloadAsync()
            {
                CorrelationId = correlationId.ToString(),
                Payload = payload,
                UrlCallback = ArgillaSettings.Current.Node.Return
            };

            string json = CustomJsonSerializer.Serialize(payloadAsync);

            Exception lastException = null;

            try
            {
                Endpoint endpoint = GetFreeEndpoint(serviceName);

                string jsonResult = HttpHelper.Post(endpoint.EndpointAsync, json);

                lastException = null;
            }
            catch
            {
                ResolveResponse resolveResponse = Resolve(serviceName);

                foreach (Endpoint endpoint in resolveResponse.Endpoints)
                {
                    try
                    {
                        string jsonResult = HttpHelper.Post(endpoint.EndpointAsync, json);

                        logger.Debug(String.Format("Json result: {0}", jsonResult));

                        lastException = null;

                        break;
                    }
                    catch (Exception e)
                    {
                        lastException = e;
                    }
                }
            }

            if (lastException != null)
            {
                logger.Error(lastException, lastException.Message);

                throw lastException;
            }
        }

        public static bool CanResolve(string serviceName)
        {
            try
            {
                ResolveResponse resolveResponse = Resolve(serviceName);

                if (resolveResponse != null && resolveResponse.Endpoints != null && resolveResponse.Endpoints.Count > 0)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region internal method

        internal static void SetErrorHandler(Func<Exception, OnClientErrorBehavior> errorHandler)
        {
            if (errorHandler == null)
            {
                return;
            }

            Client.errorHandler = errorHandler;
        }

        internal static void Unregister()
        {
            registerTimer.Enabled = false;

            InternalRegister(false);
        }

        internal static void Register()
        {
            InternalRegister(true);
        }

        internal static void Complete(string completeId, string json)
        {
            if (String.IsNullOrWhiteSpace(completeId) || String.IsNullOrWhiteSpace(json) || pending == null || !pending.ContainsKey(completeId))
            {
                Manage(new CannotCompleteException(completeId));
            }

            PendingRequest pendingRequest = pending[completeId];

            if (pendingRequest == null || pendingRequest.Action == null)
            {
                Manage(new CannotCompleteException(completeId));
            }

            pendingRequest.Action(json);
        }

        #endregion

        #region private methods

        private static Endpoint GetFreeEndpoint(string serviceName)
        {
            int counter = 0;

            lock (invocationCounter)
            {
                if (!invocationCounter.ContainsKey(serviceName))
                {
                    invocationCounter.Add(serviceName, 0);
                }

                counter = invocationCounter[serviceName];

                if (counter == int.MaxValue)
                {
                    counter = 0;
                }

                invocationCounter[serviceName] = ++counter;
            }

            logger.Debug("counter: " + counter);

            ResolveResponse resolveResponse = Resolve(serviceName);

            int endpointIndex = counter % resolveResponse.Endpoints.Count;

            logger.Debug("endpointIndex: " + endpointIndex);

            return resolveResponse.Endpoints[endpointIndex];
        }

        private static void OnRegisterTimer(object sender, ElapsedEventArgs e)
        {
            InternalRegister(true);
        }

        private static void InternalRegister(bool registration)
        {
            string resolverBaseAddress = ArgillaSettings.Current.Resolver.BaseAddress.EndsWith("/") ? ArgillaSettings.Current.Resolver.BaseAddress : ArgillaSettings.Current.Resolver.BaseAddress + "/";
            string resolverAddress = resolverBaseAddress + (registration ? "register" : "unregister");

            if (ArgillaSettings.Current.Node == null)
            {
                return;
            }

            RegisterRequest registerRequest = new RegisterRequest()
            {
                ServiceName = ArgillaSettings.Current.Node.ServiceName,
                EndpointSync = ArgillaSettings.Current.Node.EndpointSync,
                EndpointAsync = ArgillaSettings.Current.Node.EndpointAsync
            };

            string json = CustomJsonSerializer.Serialize(registerRequest);

            logger.Debug(String.Format("{0} request: {1}", (registration ? "Registration" : "Unregistration"), json));

            try
            {
                string result = HttpHelper.Post(resolverAddress, json);

                RegisterResponse registerResponse = CustomJsonSerializer.Deserialize<RegisterResponse>(result);

                if (registerResponse == null || !registerResponse.Success)
                {
                    Manage(new RegistrationException(String.Format("Cannot {1} service \"{0}\"", registerRequest.ServiceName, (registration ? "register" : "unregister"))));
                }
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    Manage(new RegistrationException(String.Format("Cannot {1} service \"{0}\"", registerRequest.ServiceName, (registration ? "register" : "unregister"))));
                }

                logger.Error(e, e.Message);
            }
        }

        private static ResolveResponse Resolve(string serviceName)
        {
            CachedResolveResponse cachedResolveResponse = resolveResponses.Where(x => x.ServiceName == serviceName).FirstOrDefault();

            if (cachedResolveResponse != null)
            {
                int lifetime = ArgillaSettings.Current.Resolver.CacheLifetime;

                if (lifetime == 0)
                {
                    lifetime = DEFAULT_RESOLVER_CACHE_LIFETIME;
                }

                if (DateTime.Now.Subtract(cachedResolveResponse.Cached).TotalMilliseconds > 1000 * lifetime)
                {
                    if (resolveResponses.Contains(cachedResolveResponse))
                    {
                        lock (LOCK)
                        {
                            if (resolveResponses.Contains(cachedResolveResponse))
                            {
                                logger.Debug(String.Format("Resolve response purged from cache for service {0}", serviceName));

                                resolveResponses.Remove(cachedResolveResponse);
                            }
                        }
                    }
                }
                else
                {
                    logger.Debug(String.Format("Resolve response found in cache for service {0}", serviceName));

                    return cachedResolveResponse as ResolveResponse;
                }
            }

            string resolverBaseAddress = ArgillaSettings.Current.Resolver.BaseAddress.EndsWith("/") ? ArgillaSettings.Current.Resolver.BaseAddress : ArgillaSettings.Current.Resolver.BaseAddress + "/";
            string resolverAddress = resolverBaseAddress + "resolve";

            logger.Info(String.Format("Resolve service {0} on {1}", serviceName, resolverAddress));

            try
            {
                string result = HttpHelper.Post(resolverAddress, CustomJsonSerializer.Serialize(new ResolveRequest() { ServiceName = serviceName }));

                logger.Debug(String.Format("Resove result: {0}", result));

                ResolveResponse resolveResponse = CustomJsonSerializer.Deserialize<ResolveResponse>(result);

                if (resolveResponse == null || resolveResponse.Endpoints == null || resolveResponse.Endpoints.Count < 1)
                {
                    Manage(new ServiceNotResolvedException(String.Format("Cannot resolve service {0}", serviceName)));
                }

                if (!resolveResponses.Contains(cachedResolveResponse))
                {
                    lock (LOCK)
                    {
                        if (!resolveResponses.Contains(cachedResolveResponse))
                        {
                            logger.Debug(string.Format("Resolve response added to cache for service {0}", serviceName));

                            resolveResponses.Add(new CachedResolveResponse(serviceName, resolveResponse));
                        }
                    }
                }

                return resolveResponse;
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    Manage(new ResolveException(resolverAddress));

                    return new ResolveResponse();
                }
                else
                {
                    throw;
                }
            }
        }

        private static void Manage(Exception e)
        {
            if (errorHandler != null)
            {
                OnClientErrorBehavior onErrorAction = errorHandler.Invoke(e);

                if (onErrorAction == OnClientErrorBehavior.Throw)
                {
                    throw e;
                }
            }
            else
            {
                throw e;
            }
        }

        #endregion
    }
}

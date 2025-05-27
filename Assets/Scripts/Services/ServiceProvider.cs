using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Debug = UnityEngine.Debug;

namespace Services
{
    public enum AccessType
    {
        Get,
        Put,
        GetSet,
        Remove,
    }

    /// <summary>
    /// It provides methods to access global services, where each service determine who can get and put data. It can store any type of data.
    /// </summary>
    public static class ServiceProvider
    {
        private static List<Service> _services;


        /// <summary>
        /// Tries to add a service if it does not exist. if it does, it gets it.
        /// </summary>
        /// <param name="serviceName">Service to alter</param>
        public static Service TryAddService([NotNull] string serviceName)
        {
            Service service;

            if (_services == null)
            {
                service = new Service(serviceName);
                _services = new List<Service> { service };
            }

            else if (!ServiceIsAvailable(serviceName))
            {
                service = new Service(serviceName);
                _services.Add(service);
            }
            else
            {
                service = GetService(serviceName);
            }

            return service;
        }

        /// <summary>
        /// Calls internal access modifier method.
        /// </summary>
        /// <param name="service">Service to alter</param>
        /// <param name="access">What access modifier is going to be applied</param>
        /// <param name="allowedCallers">Who is going to be called. Can be more than one</param>
        public static void ChangeAccess(Service service, AccessType access, params Type[] allowedCallers)
        {
            foreach (var caller in allowedCallers)
            {
                service?.ChangeAccess(access, caller);
            }
        }

        /// <summary>
        /// Method to remove a service. 
        /// <para> Note: it does not remove from any event </para>
        /// </summary>
        /// <param name="service">Service to alter</param>
        public static void RemoveService(Service service)
        {
            if (!_services.Remove(_services.Find(item => item.GetServiceName() == service.GetServiceName())))
            {
                Debug.LogWarning("No service found with that name. Try adding it first.");
            }
        }

        /// <summary>
        /// It allows the retrieval of information, without exposing the whole service.
        /// </summary>
        /// <param name="service">Service to alter</param>
        /// <param name="dataName">Name of the data to retrive</param>
        /// <param name="caller">Type of caller</param>
        /// <returns>Returns null if the service was not found, or if the data was not found. Otherwise returns Data</returns>
        [CanBeNull]
        public static object Get(Service service, string dataName, Type caller)
        {
            if (service == null) return null;

            if (!service.AccessReg.TryGetValue(caller, out var access))
            {
                Debug.LogWarning($"No service with caller {caller} registered.");
            }
            else
            {
                if (!access.Get)
                {
                    Debug.LogWarning(
                        $"The caller {caller} cannot access this service ({service.GetServiceName()}) as GET.");
                    return null;
                }

                if (service.Data.TryGetValue(dataName, out var data))
                {
                    return data;
                }

                Debug.LogWarning($"Could not found data with name {dataName}");
            }


            return null;
        }

        /// <summary>
        /// It allows loading data to service.
        /// </summary>
        /// <param name="service">Service to alter</param>
        /// <param name="dataName">Name of the data</param>
        /// <param name="caller">Type of caller of this function</param>
        /// <param name="value">Value of data</param>
        public static void Put(Service service, string dataName, Type caller, object value)
        {
            if (service == null) return;

            if (!service.AccessReg.TryGetValue(caller, out var access))
            {
                Debug.LogWarning($"No service with caller {caller} registered.");
            }
            else
            {
                if (access.Put)
                {
                    service.Data[dataName] = value;
                }
                else
                {
                    Debug.LogWarning(
                        $"The caller {caller} cannot access this service ({service.GetServiceName()}) as PUT.");
                }
            }
        }

        /// <summary>
        /// Get the service from the list. 
        /// </summary>
        /// <param name="serviceName">Service to alter</param>
        /// <returns>null if serviceName is empty or if is not found, otherwise returns the service</returns>
        [CanBeNull]
        public static Service GetService(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                Debug.LogError("Service name is null nor empty");
                return null;
            }

            var service = _services.Find(services => services.GetServiceName() == serviceName);

            if (service == null)
            {
                Debug.LogWarning("No service found with that name. Try adding it first.");
            }

            return service;
        }

        private static bool ServiceIsAvailable(string serviceName) =>
            _services != null &&
            _services.Exists((ctx => ctx.GetServiceName() == serviceName));


        public static void Dump()
        {
            string output = "[Services]\n" +
                            "# (Put, Get)\n";

            foreach (var service in _services)
            {
                output += $"\n[Services.{service.GetServiceName()}]\n" +
                          $"\n[Services.{service.GetServiceName()}.Access]\n";
                foreach (var access in service.AccessReg)
                {
                    output += $"{access.Key} =  ({access.Value.Put}, {access.Value.Get})\n";
                }

                output += $"\n[Services.{service.GetServiceName()}.Data]\n";
                foreach (var data in service.Data)
                {
                    output += $"{data.Key} = {data.Value}\n";
                }

                output += "\n";
            }

            File.WriteAllText("DUMP.toml", output);
        }
    }
}
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public enum AccessType
{
    Get,
    Set,
    GetSet,
    Remove,
}

/// <summary>
/// It stores and delivers data.
/// </summary>
internal class Service
{
    /// <summary>
    /// Access property of service
    /// </summary>
    public struct Access
    {
        public readonly bool Get;
        public readonly bool Put;

        public Access(bool get, bool put)
        {
            Get = get;
            Put = put;
        }
    }

    private readonly string _name;

    // The string is a description of what is being stored. 
    public Dictionary<string, object> Data; //the use of object is questionable

    // The Type should be the possible caller of a Get or Put
    public Dictionary<Type, Access> AccessReg;


    public Service(string name)
    {
        _name = name;
        Data = new Dictionary<string, object>();
        AccessReg = new Dictionary<Type, Access>();
    }

    /// <summary>
    /// Adds or removes access to get or put of a specific caller
    /// </summary>
    /// <param name="access">What access modifier is going to be applied</param>
    /// <param name="caller">What is going to use the data</param>
    public void ChangeAccess(AccessType access, Type caller)
    {
        switch (access)
        {
            case AccessType.Get:
                AccessReg[caller] = new Access(true, false);
                break;
            case AccessType.Set:
                AccessReg[caller] = new Access(false, true);
                break;
            case AccessType.GetSet:
                AccessReg[caller] = new Access(true, true);
                break;
            case AccessType.Remove:
                AccessReg[caller] = new Access(false, false);
                break;
            default:
                Debug.LogException(new ArgumentOutOfRangeException(nameof(access)));
                break;
        }
    }

    public string GetServiceName() => _name;
}

/// <summary>
/// It provides methods to access global services, where each service determine who can get and put data
/// </summary>
public static class ServiceProvider
{
    private static List<Service> _services;


    /// <summary>
    /// Tries to add a service if it does not exist. Fails silently.
    /// </summary>
    /// <param name="serviceName">Name of the service</param>
    public static void TryAddService([NotNull] string serviceName)
    {
        if (string.IsNullOrEmpty(serviceName))
        {
            Debug.LogError("serviceName cannot be null nor empty.");
            return;
        }

        ;

        if (_services == null)
        {
            _services = new List<Service> { new(serviceName) };
        }
        else if (!ServiceIsAvailable(serviceName))
        {
            _services.Add(new Service(serviceName));
        }
    }

    /// <summary>
    /// Calls internal access modifier method.
    /// </summary>
    /// <param name="serviceName">Name of the service</param>
    /// <param name="access">What access modifier is going to be applied</param>
    /// <param name="allowedCallers">Who is going to be called. Can be more than one</param>
    public static void ChangeAccess(string serviceName, AccessType access, params Type[] allowedCallers)
    {
        var service = GetService(serviceName);

        foreach (var caller in allowedCallers)
        {
            service?.ChangeAccess(access, caller);
        }
    }

    /// <summary>
    /// Method to remove a service. 
    /// <para> Note: it does not remove from any event </para>
    /// </summary>
    /// <param name="serviceName">Name of the service</param>
    public static void RemoveService(string serviceName)
    {
        if (ServiceIsAvailable(serviceName))
        {
            _services.Remove(_services.Find(service => service.GetServiceName() == serviceName));
        }
        else
        {
            Debug.LogWarning("No service found with that name. Try adding it first.");
        }
    }

    /// <summary>
    /// It allows the retrival of information, witout exposing the whole service.
    /// </summary>
    /// <param name="serviceName">Name of the service</param>
    /// <param name="dataName">Name of the data to retrive</param>
    /// <param name="caller">Type of caller</param>
    /// <returns>Returns null if the service was not found, or if the data was not found. Otherwise returns Data</returns>
    public static object Get(string serviceName, string dataName, Type caller)
    {
        var service = GetService(serviceName);

        if (service == null) return null;

        if (!service.AccessReg[caller].Get)
        {
            Debug.LogWarning("The caller cannot access this service as GET.");
            return null;
        }


        if (!service.Data.TryGetValue(dataName, out var data))
        {
            Debug.LogWarning($"The data {dataName} was not found.");
        }

        return data;
    }

    /// <summary>
    /// It allows loading data to service.
    /// </summary>
    /// <param name="serviceName">Name of the service</param>
    /// <param name="dataName">Name of the data</param>
    /// <param name="caller">Type of caller of this function</param>
    /// <param name="value">Value of data</param>
    public static void Put([NotNull] string serviceName, string dataName, Type caller, object value)
    {
        var service = GetService(serviceName);

        if (service == null) return;

        if (!service.AccessReg[caller].Put)
        {
            Debug.LogWarning("The caller cannot access this service as PUT.");
        }
        else
        {
            service.Data[dataName] = value;
        }
    }

    private static bool ServiceIsAvailable(string serviceName) =>
        _services != null &&
        _services.Exists((ctx => ctx.GetServiceName() == serviceName));

    /// <summary>
    /// Get the service from the list. 
    /// </summary>
    /// <param name="serviceName">Name of the service</param>
    /// <returns>null if serviceName is empty or if is not found, otherwise returns the service</returns>
    [CanBeNull]
    private static Service GetService(string serviceName)
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
}
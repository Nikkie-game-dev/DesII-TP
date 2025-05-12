using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public static class ServiceProvider
{
    private static List<Service<Type>> _services;

    private class Service<T>
    {
        private readonly string _name;
        public Dictionary<string, T> Data;
        public Type[] EnabledGet { get; }
        public Type[] EnabledPut { get; }

        public Service(string name, Type[] enabledGet, Type[] enabledPut)
        {
            _name = name;
            EnabledGet = enabledGet;
            EnabledPut = enabledPut;
        }

        public string GetServiceName() => _name;
    }

    public static void AddService([NotNull] string name, Type[] enabledGet, Type[] enabledPut)
    {
        if (ServiceIsAvailable(name))
        {
            Debug.LogWarning("Service already added");
        }
        else
        {
            _services.Add(new Service<Type>(name, enabledGet, enabledPut));
        }
    }
    
    public static void TryAddService([NotNull] string name, Type[] enabledGet, Type[] enabledPut)
    {
        if (!ServiceIsAvailable(name))
        {
            _services.Add(new Service<Type>(name, enabledGet, enabledPut));
        }
    }

    public static void RemoveService([NotNull] string name)
    {
        if (ServiceIsAvailable(name))
        {
            _services.Remove(_services.Find(service => service.GetServiceName() == name));
        }
        else
        {
            Debug.LogWarning("No service found with that name. Try adding it first.");
        }
    }

    public static Type GetData([NotNull] string server, string name, Type caller)
    {
        var service = GetService(server, name);
        
        if (service == null)
        {
            Debug.LogWarning("No service found with that name. Try adding it first.");
        }
        else if (!service.EnabledGet.Contains(caller))
        {
            Debug.LogWarning("The caller cannot access this service as GET.");
        }
        else
        {
            return service.Data.GetValueOrDefault(name);
        }
        
        return null;
    }

    public static void SetData([NotNull] string server, string name, Type caller, Type value)
    {
        var service = GetService(server, name);
        
        if (service == null)
        {
            Debug.LogWarning("No service found with that name. Try adding it first.");
        }
        else if (!service.EnabledPut.Contains(caller))
        {
            Debug.LogWarning("The caller cannot access this service as Put.");
        }
        else
        {
            service.Data[name] = value;
        }
    }

    private static bool ServiceIsAvailable(string service) =>
        _services.Exists(services => services.GetServiceName() == service);

    private static Service<Type> GetService([NotNull] string server, string name) => 
        _services.Find(services => services.GetServiceName() == server);
}
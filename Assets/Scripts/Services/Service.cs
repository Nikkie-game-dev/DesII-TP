using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// It stores and delivers data.
    /// </summary>
    public class Service
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
                case AccessType.Put:
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
}
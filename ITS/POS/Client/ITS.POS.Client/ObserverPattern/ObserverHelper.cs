using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using System.Collections.Concurrent;
using NLog;
namespace ITS.POS.Client.ObserverPattern
{
    public static class ObserverHelper
    {
        private static long counter = 0;

        private static ConcurrentDictionary<Type, List<MethodInfo>> _cache = new ConcurrentDictionary<Type, List<MethodInfo>>();

        /// <summary>
        /// Calls the Update() method of all observers that match the type of parameters
        /// </summary>
        /// <param name="observers"></param>
        /// <param name="parameters"></param>
        public static void NotifyObservers(ThreadSafeList<IObserver> observers, ObserverParams parameters, Logger logFile)
        {
            counter++;
            logFile.Trace(DateTime.Now.ToString("hh:mm:ss.fff") + " - NotifyObservers(" + counter + ") started");
            if (observers != null)
            {
                Type type = parameters.GetType();

                foreach (IObserver observer in observers.Where(x => x.GetParamsTypes().Contains(type)))
                {
                    List<MethodInfo> methods;
                    Type key = observer.GetType();
                    if (_cache.ContainsKey(key))
                    {
                        methods = _cache[key];
                    }
                    else
                    {
                        methods = observer.GetType().GetMethods().Where(x => x.Name == "Update" && x.GetParameters().Length > 0).ToList();
                        _cache.TryAdd(key, methods);
                    }
                    foreach (MethodInfo method in methods)
                    {
                        Type paramType = method.GetParameters()[0].ParameterType;
                        if (paramType.Equals(type))
                        {
                            logFile.Trace(DateTime.Now.ToString("hh:mm:ss.fff") + " - NotifyObservers(" + counter + ") Invoke");
                            object castedParameters = Convert.ChangeType(parameters, paramType);
                            method.Invoke(observer, new object[] { castedParameters });
                        }
                    }
                }
            }
        }
    }
}

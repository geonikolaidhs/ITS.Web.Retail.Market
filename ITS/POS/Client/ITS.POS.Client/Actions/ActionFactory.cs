using System;
using System.Collections.Generic;
using System.Reflection;
using ITS.POS.Client.Actions;
using ITS.POS.Model.Settings;
using System.Linq;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Creates an instance for each one of all the available actions
    /// </summary>
    public static class ActionFactory
    {
        public static List<IAction> CreateActions(IPosKernel kernel)
        {
            List<IAction> listOfActions = new List<IAction>();
            IEnumerable<Type> types = typeof(Action).Assembly.GetTypes().Where(g => g.IsSubclassOf(typeof(Action)));
            foreach (Type actionType in types)
            {
                object action = Activator.CreateInstance(actionType, kernel);
                if (action is IAction)
                {
                    listOfActions.Add((IAction)action);
                }
            }
            return listOfActions;
        }
    }
}

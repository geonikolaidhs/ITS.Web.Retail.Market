using ITS.POS.Client.Actions;
using ITS.POS.Client.Actions.Permission;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Provides the actions.
    /// </summary>
    public class ActionManager : IActionManager
    {
        public List<ActionPermission> ActionPermissions { get; private set; }
        public List<CustomActionCode> CustomActionCodes { get; private set; }

        public ActionManager(IEnumerable<IAction> actions, string actionLevelsXmlPath, string customActionCodesXmlPath)
        {
            Actions = new List<IAction>();
            ActionEActionDictionary = new ConcurrentDictionary<eActions, IAction>();
            AddActions(actions);
            LoadActionPermissions(actionLevelsXmlPath);
            LoadCustomActionCodes(customActionCodesXmlPath);
        }


        private void LoadActionPermissions(string actionLevelsXmlPath )
        {
            ActionPermissions = new List<ActionPermission>();
            if (File.Exists(actionLevelsXmlPath))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(actionLevelsXmlPath);
                XmlNodeList actionLevels = xml.GetElementsByTagName("ActionLevel");
                foreach (XmlNode actionLevel in actionLevels)
                {
                    if (actionLevel["Action"] != null && string.IsNullOrWhiteSpace(actionLevel["Action"].InnerText) == false)
                    {
                        if (actionLevel["Level"] != null && string.IsNullOrWhiteSpace(actionLevel["Level"].InnerText) == false)
                        {
                            try
                            {
                                eActions ac = (eActions)Enum.Parse(typeof(eActions), actionLevel["Action"].InnerText.Trim());
                                eKeyStatus ks = (eKeyStatus)Enum.Parse(typeof(eKeyStatus), actionLevel["Level"].InnerText.Trim());
                                ActionPermissions.Add(new ActionPermission() { ActionCode = ac, KeyStatus = ks });
                            }
                            catch (Exception ex)
                            {
                                string errorMessage = ex.GetFullMessage();
                            }
                        }
                    }
                }
            }
        }

        private void LoadCustomActionCodes(string customActionCodesXmlPath)
        {
            CustomActionCodes = new List<CustomActionCode>();
            if (File.Exists(customActionCodesXmlPath))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(customActionCodesXmlPath);
                XmlNodeList customActionCodes = xml.GetElementsByTagName("CustomActionCode");
                foreach (XmlNode customActionCode in customActionCodes)
                {
                    if (customActionCode["Code"] != null && string.IsNullOrWhiteSpace(customActionCode["Code"].InnerText) == false &&
                      (customActionCode["Action"] != null && string.IsNullOrWhiteSpace(customActionCode["Action"].InnerText) == false))
                    {
                        try
                        {
                            string customCode = customActionCode["Code"].InnerText.Trim();
                            eActions action = (eActions)Enum.Parse(typeof(eActions), customActionCode["Action"].InnerText.Trim());
                            CustomActionCodes.Add(new CustomActionCode() { Code = customCode, Action = action });
                        }
                        catch //(Exception ex)
                        {
                        }
                    }
                }
            }
        }

        ConcurrentDictionary<eActions, IAction> ActionEActionDictionary;

        /// <summary>
        /// Gets an action by code.
        /// </summary>
        /// <param name="actionCode"></param>
        /// <returns></returns>
        public IAction GetAction(eActions actionCode)
        {
            if (ActionEActionDictionary.ContainsKey(actionCode))
            {
                return ActionEActionDictionary[actionCode];
            }
            return null;
        }

        /// <summary>
        /// Determines if an action is external or internal.
        /// External actions are invoked by the user directly, while internal ones are not,
        /// </summary>
        /// <param name="actionCode"></param>
        /// <returns></returns>
        public bool IsExternalAction(eActions actionCode)
        {
            return GetExternalActionCodes().Contains(actionCode);
        }

        /// <summary>
        /// Gets the action codes of all the external actions,
        /// </summary>
        /// <returns></returns>
        public List<eActions> GetExternalActionCodes()
        {
            List<eActions> actioncodes = Actions.Where(x => x.IsInternal == false).Select(g => g.ActionCode).OrderBy(g => g).ToList();
            actioncodes.Add(eActions.NONE);
            return actioncodes;
        }

        private List<IAction> Actions { get; set; }

        private void AddAction(IAction action)
        {
            Actions.Add(action);
            ActionEActionDictionary.TryAdd(action.ActionCode, action);
        }

        private void AddActions(IEnumerable<IAction> actions)
        {
            foreach (IAction action in actions)
            {
                AddAction(action);
            }
        }

    }
}

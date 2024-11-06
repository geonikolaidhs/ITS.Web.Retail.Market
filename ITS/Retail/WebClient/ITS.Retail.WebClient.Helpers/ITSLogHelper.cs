using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public static class ITSLogHelper
    {
        /// <summary>
        /// Saves an error message to ApplicationLog
        /// </summary>
        /// <param name="errorMessage">The error information</param>
        public static void Log(LogErrorMessage errorMessage)
        {
            try
            {
                //TODO
                using (UnitOfWork logUow = XpoHelper.GetNewUnitOfWork())
                {
                    ApplicationLog logEvent = new ApplicationLog(logUow);
                    logEvent.Result = errorMessage.Result;
                    logEvent.Controller = errorMessage.Controller;
                    logEvent.Action = errorMessage.Action;
                    logEvent.Error = errorMessage.Error;
                    logEvent.Save();
                    XpoHelper.CommitChanges(logUow);
                }
            }
            catch (Exception logException)
            {
                string exceptionMessage = logException.GetFullMessage();
            }
        }
    }
}

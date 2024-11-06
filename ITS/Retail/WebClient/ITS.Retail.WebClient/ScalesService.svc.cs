using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace ITS.Retail.WebClient
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ScalesService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ScalesService.svc or ScalesService.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ScalesService : IScalesService
    {
        public string ExportChanges()
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                string message;
                POSHelper.ReloadScales(uow, out message, StoreControllerAppiSettings.CurrentStore);
                if (string.IsNullOrEmpty(message) == false)
                {
                    MvcApplication.WRMLogModule.Log(message);
                }
                else
                {
                    message = ResourcesLib.Resources.SuccefullyCompleted;
                }
                return message;
            }
        }
    }
}

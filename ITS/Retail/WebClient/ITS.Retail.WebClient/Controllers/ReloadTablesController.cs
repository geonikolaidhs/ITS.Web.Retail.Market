//#if _RETAIL_STORECONTROLLER
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class ReloadTablesController : BaseController
    {
        //
        // GET: /ReloadTables/


        [Security(ReturnsPartial = false)]
        public ActionResult Index()
        {

            ViewData["EntityNames"] = GetEntityNames();
            return View();
        }

        [Security(ReturnsPartial = false)]
        public ActionResult Submit()
        {
            ViewData["EntityNames"] = GetEntityNames();
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                foreach (string parameter in Request.Params.AllKeys.Where(x => x.EndsWith("_EntityCheckBox")))
                {
                    if (Request[parameter] == "C")   //checked
                    {
                        string entityName = parameter.Replace("_EntityCheckBox", "");
                        TableVersion version = uow.FindObject<TableVersion>(new BinaryOperator("EntityName", typeof(Item).FullName.Replace("Item", entityName)));

                        if (version != null)
                        {
                            version.Number = 0;
                            version.ForceReload = true;
                            version.Save();

                        }
                    }
                }
                XpoHelper.CommitTransaction(uow);
            }
            return View("Index");
        }


        private Dictionary<string,string> GetEntityNames()
        {
            Dictionary<string, string> localizedEntityNames = new Dictionary<string, string>();
            var list = typeof(Item).Assembly.GetTypes().Where(x => (x.GetCustomAttributes(typeof(UpdaterAttribute), false).FirstOrDefault() != null)
                            && ((x.GetCustomAttributes(typeof(UpdaterAttribute), false).FirstOrDefault() as UpdaterAttribute).Permissions.HasFlag(eUpdateDirection.MASTER_TO_STORECONTROLLER)));
            foreach (Type type in list)
            {
                if (type != null)
                {
                    localizedEntityNames.Add(type.Name, type.ToLocalizedString());
                }
            }
            ViewData["EntityNames"] = localizedEntityNames;
            return localizedEntityNames;
        }
    }
}

//#endif
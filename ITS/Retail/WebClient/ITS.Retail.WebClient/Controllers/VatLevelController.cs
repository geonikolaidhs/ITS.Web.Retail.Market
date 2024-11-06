using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using DevExpress.Data.Filtering;
using System.Reflection;
using ITS.Retail.WebClient.Providers;

namespace ITS.Retail.WebClient.Controllers
{
    public class VatLevelController : BaseObjController<VatLevel>
    {
        private static readonly Dictionary<PropertyInfo, string> propMapping = new Dictionary<PropertyInfo, string>() 
        { 
            { typeof(VatLevel).GetProperty("Code"), "VatLevelCode" }, 
            { typeof(VatLevel).GetProperty("Description"), "VatLevelDescription" } 
        };

        protected override Dictionary<PropertyInfo, string> PropertyMapping
        {
            get
            {
                return propMapping;
            }
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            this.ToolbarOptions.ForceVisible = false;
            FillLookupComboBoxes();
            ViewData["VatCategory"] = GetList<VatCategory>(XpoSession).AsEnumerable();
            ViewData["VatFactor"] = GetList<VatFactor>(XpoSession).AsEnumerable();
            return View("Index", GetList<VatLevel>(XpoSession).AsEnumerable());
        }

        public ActionResult VatCategoryGrid()
        {
            if (Request["DXCallbackArgument"].Contains("STARTEDIT") && UserCanEditRequest(typeof(VatCategory)) == false)
            {
                Session["Error"] = Resources.YouCannotEditThisElement;
                return null;
            }
            ViewData["VatCategory"] = GetList<VatCategory>(XpoSession).AsEnumerable();
            return PartialView("VatCategoryGrid");

        }

        [HttpPost]
        public ActionResult VatCategoryInsert([ModelBinder(typeof(RetailModelBinder))] VatCategory ct)
        {
            if (!TableCanInsert)
            {
                return null;
            }

            bool isDefault = false;

            if (Boolean.TryParse(Request["IsDefault"], out isDefault))
            {
                ct.IsDefault = isDefault;
            }

            AddModelErrors(ct);

            if(HasIsDefaultDuplicates(ct))
            {
                ModelState.AddModelError("IsDefault", Resources.DefaultAllreadyExists);
                Session["Error"] += Resources.DefaultAllreadyExists;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (SaveT(ct))
                    {
                        Session["Notice"] = Resources.SavedSuccesfully;
                    }
                    else
                    {
                        if (Session["Error"].ToString().Contains("Is Default already exist"))
                        {
                            Session["Error"] = Resources.DefaultAllreadyExists;
                        }
                        else
                        {
                            Session["Error"] = Resources.CodeAlreadyExists;
                        }
                    }

                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                ViewBag.CurrentItem = ct;
            }
            XpoSession.ReloadChangedObjects();
            ViewData["VatCategory"] = GetList<VatCategory>(XpoSession).AsEnumerable();
            return PartialView("VatCategoryGrid");
        }

        [HttpPost]
        public ActionResult VatCategoryUpdate([ModelBinder(typeof(RetailModelBinder))] VatCategory ct)
        {
            if (!TableCanUpdate) return null;

            AddModelErrors(ct);
            
            if (ModelState.IsValid)
            {
                try
                {
                    if (SaveT(ct))
                    {
                        Session["Notice"] = Resources.SavedSuccesfully;
                    }
                    else
                    {
                        if (Session["Error"].ToString().Contains("Is Default already exist"))
                        {
                            Session["Error"] = Resources.DefaultAllreadyExists;
                        }
                        else
                        {
                            Session["Error"] = Resources.CodeAlreadyExists;
                        }
                    }
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                ViewBag.CurrentItem = ct;
            }


            XpoSession.ReloadChangedObjects();
            ViewData["VatCategory"] = GetList<VatCategory>(XpoSession);
            return PartialView("VatCategoryGrid");
        }

        [HttpPost]
        public ActionResult VatCategoryDelete([ModelBinder(typeof(RetailModelBinder))] VatCategory ct)
        {
            if (!TableCanDelete) return null;
            try
            {
                DeleteT(ct);
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }
            
            ViewData["VatCategory"] = GetList<VatCategory>(XpoSession);
            return PartialView("VatCategoryGrid");
        }


        public ActionResult VatFactorGrid()
        {
            FillLookupComboBoxes();
            ViewData["VatFactor"] = GetList<VatFactor>(XpoSession);
            return PartialView("VatFactorGrid");
        }

        [HttpPost]
        public ActionResult VatFactorInlineEditingAddNewPartial([ModelBinder(typeof(RetailModelBinder))] VatFactor ct)
        {
            if (!TableCanInsert)
            {
                return null;
            }
            UpdateLookupObjectsVatFactor(ct);
            
            if (ct.VatLevel == null)
            {
                ModelState.AddModelError("VatLevel", Resources.VatLevelIsEmpty);
                Session["Error"] += Resources.VatLevelIsEmpty;
            }
            if (ct.VatCategory == null)
            {
                ModelState.AddModelError("VatCategory", Resources.VatCategoryIsEmpty);
                Session["Error"] += Resources.VatCategoryIsEmpty;
            }

            ct.Description = ct.Code;
            AddModelErrors(ct);

            if (ModelState.IsValid)
            {
                try
                {
                    ct.Factor = ct.Factor / 100;

                    if (SaveT(ct))
                    {
                        Session["Notice"] = Resources.SavedSuccesfully;
                    }
                    else
                    {
                        if (Session["Error"].ToString().Contains("Is Default already exist"))
                        {
                            Session["Error"] = Resources.DefaultAllreadyExists;
                        }
                        else
                        {
                            Session["Error"] = Resources.CodeAlreadyExists;
                        }
                    }
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                ViewBag.CurrentItem = ct;
            }
            FillLookupComboBoxes();

            ViewData["VatFactor"] = GetList<VatFactor>(XpoSession);
            return PartialView("VatFactorGrid");
        }
        [HttpPost]
        public ActionResult VatFactorInlineEditingUpdatePartial([ModelBinder(typeof(RetailModelBinder))] VatFactor ct)
        {
            if (!TableCanUpdate)
            {
                return null;
            }

            UpdateLookupObjectsVatFactor(ct);


            if (ct.VatLevel == null)
            {
                ModelState.AddModelError("VatLevel", Resources.VatLevelIsEmpty);
                Session["Error"] += Resources.VatLevelIsEmpty;
            }
            if (ct.VatCategory == null)
            {
                ModelState.AddModelError("VatCategory", Resources.VatCategoryIsEmpty);
                Session["Error"] += Resources.VatCategoryIsEmpty;
            }

            if (String.IsNullOrWhiteSpace(ct.Code))
            {
                Session["Error"] = Resources.CodeIsEmpty;
                ModelState.AddModelError(PropertyMapping.ContainsKey(ct.GetType().GetProperty("Code")) ? PropertyMapping[ct.GetType().GetProperty("Code")] : "Code", Resources.CodeIsEmpty);
            }
            if (HasDuplicate(ct))
            {
                ModelState.AddModelError(PropertyMapping.ContainsKey(ct.GetType().GetProperty("Code")) ? PropertyMapping[ct.GetType().GetProperty("Code")] : "Code", Resources.CodeAlreadyExists);
            }
            if (HasIsDefaultDuplicates(ct))
            {
                ModelState.AddModelError(PropertyMapping.ContainsKey(ct.GetType().GetProperty("IsDefault")) ? PropertyMapping[ct.GetType().GetProperty("IsDefault")] : "IsDefault", Resources.DefaultAllreadyExists);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ct.Factor = ct.Factor / 100;

                    if (SaveT(ct))
                    {
                        Session["Notice"] = Resources.SavedSuccesfully;
                    }
                    else
                    {
                        if (Session["Error"].ToString().Contains("Is Default already exist"))
                        {
                            Session["Error"] = Resources.DefaultAllreadyExists;
                        }
                        else
                        {
                            Session["Error"] = Resources.CodeAlreadyExists;
                        }
                    }
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;

                }
            }
            else
            {
                ViewBag.CurrentItem = ct;
            }
            FillLookupComboBoxes();
            ViewData["VatFactor"] = GetList<VatFactor>(XpoSession);
            return PartialView("VatFactorGrid");
        }

        [HttpPost]
        public ActionResult VatFactorInlineEditingDeletePartial([ModelBinder(typeof(RetailModelBinder))] VatFactor ct)
        {
            if (!TableCanDelete) return null;
            try
            {
                DeleteT(ct);
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }
            FillLookupComboBoxes();
            ViewData["VatFactor"] = GetList<VatFactor>(XpoSession);
            return PartialView("VatFactorGrid");
        }


        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            ViewBag.VatLevelComboBox = GetList<VatLevel>(XpoSession);
            ViewBag.VatCategoryComboBox = GetList<VatCategory>(XpoSession);
        }

        protected void UpdateLookupObjectsVatFactor(VatFactor a)
        {
            a.VatLevel = GetObjectByArgument<VatLevel>(a.Session, "VatLevelCb_VI") as VatLevel;
            a.VatCategory = GetObjectByArgument<VatCategory>(a.Session, "VatCategoryCb_VI") as VatCategory;            
        }
    }
}

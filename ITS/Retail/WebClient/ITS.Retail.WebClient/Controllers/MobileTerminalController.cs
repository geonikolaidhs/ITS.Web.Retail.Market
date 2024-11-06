#if _RETAIL_DUAL || _RETAIL_STORECONTROLLER

using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.ViewModel;
using ITS.Retail.Common.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable, Display(ShowSettings = true)]
    public class MobileTerminalController : BaseObjController<MobileTerminal>
    {
        public ActionResult Index()
        {
            if(CurrentOwner == null)
            {
                Session["Error"] = Resources.SelectCompany;
                return View();
            }

            if (CurrentStore == null)
            {
                Session["Error"] = Resources.PleaseSelectAStore;
            }

            this.ToolbarOptions.ForceVisible = true;
            this.ToolbarOptions.ExportButton.Visible = false;
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.OptionsButton.Visible = false;
            this.ToolbarOptions.ViewButton.OnClick = "Component.ShowPopup";
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";

            this.CustomJSProperties.AddJSProperty("editAction", "Edit");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "MobileID");
            this.CustomJSProperties.AddJSProperty("gridName", "grdMobileTerminals");

            ViewBag.Title = Resources.MobileTerminals;
            return View(GetList<MobileTerminal>(XpoSession));
        }

        public override ActionResult LoadViewPopup()
        {
            base.LoadViewPopup();

            MobileTerminalViewModel mobileTerminalViewModel = new MobileTerminalViewModel();
            if (ViewData["ID"] != null)
            {
                Guid mobileTerminalGuid;
                if (Guid.TryParse(ViewData["ID"].ToString(), out mobileTerminalGuid))
                {
                    MobileTerminal mobileTerminal = XpoSession.GetObjectByKey<MobileTerminal>(mobileTerminalGuid);
                    if (mobileTerminal == null)
                    {
                        Session["Error"] = Resources.AnErrorOccurred;
                        return Json(new { });
                    }
                    mobileTerminalViewModel.LoadPersistent<MobileTerminal>(mobileTerminal);
                }
            }
            return PartialView("LoadViewPopup",mobileTerminalViewModel);
        }


        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.MobileTerminal;

            return PartialView("LoadEditPopup");
        }

        public override ActionResult PopupEditCallbackPanel()
        {
            base.PopupEditCallbackPanel();

            return PartialView();

        }

        public ActionResult Edit(Guid Oid)
        {
            ViewBag.Title = Resources.MobileTerminal;

            MobileTerminal mobileTerminal = XpoSession.GetObjectByKey<MobileTerminal>(Oid);
            if (mobileTerminal == null)
            {
                mobileTerminal = new MobileTerminal(XpoSession);
            }

            MobileTerminalViewModel mobileTerminalViewModel = new MobileTerminalViewModel();
            mobileTerminalViewModel.LoadPersistent(mobileTerminal);

            return PartialView("Edit", mobileTerminalViewModel);
        }

        [HttpPost]
        public ActionResult Save()
        {
            try
            {
                Guid mobileTerminalOid = Guid.Empty;
                if (Guid.TryParse(Request["Oid"], out mobileTerminalOid))
                {
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        MobileTerminal mobileTerminal = uow.GetObjectByKey<MobileTerminal>(mobileTerminalOid);
                        if (mobileTerminal == null)
                        {
                            mobileTerminal = new MobileTerminal(uow);
                        }
                        MobileTerminalViewModel mobileTerminalViewModel = new MobileTerminalViewModel();
                        TryUpdateModel(mobileTerminalViewModel);
                        mobileTerminalViewModel.Store = CurrentStore.Oid;
                        mobileTerminalViewModel.Persist(mobileTerminal);

                        CriteriaOperator mobileTerminalCriteria = CriteriaOperator.And(
                                                                         CriteriaOperator.Or(new BinaryOperator("ID", mobileTerminal.ID),
                                                                                             new BinaryOperator("IPAddress", mobileTerminal.IPAddress)         
                                                                         ),
                                                                         new BinaryOperator("Store", mobileTerminal.Store.Oid),
                                                                         new BinaryOperator("Oid", mobileTerminal.Oid, BinaryOperatorType.NotEqual)
                                                                        );
                        mobileTerminalCriteria = ApplyOwnerCriteria(mobileTerminalCriteria, typeof(MobileTerminal), EffectiveOwner);
                        int numberOfExistingMobileTerminals = (int)uow.Evaluate(typeof(MobileTerminal), CriteriaOperator.Parse("Count"), mobileTerminalCriteria);
                        if ( numberOfExistingMobileTerminals > 0 )
                        {
                            Session["Error"] = String.Format(Resources.FoundMatchingMobileTerminal, numberOfExistingMobileTerminals);
                            return Json(new { });
                        }

                        mobileTerminal.Save();
                        int numberOfActiveTerminals = GetList<MobileTerminal>(uow, filter: new BinaryOperator("IsActive", true), behavior: PersistentCriteriaEvaluationBehavior.InTransaction).Count;
                        int maxPeripheralUsers = MvcApplication.LicenseStatusViewModel.MaxPeripheralUsers;
                        if (maxPeripheralUsers < numberOfActiveTerminals)
                        {
                            Session["Error"] = String.Format(Resources.MaximumAllowedPeripheralUsersAreAndCounted, maxPeripheralUsers, numberOfActiveTerminals);
                            return Json(new { });
                        }
                        XpoHelper.CommitTransaction(uow);
                    }
                }
            }
            catch(Exception exception)
            {
                Session["Error"] = exception.Message;
            }
            return Json(new { });
        }
    }
}
#endif
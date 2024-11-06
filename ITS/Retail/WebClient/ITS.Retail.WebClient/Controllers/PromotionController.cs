using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Web;
using DevExpress.Web.ASPxTreeList;
using DevExpress.Web.Mvc;
using System.Web.UI;
using ITS.Retail.Common.ViewModel;

namespace ITS.Retail.WebClient.Controllers
{
    public class PromotionController : BaseObjController<Promotion>
    {
        UnitOfWork uow;

        protected void GenerateUnitOfWork()
        {

            if (Session["uow"] == null)
            {
                uow = XpoHelper.GetNewUnitOfWork();
                Session["uow"] = uow;
            }
            else
            {
                uow = (UnitOfWork)Session["uow"];
            }
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = false)]
        public ActionResult Index()
        {
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.FilterButton.Visible = true;
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.OptionsButton.Visible = false;
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.NewButton.OnClick = "AddNewPromotion";
            this.ToolbarOptions.EditButton.OnClick = "EditPromotion";
            this.ToolbarOptions.ViewButton.Visible = false;           


            this.CustomJSProperties.AddJSProperty("gridName", "grdPromotions");
            GenerateUnitOfWork();

            CriteriaOperator filter = null;
            return View("Index", GetList<Promotion>(uow, filter).AsEnumerable<Promotion>());
        }

        public override ActionResult Grid()
        {
            return base.Grid();
        }

        [Security(ReturnsPartial = false)]
        public ActionResult Edit(string PromotionGuid)
        {
            this.ToolbarOptions.ForceVisible = false;
            GenerateUnitOfWork();

            Guid promotionGuid = (PromotionGuid == null || PromotionGuid == "null" || PromotionGuid == "-1") ? Guid.Empty : Guid.Parse(PromotionGuid);

            ViewData["EditMode"] = true;

            if (promotionGuid == Guid.Empty && TableCanInsert == false)
            {
                return new RedirectResult("~/Login");
            }
            else if (promotionGuid != Guid.Empty && TableCanUpdate == false)
            {
                return new RedirectResult("~/Login");
            }

            Promotion promotion;
            if (Session["UnsavedPromotion"] == null)
            {
                if (promotionGuid != Guid.Empty)
                {
                    ViewBag.Mode = Resources.EditDevice;
                    promotion = uow.GetObjectByKey<Promotion>(promotionGuid);
                    Session["IsNewPromotion"] = false;
                }
                else
                {
                    ViewBag.Mode = Resources.NewDevice;
                    promotion = new Promotion(uow);
                    Session["IsNewPromotion"] = true;
                }
                Session["IsRefreshed"] = false;
            }
            else
            {
                if (promotionGuid != Guid.Empty && (Session["UnsavedPromotion"] as Promotion).Oid == promotionGuid)
                {
                    Session["IsRefreshed"] = true;
                    promotion = (Promotion)Session["UnsavedPromotion"];
                }
                else if (promotionGuid == Guid.Empty)
                {
                    Session["IsRefreshed"] = false;
                    promotion = (Promotion)Session["UnsavedPromotion"];
                }
                else
                {
                    uow.ReloadChangedObjects();
                    uow.RollbackTransaction();
                    Session["IsRefreshed"] = false;
                    promotion = uow.GetObjectByKey<Promotion>(promotionGuid);
                }
            }

            FillLookupComboBoxes();
            ViewData["PromotionGuid"] = promotion.Oid.ToString();
            Session["UnsavedPromotion"] = promotion;

            return View("Edit", promotion);
        }

        [Security(ReturnsPartial = false)]
        public ActionResult Save()
        {

            GenerateUnitOfWork();
            Guid promotionGuid = Guid.Empty;

            bool correctPromotionGuid = Request["PromotionGuid"] != null && Guid.TryParse(Request["PromotionGuid"].ToString(), out promotionGuid);
            if (correctPromotionGuid)
            {
                Promotion promotion = (Session["UnsavedPromotion"] as Promotion);
                if (promotion != null)
                {
                    promotion.Code = Request["Code"];
                    promotion.Description = Request["Description"];

                    DateTime startDate;
                    DateTime.TryParse(Request["StartDate"], out startDate);
                    promotion.StartDate = startDate;

                    DateTime endDate;
                    DateTime.TryParse(Request["EndDate"], out endDate);
                    promotion.EndDate = endDate;

                    bool isActive;
                    Boolean.TryParse(Request["IsActive"], out isActive);
                    promotion.IsActive = isActive;

                    bool testMode;
                    Boolean.TryParse(Request["TestMode"], out testMode);
                    promotion.TestMode = testMode;

                }
            }
            return View();

        }

        [Security(ReturnsPartial = false)]
        public ActionResult CancelEdit()
        {
            if (!Boolean.Parse(Session["IsRefreshed"].ToString()))
            {
                if (Session["uow"] != null)
                {
                    ((UnitOfWork)Session["uow"]).ReloadChangedObjects();
                    ((UnitOfWork)Session["uow"]).RollbackTransaction();
                    ((UnitOfWork)Session["uow"]).Dispose();
                    Session["uow"] = null;
                }
                Session["IsRefreshed"] = null;
                Session["IsNewPromotion"] = null;
                Session["UnsavedPromotion"] = null;
            }
            return null;
        }

        protected override Wizard CreateWizard(List<string> arguments)
        {
            WizardStep firstStep = new WizardStep("WizardStepPromotionHeader");
            firstStep.Options.BodyPartialView = "WizardStepPromotionHeader";
            firstStep.Options.StepHeaderText = Resources.Promotion;

            //WizardStep pcStep = new WizardStep("WizardStepPriceCatalogs");
            //pcStep.Options.BodyPartialView = "WizardStepPriceCatalogs";
            //pcStep.Options.PreviousButton.OnClick = "PriceCatalogTreeList_Previous";
            //pcStep.Options.NextButton.OnClick = "PriceCatalogTreeList_Next";
            WizardStep pcStep = new WizardStep("WizardStepPriceCatalogPolicies");
            pcStep.Options.BodyPartialView = "WizardStepPriceCatalogPolicies";
            pcStep.Options.PreviousButton.OnClick = "PriceCatalogPolicyList_Previous";
            pcStep.Options.NextButton.OnClick = "PriceCatalogPolicyList_Next";
            pcStep.Options.StepHeaderText = Resources.PriceCatalogs;
            pcStep.PreviousStep = firstStep;

            WizardStep secondStep = new WizardStep("WizardStepPromotionApplicationRules");
            secondStep.Options.BodyPartialView = "WizardStepPromotionApplicationRules";
            secondStep.Options.StepHeaderText = Resources.PromotionApplicationRules;
            secondStep.PreviousStep = pcStep;

            WizardStep thirdStep = new WizardStep("WizardStepPromotionExecutions");            
            thirdStep.Options.BodyPartialView = "WizardStepPromotionExecutions";
            thirdStep.Options.StepHeaderText = Resources.PromotionExecutions;
            thirdStep.PreviousStep = secondStep;
            thirdStep.Options.FinishButton.Visible = true;

            PromotionWizardModel model = new PromotionWizardModel();
            Wizard wizard = new Wizard(firstStep, model, arguments);
            wizard.HeaderText = Resources.NewPromotion;
            wizard.Width = 0;
            wizard.Height = 0;
            return wizard;
        }

        protected override void WizardActionExecuting(WizardActionEventArgs args)
        {
            //Actions can be canceled here
            args.Wizard.CurrentStep.Options.ErrorText = "";
            PromotionWizardModel model = args.Wizard.WizardModel as PromotionWizardModel;
            if (args.WizardAction == eWizardAction.INITIALIZE && args.Wizard.CurrentStep.Options.Name == "WizardStepPromotionHeader" && args.Wizard.Arguements.Count > 0)
            {
                Guid oid;
                if (Guid.TryParse(args.Wizard.Arguements[0], out oid))
                {
                    Promotion promo = XpoSession.GetObjectByKey<Promotion>(oid);
                    if (promo != null)
                    {
                        model.LoadPersistent(promo);
                    }
                }
            }

            if ((args.Wizard.CurrentStep.Options.Name == "WizardStepPromotionHeader" || args.Wizard.CurrentStep.Options.Name == "WizardStepPriceCatalogPolicies") 
                && args.WizardAction != eWizardAction.INITIALIZE)
            {
                TryUpdateModel(model);
                model.UpdateModel(XpoSession);
            }
            else if (args.WizardAction == eWizardAction.FINISH)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    try
                    {
                        string message = null;
                        if (model.Validate(out message))
                        {
                            Promotion promo = uow.GetObjectByKey<Promotion>(model.Oid);
                            if (promo == null)
                            {
                                promo = new Promotion(uow);
                            }
                            model.Persist(promo, true);
                            promo.PromotionApplicationRuleGroup.SetPromotion(promo);

                            AssignOwner(promo);
                            uow.CommitChanges();
                        }
                        else
                        {
                            args.Wizard.CurrentStep.Options.ErrorText = message;
                            args.CancelAction = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        string exceptionMessage = ex.GetFullMessage();
                        args.CancelAction = true;
                    }
                }
            }
            PrepareViewBags(model);
        }

        private void PrepareViewBags(PromotionWizardModel model)
        {
            ViewBag.ItemsOnPromotion = model.Rule.GetItems(XpoSession, null);
            ViewBag.ItemCategoriesOnPromotion = model.Rule.GetItemCategories(XpoSession, null);
            ViewBag.PriceCatalogPolicies = GetList<PriceCatalogPolicy>(XpoSession);
        }

        public static TreeListVirtualModeCreateChildrenMethod PromotionTreeListCreateChildren(Guid Wizardid)
        {
            return (TreeListVirtualModeCreateChildrenEventArgs e) =>
            {
                if (e.NodeObject == null)
                {
                    Wizard wizard = BaseController.StaticActiveWizards.FirstOrDefault(x => x.ID == Wizardid);
                    if (wizard != null && wizard.WizardModel is PromotionWizardModel)
                    {
                        PromotionWizardModel model = (PromotionWizardModel)wizard.WizardModel;
                        e.Children = new List<PromotionRuleWizardModel>() { model.Rule };
                    }
                }
                else if (e.NodeObject is PromotionApplicationRuleGroupWizardModel)
                {
                    e.Children = ((PromotionApplicationRuleGroupWizardModel)e.NodeObject).ChildNodes;
                }
            };
        }

        public static XPCollection<PriceCatalog> GetPriceCatalogs()
        {
            return GetList<PriceCatalog>(XpoHelper.GetNewUnitOfWork());
        }

        public static void SelectNodeRecursivelly(TreeListNode node, List<Guid> keys)
        {
            Guid nodeKey;
            if( Guid.TryParse(node.Key, out nodeKey) && keys.Contains(nodeKey))
            {
                node.Selected = true;
            }

            foreach (TreeListNode childNode in node.ChildNodes)
            {
                SelectNodeRecursivelly(childNode, keys);
            }
        }

        public static TreeListVirtualModeCreateChildrenMethod PriceCatalogTreeListCreateChildren(Guid Wizardid)
        {
            return (TreeListVirtualModeCreateChildrenEventArgs e) =>
            {
                if (e.NodeObject == null)
                {
                    e.Children = GetList<PriceCatalog>(XpoHelper.GetNewUnitOfWork(), new NullOperator("ParentCatalogOid"));

                }
                else if (e.NodeObject is PriceCatalog)
                {
                    e.Children = ((PriceCatalog)e.NodeObject).PriceCatalogs;
                }
            };
        }

        public static TreeListVirtualModeNodeCreatingMethod PriceCatalogTreeListNodeCreating(Guid Wizardid)
        {

            return (TreeListVirtualModeNodeCreatingEventArgs e) =>
            {
                e.IsLeaf = (e.NodeObject as PriceCatalog).PriceCatalogs.Count == 0;
                e.NodeKeyValue = (e.NodeObject as PriceCatalog).Oid;
                e.SetNodeValue("Description", ((PriceCatalog)e.NodeObject).Description);
                e.SetNodeValue("This", e.NodeObject);
                e.SetNodeValue("Oid", ((PriceCatalog)e.NodeObject).Oid);
            };
        }

        private static PromotionRuleWizardModel GetNodeByID(PromotionRuleWizardModel node, Guid ID)
        {
            PromotionRuleWizardModel parent;
            return GetNodeByID(node, ID, out parent);
        }

        private static PromotionRuleWizardModel GetNodeByID(PromotionRuleWizardModel node, Guid ID, out PromotionRuleWizardModel parent)
        {
            parent = null;
            if (node.Oid == ID)
            {
                
                return node;
            }
            if (node is PromotionApplicationRuleGroupWizardModel)
            {
                PromotionApplicationRuleGroupWizardModel group = (PromotionApplicationRuleGroupWizardModel)node;
                var result = group.PromotionApplicationRules.FirstOrDefault(x => x.Oid == ID);
                if (result != null)
                {
                    parent = group;
                    return result;
                }
                foreach (PromotionRuleWizardModel innerNode in group.ChildPromotionApplicationRuleGroups)
                {
                    PromotionRuleWizardModel result2 = GetNodeByID(innerNode, ID, out parent);
                    if (result2 != null)
                    {
                        if(parent == null)
                        {
                            parent = group;
                        }
                        return result2;
                    }
                }
            }
            return null;
        }

        public static TreeListVirtualModeNodeCreatingMethod PromotionTreeListNodeCreating(Guid Wizardid)
        {

            return (TreeListVirtualModeNodeCreatingEventArgs e) =>
            {
                e.IsLeaf = !(e.NodeObject is PromotionApplicationRuleGroupWizardModel);
                e.NodeKeyValue = (e.NodeObject as PromotionRuleWizardModel).Oid;
                e.SetNodeValue("Description", ((PromotionRuleWizardModel)e.NodeObject).Description);     
                e.SetNodeValue("This",e.NodeObject);
                e.SetNodeValue("Oid", ((PromotionRuleWizardModel)e.NodeObject).Oid);
            };
        }

        public ActionResult WizardStepPromotionApplicationRules(Guid WizardID)
        {
            Wizard wiz = this.ActiveWizards.FirstOrDefault(x => x.ID == WizardID);
            if(wiz!=null)
            {
                if(Request["CANCELEDIT"] != null)
                {
                    (wiz.WizardModel as PromotionWizardModel).Rule.RemoveInvalid();
                }
                ViewData["Wizard"] = wiz;
            }
            return PartialView(wiz.WizardModel);
        }

        public ActionResult PriceCatalogsTreeList(Guid WizardID, Guid RuleID)
        {
            Wizard wiz = this.ActiveWizards.FirstOrDefault(x => x.ID == WizardID);
            if(wiz!=null)
            {
                ViewData["Wizard"] = wiz;
                PromotionRuleWizardModel model = GetNodeByID((wiz.WizardModel as PromotionWizardModel).Rule, RuleID);
                return PartialView(model);
            }
            return PartialView();
        }

        public ActionResult PriceCatalogPoliciesList(Guid WizardID)
        {
            Wizard wiz = this.ActiveWizards.FirstOrDefault(x => x.ID == WizardID);
            if (wiz != null)
            {
                ViewData["Wizard"] = wiz;
            }
            return PartialView();
        }

        public static PromotionThenWizardModel GetEditedGridView(GridViewEditFormTemplateContainer c)
        {
            var obj = DataBinder.Eval(c.DataItem, "This");
            if (obj == null || obj is PromotionThenWizardModel == false)
            {
                return null;
            }
            return (PromotionThenWizardModel)obj;
        }

        public static PromotionRuleWizardModel GetEditedTreeListNode(TreeListEditFormTemplateContainer c)
        {
            var obj = DataBinder.Eval(c.DataItem, "This");
            if (obj == null || obj is PromotionRuleWizardModel == false)
            {
                return null;
            }
            return (PromotionRuleWizardModel)obj;
        }

        public ActionResult PromotionRuleTreeListAddNew(string PromotionRuleTypeSelector_VI, Guid WizardID, Guid ParentOid)
        {
            Wizard wiz = this.ActiveWizards.FirstOrDefault(x => x.ID == WizardID);
            if (wiz != null)
            {
                Type type = typeof(PromotionRuleWizardModel).Assembly.GetType("ITS.Retail.WebClient.ViewModel." + PromotionRuleTypeSelector_VI);
                PromotionApplicationRuleGroupWizardModel parent = GetNodeByID((wiz.WizardModel as PromotionWizardModel).Rule, ParentOid) as PromotionApplicationRuleGroupWizardModel;
                if(type == typeof(PromotionApplicationRuleGroupWizardModel))
                {
                    PromotionApplicationRuleGroupWizardModel group = new PromotionApplicationRuleGroupWizardModel() { Operator = eGroupOperatorType.And };
                    parent.ChildPromotionApplicationRuleGroups.Add(group);
                    group.ParentPromotionApplicationRuleGroupOid = parent.Oid;
                    ViewData["ObjectID"] = group.Oid.ToString().Replace("-", "");
                }
                else if(type !=null)
                {
                    PromotionApplicationRuleWizardModel newMod = Activator.CreateInstance(type) as PromotionApplicationRuleWizardModel;
                    parent.PromotionApplicationRules.Add(newMod);
                    ViewData["ObjectID"] = newMod.Oid.ToString().Replace("-", "");
                }
                ViewData["Wizard"] = wiz;
            }          
            return PartialView("WizardStepPromotionApplicationRules", wiz.WizardModel);
        }
        
        public ActionResult PromotionRuleTreeListUpdate(Guid Oid, Guid WizardID)
        {
            Wizard wiz = this.ActiveWizards.FirstOrDefault(x => x.ID == WizardID);
            if (wiz != null)
            {
                PromotionRuleWizardModel model = GetNodeByID((wiz.WizardModel as PromotionWizardModel).Rule, Oid);
                PromotionRuleWizardModel model2 = (PromotionRuleWizardModel)model.ShallowCopy();
                TryUpdateModel(model2);
                model2.UpdateModel(XpoSession);
                if (TryValidateModel(model2))
                {
                    TryUpdateModel(model);
                    model.UpdateModel(XpoSession);
                }
                else
                {
                    ViewBag.EditItem = model2;
                }
                ViewData["Wizard"] = wiz;
            }
            return PartialView("WizardStepPromotionApplicationRules", wiz.WizardModel);
        }
        
        public ActionResult PromotionRuleTreeListNodeDrag(Guid Oid, Guid? ParentOid, Guid WizardID)
        {
            Wizard wiz = this.ActiveWizards.FirstOrDefault(x => x.ID == WizardID);
            if(wiz!=null)
            {
                if (ParentOid.HasValue)
                {
                    PromotionRuleWizardModel newParent, oldParent, movingNode = GetNodeByID((wiz.WizardModel as PromotionWizardModel).Rule, Oid, out oldParent);
                    newParent = GetNodeByID((wiz.WizardModel as PromotionWizardModel).Rule, ParentOid.Value);
                    if (movingNode!=null && newParent is PromotionApplicationRuleGroupWizardModel && oldParent is PromotionApplicationRuleGroupWizardModel)
                    {
                        PromotionApplicationRuleGroupWizardModel nParent = (PromotionApplicationRuleGroupWizardModel)newParent,
                            oParent = (PromotionApplicationRuleGroupWizardModel)oldParent;
                        if(movingNode  is PromotionApplicationRuleGroupWizardModel)
                        {
                            PromotionApplicationRuleGroupWizardModel movingNode1 = (PromotionApplicationRuleGroupWizardModel)movingNode;
                            oParent.ChildPromotionApplicationRuleGroups.Remove(movingNode1);
                            movingNode1.ParentPromotionApplicationRuleGroupOid = nParent.Oid;
                            nParent.ChildPromotionApplicationRuleGroups.Add(movingNode1);
                        }
                        else if (movingNode is PromotionApplicationRuleWizardModel)
                        {
                            PromotionApplicationRuleWizardModel movingNode1 = (PromotionApplicationRuleWizardModel)movingNode;
                            oParent.PromotionApplicationRules.Remove(movingNode1);
                            nParent.PromotionApplicationRules.Add(movingNode1);
                        }
                    }
                }
               
                ViewData["Wizard"] = wiz;
            }
            return PartialView("WizardStepPromotionApplicationRules", wiz.WizardModel);
        }
        
        public ActionResult PromotionRuleTreeListDelete(Guid Oid, Guid WizardID)
        {
            Wizard wiz = this.ActiveWizards.FirstOrDefault(x => x.ID == WizardID);
            if (wiz != null)
            {
                PromotionRuleWizardModel model = GetNodeByID((wiz.WizardModel as PromotionWizardModel).Rule, Oid);
                if(model != null)
                {
                    model.IsDeleted = true;
                }
                ViewData["Wizard"] = wiz;
            }
            return PartialView("WizardStepPromotionApplicationRules");
        }

        public static object CustomerRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            if (e.Filter == "") { return null; }
            string proccessed_filter = e.Filter.Replace("*", "%");
            if (!proccessed_filter.Contains("%"))
            {
                proccessed_filter = String.Format("%{0}%", proccessed_filter);
            }
            
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();

            CriteriaOperator crop = CriteriaOperator.Or(new BinaryOperator("CompanyName", proccessed_filter, BinaryOperatorType.Like),
                                                        new BinaryOperator("Trader.TaxCode", proccessed_filter, BinaryOperatorType.Like),
                                                        new BinaryOperator("Code", proccessed_filter, BinaryOperatorType.Like));
            XPCollection<Customer> searched_item_customers = GetList<Customer>(uow, crop, "Trader.TaxCode");

            searched_item_customers.SkipReturnedObjects = e.BeginIndex;
            searched_item_customers.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;

            return searched_item_customers;
        }

        public ActionResult AddNewResult(Guid WizardID, string PromotionThenTypeSelector_VI) 
        {
            Wizard wiz = this.ActiveWizards.FirstOrDefault(x => x.ID == WizardID);
            if (wiz != null)
            {
                ViewData["Wizard"] = wiz;
                Type type = typeof(PromotionRuleWizardModel).Assembly.GetType("ITS.Retail.WebClient.ViewModel." + PromotionThenTypeSelector_VI);
                if(type != null)
                {
                    PromotionThenWizardModel newMod = Activator.CreateInstance(type) as PromotionThenWizardModel;
                    if (newMod is PromotionExecutionWizardModel)
                    {
                        if (newMod is PromotionItemExecutionWizardModel)
                        {
                            ((PromotionItemExecutionWizardModel)newMod).OncePerItem = true;
                        }
                        else if (newMod is PromotionItemCategoryExecutionWizardModel)
                        {
                            ((PromotionItemCategoryExecutionWizardModel)newMod).OncePerItem = true;
                        }
                        (wiz.WizardModel as PromotionWizardModel).PromotionExecutions.Add((PromotionExecutionWizardModel)newMod);
                    }
                    else if(newMod is PromotionResultWizardModel)
                    {
                        (wiz.WizardModel as PromotionWizardModel).PromotionResults.Add((PromotionResultWizardModel)newMod);
                    }
                    ViewData["ObjectID"] = newMod.Oid.ToString().Replace("-", "");
                }             
                return PartialView("WizardStepPromotionExecutions", wiz.WizardModel);
            }
            return PartialView("WizardStepPromotionExecutions");
        }

        public ActionResult UpdateResult(Guid WizardID, Guid Oid) 
        {
            Wizard wiz = this.ActiveWizards.FirstOrDefault(x => x.ID == WizardID);
            if (wiz != null)
            {
                ViewData["Wizard"] = wiz;
                PromotionThenWizardModel mod = (wiz.WizardModel as PromotionWizardModel).PromotionResults.FirstOrDefault(x => x.Oid == Oid);
                if (mod == null)
                {
                    mod = (wiz.WizardModel as PromotionWizardModel).PromotionExecutions.FirstOrDefault(x => x.Oid == Oid);
                }
                if(mod != null)
                {
                    PromotionThenWizardModel mod2 = mod.ShallowCopy() as PromotionThenWizardModel;
                    bool result = TryUpdateModel(mod2);
                    mod2.UpdateModel(XpoSession);
                    ModelState.Clear();
                    result = TryValidateModel(mod2);
                    ViewBag.EditItem = mod2;
                    if(result)
                    {
                        TryUpdateModel(mod);
                        mod.UpdateModel(XpoSession);
                    }                 
                }
                return PartialView("WizardStepPromotionExecutions", wiz.WizardModel);
            }
            return PartialView("WizardStepPromotionExecutions");
        }

        public ActionResult DeleteResult(Guid Oid, Guid WizardID) 
        {
            Wizard wiz = this.ActiveWizards.FirstOrDefault(x => x.ID == WizardID);
            if (wiz != null)
            {
                PromotionThenWizardModel mod = (wiz.WizardModel as PromotionWizardModel).PromotionResults.FirstOrDefault(x => x.Oid == Oid);
                if (mod == null)
                {
                    mod = (wiz.WizardModel as PromotionWizardModel).PromotionExecutions.FirstOrDefault(x => x.Oid == Oid);
                }
                if(mod !=null)
                {
                    mod.IsDeleted = true;
                }
                ViewData["Wizard"] = wiz;
                return PartialView("WizardStepPromotionExecutions", wiz.WizardModel);
            }
            return PartialView("WizardStepPromotionExecutions");
        }

        public ActionResult WizardStepPromotionExecutions(Guid WizardID) 
        {
            Wizard wiz = this.ActiveWizards.FirstOrDefault(x => x.ID == WizardID);
            if (wiz != null)
            {                
                ViewData["Wizard"] = wiz;
                PrepareViewBags(wiz.WizardModel as PromotionWizardModel);
                return PartialView("WizardStepPromotionExecutions", wiz.WizardModel);
            }
            return PartialView("WizardStepPromotionExecutions");
        }

        public ActionResult CellDataPopup(String PromotionID, String ViewMode)
        {
            Guid PromotionGuid;
            if (!Guid.TryParse(PromotionID, out PromotionGuid))
            {
                PromotionGuid = Guid.Empty;
            }
            Promotion promotion = XpoSession.FindObject<Promotion>(new BinaryOperator("Oid", PromotionGuid));
            ViewData["ViewMode"] = ViewMode;
            return PartialView(promotion);
        }

        public ActionResult SelectCustomDataView()
        {
            return PartialView();
        }

        public ActionResult CustomDataViewParameters(Guid? customDataViewOid)
        {
            PromotionCustomDataViewApplicationRuleWizardModel promotionCustomDataViewApplicationRuleWizardModel = new PromotionCustomDataViewApplicationRuleWizardModel();
            if (customDataViewOid.HasValue)
            {
                CustomDataView customDataView = XpoSession.GetObjectByKey<CustomDataView>(customDataViewOid.Value);

                promotionCustomDataViewApplicationRuleWizardModel.CustomDataView = customDataView.Oid;

                if (customDataView != null && customDataView.Parameters != null && customDataView.Parameters.Count > 0)
                {
                    foreach (CustomDataViewParameter customDataViewParameter in customDataView.Parameters)
                    {
                        promotionCustomDataViewApplicationRuleWizardModel.Parameters.Add(
                                                                                new PromotionCustomDataViewParameterWizardModel()
                                                                                {
                                                                                    //Description = customDataViewParameter.Description,
                                                                                    Name = customDataViewParameter.Name,
                                                                                    Type = customDataViewParameter.ParameterType
                                                                                }
                                                                            );
                    }
                }
                Dictionary<string,string> customDataViewColumns = customDataView.GetDataViewColumnNames();
                foreach ( KeyValuePair<string,string> pair in customDataViewColumns )
                {
                    promotionCustomDataViewApplicationRuleWizardModel.Conditions.Add(new PromotionCustomDataViewConditionViewModel()
                                                                                     {
                                                                                        DataViewColumn = pair.Key                  
                                                                                     });
                }
            }
            return PartialView(promotionCustomDataViewApplicationRuleWizardModel);
        }
    }
}

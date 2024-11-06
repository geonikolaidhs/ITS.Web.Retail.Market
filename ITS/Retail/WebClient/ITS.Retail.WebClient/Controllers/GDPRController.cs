using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Common;
using ITS.Retail.Model;
using DevExpress.Web.Mvc;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.ResourcesLib;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.Linq;
using ITS.Retail.WebClient.AuxillaryClasses;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.Platform;
using ITS.Retail.WebClient.ViewModel;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Common.Helpers;
using ITS.Retail.WebClient.Attributes;
using System.Threading;
using System.Globalization;
using System.Reflection;
using ITS.Retail.Platform.Enumerations;
using System.IO;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.WebClient.Controllers
{
    [CustomDataViewShow]
    [StoreControllerEditable]
    public class GDPRController : BaseController
    {
        private bool AllowRun()
        {
            return !CurrentUser.Role.GDPRActions && CurrentUser.Role.Type != eRoleType.SystemAdministrator ? false : true;
        }
        public ActionResult Export()
        {
            if (!AllowRun()) return Content(Resources.PermissionDenied);
            this.CustomJSProperties.AddJSProperty("editAction", "Edit");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "CustomerID");
            this.CustomJSProperties.AddJSProperty("gridName", "grdGDPRCustomer");
            if (CurrentOwner.OwnerApplicationSettings.CustomerExportProtocolReport == null)
            {
                Session["Error"] = Resources.GDPRPrintFormsNotSelected;
                Session["GDPRError"] = Resources.GDPRPrintFormsNotSelected;
            }
            return PartialView("Export");
        }
        public ActionResult CustomerGrid()
        {
            if (!AllowRun()) return Content(Resources.PermissionDenied);
            CriteriaToExpressionConverter conv = new CriteriaToExpressionConverter();
            ViewData["CallbackMode"] = "";
            if (Request["DXCallbackArgument"].Contains("SEARCH") == false)
            {

                if (Session["CustomerFilter"] == null)
                {
                    Session["CustomerFilter"] = new BinaryOperator("Oid", Guid.Empty);
                }
                return PartialView("CustomerGrid", new XPQuery<Customer>(XpoHelper.GetNewUnitOfWork()).AppendWhere(conv, ApplyOwnerCriteria((CriteriaOperator)Session["CustomerFilter"], typeof(Customer))));
            }
            else if (Request["DXCallbackArgument"].Contains("APPLYCOLUMNFILTER"))
            {
                ViewData["CallbackMode"] = "APPLYCOLUMNFILTER";
            }

            CriteriaOperator criteria;
            if (Request.HttpMethod == "POST")
            {
                ViewData["CallbackMode"] = "SEARCH";
                string code = (Request["customer_code"] == null || Request["customer_code"] == "null") ? "" : Request["customer_code"];
                string cardid = (Request["card_id"] == null || Request["card_id"] == "null") ? "" : Request["card_id"];
                string name = (Request["customer_name"] == null || Request["customer_name"] == "null") ? "" : Request["customer_name"];
                string taxn = (Request["customer_tax_number"] == null || Request["customer_tax_number"] == "null") ? "" : Request["customer_tax_number"];
                string lcod = (Request["loyalty_code"] == null || Request["loyalty_code"] == "null") ? "" : Request["loyalty_code"];
                string FcreatedOn = Request["FcreatedOn"] == null || Request["FcreatedOn"] == "null" ? "" : Request["FcreatedOn"];
                string FupdatedOn = Request["FupdatedOn"] == null || Request["FupdatedOn"] == "null" ? "" : Request["FupdatedOn"];

                CriteriaOperator codeFilter, nameFilter = null, lastNameFilter = null, cardidFilter = null, taxnFilter = null, activeFilter, companyNameFilter = null;
                if (name != "")
                {
                    nameFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.FirstName"), name);
                    // new BinaryOperator("Trader.FirstName", "%" + name + "%", BinaryOperatorType.Like);
                    lastNameFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.LastName"), name);
                    // new BinaryOperator("Trader.LastName", "%" + name + "%", BinaryOperatorType.Like);
                    companyNameFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("CompanyName"), name);
                    // new BinaryOperator("CompanyName", "%" + name + "%", BinaryOperatorType.Like);
                }

                if (taxn != "")
                {
                    taxnFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.TaxCode"), taxn);
                    // new BinaryOperator("Trader.TaxCode", "%" + taxn + "%", BinaryOperatorType.Like);
                }

                if (cardid != "")
                {
                    cardidFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("CardID"), cardid);
                    // new BinaryOperator("CardID", "%" + cardid + "%", BinaryOperatorType.Like);
                }

                if (code.Replace('%', '*').Contains("*"))
                {
                    codeFilter = new BinaryOperator("Code", code.Replace('*', '%'), BinaryOperatorType.Like);
                }
                else
                {
                    codeFilter = CreateCriteria(code, "Code");
                }

                activeFilter = null;
                bool isActiveFilter;
                if (Boolean.TryParse(Request["is_active"], out isActiveFilter))
                {
                    activeFilter = new BinaryOperator("IsActive", isActiveFilter);
                }

                CriteriaOperator createdOnFilter = null;
                if (FcreatedOn != "")
                {
                    createdOnFilter = new BinaryOperator("CreatedOnTicks", DateTime.Parse(FcreatedOn).Ticks, BinaryOperatorType.GreaterOrEqual);
                }

                CriteriaOperator updatedOnFilter = null;
                if (FupdatedOn != "")
                {
                    updatedOnFilter = new BinaryOperator("UpdatedOnTicks", DateTime.Parse(FupdatedOn).Ticks, BinaryOperatorType.GreaterOrEqual);
                }

                criteria = CriteriaOperator.And(codeFilter,
                                                CriteriaOperator.Or(nameFilter, lastNameFilter, companyNameFilter),
                                                taxnFilter,
                                                cardidFilter,
                                                CreateCriteria(lcod, "Loyalty"),
                                                activeFilter, createdOnFilter, updatedOnFilter
                                                );

                if ((bool)Session["IsAdministrator"] == false)
                {
                    CompanyNew owner;
                    XPCollection<CompanyNew> sup = BOApplicationHelper.GetUserEntities<CompanyNew>(XpoHelper.GetNewUnitOfWork(), CurrentUser);
                    owner = sup[0];
                    CriteriaOperator permissions = new BinaryOperator("Owner.Oid", owner.Oid);
                    criteria = CriteriaOperator.And(criteria, permissions);
                }

                if (ReferenceEquals(criteria, null))
                {
                    criteria = new BinaryOperator("Oid", Guid.Empty, BinaryOperatorType.NotEqual);
                }
            }
            else
            {
                criteria = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");
            }
            Session["CustomerFilter"] = criteria;

            var tmp = new XPQuery<Customer>(XpoHelper.GetNewUnitOfWork()).AppendWhere(conv, ApplyOwnerCriteria((CriteriaOperator)Session["CustomerFilter"], typeof(Customer)));

            string CustomerID = Request.Params["CustomerID"];
            Guid CustomerGuid;

            if (!Guid.TryParse(CustomerID, out CustomerGuid))
            {
                CustomerGuid = Guid.Empty;
            }

            Session["CurrentElement"] = ViewData["CurrentElement"];
            if (CurrentOwner.OwnerApplicationSettings.CustomerExportProtocolReport == null)
                Session["GDPRError"] = Resources.GDPRPrintFormsNotSelected;
            else
                Session["GDPRError"] = null;
            return PartialView("CustomerGrid", tmp.AsQueryable());
        }
        public ActionResult CustomerAnonymizeGrid()
        {
            if (!AllowRun()) return Content(Resources.PermissionDenied);
            CriteriaToExpressionConverter conv = new CriteriaToExpressionConverter();
            ViewData["CallbackMode"] = "";
            if (Request["DXCallbackArgument"].Contains("SEARCH") == false)
            {

                if (Session["CustomerFilter"] == null)
                {
                    Session["CustomerFilter"] = new BinaryOperator("Oid", Guid.Empty);
                }
                return PartialView("CustomerAnonymizeGrid", new XPQuery<Customer>(XpoHelper.GetNewUnitOfWork()).AppendWhere(conv, ApplyOwnerCriteria((CriteriaOperator)Session["CustomerFilter"], typeof(Customer))));
            }
            else if (Request["DXCallbackArgument"].Contains("APPLYCOLUMNFILTER"))
            {
                ViewData["CallbackMode"] = "APPLYCOLUMNFILTER";
            }

            CriteriaOperator criteria;
            if (Request.HttpMethod == "POST")
            {
                ViewData["CallbackMode"] = "SEARCH";
                string code = (Request["customer_code"] == null || Request["customer_code"] == "null") ? "" : Request["customer_code"];
                string cardid = (Request["card_id"] == null || Request["card_id"] == "null") ? "" : Request["card_id"];
                string name = (Request["customer_name"] == null || Request["customer_name"] == "null") ? "" : Request["customer_name"];
                string taxn = (Request["customer_tax_number"] == null || Request["customer_tax_number"] == "null") ? "" : Request["customer_tax_number"];
                string lcod = (Request["loyalty_code"] == null || Request["loyalty_code"] == "null") ? "" : Request["loyalty_code"];
                string FcreatedOn = Request["FcreatedOn"] == null || Request["FcreatedOn"] == "null" ? "" : Request["FcreatedOn"];
                string FupdatedOn = Request["FupdatedOn"] == null || Request["FupdatedOn"] == "null" ? "" : Request["FupdatedOn"];

                CriteriaOperator codeFilter, nameFilter = null, lastNameFilter = null, cardidFilter = null, taxnFilter = null, activeFilter, companyNameFilter = null;
                if (name != "")
                {
                    nameFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.FirstName"), name);
                    // new BinaryOperator("Trader.FirstName", "%" + name + "%", BinaryOperatorType.Like);
                    lastNameFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.LastName"), name);
                    // new BinaryOperator("Trader.LastName", "%" + name + "%", BinaryOperatorType.Like);
                    companyNameFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("CompanyName"), name);
                    // new BinaryOperator("CompanyName", "%" + name + "%", BinaryOperatorType.Like);
                }

                if (taxn != "")
                {
                    taxnFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.TaxCode"), taxn);
                    // new BinaryOperator("Trader.TaxCode", "%" + taxn + "%", BinaryOperatorType.Like);
                }

                if (cardid != "")
                {
                    cardidFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("CardID"), cardid);
                    // new BinaryOperator("CardID", "%" + cardid + "%", BinaryOperatorType.Like);
                }

                if (code.Replace('%', '*').Contains("*"))
                {
                    codeFilter = new BinaryOperator("Code", code.Replace('*', '%'), BinaryOperatorType.Like);
                }
                else
                {
                    codeFilter = CreateCriteria(code, "Code");
                }

                activeFilter = null;
                bool isActiveFilter;
                if (Boolean.TryParse(Request["is_active"], out isActiveFilter))
                {
                    activeFilter = new BinaryOperator("IsActive", isActiveFilter);
                }

                CriteriaOperator createdOnFilter = null;
                if (FcreatedOn != "")
                {
                    createdOnFilter = new BinaryOperator("CreatedOnTicks", DateTime.Parse(FcreatedOn).Ticks, BinaryOperatorType.GreaterOrEqual);
                }

                CriteriaOperator updatedOnFilter = null;
                if (FupdatedOn != "")
                {
                    updatedOnFilter = new BinaryOperator("UpdatedOnTicks", DateTime.Parse(FupdatedOn).Ticks, BinaryOperatorType.GreaterOrEqual);
                }

                criteria = CriteriaOperator.And(codeFilter,
                                                CriteriaOperator.Or(nameFilter, lastNameFilter, companyNameFilter),
                                                taxnFilter,
                                                cardidFilter,
                                                CreateCriteria(lcod, "Loyalty"),
                                                activeFilter, createdOnFilter, updatedOnFilter
                                                );

                if ((bool)Session["IsAdministrator"] == false)
                {
                    CompanyNew owner;
                    XPCollection<CompanyNew> sup = BOApplicationHelper.GetUserEntities<CompanyNew>(XpoHelper.GetNewUnitOfWork(), CurrentUser);
                    owner = sup[0];
                    CriteriaOperator permissions = new BinaryOperator("Owner.Oid", owner.Oid);
                    criteria = CriteriaOperator.And(criteria, permissions);
                }

                if (ReferenceEquals(criteria, null))
                {
                    criteria = new BinaryOperator("Oid", Guid.Empty, BinaryOperatorType.NotEqual);
                }
            }
            else
            {
                criteria = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");
            }
            Session["CustomerFilter"] = criteria;

            var tmp = new XPQuery<Customer>(XpoHelper.GetNewUnitOfWork()).AppendWhere(conv, ApplyOwnerCriteria((CriteriaOperator)Session["CustomerFilter"], typeof(Customer)));

            string CustomerID = Request.Params["CustomerID"];
            Guid CustomerGuid;
            if (!Guid.TryParse(CustomerID, out CustomerGuid))
            {
                CustomerGuid = Guid.Empty;
            }
            Session["CurrentElement"] = ViewData["CurrentElement"];
            if (CurrentOwner.OwnerApplicationSettings.CustomerAnonymizationProtocolReport == null)
                Session["GDPRError"] = Resources.GDPRPrintFormsNotSelected;
            else
                Session["GDPRError"] = null;
            return PartialView("CustomerAnonymizeGrid", tmp.AsQueryable());
        }
        public ActionResult LoadAssosiatedCustomerEditPopup()
        {

            return PartialView("LoadAssosiatedCustomerEditPopup");
        }
        public ActionResult Anonymize()
        {
            if (!AllowRun()) return Content(Resources.PermissionDenied);
            this.CustomJSProperties.AddJSProperty("editAction", "Edit");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "CustomerID");
            this.CustomJSProperties.AddJSProperty("gridName", "grdGDPRAnonymizeCustomer");
            if (CurrentOwner.OwnerApplicationSettings.CustomerAnonymizationProtocolReport == null)
            {
                Session["Error"] = Resources.GDPRPrintFormsNotSelected;
                Session["GDPRError"] = Resources.GDPRPrintFormsNotSelected;
            }
                
            return PartialView("Anonymize");
        }
        protected CriteriaOperator CreateCriteria(string value, string fieldname)
        {
            string fil = "";
            value = value.Replace("'", "").Replace("\"", "");
            if (string.IsNullOrEmpty(value) == false)
            {
                if (value.Replace('%', '*').Contains("*"))
                {
                    fil += String.Format("{0} like '{1}'", fieldname, value.Replace('*', '%'));

                }
                else
                {
                    fil += String.Format("{0}='{1}'", fieldname, value);
                }
            }
            return CriteriaOperator.Parse(fil, "");
        }
        public ActionResult ExportCustomer()
        {
            if (!AllowRun()) return Content(Resources.PermissionDenied);
            Guid Oid = Guid.Parse(Request.Params["ID"]);
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            Customer customer = uow.GetObjectByKey<Customer>(Oid);
            tw.WriteLine("Customer");
            tw.WriteLine(" ");
            foreach (PropertyInfo propertyInfo in customer.GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttributes(typeof(NonPersistentAttribute), false).Count() == 0)
                {
                    if (propertyInfo.GetValue(customer, null) != null)
                        if (propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(customer, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(customer, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(customer, null).GetType().ToString().Contains("List") && propertyInfo.GetValue(customer, null).GetType() != typeof(Guid))
                            tw.WriteLine(propertyInfo.Name + ": " + propertyInfo.GetValue(customer, null).ToString());
                }
            }
            tw.WriteLine(" ");
            tw.WriteLine("Trader");
            tw.WriteLine(" ");
            foreach (PropertyInfo propertyInfo in customer.Trader.GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttributes(typeof(NonPersistentAttribute), false).Count() == 0)
                {
                    if (propertyInfo.GetValue(customer.Trader, null) != null)
                        if (propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(customer.Trader, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(customer.Trader, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(customer.Trader, null).GetType().ToString().Contains("List") && propertyInfo.GetValue(customer.Trader, null).GetType() != typeof(Guid))
                            tw.WriteLine(propertyInfo.Name + ": " + propertyInfo.GetValue(customer.Trader, null).ToString());
                }
            }
            tw.WriteLine(" ");
            tw.WriteLine("Addresses - Phones");
            tw.WriteLine(" ");
            int ctr = 0;

            foreach (Address address in customer.Trader.Addresses)
            {
                ctr += 1;
                tw.WriteLine(" ");
                tw.WriteLine("Address " + ctr.ToString());
                tw.WriteLine(" ");
                foreach (PropertyInfo propertyInfo in address.GetType().GetProperties())
                {
                    if (propertyInfo.GetCustomAttributes(typeof(NonPersistentAttribute), false).Count() == 0)
                    {
                        if (propertyInfo.GetValue(address, null) != null)
                            if (propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(address, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(address, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(address, null).GetType().ToString().Contains("List") && propertyInfo.GetValue(address, null).GetType() != typeof(Guid))
                                tw.WriteLine(propertyInfo.Name + ": " + propertyInfo.GetValue(address, null).ToString());
                    }
                }
                int xtr = 0;
                tw.WriteLine(" ");
                tw.WriteLine("Phones ");
                tw.WriteLine(" ");
                foreach (Phone phone in address.Phones)
                {
                    xtr += 1;
                    tw.WriteLine(" ");
                    tw.WriteLine("Phone " + xtr.ToString());
                    tw.WriteLine(" ");
                    foreach (PropertyInfo propertyInfo in phone.GetType().GetProperties())
                    {
                        if (propertyInfo.GetCustomAttributes(typeof(NonPersistentAttribute), false).Count() == 0)
                        {
                            if (propertyInfo.GetValue(phone, null) != null)
                                if (propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(phone, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(phone, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(phone, null).GetType().ToString().Contains("List") && propertyInfo.GetValue(phone, null).GetType() != typeof(Guid))
                                    tw.WriteLine(propertyInfo.Name + ": " + propertyInfo.GetValue(phone, null).ToString());
                        }
                    }
                }
            }
            tw.WriteLine(" ");
            tw.WriteLine("Customer Childs");
            tw.WriteLine(" ");
            ctr = 0;
            foreach (CustomerChild child in customer.CustomerChilds)
            {
                ctr += 1;
                tw.WriteLine(" ");
                tw.WriteLine("Customer Child " + ctr.ToString());
                tw.WriteLine(" ");

                foreach (PropertyInfo propertyInfo in child.GetType().GetProperties())
                {
                    if (propertyInfo.GetCustomAttributes(typeof(NonPersistentAttribute), false).Count() == 0)
                    {
                        if (propertyInfo.GetValue(child, null) != null)
                            if (propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(child, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(child, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(child, null).GetType().ToString().Contains("List") && propertyInfo.GetValue(child, null).GetType() != typeof(Guid))
                                tw.WriteLine(propertyInfo.Name + ": " + propertyInfo.GetValue(child, null).ToString());
                    }
                }
            }
            tw.WriteLine(" ");
            tw.WriteLine("Documents");
            tw.WriteLine(" ");
            foreach (DocumentHeader document in customer.DocumentHeaders)
            {
                tw.WriteLine(document.FinalizedDate.ToString() + ";" + document.DocumentType.Description + ";" + document.DocumentSeriesCode + ";" + document.DocumentNumber.ToString() + ";" + document.GrossTotal.ToString());
            }
            tw.Flush();
            tw.Close();
            customer.GDPRExportDate = DateTime.Now;
            User user = uow.GetObjectByKey<User>(CurrentUser.Oid);
            customer.GDPRExportUser = user;
            customer.Save();
            uow.CommitTransaction();
            //customer.Trader.GDPRExportProtocolNumber = "";
            return File(memoryStream.GetBuffer(), "text/plain", customer.CompanyName + "_" + customer.Code + ".txt");
            //return PartialView("Anonymize");
        }
        public ActionResult PrintExportCustomer()
        {
            if (!AllowRun()) return Content(Resources.PermissionDenied);
            Guid Oid = Guid.Parse(Request.Params["ID"]);
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();

            Customer customer = uow.GetObjectByKey<Customer>(Oid);
            if (customer.GDPRExportProtocolNumber == 0)
            {
                GDPRProtocolNumbers protocolNumbers = GetList<GDPRProtocolNumbers>(customer.Session).FirstOrDefault();
                if (protocolNumbers == null)
                {
                    protocolNumbers = new GDPRProtocolNumbers(customer.Session);
                    protocolNumbers.ExportProtocolNumber = 0;
                    protocolNumbers.AnonymizationProtocolNumber = 0;
                }
                protocolNumbers.ExportProtocolNumber += 1;
                protocolNumbers.Save();
                protocolNumbers.Session.CommitTransaction();

                customer.GDPRExportProtocolNumber = protocolNumbers.ExportProtocolNumber;
                customer.Save();
            }
            ViewData["CustomerExportProtocolReport"] = CurrentOwner.OwnerApplicationSettings.CustomerExportProtocolReport.Oid.ToString();
            ViewData["CustomerOID"] = Oid.ToString();
            uow.CommitTransaction();
            return PartialView();
        }
        public ActionResult PrintAnonymizationCustomer()
        {
            if (!AllowRun()) return Content(Resources.PermissionDenied);
            Guid Oid = Guid.Parse(Request.Params["ID"]);
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            Customer customer = uow.GetObjectByKey<Customer>(Oid);
            if (customer.GDPRAnonymizationProtocolNumber == 0)
            {
                GDPRProtocolNumbers protocolNumbers = GetList<GDPRProtocolNumbers>(customer.Session).FirstOrDefault();
                if (protocolNumbers == null)
                {
                    protocolNumbers = new GDPRProtocolNumbers(customer.Session);
                    protocolNumbers.ExportProtocolNumber = 0;
                    protocolNumbers.AnonymizationProtocolNumber = 0;
                }
                protocolNumbers.AnonymizationProtocolNumber += 1;
                protocolNumbers.Save();
                protocolNumbers.Session.CommitTransaction();
                customer.GDPRAnonymizationProtocolNumber = protocolNumbers.AnonymizationProtocolNumber;
                customer.Save();
            }
            ViewData["CustomerAnonymizationProtocolReport"] = CurrentOwner.OwnerApplicationSettings.CustomerAnonymizationProtocolReport.Oid.ToString();
            ViewData["CustomerOID"] = Oid.ToString();
            uow.CommitTransaction();
            return PartialView();
        }
        public ActionResult ExportSupplier()
        {
            if (!AllowRun()) return Content(Resources.PermissionDenied);
            this.CustomJSProperties.AddJSProperty("editAction", "Edit");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "SupplierID");
            this.CustomJSProperties.AddJSProperty("gridName", "grdGDPRExportSupplier");
            if (CurrentOwner.OwnerApplicationSettings.SupplierExportProtocolReport == null)
            {
                Session["Error"] = Resources.GDPRPrintFormsNotSelected;
                Session["GDPRError"] = Resources.GDPRPrintFormsNotSelected;
            }

            return PartialView("ExportSupplier");
        }
        public ActionResult ExportSupplierGrid()
        {
            if (!AllowRun()) return Content(Resources.PermissionDenied);
            CriteriaToExpressionConverter conv = new CriteriaToExpressionConverter();
            ViewData["CallbackMode"] = "";
            if (Request["DXCallbackArgument"].Contains("SEARCH") == false)
            {
                if (Session["SupplierFilter"] == null) Session["SupplierFilter"] = new BinaryOperator("Oid", Guid.Empty);
                return PartialView("ExportSupplierGrid", new XPQuery<SupplierNew>(XpoHelper.GetNewUnitOfWork()).AppendWhere(conv, ApplyOwnerCriteria((CriteriaOperator)Session["SupplierFilter"], typeof(SupplierNew))));
            }
            else if (Request["DXCallbackArgument"].Contains("APPLYCOLUMNFILTER"))
            {
                ViewData["CallbackMode"] = "APPLYCOLUMNFILTER";
            }
            CriteriaOperator criteria;
            if (Request.HttpMethod == "POST")
            {
                ViewData["CallbackMode"] = "SEARCH";
                string code = (Request["supplier_code"] == null || Request["supplier_code"] == "null") ? "" : Request["supplier_code"];
                string name = (Request["supplier_name"] == null || Request["supplier_name"] == "null") ? "" : Request["supplier_name"];
                string taxn = (Request["supplier_tax_number"] == null || Request["supplier_tax_number"] == "null") ? "" : Request["supplier_tax_number"];
                string lcod = (Request["loyalty_code"] == null || Request["loyalty_code"] == "null") ? "" : Request["loyalty_code"];
                CriteriaOperator codeFilter, nameFilter, lastNameFilter, taxnFilter, activeFilter, companyNameFilter;
                if (String.IsNullOrWhiteSpace(code))
                    codeFilter = null;
                else if (code.Replace('%', '*').Contains("*"))
                    codeFilter = new BinaryOperator("Code", code.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                else
                    codeFilter = CreateCriteria(code, "Code");
                if (String.IsNullOrWhiteSpace(name))
                {
                    nameFilter = lastNameFilter = companyNameFilter = null;
                }
                else
                {
                    nameFilter = new BinaryOperator("Trader.FirstName", "%" + name + "%", BinaryOperatorType.Like);
                    lastNameFilter = new BinaryOperator("Trader.LastName", "%" + name + "%", BinaryOperatorType.Like);
                    companyNameFilter = new BinaryOperator("CompanyName", "%" + name + "%", BinaryOperatorType.Like);
                }
                if (String.IsNullOrWhiteSpace(taxn))
                {
                    taxnFilter = null;
                }
                else
                {
                    taxnFilter = new BinaryOperator("Trader.TaxCode", "%" + taxn + "%", BinaryOperatorType.Like);
                }
                int j;
                activeFilter = null;
                if (Int32.TryParse(Request["is_active"].ToString(), out j))
                {
                    activeFilter = new BinaryOperator("IsActive", j == 1);
                }
                criteria = CriteriaOperator.And(codeFilter,
                                                                CriteriaOperator.Or(nameFilter, lastNameFilter, companyNameFilter),
                                                                taxnFilter,
                                                                CreateCriteria(lcod, "Loyalty"),
                                                                activeFilter
                                                                );
                if (ReferenceEquals(criteria, null))
                {
                    criteria = new BinaryOperator("Oid", Guid.Empty, BinaryOperatorType.NotEqual);
                }


            }
            else
            {
                criteria = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");

            }
            Session["SupplierFilter"] = criteria;
            Session["CurrentElement"] = ViewData["CurrentElement"];
            if (CurrentOwner.OwnerApplicationSettings.SupplierExportProtocolReport == null)
                Session["GDPRError"] = Resources.GDPRPrintFormsNotSelected;
            else
                Session["GDPRError"] = null;
            return PartialView(new XPQuery<SupplierNew>(XpoHelper.GetNewUnitOfWork()).AppendWhere(conv, ApplyOwnerCriteria((CriteriaOperator)Session["SupplierFilter"], typeof(SupplierNew))));

        }
        public ActionResult ExportSupplierData()
        {
            if (!AllowRun()) return Content(Resources.PermissionDenied);
            Guid Oid = Guid.Parse(Request.Params["ID"]);
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            SupplierNew supplier = uow.GetObjectByKey<SupplierNew>(Oid);
            tw.WriteLine("Supplier");
            tw.WriteLine(" ");
            foreach (PropertyInfo propertyInfo in supplier.GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttributes(typeof(NonPersistentAttribute), false).Count() == 0)
                {
                    if (propertyInfo.GetValue(supplier, null) != null)
                        if (propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(supplier, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(supplier, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(supplier, null).GetType().ToString().Contains("List") && propertyInfo.GetValue(supplier, null).GetType() != typeof(Guid))
                            tw.WriteLine(propertyInfo.Name + ": " + propertyInfo.GetValue(supplier, null).ToString());
                }
            }
            tw.WriteLine(" ");
            tw.WriteLine("Trader");
            tw.WriteLine(" ");
            foreach (PropertyInfo propertyInfo in supplier.Trader.GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttributes(typeof(NonPersistentAttribute), false).Count() == 0)
                {
                    if (propertyInfo.GetValue(supplier.Trader, null) != null)
                        if (propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(supplier.Trader, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(supplier.Trader, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(supplier.Trader, null).GetType().ToString().Contains("List") && propertyInfo.GetValue(supplier.Trader, null).GetType() != typeof(Guid))
                            tw.WriteLine(propertyInfo.Name + ": " + propertyInfo.GetValue(supplier.Trader, null).ToString());
                }
            }
            tw.WriteLine(" ");
            tw.WriteLine("Addresses - Phones");
            tw.WriteLine(" ");
            int ctr = 0;

            foreach (Address address in supplier.Trader.Addresses)
            {
                ctr += 1;
                tw.WriteLine(" ");
                tw.WriteLine("Address " + ctr.ToString());
                tw.WriteLine(" ");
                foreach (PropertyInfo propertyInfo in address.GetType().GetProperties())
                {
                    if (propertyInfo.GetCustomAttributes(typeof(NonPersistentAttribute), false).Count() == 0)
                    {
                        if (propertyInfo.GetValue(address, null) != null)
                            if (propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(address, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(address, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(address, null).GetType().ToString().Contains("List") && propertyInfo.GetValue(address, null).GetType() != typeof(Guid))
                                tw.WriteLine(propertyInfo.Name + ": " + propertyInfo.GetValue(address, null).ToString());
                    }
                }
                int xtr = 0;
                tw.WriteLine(" ");
                tw.WriteLine("Phones ");
                tw.WriteLine(" ");
                foreach (Phone phone in address.Phones)
                {
                    xtr += 1;
                    tw.WriteLine(" ");
                    tw.WriteLine("Phone " + xtr.ToString());
                    tw.WriteLine(" ");
                    foreach (PropertyInfo propertyInfo in phone.GetType().GetProperties())
                    {
                        if (propertyInfo.GetCustomAttributes(typeof(NonPersistentAttribute), false).Count() == 0)
                        {
                            if (propertyInfo.GetValue(phone, null) != null)
                                if (propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(phone, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(phone, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(phone, null).GetType().ToString().Contains("List") && propertyInfo.GetValue(phone, null).GetType() != typeof(Guid))
                                    tw.WriteLine(propertyInfo.Name + ": " + propertyInfo.GetValue(phone, null).ToString());
                        }
                    }
                }
            }

            
            tw.WriteLine(" ");
            tw.WriteLine("Documents");
            tw.WriteLine(" ");
            foreach (DocumentHeader document in supplier.DocumentHeaders)
            {
                tw.WriteLine(document.FinalizedDate.ToString() + ";" + document.DocumentType.Description + ";" + document.DocumentSeriesCode + ";" + document.DocumentNumber.ToString() + ";" + document.GrossTotal.ToString());
            }
            tw.Flush();
            tw.Close();
            supplier.GDPRExportDate = DateTime.Now;
            User user = uow.GetObjectByKey<User>(CurrentUser.Oid);
            supplier.GDPRExportUser = user;
            supplier.Save();
            uow.CommitTransaction();
            //customer.Trader.GDPRExportProtocolNumber = "";
            return File(memoryStream.GetBuffer(), "text/plain", supplier.CompanyName + "_" + supplier.Code + ".txt");
            //return PartialView("Anonymize");
        }
        public ActionResult PrintExportSupplier()
        {
            if (!AllowRun()) return Content(Resources.PermissionDenied);
            Guid Oid = Guid.Parse(Request.Params["ID"]);
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();

            SupplierNew supplier = uow.GetObjectByKey<SupplierNew>(Oid);
            if (supplier.GDPRExportProtocolNumber == 0)
            {
                GDPRProtocolNumbers protocolNumbers = GetList<GDPRProtocolNumbers>(supplier.Session).FirstOrDefault();
                if (protocolNumbers == null)
                {
                    protocolNumbers = new GDPRProtocolNumbers(supplier.Session);
                    protocolNumbers.ExportProtocolNumber = 0;
                    protocolNumbers.AnonymizationProtocolNumber = 0;
                }
                protocolNumbers.ExportProtocolNumber += 1;
                protocolNumbers.Save();
                protocolNumbers.Session.CommitTransaction();

                supplier.GDPRExportProtocolNumber = protocolNumbers.ExportProtocolNumber;
                supplier.Save();
            }
            ViewData["SupplierExportProtocolReport"] = CurrentOwner.OwnerApplicationSettings.SupplierExportProtocolReport.Oid.ToString();
            ViewData["SupplierOID"] = Oid.ToString();
            uow.CommitTransaction();
            return PartialView();
        }
        public ActionResult AnonymizeSupplier()
        {
            if (!AllowRun()) return Content(Resources.PermissionDenied);
            this.CustomJSProperties.AddJSProperty("editAction", "Edit");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "SupplierID");
            this.CustomJSProperties.AddJSProperty("gridName", "grdGDPRAnonymizeSupplier");
            if (CurrentOwner.OwnerApplicationSettings.SupplierAnonymizationProtocolReport == null)
            {
                Session["Error"] = Resources.GDPRPrintFormsNotSelected;
                Session["GDPRError"] = Resources.GDPRPrintFormsNotSelected;
            }

            return PartialView("AnonymizeSupplier");
        }
        public ActionResult AnonymizeSupplierGrid()
        {
            if (!AllowRun()) return Content(Resources.PermissionDenied);
            CriteriaToExpressionConverter conv = new CriteriaToExpressionConverter();
            ViewData["CallbackMode"] = "";
            if (Request["DXCallbackArgument"].Contains("SEARCH") == false)
            {
                if (Session["SupplierFilter"] == null) Session["SupplierFilter"] = new BinaryOperator("Oid", Guid.Empty);
                return PartialView("AnonymizeSupplierGrid", new XPQuery<SupplierNew>(XpoHelper.GetNewUnitOfWork()).AppendWhere(conv, ApplyOwnerCriteria((CriteriaOperator)Session["SupplierFilter"], typeof(SupplierNew))));
            }
            else if (Request["DXCallbackArgument"].Contains("APPLYCOLUMNFILTER"))
            {
                ViewData["CallbackMode"] = "APPLYCOLUMNFILTER";
            }
            CriteriaOperator criteria;
            if (Request.HttpMethod == "POST")
            {
                ViewData["CallbackMode"] = "SEARCH";
                string code = (Request["supplier_code"] == null || Request["supplier_code"] == "null") ? "" : Request["supplier_code"];
                string name = (Request["supplier_name"] == null || Request["supplier_name"] == "null") ? "" : Request["supplier_name"];
                string taxn = (Request["supplier_tax_number"] == null || Request["supplier_tax_number"] == "null") ? "" : Request["supplier_tax_number"];
                string lcod = (Request["loyalty_code"] == null || Request["loyalty_code"] == "null") ? "" : Request["loyalty_code"];
                CriteriaOperator codeFilter, nameFilter, lastNameFilter, taxnFilter, activeFilter, companyNameFilter;
                if (String.IsNullOrWhiteSpace(code))
                    codeFilter = null;
                else if (code.Replace('%', '*').Contains("*"))
                    codeFilter = new BinaryOperator("Code", code.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                else
                    codeFilter = CreateCriteria(code, "Code");
                if (String.IsNullOrWhiteSpace(name))
                {
                    nameFilter = lastNameFilter = companyNameFilter = null;
                }
                else
                {
                    nameFilter = new BinaryOperator("Trader.FirstName", "%" + name + "%", BinaryOperatorType.Like);
                    lastNameFilter = new BinaryOperator("Trader.LastName", "%" + name + "%", BinaryOperatorType.Like);
                    companyNameFilter = new BinaryOperator("CompanyName", "%" + name + "%", BinaryOperatorType.Like);
                }
                if (String.IsNullOrWhiteSpace(taxn))
                {
                    taxnFilter = null;
                }
                else
                {
                    taxnFilter = new BinaryOperator("Trader.TaxCode", "%" + taxn + "%", BinaryOperatorType.Like);
                }
                int j;
                activeFilter = null;
                if (Int32.TryParse(Request["is_active"].ToString(), out j))
                {
                    activeFilter = new BinaryOperator("IsActive", j == 1);
                }
                criteria = CriteriaOperator.And(codeFilter,
                                                                CriteriaOperator.Or(nameFilter, lastNameFilter, companyNameFilter),
                                                                taxnFilter,
                                                                CreateCriteria(lcod, "Loyalty"),
                                                                activeFilter
                                                                );
                if (ReferenceEquals(criteria, null))
                {
                    criteria = new BinaryOperator("Oid", Guid.Empty, BinaryOperatorType.NotEqual);
                }


            }
            else
            {
                criteria = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");

            }
            Session["SupplierFilter"] = criteria;
            Session["CurrentElement"] = ViewData["CurrentElement"];
            if (CurrentOwner.OwnerApplicationSettings.SupplierAnonymizationProtocolReport == null)
                Session["GDPRError"] = Resources.GDPRPrintFormsNotSelected;
            else
                Session["GDPRError"] = null;
            return PartialView(new XPQuery<SupplierNew>(XpoHelper.GetNewUnitOfWork()).AppendWhere(conv, ApplyOwnerCriteria((CriteriaOperator)Session["SupplierFilter"], typeof(SupplierNew))));

        }
        public ActionResult PrintAnonymizationSupplier()
        {
            if (!AllowRun()) return Content(Resources.PermissionDenied);
            Guid Oid = Guid.Parse(Request.Params["ID"]);
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            SupplierNew supplier = uow.GetObjectByKey<SupplierNew>(Oid);
            if (supplier.GDPRAnonymizationProtocolNumber == 0)
            {
                GDPRProtocolNumbers protocolNumbers = GetList<GDPRProtocolNumbers>(supplier.Session).FirstOrDefault();
                if (protocolNumbers == null)
                {
                    protocolNumbers = new GDPRProtocolNumbers(supplier.Session);
                    protocolNumbers.ExportProtocolNumber = 0;
                    protocolNumbers.AnonymizationProtocolNumber = 0;
                }
                protocolNumbers.AnonymizationProtocolNumber += 1;
                protocolNumbers.Save();
                protocolNumbers.Session.CommitTransaction();
                supplier.GDPRAnonymizationProtocolNumber = protocolNumbers.AnonymizationProtocolNumber;
                supplier.Save();
            }
            ViewData["SupplierAnonymizationProtocolReport"] = CurrentOwner.OwnerApplicationSettings.SupplierAnonymizationProtocolReport.Oid.ToString();
            ViewData["SupplierOID"] = Oid.ToString();
            uow.CommitTransaction();
            return PartialView();
        }
        public ActionResult AnonymizeCustomer()
        {
            if (!AllowRun()) return Content(Resources.PermissionDenied);
            Guid Oid = Guid.Parse(Request.Params["objid"]);
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            Customer customer = uow.GetObjectByKey<Customer>(Oid);
            foreach (PropertyInfo propertyInfo in customer.GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttributes(typeof(GDPRAttribute), false).Count() > 0)
                {
                    if (propertyInfo.GetValue(customer, null) != null)
                        if (propertyInfo.Name.ToUpper() != "OID" && propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(customer, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(customer, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(customer, null).GetType().ToString().Contains("List"))
                            SetAnonymizedValue(customer, propertyInfo);
                }
            }
            Trader trader = customer.Trader;
            foreach (PropertyInfo propertyInfo in trader.GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttributes(typeof(GDPRAttribute), false).Count() > 0)
                {
                    if (propertyInfo.GetValue(trader, null) != null)
                        if (propertyInfo.Name.ToUpper() != "OID" && propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(trader, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(trader, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(trader, null).GetType().ToString().Contains("List"))
                            SetAnonymizedValue(trader, propertyInfo);
                }
            }
            //trader.Save();
            int ctr = 0;
            foreach (Address address in customer.Trader.Addresses)
            {
                ctr += 1;
                Address zaddress = address;
                foreach (PropertyInfo propertyInfo in zaddress.GetType().GetProperties())
                {
                    if (propertyInfo.GetCustomAttributes(typeof(GDPRAttribute), false).Count() > 0)
                    {
                        if (propertyInfo.GetValue(zaddress, null) != null)
                            if (propertyInfo.Name.ToUpper() != "OID" && propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(zaddress, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(zaddress, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(zaddress, null).GetType().ToString().Contains("List"))
                                SetAnonymizedValue(zaddress, propertyInfo);
                    }
                }
                foreach (Phone phone in address.Phones)
                {
                    Phone zphone = phone;
                    foreach (PropertyInfo propertyInfo in zphone.GetType().GetProperties())
                    {
                        if (propertyInfo.GetCustomAttributes(typeof(GDPRAttribute), false).Count() > 0)
                        {
                            if (propertyInfo.GetValue(zphone, null) != null)
                                if (propertyInfo.Name.ToUpper() != "OID" && propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(zphone, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(zphone, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(zphone, null).GetType().ToString().Contains("List"))
                                    SetAnonymizedValue(zphone, propertyInfo);
                        }
                    }
                }
            }

            foreach (CustomerChild child in customer.CustomerChilds)
            {
                CustomerChild zchild = child;
                foreach (PropertyInfo propertyInfo in zchild.GetType().GetProperties())
                {
                    if (propertyInfo.GetCustomAttributes(typeof(GDPRAttribute), false).Count() > 0)
                    {
                        if (propertyInfo.GetValue(zchild, null) != null)
                            if (propertyInfo.Name.ToUpper() != "OID" && propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(zchild, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(zchild, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(zchild, null).GetType().ToString().Contains("List"))
                                SetAnonymizedValue(zchild, propertyInfo);
                    }
                }
            }

            foreach (DocumentHeader document in customer.DocumentHeaders)
            {
                DocumentHeader zdocument = document;
                foreach (PropertyInfo propertyInfo in zdocument.GetType().GetProperties())
                {
                    if (propertyInfo.GetCustomAttributes(typeof(GDPRAttribute), false).Count() > 0)
                    {
                        if (propertyInfo.GetValue(zdocument, null) != null)
                            if (propertyInfo.Name.ToUpper() != "OID" && propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(zdocument, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(zdocument, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(zdocument, null).GetType().ToString().Contains("List"))
                                SetAnonymizedValue(zdocument, propertyInfo);
                    }
                }
            }
            customer.GDPRAnonymizationDate = DateTime.Now;
            User user = uow.GetObjectByKey<User>(CurrentUser.Oid);
            customer.GDPRAnonymizationUser = user;
            customer.Save();
            uow.CommitTransaction();
            return PartialView();
        }
        public ActionResult AnonymizeSupplierData()
        {
            if (!AllowRun()) return Content(Resources.PermissionDenied);
            Guid Oid = Guid.Parse(Request.Params["objid"]);
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            SupplierNew supplier = uow.GetObjectByKey<SupplierNew>(Oid);
            foreach (PropertyInfo propertyInfo in supplier.GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttributes(typeof(GDPRAttribute), false).Count() > 0)
                {
                    if (propertyInfo.GetValue(supplier, null) != null)
                        if (propertyInfo.Name.ToUpper() != "OID" && propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(supplier, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(supplier, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(supplier, null).GetType().ToString().Contains("List"))
                            SetAnonymizedValue(supplier, propertyInfo);
                }
            }
            Trader trader = supplier.Trader;
            foreach (PropertyInfo propertyInfo in trader.GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttributes(typeof(GDPRAttribute), false).Count() > 0)
                {
                    if (propertyInfo.GetValue(trader, null) != null)
                        if (propertyInfo.Name.ToUpper() != "OID" && propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(trader, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(trader, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(trader, null).GetType().ToString().Contains("List"))
                            SetAnonymizedValue(trader, propertyInfo);
                }
            }
            //trader.Save();
            int ctr = 0;
            foreach (Address address in supplier.Trader.Addresses)
            {
                ctr += 1;
                Address zaddress = address;
                foreach (PropertyInfo propertyInfo in zaddress.GetType().GetProperties())
                {
                    if (propertyInfo.GetCustomAttributes(typeof(GDPRAttribute), false).Count() > 0)
                    {
                        if (propertyInfo.GetValue(zaddress, null) != null)
                            if (propertyInfo.Name.ToUpper() != "OID" && propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(zaddress, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(zaddress, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(zaddress, null).GetType().ToString().Contains("List"))
                                SetAnonymizedValue(zaddress, propertyInfo);
                    }
                }
                foreach (Phone phone in address.Phones)
                {
                    Phone zphone = phone;
                    foreach (PropertyInfo propertyInfo in zphone.GetType().GetProperties())
                    {
                        if (propertyInfo.GetCustomAttributes(typeof(GDPRAttribute), false).Count() > 0)
                        {
                            if (propertyInfo.GetValue(zphone, null) != null)
                                if (propertyInfo.Name.ToUpper() != "OID" && propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(zphone, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(zphone, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(zphone, null).GetType().ToString().Contains("List"))
                                    SetAnonymizedValue(zphone, propertyInfo);
                        }
                    }
                }
            }
            foreach (DocumentHeader document in supplier.DocumentHeaders)
            {
                DocumentHeader zdocument = document;
                foreach (PropertyInfo propertyInfo in zdocument.GetType().GetProperties())
                {
                    if (propertyInfo.GetCustomAttributes(typeof(GDPRAttribute), false).Count() > 0)
                    {
                        if (propertyInfo.GetValue(zdocument, null) != null)
                            if (propertyInfo.Name.ToUpper() != "OID" && propertyInfo.Name != "IsDeleted" && propertyInfo.Name != "IsLoading" && propertyInfo.Name != "Session" && propertyInfo.Name != "ClassInfo" && propertyInfo.Name != "Loading" && propertyInfo.Name != "This" && propertyInfo.Name != "ObjectSignature" && !propertyInfo.GetValue(zdocument, null).GetType().ToString().Contains("Collection") && !propertyInfo.GetValue(zdocument, null).GetType().ToString().Contains("ViewModel") && !propertyInfo.GetValue(zdocument, null).GetType().ToString().Contains("List"))
                                SetAnonymizedValue(zdocument, propertyInfo);
                    }
                }
            }
            supplier.GDPRAnonymizationDate = DateTime.Now;
            User user = uow.GetObjectByKey<User>(CurrentUser.Oid);
            supplier.GDPRAnonymizationUser = user;
            supplier.Save();
            uow.CommitTransaction();
            return PartialView();
        }
        private static void SetAnonymizedValue(object Obj, PropertyInfo propertyInfo)
        {
            if (Obj != null)
            {
                if (Nullable.GetUnderlyingType(propertyInfo.MemberType.GetType()) == null)
                {
                    if (!propertyInfo.GetValue(Obj, null).GetType().IsEnum)
                    {
                        switch (propertyInfo.PropertyType.Name)
                        {
                            case "String":
                                propertyInfo.SetValue(Obj, "", null);
                                break;
                            case "DateTime":
                                propertyInfo.SetValue(Obj, new DateTime(0), null);
                                break;
                            case "Int16":
                                propertyInfo.SetValue(Obj, 0, null);
                                break;
                            case "Int32":
                                propertyInfo.SetValue(Obj, 0, null);
                                break;
                            case "Int64":
                                propertyInfo.SetValue(Obj, 0, null);
                                break;
                            case "UInteger":
                                propertyInfo.SetValue(Obj, 0, null);
                                break;
                            case "UInt16":
                                propertyInfo.SetValue(Obj, 0, null);
                                break;
                            case "UInt32":
                                propertyInfo.SetValue(Obj, 0, null);
                                break;
                            case "UInt64":
                                propertyInfo.SetValue(Obj, 0, null);
                                break;
                            case "Guid":
                                propertyInfo.SetValue(Obj, Guid.Empty, null);
                                break;
                            case "Boolean":
                                propertyInfo.SetValue(Obj, false, null);
                                break;
                            case "Decimal":
                                propertyInfo.SetValue(Obj, (decimal)0, null);
                                break;
                            case "Single":
                                propertyInfo.SetValue(Obj, (Single)0, null);
                                break;
                            case "Double":
                                propertyInfo.SetValue(Obj, (Double)0, null);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        propertyInfo.SetValue(Obj, Enum.GetValues(propertyInfo.GetValue(Obj, null).GetType()).GetValue(0), null);
                    }
                }
                else
                {
                    propertyInfo.SetValue(Obj, null, null);
                }

            }
        }
    }
}

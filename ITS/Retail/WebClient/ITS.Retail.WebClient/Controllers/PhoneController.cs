using System;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.Common;
using DevExpress.Data.Filtering;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Providers;

namespace ITS.Retail.WebClient.Controllers
{
    public class PhoneController : BaseObjController<Phone>
    {

        public ActionResult ParameterGrid(string strAddressID, bool displayCommands)
        {
            FillLookupComboBoxes();
            string AddressID = strAddressID;
            Guid  AddressGuID = AddressID==null || AddressID=="null" || AddressID=="-1" ?Guid.Empty : Guid.Parse(AddressID);
            ViewData["displayCommands"] = displayCommands;
            ViewData["addressid"] = AddressID;
            
            bool isNewAddress = false;
            if(AddressGuID != Guid.Empty && Session["NewTrader"] != null && Boolean.Parse(Session["NewTrader"].ToString()))
            {
                Trader newTrader = Session["Trader"] as Trader;
                foreach(Address newAddress in newTrader.Addresses)
                {
                    if(newAddress.Oid == AddressGuID)
                    {
                        isNewAddress = true;
                        break;
                    }
                }
            }

            if (Session["NewAddress"] != null)
            {
                Address ad = (Address)Session["NewAddress"];
                ViewData["Phones"] = ad.Phones;
                ViewData["extraTitle"] = ad.Description;
                ViewData["addressid"] = ad.Oid;
                return PartialView("Grid", ad.Phones.AsEnumerable<Phone>());
            }
            else if (isNewAddress)
            {
                if (AddressID.Length > 1)
                {
                    Trader t = (Trader)Session["Trader"];
                    foreach (Address ad in t.Addresses)
                    {
                        if (ad.Oid == AddressGuID)
                        {
                            ViewData["Phones"] = ad.Phones.AsEnumerable<Phone>();
                            return PartialView("Grid", ad.Phones.AsEnumerable<Phone>());
                        }
                    }
                    return PartialView("Grid", null);
                }
                else
                {
                    ViewData["Phones"] = Session["NewPhones"];
                    return PartialView("Grid", Session["NewPhones"]);
                }

            }
            else 
            {
                CriteriaOperator adrFilter = (AddressID == "" || AddressID == "-1" ? CriteriaOperator.Parse("Oid='" + Guid.Empty + "'") : CriteriaOperator.Parse("Oid='" + AddressID + "'"));
                Address t = XpoHelper.GetNewUnitOfWork().FindObject<Address>(adrFilter);
                ViewData["extraTitle"] = t.Description;
                CriteriaOperator filter = (AddressID == "" || AddressID == "-1" ? CriteriaOperator.Parse("Oid='" + Guid.Empty + "'") : CriteriaOperator.Parse("Address.Oid='" + AddressID + "'"));
                Session["PhoneFilter"] = filter;
                ViewData["Phones"] = GetList<Phone>(XpoHelper.GetNewUnitOfWork(), filter, "Number").AsEnumerable<Phone>();
                return PartialView("Grid", ViewData["Phones"]);
            }
        }

        [HttpPost]
        public ActionResult InlineEditingAddNewPartial([ModelBinder(typeof(RetailModelBinder))] Phone ct)
        {
            string AddressID = Request["AddressID"] == null || Request["AddressID"] == "null" ? "" : Request["AddressID"];
            Guid AddressGUID = AddressID == "" ? Guid.Empty : Guid.Parse(AddressID);
            ViewData["displayCommands"] = true;
            ViewData["addressid"] = AddressID;

            if (ModelState.IsValid)
            {
                UpdateLookupObjects(ct);
                if (Session["NewAddress"] != null)
                {
                    Address adr = (Address)Session["NewAddress"];
                    Phone nct = new Phone(adr.Session);
                    ViewData["addressid"] = adr.Oid.ToString();
                    nct.GetData(ct);
                    nct.Number = Request["PhoneNumber"];
                    adr.Phones.Add(nct);
                    FillLookupComboBoxes();
                    ViewData["Phones"] = adr.Phones;
                    return PartialView("Grid", adr.Phones.AsEnumerable<Phone>());
                }
                else if (Session["NewTrader"] != null && Boolean.Parse(Session["NewTrader"].ToString()))
                {
                    Trader t = (Trader)Session["Trader"];
                    Phone nct = new Phone(t.Session);
                    nct.GetData( ct);
                    nct.Number = Request["PhoneNumber"];
                    foreach (Address ad in t.Addresses)
                    {
                        if (ad.Oid == AddressGUID)
                        {
                            ad.Phones.Add(nct);
                            ViewData["Phones"] =  ad.Phones;
                            FillLookupComboBoxes();
                            return PartialView("Grid", ad.Phones.AsEnumerable<Phone>());
                        }
                    }
                }
                else 
                {
                    try
                    {
                        UpdateLookupObjects(ct);
                        ct.Number = Request["PhoneNumber"];
                        Save(ct);
                    }
                    catch (Exception e)
                    {
                        Session["Error"]  = e.Message+Environment.NewLine+e.StackTrace;
                    }
                }
            }
            else
                Session["Error"]  = Resources.AnErrorOccurred;

            FillLookupComboBoxes();         
            ViewData["Phones"] = GetList<Phone>(XpoHelper.GetNewUnitOfWork(), (CriteriaOperator)Session["PhoneFilter"], "Number").AsEnumerable<Phone>();
            return PartialView("Grid", ViewData["Phones"]);
        }
        [HttpPost]
        public ActionResult InlineEditingUpdatePartial([ModelBinder(typeof(RetailModelBinder))] Phone ct)
        {
            string AddressID = Request["AddressID"] == null || Request["AddressID"] == "null" ? "" : Request["AddressID"];
            Guid AddressGUID = AddressID == "" ? Guid.Empty : Guid.Parse(AddressID);
            ViewData["displayCommands"] = true;
            ViewData["addressid"] = AddressID;
            if (ModelState.IsValid)
            {
                UpdateLookupObjects(ct);
                if (Session["NewAddress"] != null)
                {
                    Address adr = (Address)Session["NewAddress"];
                    Phone p = adr.Phones.FirstOrDefault(g => g.Oid==ct.Oid);
                    if (p != null)
                    {
                        p.GetData(ct);
                        p.Address = adr;
                        p.Number = Request["PhoneNumber"];
                    }

                    ViewData["addressid"] = adr.Oid.ToString();
                    FillLookupComboBoxes();
                    ViewData["Phones"] = adr.Phones;
                    return PartialView("Grid", adr.Phones.AsEnumerable<Phone>());
                }
                else if (Session["NewTrader"] != null && Boolean.Parse(Session["NewTrader"].ToString()))
                {
                    //XPCollection<Phone> col=null;
                    Trader t = (Trader)Session["Trader"];
                    Address address = t.Addresses.First(g => g.Oid == AddressGUID);
                    if (address != null)
                    {
                        Phone p = address.Phones.First(g => g.Oid == ct.Oid);
                        if (p != null)
                        {
                            p.PhoneType = ct.PhoneType != null ? p.Session.FindObject<PhoneType>(new BinaryOperator("Oid", ct.PhoneType.Oid)) : null;
                            p.Number = Request["PhoneNumber"];
                        }
                    }
                    FillLookupComboBoxes();
                    ViewData["Phones"] = address.Phones;
                    return PartialView("Grid", address.Phones);
                }
                else 
                {
                    try
                    {
                        Save(ct);
                    }
                    catch (Exception e)
                    {
                        Session["Error"]  = e.Message+Environment.NewLine+e.StackTrace;
                    }
                }
            }
            else
                Session["Error"]  = Resources.AnErrorOccurred;

            FillLookupComboBoxes();            
            ViewData["Phones"] = GetList<Phone>(XpoHelper.GetNewUnitOfWork(), (CriteriaOperator)Session["PhoneFilter"], "Number").AsEnumerable<Phone>();
            return PartialView("Grid", ViewData["Phones"]);
        }

        [HttpPost]
        public ActionResult InlineEditingDeletePartial([ModelBinder(typeof(RetailModelBinder))] Phone ct)
        {
            string AddressID = Request["AddressID"] == null || Request["AddressID"] == "null" ? "" : Request["AddressID"];
            Guid AddressGUID = AddressID == "" ? Guid.Empty : Guid.Parse(AddressID);
            ViewData["displayCommands"] = true;
            ViewData["addressid"] = AddressID;
            if(Session["NewAddress"]!=null)
            {
                Address adr = (Address)Session["NewAddress"];
                foreach (Phone p in adr.Phones)
                {
                    if (p.Oid == ct.Oid)
                    {
                        adr.Phones.Remove(p);
                        p.Delete();
                        break;
                    }
                }
                ViewData["addressid"] = adr.Oid.ToString();
                FillLookupComboBoxes();
                ViewData["Phones"] = adr.Phones;
                return PartialView("Grid", adr.Phones.AsEnumerable<Phone>());
            }
            else if (Session["NewTrader"] != null && Boolean.Parse(Session["NewTrader"].ToString()))
            {
                //IEnumerable<Phone> col = null;
                Trader t = (Trader)Session["Trader"];
                Address address = t.Addresses.FirstOrDefault(g => g.Oid == AddressGUID);
                if (address != null)                    
                {                    
                    Phone p = address.Phones.FirstOrDefault(g => g.Oid == ct.Oid);
                    p.Delete();
                }
                
                FillLookupComboBoxes();
                ViewData["Phones"] = address.Phones;
                return PartialView("Grid", address.Phones);
            }
            else
            {
                try
                {
                    Delete(ct);
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message;// +Environment.NewLine + e.StackTrace;
                }
            }
            FillLookupComboBoxes();
            
            ViewData["Phones"] = GetList<Phone>(XpoHelper.GetNewUnitOfWork(), (CriteriaOperator)Session["PhoneFilter"], "Number").AsEnumerable<Phone>();
            return PartialView("Grid", ViewData["Phones"]);
        }

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            ViewBag.PhoneTypeComboBox = GetList<PhoneType>(XpoSession);
        }

        protected override void UpdateLookupObjects(Phone a)
        {
            base.UpdateLookupObjects(a);
            a.PhoneType = GetObjectByArgument<PhoneType>(a.Session, "PhoneTypeCb_VI");
            a.Address = (Session["NewAddress"] == null) ? GetObjectByArgument<Address>(a.Session, "AddressID"): null;
            
        }
    }
}
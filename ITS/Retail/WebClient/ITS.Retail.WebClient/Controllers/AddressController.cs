using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.Common;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Providers;

namespace ITS.Retail.WebClient.Controllers
{
    public class AddressController : BaseObjController<Address>
    {
        public ActionResult AddressGrid(string strTraderID, bool displayCommands, string supplierID)
        {
            Guid SupID;
            if (!Guid.TryParse(supplierID, out SupID))
                SupID = Guid.Empty;
            ViewData["Supplier_ID"] = SupID;
            ViewData["GDPREnabled"] = CurrentUser.Role.GDPREnabled;
            Guid TraderID = strTraderID == null || strTraderID == "" || strTraderID == "null" ? Guid.Empty : Guid.Parse(strTraderID);
            ViewData["Trader_ID"] = TraderID.ToString();
            FillLookupComboBoxes();
            ViewData["displayCommands"] = displayCommands;
            Session["AddressFilter"] = CriteriaOperator.And(new BinaryOperator("Trader.Oid", TraderID));

            if (Request["DXCallbackArgument"] != null && Request["DXCallbackArgument"].Contains("STARTEDIT") && Request["DXCallbackName"].Contains("grdAddress"))
            {
                string adrOid = Request["DXCallbackArgument"].ToString().Substring(Request["DXCallbackArgument"].ToString().Length - 37, 36);
                Guid adrId;
                if (Guid.TryParse(adrOid, out adrId))
                {
                    ViewData["centralStoreBlock"] = false;
                    Trader tr = (Trader)Session["Trader"];
                    Session["NewAddress"] = tr.Addresses.FirstOrDefault(adr => adr.Oid == adrId);
                    foreach (Address adr in tr.Addresses)
                    {
                        if (adr.Store != null && adr.Store.Central != null && adr.Store.Central.Address.Oid == adrId)
                        {
                            ViewData["centralStoreBlock"] = true;
                            break;
                        }
                    }

                }
            }
            if (Request["DXCallbackArgument"] != null && Request["DXCallbackArgument"].Contains("ADDNEW") && Request["DXCallbackName"].Contains("grdAddress"))
            {

                if (Session["Trader"] != null)
                {
                    Trader tr = (Trader)Session["Trader"];
                    Address adr = new Address(tr.Session);
                    Session["NewAddress"] = adr;
                }
                else
                {
                    UnitOfWork uow;
                    uow = (Session["uow"] != null) ? (UnitOfWork)Session["uow"] : XpoHelper.GetNewUnitOfWork();
                    Session["uow"] = uow;
                    Address adr = new Address(uow);
                    adr.Trader = adr.Session.FindObject<Trader>(new BinaryOperator("Oid", TraderID));
                    Session["NewAddress"] = adr;
                }
            }

            if (Request["DXCallbackArgument"] != null && Request["DXCallbackArgument"].Contains("CANCELEDIT") && Request["DXCallbackName"].Contains("grdAddress"))
            {
                Session["NewAddress"] = null;
            }

            if (Session["NewTrader"] != null && Boolean.Parse(Session["NewTrader"].ToString()) && (((Trader)Session["Trader"]).Oid == TraderID || TraderID == Guid.Empty))
            {
                ViewData["displayCommands"] = true;
                ViewData["Trader_ID"] = ((Trader)Session["Trader"]).Oid.ToString();
                Session["AddressFilter"] = null;
                IEnumerable<Address> getListAsEnumerabl = (((Trader)Session["Trader"]).Addresses.AsEnumerable<Address>());
                return PartialView("Grid", getListAsEnumerabl);
            }

            Trader t = XpoHelper.GetNewUnitOfWork().FindObject<Trader>(new BinaryOperator("Oid", TraderID));
            if (t == null)
            {
                if (Session["Trader"] != null)
                {
                    Trader nt = (Trader)Session["Trader"];
                    if (nt.Oid == TraderID)
                    {
                        ActionResult ret = PartialView("Grid", (nt.Addresses));
                        return ret;
                    }
                }
            }
            else
            {
                ActionResult ret = PartialView("Grid", (t.Addresses));
                return ret;

            }
            Session["AddressFilter"] = new BinaryOperator("Oid", Guid.Empty);
            return PartialView("Grid", GetList<Address>(XpoHelper.GetNewUnitOfWork(), new BinaryOperator("Oid", Guid.Empty)));
        }

        [HttpPost]
        public ActionResult InlineEditingAddNewPartial([ModelBinder(typeof(RetailModelBinder))] Address ct)
        {
            ViewData["GDPREnabled"] = CurrentUser.Role.GDPREnabled;
            FillLookupComboBoxes();
            bool displayCommands = Request["displayCommands"] == null || Request["displayCommands"] == "null" ? true : Boolean.Parse(Request["displayCommands"]);
            ViewData["displayCommands"] = displayCommands;
            Guid SupID;
            if (!Guid.TryParse(Request["adrSupplierID"].ToString(), out SupID))
            {
                SupID = Guid.Empty;
            }
            ViewData["Supplier_ID"] = SupID;
            Trader t = null;
            if (Session["NewTrader"] != null && Boolean.Parse(Session["NewTrader"].ToString()))
            {
                t = (Trader)Session["Trader"];
                ViewData["Trader_ID"] = t.Oid;
            }
            if (String.IsNullOrWhiteSpace(ct.Street))
            {
                ModelState.AddModelError("Street", Resources.StreetIsEmpty);
            }
            if (String.IsNullOrWhiteSpace(ct.Profession))
            {
                ModelState.AddModelError("Profession", Resources.AddressProfessionIsEmpty);
            }
            if (ModelState.IsValid)
            {
                {
                    if (Session["NewAddress"] != null)
                    {
                        Address adr = (Address)Session["NewAddress"];
                        adr.GetData(ct, new List<string>() { "Session" });

                        //adr.Street = ct.Street;
                        //adr.POBox = ct.POBox;
                        //adr.PostCode = ct.PostCode;
                        //adr.City = ct.City;
                        UpdateLookupObjects(ct);
                        Guid phoneGuid;
                        if (Request["DefaultPhoneCb_VI"] != null)
                        {
                            if (Guid.TryParse(Request["DefaultPhoneCb_VI"].ToString(), out phoneGuid))
                            {
                                adr.DefaultPhone = adr.Phones.FirstOrDefault(g => g.Oid == phoneGuid);
                            }
                        }
                        if (Request["IsStore"] != null)
                        {
                            if (Request["IsStore"].ToString().ToUpper() == "C")
                            {
                                Store st = new Store(adr.Session);
                                st.Name = Request["StoreName"].ToString();
                                st.Code = Request["StoreCode"].ToString();
                                st.IsCentralStore = Request["IsCentralStore"].ToString().ToUpper() == "C";
                                if (!st.IsCentralStore)
                                {
                                    Guid centralStoreGuid = Guid.Empty;
                                    if (Guid.TryParse(Request["CentralCb_VI"], out centralStoreGuid))
                                    {
                                        foreach (Address fadr in t.Addresses)
                                        {
                                            if (fadr.Store != null && fadr.Store.Oid == centralStoreGuid)
                                            {
                                                st.Central = fadr.Store;
                                                break;
                                            }
                                        }
                                    }
                                }
                                st.Address = adr;
                                CompanyNew sup = Session["Supplier"] as CompanyNew;
                                sup.Stores.Add(st);
                            }
                        }
                        t.Addresses.Add(adr);
                        Session["NewAddress"] = null;
                    }
                    else
                    {
                        throw new Exception("Unhandled Situation: Code -Adr1-");
                    }
                    return PartialView("Grid", (t.Addresses));
                }
            }
            else
                Session["Error"] = Resources.AnErrorOccurred;

            if (Session["NewTrader"] != null && Boolean.Parse(Session["NewTrader"].ToString()))
            {

                return (PartialView("Grid", (((Trader)Session["Trader"]).Addresses.AsEnumerable<Address>())));
            }
            else
            {
                CriteriaOperator session = (CriteriaOperator)Session["AddressFilter"];
                return PartialView("Grid", (GetList<Address>(XpoHelper.GetNewUnitOfWork(), session).AsEnumerable<Address>()));
            }
        }
        [HttpPost]
        public ActionResult InlineEditingUpdatePartial([ModelBinder(typeof(RetailModelBinder))] Address ct)
        {
            Guid SupID;
            ViewData["GDPREnabled"] = CurrentUser.Role.GDPREnabled;
            if (!Guid.TryParse(Request["adrSupplierID"].ToString(), out SupID))
                SupID = Guid.Empty;
            ViewData["Supplier_ID"] = SupID;
            if (String.IsNullOrWhiteSpace(ct.Profession))
            {
                ModelState.AddModelError("Profession", Resources.AddressProfessionIsEmpty);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["NewTrader"] != null && Boolean.Parse(Session["NewTrader"].ToString()))
                    {
                        Trader t = (Trader)Session["Trader"];
                        Address ad = t.Addresses.FirstOrDefault(g => g.Oid == ct.Oid);
                        if (ad != null)
                        {
                            UpdateLookupObjects(ct);
                            Store store = ad.Store;
                            ad.GetData(ct);
                            ad.Store = store;
                            /* Supplier Store */
                            if (Request["IsStore"] != null)
                            {
                                if (Request["IsStore"].ToString().ToUpper() == "C")
                                {
                                    Store st = ad.Store;
                                    if (ad.Store == null)
                                        st = new Store(ad.Session);

                                    st.Name = Request["StoreName"].ToString();
                                    st.Code = Request["StoreCode"].ToString();
                                    st.Address = ct;
                                    st.IsCentralStore = Request["IsCentralStore"].ToString().ToUpper() == "C";
                                    if (!st.IsCentralStore)
                                    {
                                        Guid centralStoreGuid = Guid.Empty;
                                        if (Guid.TryParse(Request["CentralCb_VI"], out centralStoreGuid))
                                        {
                                            foreach (Address fadr in ad.Trader.Addresses)
                                            {
                                                if (fadr.Store != null && fadr.Store.Oid == centralStoreGuid)
                                                {
                                                    st.Central = fadr.Store;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    ad.Store = ct.Store;
                                    CompanyNew sup = Session["Supplier"] as CompanyNew;
                                    sup.Stores.Add(st);

                                }
                                else
                                {
                                    if (ad.Store != null)
                                    {
                                        ad.Store.Delete();
                                        ad.Store = null;
                                    }
                                }
                            }
                            ad.Trader = t;
                            Guid phoneGuid;
                            if (Request["DefaultPhoneCb_VI"] != null && Guid.TryParse(Request["DefaultPhoneCb_VI"].ToString(), out phoneGuid))
                            {
                                ad.DefaultPhone = ad.Phones.FirstOrDefault(x => x.Oid == phoneGuid);
                            }
                        }
                        ViewData["Trader_ID"] = t.Oid;
                    }
                    else
                    {
                        throw new Exception("Unhandled Situation.  Code -Adr5-");
                    }
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
                Session["Error"] = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            bool displayCommands = Request["displayCommands"] == null || Request["displayCommands"] == "null" ? true : Boolean.Parse(Request["displayCommands"]);
            ViewData["displayCommands"] = displayCommands;

            if (Session["NewTrader"] != null && Boolean.Parse(Session["NewTrader"].ToString()))
            {

                return PartialView("Grid", (((Trader)Session["Trader"]).Addresses));

            }
            else
            {
                return PartialView("Grid", (GetList<Address>(XpoHelper.GetNewUnitOfWork(), (CriteriaOperator)Session["AddressFilter"]).AsEnumerable<Address>()));
            }
        }

        [HttpPost]
        public ActionResult InlineEditingDeletePartial([ModelBinder(typeof(RetailModelBinder))] Address ct)
        {
            Guid SupID;
            ViewData["GDPREnabled"] = CurrentUser.Role.GDPREnabled;
            if (!Guid.TryParse(Request["adrSupplierID"].ToString(), out SupID))
            {
                SupID = Guid.Empty;
            }
            ViewData["Supplier_ID"] = SupID;
            try
            {
                if (Session["NewTrader"] != null && Boolean.Parse(Session["NewTrader"].ToString()))
                {
                    Trader t = (Trader)Session["Trader"];
                    Address address = t.Addresses.FirstOrDefault(adrs => adrs.Oid == ct.Oid);
                    XPCollection<DocumentHeader> documentsFoundWithAddress = GetList<DocumentHeader>(t.Session, CriteriaOperator.And(new BinaryOperator("BillingAddress", address),
                                                                                                                CriteriaOperator.Or(new BinaryOperator("Customer.Trader", t),
                                                                                                                                    new BinaryOperator("Supplier.Trader", t))));
                    if (documentsFoundWithAddress.Count == 0)
                    {
                        t.Addresses.Remove(address);
                        Customer cust = t.Session.FindObject<Customer>(new BinaryOperator("Trader", t.Oid));
                        SupplierNew suppl = t.Session.FindObject<SupplierNew>(new BinaryOperator("Trader", t.Oid));
                        if (cust != null)
                        {
                            Guid defaultAdr;
                            if (Guid.TryParse(Request["currentDefaultAddress"], out defaultAdr))
                            {
                                if (defaultAdr == address.Oid)
                                {
                                    cust.DefaultAddress = null;
                                }
                            }
                        }

                        if (suppl != null)
                        {
                            Guid defaultAdr;
                            if (Guid.TryParse(Request["currentDefaultAddress"], out defaultAdr))
                            {
                                if (defaultAdr == address.Oid)
                                {
                                    suppl.DefaultAddress = null;
                                }
                            }
                        }
                        address.Delete();
                        ViewData["Trader_ID"] = t.Oid;

                    }
                    else
                    {
                        ViewData["Trader_ID"] = t.Oid;
                        throw new Exception(Resources.AddressUsedAsBilling);
                    }

                }
                else
                {
                    throw new Exception("Unhandled Situation.  Code -Adr3-");
                }
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }

            FillLookupComboBoxes();
            bool displayCommands = Request["displayCommands"] == null || Request["displayCommands"] == "null" ? true : Boolean.Parse(Request["displayCommands"]);
            ViewData["displayCommands"] = displayCommands;
            if (Session["NewTrader"] != null && Boolean.Parse(Session["NewTrader"].ToString()))
            {
                return (PartialView("Grid", (((Trader)Session["Trader"]).Addresses.AsEnumerable<Address>())));
            }
            else
            {
                return PartialView("Grid", GetList<Address>(XpoHelper.GetNewUnitOfWork(), (CriteriaOperator)Session["AddressFilter"]).AsEnumerable<Address>());
            }
        }

        public ActionResult UpdatePhoneCombobox()
        {
            if (Session["EditingStore"] is Store)
            {
                Store store = (Store)Session["EditingStore"];
                ViewBag.DefaultPhoneComboBox = store.Address.Phones.OrderBy(phone => phone.PhoneType.Description);
                PartialView("UpdatePhoneCombobox", store.Address.DefaultPhone);
            }
            FillLookupComboBoxes();
            Address address = (Address)Session["NewAddress"];
            return PartialView("UpdatePhoneCombobox", address != null && address.DefaultPhone != null ? address.DefaultPhone.Oid : Guid.Empty);
        }

        public JsonResult CreateStore(string SupplierID, string AddressID)
        {

            Guid SupID, AdrID;
            if (!Guid.TryParse(SupplierID, out SupID))
            {
                throw new Exception();
            }
            if (!Guid.TryParse(AddressID, out AdrID))
            {
                throw new Exception();
            }


            CompanyNew sup = XpoSession.FindObject<CompanyNew>(new BinaryOperator("Oid", SupID));
            Address adr = XpoSession.FindObject<Address>(new BinaryOperator("Oid", AdrID));
            if (sup == null && adr == null)
            {
                bool isnewtrader = (bool)Session["NewTrader"];
                Trader trd = Session["Trader"] as Trader;
                sup = Session["Supplier"] as CompanyNew;
                UnitOfWork uow = Session["TraderUow"] as UnitOfWork;

                if (sup == null || sup.Oid != SupID)
                    throw new Exception();
                Address addressToFind = null;
                foreach (Address ad in sup.Trader.Addresses)
                {
                    if (ad.Oid == AdrID)
                    {
                        addressToFind = ad;
                        break;
                    }
                }
                if (addressToFind == null)
                {
                    throw new Exception();
                }
                Store str = new Store(uow);
                str.Address = addressToFind;
                sup.Stores.Add(str);
                return Json(new { storeid = str.Oid.ToString(), supplieroid = sup.Oid.ToString().Replace('-', '_'), traderoid = sup.Trader.Oid.ToString().Replace('-', '_') });
            }
            else
            {

                if (adr.Trader.Oid == sup.Trader.Oid)
                {
                    if (adr.Store != null)
                    {
                        sup.Stores.Add(adr.Store);
                        sup.Save();
                        //sup.Session.CommitTransaction();
                        XpoHelper.CommitTransaction(sup.Session);
                        return Json(new { storeid = adr.Store.Oid.ToString(), supplieroid = sup.Oid.ToString().Replace('-', '_'), traderoid = sup.Trader.Oid.ToString().Replace('-', '_') });
                    }
                    else
                    {
                        Store str = new Store(XpoSession);
                        str.Address = adr;

                        sup.Stores.Add(str);
                        str.Save();
                        sup.Save();
                        //XpoSession.CommitTransaction();
                        XpoHelper.CommitTransaction(XpoSession);
                        return Json(new { storeid = str.Oid.ToString(), supplieroid = sup.Oid.ToString().Replace('-', '_'), traderoid = sup.Trader.Oid.ToString().Replace('-', '_') });
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
        }


        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            ViewBag.AddressTypeComboBox = GetList<AddressType>(XpoSession, sortingField: "Description");
            ViewBag.VatLevels = GetList<VatLevel>(XpoSession, sortingField: "Description");

            string objectName = Request.Params["DXCallbackName"];
            Guid key;
            if (ViewData["Trader_ID"] != null && ViewData["Trader_ID"].ToString() != "" && ViewData["Trader_ID"].ToString() != "null")
            {
                key = Guid.Parse(ViewData["Trader_ID"].ToString());
            }
            else if (objectName.Contains("grdAddressEdit"))
            {
                key = Guid.Parse(Request["TraderID"]);
            }
            else if (objectName.Contains("grdAddress"))
            {
                key = Guid.Parse(objectName.Replace("grdAddress", "").Replace('_', '-'));
            }
            else
            {
                key = (Request.Params["current_address"] == null || Request.Params["current_address"].ToString() == "-1" || Request.Params["current_address"].ToString() == "null" || Request.Params["current_address"].ToString() == "")
                ? Guid.Empty : Guid.Parse(Request.Params["current_address"].ToString());
            }


            if (Session["NewAddress"] != null)
            {
                Address ad = (Address)Session["NewAddress"];
                ViewBag.DefaultPhoneComboBox = ad.Phones.OrderBy(phone => phone.PhoneType);

            }
            else if (Session["Trader"] != null)
            {
                Trader tr = (Trader)Session["Trader"];
                foreach (Address ad in tr.Addresses)
                {
                    if (ad.Oid == key)
                    {
                        ViewBag.DefaultPhoneComboBox = ad.Phones.OrderBy(phone => phone.PhoneType);
                    }
                }
                List<Store> storelist = new List<Store>();
                foreach (Address ad in tr.Addresses)
                {
                    if (ad.Store != null && ad.Store.IsCentralStore)
                    {
                        storelist.Add(ad.Store);
                    }
                }
                ViewBag.CentralStoreList = storelist;
            }
            else
            {
                ViewBag.DefaultPhoneComboBox = GetList<Phone>(XpoHelper.GetNewUnitOfWork(), new BinaryOperator("Address.Oid", key),
                    //CriteriaOperator.Parse("Address.Oid='" + key.ToString() + "'", ""),
                    "PhoneType.Description");
            }
        }

        protected override void UpdateLookupObjects(Address a)
        {
            base.UpdateLookupObjects(a);
            a.AddressType = GetObjectByArgument<AddressType>(a.Session, "AddressTypeCb_VI");
            a.Trader = GetObjectByArgument<Trader>(a.Session, "TraderID");
            a.VatLevel = GetObjectByArgument<VatLevel>(a.Session, "VatLevelCb_VI");
        }

    }
}

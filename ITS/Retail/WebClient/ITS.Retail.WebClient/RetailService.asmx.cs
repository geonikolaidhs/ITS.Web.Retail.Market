using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using System.Xml;
using DevExpress.Xpo.DB;
using System.IO;
using System.Diagnostics;
using Shell32;
using ITS.Retail.WebClient.Helpers;
using Newtonsoft.Json;
using System.Data.SQLite;
using Ionic.Zip;

using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using ITS.Retail.WebClient.gr.gsis.ws;
using ITS.Retail.Common.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

#if !_RETAIL_STORECONTROLLER
using System.Collections;
using System.Xml.Serialization;
using ITS.Retail.Platform.Enumerations;
using System.Security.Cryptography;
using ITS.Retail.Platform;
using ITS.Retail.ResourcesLib;



namespace ITS.Retail.WebClient
{
    #region Auxilliary Classes
    public struct TransferedDocumentDetail
    {
        public String Barcode, ItemCode, ItemName;
        public double FinalUnitPrice, FirstDiscount, GrossTotal, UnitPrice, Qty, NetTotal, NetTotalAfterDiscount, SecondDiscount, TotalDiscount, TotalVatAmount, UnitPriceAfterDiscount, VatAmount, VatFactor;
        public Guid ItemOid;
    }

    public struct TCustomer
    {
        public String Code, CompanyName, TaxCode;
        public Guid Oid, PriceListOid, VatLevel, StoreOid;
        public String DefaultAddress, DefaultPhone;
        public long UpdateOn;

    }

    public struct TDocumentStatus
    {
        public Guid Oid;
        public String Description;
        public bool IsDefault;
    }    

    public struct TAppSettings
    {
        public bool BarcodePad, CodePad, DiscountPermited;
        public String BarcodePadChar, CodePadChar;
        public int BarcodePadLength, CodePadLength, ComputeDigits, ComputeValueDigits, DisplayDigits, DisplayValueDigits;
    }

    public struct TStore
    {
        public Guid Oid;
        public string Code, Name;
        public bool IsCentralStore;
        public long UpdatedOn;
    }

    public struct TUserStoreAccess
    {
        public Guid UserID, StoreID;
        public long UpdatedOn;
    }

    public enum InactiveItemReason
    {        
        INACTIVE,
        NOPRICE,
        NOT_FOUND
    }

    public struct TInvalidItem
    {
        public Guid ItemOid;
        public InactiveItemReason Reason;
    }

    public struct TDocumentHeader
    {
        public long finalizedDateTime;
        public double netTotal, discount, final;
    }
    #endregion


    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "https://weborders.masoutis.gr/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class RetailService : System.Web.Services.WebService
    {


        private int MAX_ITEMS = 1000;
        private readonly String DELIMITER = "\t&\t";
        ApplicationSettings application_settings;
        public RetailService()
        {

            this.application_settings = XpoHelper.GetNewUnitOfWork().FindObject<ApplicationSettings>(null);

        }

        /// <summary>
        ///  type=1 : customers only, type=2 : suppliers only ,type = else : any
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pass"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [WebMethod]
        public Guid Login(string username, string pass, int type)
        {
            try
            {
                User user = (User)XpoHelper.GetNewUnitOfWork().FindObject<User>(CriteriaOperator.Parse("UserName='" + username + "' and Password='" + UserHelper.EncodePassword(pass) + "'", ""));
                if (user == null)
                {
                    return Guid.Empty;
                }

                bool isValidType = true;
                switch (type)
                {
                    case 1: // check if customer
                        isValidType = UserHelper.IsCustomer(user);
                        break;
                    case 2: // check if supplier
                        isValidType = UserHelper.IsCompanyUser(user);
                        break;
                }


                if (isValidType)
                {
                    return user.Oid;
                }

                return Guid.Empty;
            }
            catch (Exception ex)
            {
                MvcApplication.WRMLogModule.Log(ex, "RetailService: Login", KernelLogLevel.Error);                
                return Guid.Empty;
            }
        }

        protected bool CanPostType(string type, string device)
        {
            return true;
        }

        [WebMethod]
        public bool PostData(string typeName, string data, string device)
        {
            if (!CanPostType(typeName, device))
            {
                return false;
            }
            return true;
        }

        [WebMethod]
        public string GetSeries(string Userid, int mode)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {

                    User user = uow.FindObject<User>(CriteriaOperator.Parse("Oid='" + Userid + "'", ""));
                    if (user != null)
                    {

                        if (mode == 0)
                        {
                            XmlCreator loginxml = new XmlCreator("Login");
                            List<string> data = new List<string>();

                            Customer customer = ITS.Retail.Common.Helpers.BOApplicationHelper.GetUserEntities<Customer>(uow, user).FirstOrDefault<Customer>();
                            if (customer == null)
                            {
                                NewXmlCreator a = new NewXmlCreator("1", "Customer not found");
                                a.Xmlclose();
                                return a.MyXml;
                            }

                            Store store = customer.DefaultStore;
                            if (store == null)
                            {
                                NewXmlCreator a = new NewXmlCreator("1", "Store where user buys from not found");
                                a.Xmlclose();
                                return a.MyXml;
                            }


                            data.Clear();
                            data.Add("customerid|" + customer.Oid);
                            data.Add("customername|" + customer.Trader.LastName);
                            loginxml.CreateNodes("customer", data.ToArray());


                            data.Clear();
                            data.Add("storeid|" + store.Oid);
                            loginxml.CreateNodes("store", data.ToArray());

                            data.Clear();
                            data.Add("companyid|" + store.Owner.Oid);
                            data.Add("companyname|" + store.Owner.CompanyName);
                            loginxml.CreateNodes("company", data.ToArray());
                            XPQuery<StoreDocumentSeriesType> query = new XPQuery<StoreDocumentSeriesType>(uow);

                            foreach (var item in (from obj in query
                                                  where obj.DocumentSeries.Store.Oid == customer.DefaultStore.Oid
                                                  select new { Oid = obj.DocumentType.Oid, Description = obj.DocumentType.Description, IsDefault = obj.DocumentType.IsDefault }).Distinct().ToList())
                            {
                                data.Clear();
                                data.Add("oid|" + item.Oid);
                                data.Add("name|" + item.Description);
                                data.Add("default|" + item.IsDefault);
                                loginxml.CreateNodes("types", data.ToArray());
                            }


                            foreach (var item in (from obj in query
                                                  where obj.DocumentSeries.Store.Oid == customer.DefaultStore.Oid
                                                  select new { Oid = obj.DocumentType.Oid, Description = obj.DocumentType.Description }).Distinct().ToList())
                            {
                                List<DocumentSeries> Dc = customer.DefaultStore.DocumentSeries.Where(g => g.StoreDocumentSeriesTypes.Where(x => x.DocumentType.Oid.Equals(item.Oid)).Count() > 0).ToList();
                                for (int i = 0; i < Dc.Count; i++)
                                {
                                    data.Clear();
                                    data.Add("oid|" + Dc[i].Oid);
                                    data.Add("name|" + Dc[i].Description);
                                    data.Add("type|" + item.Oid);
                                    loginxml.CreateNodes("series", data.ToArray());
                                }
                            }


                            XPCollection<DocumentStatus> status = GetList<DocumentStatus>(uow, "Code");

                            foreach (DocumentStatus st in status)
                            {
                                data.Clear();
                                data.Add("oid|" + st.Oid);
                                data.Add("description|" + st.Description);
                                data.Add("IsDefault|" + st.IsDefault);
                                loginxml.CreateNodes("status", data.ToArray());
                            }

                            loginxml.Xmlclose();
                            return loginxml.MyXml;
                        }
                        else
                        {
                            NewXmlCreator a = new NewXmlCreator("1", "Trader not found");
                            a.Xmlclose();
                            return a.MyXml;
                        }
                    }
                    else
                    {
                        NewXmlCreator a = new NewXmlCreator("1", "Login failed");
                        a.Xmlclose();
                        return a.MyXml;
                    }
                }
            }
            catch (Exception ex)
            {
                MvcApplication.WRMLogModule.Log(ex, "RetailService: GetSeries",KernelLogLevel.Error);
                NewXmlCreator a = new NewXmlCreator("1", ex.Message + ex.StackTrace);
                a.Xmlclose();
                return a.MyXml;
            }
        }


        [WebMethod]
        public TDocumentStatus[] GetDocumentStatus()
        {
            IEnumerable<TDocumentStatus> list = from obj in GetList<DocumentStatus>(XpoHelper.GetNewUnitOfWork(), "Code")
                                                select new TDocumentStatus { Oid = obj.Oid, Description = obj.Description, IsDefault = obj.IsDefault };
            return list.ToArray();
        }

        [WebMethod]
        public string GetItem(string Sfilter, string Sfields, string UserID)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Guid cguid;
                    bool cgcorrect = Guid.TryParse(UserID, out cguid);
                    if (!cgcorrect)
                    {
                        return "-1";
                    }
                    User user = uow.FindObject<User>(new BinaryOperator("Oid", cguid));

                    Customer customer = BOApplicationHelper.GetUserEntities<Customer>(uow, user).FirstOrDefault<Customer>();
                    //XPCollection<Item> validItems = cust.GetPriceCatalog().GetItems();


                    PriceCatalog pc = customer.GetDefaultPriceCatalog();

                    if (customer.Owner.OwnerApplicationSettings.PadItemCodes && Sfilter.ToUpper().Contains("CODE"))
                    {
                        int lp, rp;
                        lp = Sfilter.IndexOf("'");
                        rp = Sfilter.LastIndexOf("'");
                        if (rp - lp + 1 < customer.Owner.OwnerApplicationSettings.ItemCodeLength)
                        {
                            String left, code, right;
                            left = Sfilter.Substring(0, lp + 1);
                            right = Sfilter.Substring(rp);
                            code = Sfilter.Substring(lp + 1, rp - lp - 1);

                            Sfilter = left + code.PadLeft(customer.Owner.OwnerApplicationSettings.ItemCodeLength, customer.Owner.OwnerApplicationSettings.ItemCodePaddingCharacter[0]) + right;
                        }
                    }

                    CompanyNew supplier = customer.DefaultStore.Owner;
                    CriteriaOperator filter2 = null;

                    filter2 = CriteriaOperator.And(CriteriaOperator.Parse(Sfilter), new BinaryOperator("Owner.Oid", supplier.Oid));

                    Item itm = uow.FindObject<Item>(filter2);
                    if (itm == null || itm.IsActive == false)
                        return "-1";
                    CriteriaOperator criteria = CriteriaOperator.And(
                        new BinaryOperator("Item", itm),
                        new BinaryOperator("Barcode", itm.DefaultBarcode),
                        new BinaryOperator("PriceCatalog", pc),
                        new BinaryOperator("IsActive",true)
                        );

                    PriceCatalogDetail pcd = uow.FindObject<PriceCatalogDetail>(criteria);
                    if (pcd == null)
                    {
                        return "-1";
                    }


                    using (XPCollection XP = new XPCollection(XpoHelper.GetNewUnitOfWork(), typeof(Item), filter2))
                    {
                        if (XP.Count > 0)
                        {
                            if (string.IsNullOrEmpty(Sfields))
                                return Serialize(XP, "Item");
                            else
                                return Serialize(XP, Sfields, "Item");
                        }
                        else
                        {
                            return "-1";
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                MvcApplication.WRMLogModule.Log(ee, "RetailService: GetItem", KernelLogLevel.Error);
                NewXmlCreator a = new NewXmlCreator("1", ee.Message + ee.StackTrace);
                a.Xmlclose();
                return a.MyXml;
            }
        }

        [WebMethod]
        public string GetItemWithBarcodeCustomer(string barcode, string Sfields, string CustomerID)
        {
            try
            {

                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Guid cguid;
                    bool cgcorrect = Guid.TryParse(CustomerID, out cguid);
                    if (!cgcorrect)
                    {
                        MvcApplication.WRMLogModule.Log("RetailService: GetItemWithBarcodeCustomer",KernelLogLevel.Info);
                        NewXmlCreator a = new NewXmlCreator("1", "Customer ID exception");
                        a.Xmlclose();
                        return a.MyXml;
                    }

                    Customer customer = uow.GetObjectByKey<Customer>(cguid);
                    if (customer == null)
                    {
                        MvcApplication.WRMLogModule.Log("RetailService: GetItemWithBarcodeCustomer Null Customer", KernelLogLevel.Info);
                        NewXmlCreator a = new NewXmlCreator("1", "Customer null exception");
                        a.Xmlclose();
                        return a.MyXml;
                    }

                    if (customer.Owner.OwnerApplicationSettings.PadBarcodes)
                    {
                        barcode = barcode.PadLeft(customer.Owner.OwnerApplicationSettings.BarcodeLength, customer.Owner.OwnerApplicationSettings.BarcodePaddingCharacter[0]);
                    }

                    PriceCatalog pc = customer.GetDefaultPriceCatalog();
                    Barcode bc = uow.FindObject<Barcode>(new BinaryOperator("Code", barcode));
                    CompanyNew supplier = customer.DefaultStore.Owner;
                    if (bc == null /*|| bc.ItemBase.IsActive == false*/ || bc.IsActive == false)
                    {
                        return "-1";
                    }
                    Item item = ItemHelper.GetItemOfSupplier(uow, bc, supplier);

                    decimal itemprice = item.GetUnitPrice(pc, bc);


                    if (itemprice > 0)
                    {

                        XPCollection XP = new XPCollection(uow, typeof(Item), new BinaryOperator("Oid", item.Oid));
                        if (string.IsNullOrEmpty(Sfields))
                        {
                            return Serialize(XP, "Item");
                        }
                        else
                        {
                            return Serialize(XP, Sfields, "Item");
                        }
                    }
                    else
                    {
                        return "-1";
                    }
                }
            }
            catch (Exception ee)
            {
                MvcApplication.WRMLogModule.Log(ee, "RetailService: GetItemGetItemWithBarcode", KernelLogLevel.Error);

                NewXmlCreator a = new NewXmlCreator("1", ee.Message + " " + ee.StackTrace);

                a.Xmlclose();
                return a.MyXml;
            }
        }

        [WebMethod]
        public string GetItemWithBarcode(string barcode, string Sfields, string UserID)
        {
            try
            {

                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Guid cguid;
                    bool cgcorrect = Guid.TryParse(UserID, out cguid);
                    if (!cgcorrect)
                        return "-1";
                    User user = uow.FindObject<User>(new BinaryOperator("Oid", cguid));

                    Customer customer = BOApplicationHelper.GetUserEntities<Customer>(uow, user).FirstOrDefault<Customer>();
                    if (customer.Owner.OwnerApplicationSettings.PadBarcodes)
                    {
                        barcode = barcode.PadLeft(customer.Owner.OwnerApplicationSettings.BarcodeLength, customer.Owner.OwnerApplicationSettings.BarcodePaddingCharacter[0]);
                    }

                    PriceCatalog pc = customer.GetDefaultPriceCatalog();
                    Barcode bc = uow.FindObject<Barcode>(new BinaryOperator("Code", barcode));
                    CompanyNew supplier = customer.DefaultStore.Owner;

                    if (bc == null || bc.IsActive == false)
                        return "-1";

                    Item item = ItemHelper.GetItemOfSupplier(uow, bc, supplier);
                    decimal itemprice = item.GetUnitPrice(pc, bc);

                    if (itemprice > 0)
                    {

                        XPCollection XP = new XPCollection(uow, typeof(Item), new BinaryOperator("Oid", item.Oid));
                        if (string.IsNullOrEmpty(Sfields))
                            return Serialize(XP, "Item");
                        else
                            return Serialize(XP, Sfields, "Item");
                    }
                    else
                    {
                        return "-1";

                    }
                }
            }
            catch (Exception ee)
            {
                MvcApplication.WRMLogModule.Log(ee, "RetailService: GetItemGetItemWithBarcode", KernelLogLevel.Error);

                NewXmlCreator a = new NewXmlCreator("1", ee.Message + " " + ee.StackTrace);

                a.Xmlclose();
                return a.MyXml;
            }
        }

        [WebMethod]
        public string GetPrice(string UserID, string ItemOid, string barcode)
        {

            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Guid uid;
                    if (!Guid.TryParse(UserID, out uid))
                    {
                        NewXmlCreator a = new NewXmlCreator("1", "Customer not found");
                        a.Xmlclose();
                        return a.MyXml;
                    }
                    User user = uow.FindObject<User>(new BinaryOperator("Oid", uid));

                    Customer customer = BOApplicationHelper.GetUserEntities<Customer>(uow, user).FirstOrDefault<Customer>();

                    Item item = uow.FindObject<Item>(CriteriaOperator.Parse("Oid='" + ItemOid + "'", ""));

                    if (customer != null)
                    {
                        if (customer.Owner.OwnerApplicationSettings.PadBarcodes)
                        {
                            barcode = barcode.PadLeft(customer.Owner.OwnerApplicationSettings.BarcodeLength, customer.Owner.OwnerApplicationSettings.BarcodePaddingCharacter[0]);
                        }

                        PriceCatalog catalaog = customer.GetDefaultPriceCatalog();
                        decimal price = 0;

                        if (string.IsNullOrEmpty(barcode))
                        {
                            price = item.GetUnitPrice(catalaog);
                            //   DocumentDetail detail = new BODocumentDetail().ComputeDocumentLine(uow, cust, item, null, 0);
                        }
                        else
                        {
                            Barcode bar = XpoHelper.GetNewSession().FindObject<Barcode>(CriteriaOperator.Parse("Code='" + barcode + "'", ""));
                            if (bar != null)
                                price = item.GetUnitPrice(catalaog, bar);
                            else
                                price = item.GetUnitPrice(catalaog);

                            // DocumentDetail detail = new BODocumentDetail().ComputeDocumentLine(uow, cust, item, bar, 0);
                        }

                        List<string> data = new List<string>();
                        XmlCreator itemxml = new XmlCreator("Item");

                        data.Add("price|" + price.ToString());
                        itemxml.CreateNodes("price", data.ToArray());
                        itemxml.Xmlclose();
                        return itemxml.MyXml;

                    }
                    else
                    {
                        NewXmlCreator a = new NewXmlCreator("1", "Customer not found");
                        a.Xmlclose();
                        return a.MyXml;
                    }
                }
            }
            catch (Exception ex)
            {
                MvcApplication.WRMLogModule.Log(ex, "RetailService: GetPrice", KernelLogLevel.Error);
                string message = ex.Message;
                if (message.Contains("PriceNotExistException"))
                    message = "Το είδος δεν υπάρχει στον κατάλογο";
                else
                    message = ex.Message + " " + ex.StackTrace;
                NewXmlCreator a = new NewXmlCreator("1", message);
                a.Xmlclose();
                return a.MyXml;
            }
        }

        [WebMethod]
        public double GetPriceCustomer(string CustomerID, string ItemOid, string barcode)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Guid uid;
                    if (!Guid.TryParse(CustomerID, out uid))
                    {
                        return -1;
                    }


                    Customer customer = uow.GetObjectByKey<Customer>(uid);

                    Item item = uow.FindObject<Item>(CriteriaOperator.Parse("Oid='" + ItemOid + "'", ""));

                    if (customer != null)
                    {
                        if (customer.Owner.OwnerApplicationSettings.PadBarcodes)
                        {
                            barcode = barcode.PadLeft(customer.Owner.OwnerApplicationSettings.BarcodeLength, customer.Owner.OwnerApplicationSettings.BarcodePaddingCharacter[0]);
                        }
                        PriceCatalog catalaog = customer.GetDefaultPriceCatalog();
                        decimal price = 0;

                        if (string.IsNullOrEmpty(barcode))
                        {
                            price = item.GetUnitPrice(catalaog);
                            //   DocumentDetail detail = new BODocumentDetail().ComputeDocumentLine(uow, cust, item, null, 0);
                        }
                        else
                        {
                            Barcode bar = uow.FindObject<Barcode>(CriteriaOperator.Parse("Code='" + barcode + "'", ""));
                            if (bar != null)
                                price = item.GetUnitPrice(catalaog, bar);
                            else
                                price = item.GetUnitPrice(catalaog);

                            // DocumentDetail detail = new BODocumentDetail().ComputeDocumentLine(uow, cust, item, bar, 0);
                        }

                        List<string> data = new List<string>();
                        XmlCreator itemxml = new XmlCreator("Item");

                        return (double)price;

                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MvcApplication.WRMLogModule.Log(ex, "RetailService: GetPrice", KernelLogLevel.Error);
                string message = ex.Message;
                if (message.Contains("PriceNotExistException"))
                    message = "Το είδος δεν υπάρχει στον κατάλογο";
                else
                    message = ex.Message + " " + ex.StackTrace;
                return -1;
            }
        }

        [WebMethod]
        public TransferedDocumentDetail[] GetDocumentDetailCustomer(string customerString, string barcode, double qty)
        {
            try
            {

                Guid userGuid;
                if (Guid.TryParse(customerString, out userGuid))
                {
                    decimal Qty = (decimal)qty;
                    if (Qty >= 0)
                    {
                        UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                        Customer customer = uow.GetObjectByKey<Customer>(userGuid);
                        if (customer.Owner.OwnerApplicationSettings.PadBarcodes)
                        {
                            barcode = barcode.PadLeft(customer.Owner.OwnerApplicationSettings.BarcodeLength, customer.Owner.OwnerApplicationSettings.BarcodePaddingCharacter[0]);
                        }
                        Store store = customer.DefaultStore;
                        PriceCatalog priceCatalog = customer.GetDefaultPriceCatalog();
                        DocumentHeader docHead = new DocumentHeader(uow);
                        docHead.Customer = customer;
                        docHead.Store = store;
                        Barcode barcod = uow.FindObject<Barcode>(new BinaryOperator("Code", barcode));
                        PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetail(store,barcod.Code, customer);
                        PriceCatalogDetail pcdt = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                        CompanyNew supplier = customer.DefaultStore.Owner;
                        Item item = ItemHelper.GetItemOfSupplier(uow, barcod, supplier);
                        DocumentDetail detail = DocumentHelper.ComputeDocumentLine(ref docHead, item, barcod, Qty, false, -1, false, "", null);
                        DocumentHelper.AddItem(ref docHead, detail);

                        List<TransferedDocumentDetail> details = new List<TransferedDocumentDetail>();
                        bool isWholeSale = docHead.DocumentType == null ? true : docHead.DocumentType.IsForWholesale;
                        foreach (DocumentDetail det in docHead.DocumentDetails)
                        {
                            TransferedDocumentDetail dt;
                            dt.Barcode = det.Barcode.Code;
                            dt.FinalUnitPrice = (double)det.FinalUnitPrice;
                            dt.FirstDiscount = (double)det.PriceCatalogDiscountPercentage;
                            dt.GrossTotal = (double)det.GrossTotal;
                            dt.ItemCode = det.Item.Code;
                            dt.ItemName = det.Item.Name;
                            dt.ItemOid = det.Item.Oid;
                            dt.NetTotal = (double)det.NetTotal;
                            dt.NetTotalAfterDiscount = (double)det.NetTotal;
                            dt.Qty = (double)det.Qty;
                            dt.SecondDiscount = (double)det.GetCustomDiscountsPercentage(isWholeSale);
                            dt.TotalDiscount = (double)det.TotalDiscount;
                            dt.TotalVatAmount = (double)det.TotalVatAmount;
                            dt.UnitPrice = (double)det.UnitPrice;
                            dt.UnitPriceAfterDiscount = (double)det.UnitPriceAfterDiscount;
                            dt.VatAmount = (double)det.TotalVatAmount;
                            dt.VatFactor = (double)det.VatFactor;
                            details.Add(dt);
                        }
                        return details.ToArray<TransferedDocumentDetail>();
                    }
                    else
                    {
                        List<TransferedDocumentDetail> dt = new List<TransferedDocumentDetail>();
                        TransferedDocumentDetail d = new TransferedDocumentDetail();
                        d.Qty = -1;
                        d.ItemName = "Wrong Qty";
                        d.Barcode = d.ItemCode = null;
                        d.ItemOid = Guid.Empty;
                        dt.Add(d);
                        return dt.ToArray<TransferedDocumentDetail>();
                    }

                }
                else
                {
                    List<TransferedDocumentDetail> dt = new List<TransferedDocumentDetail>();
                    TransferedDocumentDetail d = new TransferedDocumentDetail();
                    d.Qty = -1;
                    d.ItemName = "Wrong Customer ID";
                    d.Barcode = d.ItemCode = null;
                    d.ItemOid = Guid.Empty;
                    dt.Add(d);
                    return dt.ToArray<TransferedDocumentDetail>();
                }
            }
            catch (Exception ee)
            {
                MvcApplication.WRMLogModule.Log(ee, "RetailService: GetDocumentDetail", KernelLogLevel.Error);
            }
            return null;
        }

        [WebMethod]
        public TransferedDocumentDetail[] GetDocumentDetail(string userid, string barcode, double qty)
        {
            try
            {

                Guid userGuid;
                if (Guid.TryParse(userid, out userGuid))
                {
                    decimal Qty = (decimal)qty;
                    if (Qty >= 0)
                    {
                        UnitOfWork uow = XpoHelper.GetNewUnitOfWork();

                        User user = uow.FindObject<User>(new BinaryOperator("Oid", userGuid));
                        Customer customer = BOApplicationHelper.GetUserEntities<Customer>(uow, user).FirstOrDefault<Customer>();
                        if (customer.Owner.OwnerApplicationSettings.PadBarcodes)
                        {
                            barcode = barcode.PadLeft(customer.Owner.OwnerApplicationSettings.BarcodeLength, customer.Owner.OwnerApplicationSettings.BarcodePaddingCharacter[0]);
                        }
                        Store store = customer.DefaultStore;
                        PriceCatalog priceCatalog = customer.GetDefaultPriceCatalog();
                        DocumentHeader docHead = new DocumentHeader(uow) ;

                        docHead.DocumentType = uow.FindObject<DocumentType>(new BinaryOperator("IsDefault", true));
                        if (docHead.DocumentType == null)
                            docHead.DocumentType = uow.FindObject<DocumentType>(null);

                        docHead.Store = store;
                        docHead.Customer = customer;
                        Barcode barcod = uow.FindObject<Barcode>(new BinaryOperator("Code", barcode));
                        PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetail(store, barcod.Code, customer);
                        PriceCatalogDetail pcdt = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                        CompanyNew supplier = customer.DefaultStore.Owner;
                        Item item = ItemHelper.GetItemOfSupplier(uow, barcod, supplier);

                        DocumentDetail detail = DocumentHelper.ComputeDocumentLine(ref docHead, item, barcod, Qty, false, -1, false, "", null);//TOCHECK
                        DocumentHelper.AddItem(ref docHead, detail);

                        List<TransferedDocumentDetail> details = new List<TransferedDocumentDetail>();
                        bool isWholeSale = docHead.DocumentType == null ? true : docHead.DocumentType.IsForWholesale;
                        foreach (DocumentDetail det in docHead.DocumentDetails)
                        {
                            TransferedDocumentDetail dt;
                            dt.Barcode = det.Barcode.Code;
                            dt.FinalUnitPrice = (double)det.FinalUnitPrice;
                            dt.FirstDiscount = (double)det.PriceCatalogDiscountPercentage;
                            dt.GrossTotal = (double)det.GrossTotal;
                            dt.ItemCode = det.Item.Code;
                            dt.ItemName = det.Item.Name;
                            dt.ItemOid = det.Item.Oid;
                            dt.NetTotal = (double)det.NetTotal;
                            dt.NetTotalAfterDiscount = (double)det.NetTotal;
                            dt.Qty = (double)det.Qty;
                            dt.SecondDiscount = (double)det.GetCustomDiscountsPercentage(isWholeSale);
                            dt.TotalDiscount = (double)det.TotalDiscount;
                            dt.TotalVatAmount = (double)det.TotalVatAmount;
                            dt.UnitPrice = (double)det.UnitPrice;
                            dt.UnitPriceAfterDiscount = (double)det.UnitPriceAfterDiscount;
                            dt.VatAmount = (double)det.TotalVatAmount;
                            dt.VatFactor = (double)det.VatFactor;
                            details.Add(dt);
                        }


                        return details.ToArray<TransferedDocumentDetail>();


                    }
                    else
                    {
                        List<TransferedDocumentDetail> dt = new List<TransferedDocumentDetail>();
                        TransferedDocumentDetail d = new TransferedDocumentDetail();
                        d.Qty = -1;
                        d.ItemName = "Wrong Qty";
                        d.Barcode = d.ItemCode = null;
                        d.ItemOid = Guid.Empty;
                        dt.Add(d);
                        return dt.ToArray<TransferedDocumentDetail>();
                    }

                }
                else
                {
                    List<TransferedDocumentDetail> dt = new List<TransferedDocumentDetail>();
                    TransferedDocumentDetail d = new TransferedDocumentDetail();
                    d.Qty = -1;
                    d.ItemName = "Wrong Customer ID";
                    d.Barcode = d.ItemCode = null;
                    d.ItemOid = Guid.Empty;
                    dt.Add(d);
                    return dt.ToArray<TransferedDocumentDetail>();

                }


            }
            catch (Exception ee)
            {
                MvcApplication.WRMLogModule.Log(ee,"RetailService: GetDocumentDetail", KernelLogLevel.Error);
            }
            return null;
        }

        [WebMethod]
        public List<TInvalidItem> ValidateOrder(string data)
        {
            NewXmlParser xmldata = new NewXmlParser(data);
            List<TInvalidItem> problematicItems = new List<TInvalidItem>();
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Customer customer = uow.FindObject<Customer>(CriteriaOperator.Parse("Oid='" + xmldata.GetProperty("Header", "customerid") + "'", ""));
                CompanyNew supplier = customer.DefaultStore.Owner;
                XmlNodeList nodelist = xmldata.GetNodeList("lines");
                for (int i = 0; i < nodelist.Count; i++)
                {
                    Barcode barcode = uow.FindObject<Barcode>(CriteriaOperator.Parse("Code='" + xmldata.GetProperty(nodelist[i], "item") + "'", ""));
                    Item item = (barcode == null) ? null : ItemHelper.GetItemOfSupplier(uow, barcode, supplier);
                    if (item == null)
                    {
                        item = uow.FindObject<Item>(CriteriaOperator.And(new BinaryOperator("Code", xmldata.GetProperty(nodelist[i], "item")), new BinaryOperator("Owner.Oid", supplier.Oid)));
                        barcode = item.DefaultBarcode;
                    }
                    if (item.IsActive == false)
                    {
                        problematicItems.Add(new TInvalidItem { ItemOid = item.Oid, Reason = InactiveItemReason.INACTIVE });
                    }
                    else if (item.GetUnitPrice(customer.GetDefaultPriceCatalog(), barcode) < 0)
                    {
                        problematicItems.Add(new TInvalidItem { ItemOid = item.Oid, Reason = InactiveItemReason.NOPRICE });
                    }
                }
            }
            return problematicItems;

        }

        [WebMethod]
        public string PostDocument(string data)
        {
            try
            {

                NewXmlParser xmldata = new NewXmlParser(data);

                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    /*UnitOfWork uow2 = XpoHelper.GetNewUnitOfWork();
                    ApplicationLog logAtDB = new ApplicationLog(uow2);
                    logAtDB.Controller = "RetailService";
                    logAtDB.Action = "PostDocument";
                    logAtDB.IPAddress = this.Context.Request.UserHostAddress;
                    logAtDB.DeviceName = this.Context.Request.UserHostName;
                    logAtDB.UserAgent = this.Context.Request.UserAgent;*/

                    DocumentHeader head = new DocumentHeader(uow);
                    User createdBy = null;
                    if (!string.IsNullOrEmpty(xmldata.GetProperty("Header", "User")))
                    {
                        Guid userGuid;
                        Guid.TryParse(xmldata.GetProperty("Header", "User"), out userGuid);
                        createdBy = uow.GetObjectByKey<User>(userGuid);
                        head.CreatedBy = createdBy;
                        head.UpdatedBy = createdBy;
                    }
                    Guid? userId = (createdBy != null) ? (Guid?)createdBy.Oid : null;
                    /*if (createdBy != null)
                        logAtDB.CreatedBy = uow2.GetObjectByKey<User>(createdBy.Oid);

                    logAtDB.Save();
                    XpoHelper.CommitChanges(uow2);*/
                    if (xmldata.GetProperty("Header", "Division") == "0")
                    {
                        head.Division = eDivision.Sales;
                    }
                    else if (xmldata.GetProperty("Header", "Division") == "1")
                    {
                        head.Division = eDivision.Purchase;
                    }
                    else if (xmldata.GetProperty("Header", "Division") == "2")
                    {
                        head.Division = eDivision.Store;
                    }

                    head.DocumentType = uow.FindObject<DocumentType>(new BinaryOperator("IsDefault", true));
                    if (head.DocumentType == null)
                    {
                        head.DocumentType = uow.FindObject<DocumentType>(null);
                    }

                    if (head.DocumentType.Division.Section != head.Division)
                    {
                        head.Division = head.DocumentType.Division.Section;
                    }

                    if (string.IsNullOrEmpty(xmldata.GetProperty("Header", "companyid")))
                    {
                        MvcApplication.WRMLogModule.Log(null, "Company is null", "RetailService", "PostDocument", this.Context.Request.UserAgent,
                            this.Context.Request.UserHostAddress, "", userId, KernelLogLevel.Info);
                        NewXmlCreator a = new NewXmlCreator("1", "Company is null");
                        a.Xmlclose();
                        return a.MyXml;
                    }

                    if (string.IsNullOrEmpty(xmldata.GetProperty("Header", "customerid")))
                    {
                        MvcApplication.WRMLogModule.Log(null, "Customer is null", "RetailService", "PostDocument", this.Context.Request.UserAgent,
                           this.Context.Request.UserHostAddress, "", userId, KernelLogLevel.Info);
                        NewXmlCreator a = new NewXmlCreator("1", "Customer is null");
                        a.Xmlclose();
                        return a.MyXml;
                    }

                    if (string.IsNullOrEmpty(xmldata.GetProperty("Header", "storeid")))
                    {
                        MvcApplication.WRMLogModule.Log(null, "Store is null", "RetailService", "PostDocument", this.Context.Request.UserAgent,
                          this.Context.Request.UserHostAddress, "", userId, KernelLogLevel.Info);
                        NewXmlCreator a = new NewXmlCreator("1", "Store is null");
                        a.Xmlclose();
                        return a.MyXml;
                    }


                    if (string.IsNullOrEmpty(xmldata.GetProperty("Header", "Status")))
                    {
                        MvcApplication.WRMLogModule.Log(null, "Status is null", "RetailService", "PostDocument", this.Context.Request.UserAgent,
                          this.Context.Request.UserHostAddress, "", userId, KernelLogLevel.Info);
                        NewXmlCreator a = new NewXmlCreator("1", "Status is null");
                        a.Xmlclose();
                        return a.MyXml;
                    }

                    Customer customer = uow.FindObject<Customer>(CriteriaOperator.Parse("Oid='" + xmldata.GetProperty("Header", "customerid") + "'", ""));

                    head.Store = customer.DefaultStore;
                    head.Customer = customer;
                    head.PriceCatalogPolicy = PriceCatalogHelper.GetPriceCatalogPolicy(head.Store, head.Customer);

                    if (!string.IsNullOrEmpty(xmldata.GetProperty("Header", "deliveryAddress")))
                    {
                        head.DeliveryAddress = xmldata.GetProperty("Header", "deliveryAddress");
                    }
                    else
                    {
                        head.DeliveryAddress = head.Customer.DefaultAddress == null ? "" : head.Customer.DefaultAddress.Description;
                    }

                    head.Remarks = "";
                    Guid headstatoid;
                    head.Status = null;
                    if (Guid.TryParse(xmldata.GetProperty("Header", "Status"), out headstatoid))
                    {
                        head.Status = uow.FindObject<DocumentStatus>(new BinaryOperator("Oid", headstatoid));
                    }
                    if (head.Status == null)
                    {
                        head.Status = uow.FindObject<DocumentStatus>(new BinaryOperator("IsDefault", true));
                    }
                    if (head.Status == null)
                    {
                        head.Status = uow.FindObject<DocumentStatus>(null);
                    }


                    if (!string.IsNullOrEmpty(xmldata.GetProperty("Header", "finalizeDate")))
                    {
                        try
                        {
                            long l = long.Parse(xmldata.GetProperty("Header", "finalizeDate"));
                            head.FinalizedDate = new DateTime(l);
                        }
                        catch (Exception)
                        {
                            head.FinalizedDate = DateTime.Now;
                        }
                    }
                    else
                    {
                        head.FinalizedDate = DateTime.Now;
                    }

                    try
                    {
                        if (!string.IsNullOrEmpty(xmldata.GetProperty("Header", "comments")))
                        {
                            head.Remarks = xmldata.GetProperty("Header", "comments");
                        }
                    }
                    catch// (Exception e)
                    {
                        //no comments found in the xml
                    }

                    DocumentSeries docSeries = StoreHelper.StoreSeriesForDocumentType(head.Store, head.DocumentType).FirstOrDefault();

                    if (docSeries != null)
                    {
                        head.DocumentSeries = docSeries;
                    }
                    else
                    {
                        string errorMessage = "No Document Series has been set up for specified store. Customer:" + head.Customer.CompanyName + 
                                                ", Store:" + head.Store != null ? head.Store.Name : "No store defined";
                        MvcApplication.WRMLogModule.Log(null, errorMessage, "RetailService", "PostDocument", this.Context.Request.UserAgent,
                          this.Context.Request.UserHostAddress, "", userId, KernelLogLevel.Info);                       
                        NewXmlCreator a = new NewXmlCreator("1", "Δεν εχουν καθοριστεί σειρές παραστατικών");
                        a.Xmlclose();
                        return a.MyXml;
                    }
                    CompanyNew supplier = head.Customer.DefaultStore.Owner;

                    XmlNodeList nodelist = xmldata.GetNodeList("lines");

                    for (int i = 0; i < nodelist.Count; i++)
                    {
                        DocumentDetail line;
                        Barcode barcode = uow.FindObject<Barcode>(CriteriaOperator.Parse("Code='" + xmldata.GetProperty(nodelist[i], "item") + "'", ""));
                        Item item = (barcode == null) ? null : ItemHelper.GetItemOfSupplier(uow, barcode, supplier);
                        if (item == null)
                        {
                            item = uow.FindObject<Item>(CriteriaOperator.And(new BinaryOperator("Code", xmldata.GetProperty(nodelist[i], "item")), new BinaryOperator("Owner.Oid", supplier.Oid)));
                            barcode = item.DefaultBarcode;
                        }
                        decimal Qty = Convert.ToInt32(xmldata.GetProperty(nodelist[i], "qty")) / 1000.0m;

                        decimal discount = 0;
                        List<DocumentDetailDiscount> discounts = new List<DocumentDetailDiscount>();
                        try
                        {
                            discount = Convert.ToInt32(xmldata.GetProperty(nodelist[i], "discount")) / 1000.0m;
                            if (discount > 0)
                            {
                                DocumentDetailDiscount discountNonPerst = new DocumentDetailDiscount(uow)
                                {
                                    DiscountSource = eDiscountSource.CUSTOM,
                                    DiscountType = eDiscountType.PERCENTAGE,
                                    Percentage = discount,
                                    Priority = 1
                                };
                                discounts.Add(discountNonPerst);
                            }

                        }
                        catch(Exception ex)
                        {
                            //no discount found
                            string exceptionMessage = ex.GetFullMessage();
                        }

                        decimal unitPrice = -1;
                        try
                        {
                            unitPrice = Convert.ToInt32(xmldata.GetProperty(nodelist[i], "unitPrice")) / 10000.0m;
                        }
                        catch (Exception ex)
                        {
                            //No custom price found
                            string exceptionMessage = ex.GetFullMessage();
                        }

                        PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetail(head.Store, barcode.Code, head.Customer);
                        PriceCatalogDetail pcdt = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                        if (head.Customer.Owner.OwnerApplicationSettings.RecomputePrices)
                        {
                            line = DocumentHelper.ComputeDocumentLine(ref head, item, barcode, Qty, false, -1, false, "", discounts);
                        }
                        else
                        {
                            bool foundCustomPrice = unitPrice > 0;
                            if (pcdt.Discount > 0)
                            {
                                discounts.Add(DiscountHelper.CreatePriceCatalogDetailDiscount(uow, pcdt.Discount));
                            }

                            line = DocumentHelper.ComputeDocumentLine(ref head, item, barcode, Qty, false, unitPrice, foundCustomPrice, "", discounts);
                        }
                        DocumentHelper.AddItem(ref head, line);
                    }
                    if (head.DocumentType.MergedSameDocumentLines)
                    {
                        DocumentHelper.MergeDocumentLines(ref head);
                    }
                    head.Save();
                    XpoHelper.CommitChanges(uow);

                    MvcApplication.WRMLogModule.Log(null, "Successful Post Document", "RetailService", "PostDocument", this.Context.Request.UserAgent,
                      this.Context.Request.UserHostAddress, "", userId, KernelLogLevel.Info);
                    return "1";
                }



            }
            catch (Exception e)
            {
                MvcApplication.WRMLogModule.Log(e, "RetailService: GetPostDocument", KernelLogLevel.Error);
                NewXmlCreator a = new NewXmlCreator("1", e.Message);
                a.Xmlclose();
                return a.MyXml;
            }
        }

        private XmlNode FindNodeByNameAndAttribute(XmlNode node, string name, string attribute_name, string attribute_value)
        {
            XmlNode the_node = null;
            foreach (XmlNode child_node in node.ChildNodes)
            {
                if (child_node.Name.Equals(name) && child_node.Attributes[attribute_name].Value.Equals(attribute_value))
                {
                    the_node = child_node;
                }
                if (child_node.ChildNodes.Count > 0 && the_node == null)
                {
                    the_node = FindNodeByNameAndAttribute(child_node, name, attribute_name, attribute_value);
                }
            }
            return the_node;
        }

        /// <summary>
        /// Returns the version of the executing assembly
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string GetVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            return string.Format("v{0}.{1}.{2}.{3}", new object[] { version.Major, version.Minor, version.Build, version.Revision });
        }

        protected string Serialize(XPCollection XP, string fields, string InnerTag)
        {
            try
            {
                string[] pedia = fields.Split(',');
                PropertyDescriptor prop;
                XmlCreator newxml = new XmlCreator("ITS");
                foreach (object obj in XP)
                {
                    List<string> xmldata = new List<string>();
                    foreach (DevExpress.Xpo.Metadata.XPMemberInfo info in XP.ObjectClassInfo.PersistentProperties)
                    {
                        if (Array.Exists(pedia, s => s.Equals(info.Name)))
                        {
                            prop = ((ICustomTypeDescriptor)obj).GetProperties()[info.Name];
                            object val = prop.GetValue(obj);
                            if (val as XPCollection == null)
                                xmldata.Add(info.Name + "|" + val.ToString());
                            else
                            {
                                XPCollection x = val as XPCollection;
                                BaseObj x0 = x[0] as BaseObj;
                                string str = null;
                                try
                                {
                                    str = x0.GetMemberValue("Description").ToString();
                                }
                                catch (Exception)
                                {
                                    try
                                    {
                                        str = x0.GetMemberValue("Code").ToString();
                                    }
                                    catch
                                    {
                                        str = x0.GetMemberValue("Oid").ToString();
                                    }

                                }
                                if (str != null)
                                    xmldata.Add(info.Name + "|" + str);
                            }
                        }
                    }
                    newxml.CreateNodes(InnerTag, xmldata.ToArray());
                }
                newxml.Xmlclose();

                return newxml.MyXml;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected string Serialize(XPCollection XP, string InnerTag)
        {
            try
            {
                PropertyDescriptor prop;
                XmlCreator newxml = new XmlCreator("ITS");
                foreach (object obj in XP)
                {
                    List<string> xmldata = new List<string>();
                    foreach (DevExpress.Xpo.Metadata.XPMemberInfo info in XP.ObjectClassInfo.PersistentProperties)
                    {
                        if (info.Name == "OptimisticLockField")
                            continue;

                        prop = ((ICustomTypeDescriptor)obj).GetProperties()[info.Name];
                        xmldata.Add(info.Name + "|" + prop.GetValue(obj));
                    }
                    newxml.CreateNodes(InnerTag, xmldata.ToArray());
                }
                newxml.Xmlclose();

                return newxml.MyXml;
            }
            catch //(Exception ex)
            {
                throw;
            }
        }

        protected XPCollection<T> GetList<T>(UnitOfWork uow, CriteriaOperator filter, string sortingField = "Oid")
        {
            XPCollection<T> col = new XPCollection<T>(uow, filter);
            SortingCollection srtcol = new SortingCollection();
            srtcol.Add(new SortProperty(sortingField, SortingDirection.Ascending));
            col.Sorting = srtcol;
            col.DeleteObjectOnRemove = true;
            return col;
        }

        protected XPCollection<T> GetList<T>(UnitOfWork uow, string sortingField = "Oid")
        {
            XPCollection<T> col = new XPCollection<T>(uow);
            SortingCollection srtcol = new SortingCollection();
            srtcol.Add(new SortProperty(sortingField, SortingDirection.Ascending));
            col.Sorting = srtcol;
            col.DeleteObjectOnRemove = true;
            return col;
        }

        [WebMethod]
        public UserType GetUserType(string userid)
        {
            Guid userguid;
            if (Guid.TryParse(userid, out userguid))
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    User user = uow.GetObjectByKey<User>(userguid);
                    if (user.Role.Type == eRoleType.Customer || user.Role.Type == eRoleType.Supplier)
                    {
                        return UserType.TRADER;
                    }
                    else if (user.Role.Type == eRoleType.CompanyUser)
                    {
                        return UserType.COMPANYUSER;
                    }
                    else if (user.Role.Type == eRoleType.CompanyAdministrator || user.Role.Type == eRoleType.SystemAdministrator)
                    {
                        return UserType.ADMIN;
                    }
                    return UserType.NONE;
                }
            }
            return UserType.NONE;
        }

        [WebMethod]
        public TCustomer[] GetCustomersOfUser(string userid, long updatedOn)
        {
            Guid userguid;
            if (Guid.TryParse(userid, out userguid))
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    User user = uow.GetObjectByKey<User>(userguid);
                    bool iscust = UserHelper.IsCustomer(user);
                    bool issupp = UserHelper.IsCompanyUser(user);
                    if (iscust && issupp)
                        return null;
                    if (iscust)
                    {
                        XPCollection<Customer> cust = BOApplicationHelper.GetUserEntities<Customer>(XpoHelper.GetNewUnitOfWork(), user);
                        IEnumerable<TCustomer> v = from customer in cust
                                                   select new TCustomer
                                                   {
                                                       Code = customer.Code,
                                                       CompanyName = customer.CompanyName,
                                                       DefaultAddress = (customer.DefaultAddress != null) ? customer.DefaultAddress.Description : "",
                                                       DefaultPhone = (customer.DefaultAddress != null && customer.DefaultAddress.DefaultPhone != null) ? customer.DefaultAddress.DefaultPhone.Number : "",
                                                       Oid = customer.Oid,
                                                       PriceListOid = customer.GetDefaultPriceCatalog() == null ? Guid.Empty : customer.GetDefaultPriceCatalog().Oid,
                                                       TaxCode = customer.Trader.TaxCode,
                                                       VatLevel = customer.VatLevel == null ? Guid.Empty : customer.VatLevel.Oid,
                                                       StoreOid = customer.DefaultStore == null ? Guid.Empty : customer.DefaultStore.Oid
                                                   };
                        return v.ToArray();
                    }
                    if (issupp)
                    {
                        IEnumerable<Store> stores = UserHelper.GetStoresThatUserOwns(user);
                        IEnumerable<TCustomer> totalList = new List<TCustomer>();
                        foreach (Store store in stores)
                        {
                            IEnumerable<TCustomer> storeList = from customer in store.Customers.Where(g => g.UpdatedOnTicks >= updatedOn).OrderBy(g => g.UpdatedOnTicks)
                                                               select new TCustomer
                                                               {
                                                                   Code = customer.Code,
                                                                   CompanyName = customer.CompanyName,
                                                                   DefaultAddress = (customer.DefaultAddress != null) ? customer.DefaultAddress.Description : "",
                                                                   DefaultPhone = (customer.DefaultAddress != null && customer.DefaultAddress.DefaultPhone != null) ? customer.DefaultAddress.DefaultPhone.Number : "",
                                                                   Oid = customer.Oid,
                                                                   UpdateOn = customer.UpdatedOnTicks,
                                                                   VatLevel = customer.VatLevel == null ? Guid.Empty : customer.VatLevel.Oid,
                                                                   PriceListOid = customer.GetDefaultPriceCatalog() == null ? Guid.Empty : customer.GetDefaultPriceCatalog().Oid,
                                                                   TaxCode = customer.Trader.TaxCode,
                                                                   StoreOid = customer.DefaultStore == null ? Guid.Empty : customer.DefaultStore.Oid
                                                               };
                            totalList = totalList.Union(storeList);
                        }
                        return totalList.OrderBy(g => g.CompanyName).ToArray();
                    }
                    return null;
                }
            }
            return null;
        }

        [WebMethod]
        public TCustomer[] GetCustomers(long updatedOn, string userID)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                User user = uow.FindObject<User>(new BinaryOperator("Oid", userID, BinaryOperatorType.Equal));
                if (user == null || !UserHelper.IsCompanyUser(user))
                {
                    return null;
                }

                XPCollection<Customer> newXPCollection = new XPCollection<Customer>(uow);
                IEnumerable<TCustomer> Visible = from customer in newXPCollection.Where(g => g.UpdatedOnTicks >= updatedOn).OrderBy(g => g.UpdatedOnTicks).Take(MAX_ITEMS)
                                                 select new TCustomer
                                                 {
                                                     Code = customer.Code,
                                                     CompanyName = customer.CompanyName,
                                                     DefaultAddress = (customer.DefaultAddress != null) ? customer.DefaultAddress.Description : "",
                                                     DefaultPhone = (customer.DefaultAddress != null && customer.DefaultAddress.DefaultPhone != null) ? customer.DefaultAddress.DefaultPhone.Number : "",
                                                     Oid = customer.Oid,
                                                     UpdateOn = customer.UpdatedOnTicks,
                                                     VatLevel = customer.VatLevel == null ? Guid.Empty : customer.VatLevel.Oid,
                                                     PriceListOid = customer.GetDefaultPriceCatalog() == null ? Guid.Empty : customer.GetDefaultPriceCatalog().Oid,
                                                     TaxCode = customer.Trader.TaxCode,
                                                     StoreOid = customer.DefaultStore == null ? Guid.Empty : customer.DefaultStore.Oid
                                                 };

                return Visible.ToArray();
            }
        }

        [WebMethod]
        public int GetTotalUpdates(long Customerupdatedon, long Itemupdatedon, long IATupdatedon, long Barcodeupdatedon, long ICupdatedon,
            long PCupdatedon, long PCDupdatedon, long VCupdatedon, long VFupdatedon, long VLupdatedon, long offerUpdatedOn, long offerDetailUpdatedOn,
            long documentStatusUpdatedOn, long storesUpdatedOn, long linkedItemsUpdatedOn, long mmUpdatedOn, long documentTypeUpdatedOn, string userID)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Guid oid;
                if (Guid.TryParse(userID, out oid))
                {
                    User user = uow.FindObject<User>(new BinaryOperator("Oid", oid));
                    if (user == null)
                    {
                        throw new Exception("User with id '" + oid + "' not found");
                    }
                    CompanyNew company = UserHelper.GetCompany(user);

                    if (company == null)
                    {
                        throw new Exception("User is not a Company User");
                    }

                    BinaryOperator isActiveCrop = new BinaryOperator("IsActive", true);
                    int a1 = (int)uow.Evaluate<Customer>(CriteriaOperator.Parse("Count"), RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(isActiveCrop, new BinaryOperator("UpdatedOnTicks", Customerupdatedon, BinaryOperatorType.Greater)),typeof(Customer),company));
                    a1 += (int)uow.Evaluate<Item>(CriteriaOperator.Parse("Count"), RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(isActiveCrop, new BinaryOperator("UpdatedOnTicks", Itemupdatedon, BinaryOperatorType.Greater)),typeof(Item),company));
                    a1 += (int)uow.Evaluate<ItemAnalyticTree>(CriteriaOperator.Parse("Count"), CriteriaOperator.And(isActiveCrop, new BinaryOperator("UpdatedOnTicks", IATupdatedon, BinaryOperatorType.Greater),new BinaryOperator("Object.Owner.Oid",company.Oid)));
                    a1 += (int)uow.Evaluate<Barcode>(CriteriaOperator.Parse("Count"), CriteriaOperator.And(isActiveCrop, new BinaryOperator("UpdatedOnTicks", Barcodeupdatedon, BinaryOperatorType.Greater)));
                    a1 += (int)uow.Evaluate<ItemCategory>(CriteriaOperator.Parse("Count"), RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(isActiveCrop, new BinaryOperator("UpdatedOnTicks", ICupdatedon, BinaryOperatorType.Greater)),typeof(ItemCategory),company));
                    a1 += (int)uow.Evaluate<PriceCatalog>(CriteriaOperator.Parse("Count"), RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(isActiveCrop, new BinaryOperator("UpdatedOnTicks", PCupdatedon, BinaryOperatorType.Greater)),typeof(PriceCatalog),company));
                    a1 += (int)uow.Evaluate<PriceCatalogDetail>(CriteriaOperator.Parse("Count"), CriteriaOperator.And(isActiveCrop, new BinaryOperator("UpdatedOnTicks", PCDupdatedon, BinaryOperatorType.Greater), new BinaryOperator("PriceCatalog.Owner.Oid",company.Oid)));
                    a1 += (int)uow.Evaluate<VatCategory>(CriteriaOperator.Parse("Count"), RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(isActiveCrop, new BinaryOperator("UpdatedOnTicks", VCupdatedon, BinaryOperatorType.Greater)),typeof(VatCategory),company));
                    a1 += (int)uow.Evaluate<VatFactor>(CriteriaOperator.Parse("Count"), RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(isActiveCrop, new BinaryOperator("UpdatedOnTicks", VFupdatedon, BinaryOperatorType.Greater)),typeof(VatFactor),company));
                    a1 += (int)uow.Evaluate<VatLevel>(CriteriaOperator.Parse("Count"), RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(isActiveCrop, new BinaryOperator("UpdatedOnTicks", VLupdatedon, BinaryOperatorType.Greater)),typeof(VatLevel),company));
                    a1 += (int)uow.Evaluate<Offer>(CriteriaOperator.Parse("Count"), RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(isActiveCrop, new BinaryOperator("UpdatedOnTicks", offerUpdatedOn, BinaryOperatorType.Greater)),typeof(Offer),company));
                    a1 += (int)uow.Evaluate<OfferDetail>(CriteriaOperator.Parse("Count"), CriteriaOperator.And(isActiveCrop, new BinaryOperator("UpdatedOnTicks", offerDetailUpdatedOn, BinaryOperatorType.Greater),new BinaryOperator("Offer.Owner.Oid",company.Oid)));
                    a1 += (int)uow.Evaluate<DocumentStatus>(CriteriaOperator.Parse("Count"), RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(isActiveCrop, new BinaryOperator("UpdatedOnTicks", documentStatusUpdatedOn, BinaryOperatorType.Greater)),typeof(DocumentStatus),company));
                    a1 += (int)uow.Evaluate<Store>(CriteriaOperator.Parse("Count"), RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(isActiveCrop, new BinaryOperator("UpdatedOnTicks", storesUpdatedOn, BinaryOperatorType.Greater)),typeof(Store),company));
                    a1 += (int)uow.Evaluate<LinkedItem>(CriteriaOperator.Parse("Count"), CriteriaOperator.And(isActiveCrop, new BinaryOperator("UpdatedOnTicks", linkedItemsUpdatedOn, BinaryOperatorType.Greater),new BinaryOperator("Item.Owner.Oid",company.Oid)));
                    a1 += (int)uow.Evaluate<MeasurementUnit>(CriteriaOperator.Parse("Count"), RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(isActiveCrop, new BinaryOperator("UpdatedOnTicks", mmUpdatedOn, BinaryOperatorType.Greater)),typeof(MeasurementUnit),company));
                    a1 += (int)uow.Evaluate<DocumentType>(CriteriaOperator.Parse("Count"), RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(isActiveCrop, new BinaryOperator("UpdatedOnTicks", documentTypeUpdatedOn, BinaryOperatorType.Greater)),typeof(DocumentType),company));

                    return a1;
                }
                else
                {
                    throw new Exception("Invalid User ID");
                }
            }
        }

        //[WebMethod]
        //public TUserStoreAccess[] getAllUserStoreAccess(long updatedon, string userID)
        //{
        //    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
        //    {
        //        User user = uow.FindObject<User>(new BinaryOperator("Oid", userID, BinaryOperatorType.Equal));
        //        if (user == null || !UserHelper.IsCompanyUser(user))
        //        {
        //            return null;
        //        }

        //        XPCollection<UserTypeAccess> newXPCollection = new XPCollection<UserTypeAccess>(uow, CriteriaOperator.And(new NotOperator(new NullOperator("EntityOid")),
        //                                                                                                                new NotOperator(new NullOperator("User")),
        //                                                                                                                new BinaryOperator("EntityType", "ITS.Retail.Model.Store", BinaryOperatorType.Equal)));
        //        IEnumerable<TUserStoreAccess> Visible = from obj in newXPCollection.Where(g => g.UpdatedOnTicks >= updatedon).OrderBy(g => g.UpdatedOnTicks)
        //                                                select new TUserStoreAccess
        //                                          {

        //                                              UpdatedOn = obj.UpdatedOnTicks,
        //                                              StoreID = obj.EntityOid,
        //                                              UserID = obj.User.Oid
        //                                          };
        //        return Visible.ToArray();
        //    }
        //}

        [WebMethod]
        public TUserStoreAccess[] getUserStoreAccess(string userID)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                User user = uow.FindObject<User>(new BinaryOperator("Oid", userID, BinaryOperatorType.Equal));
                if (user == null || !UserHelper.IsCompanyUser(user))
                {
                    return null;
                }

                XPCollection<UserTypeAccess> newXPCollection = new XPCollection<UserTypeAccess>(uow, CriteriaOperator.And(new NotOperator(new NullOperator("EntityOid")),
                                                                                                                        new BinaryOperator("User", user, BinaryOperatorType.Equal),
                                                                                                                        new BinaryOperator("EntityType", "ITS.Retail.Model.Store", BinaryOperatorType.Equal)));
                IEnumerable<TUserStoreAccess> Visible = from obj in newXPCollection
                                                        select new TUserStoreAccess
                                                        {
                                                            UpdatedOn = obj.UpdatedOnTicks,
                                                            StoreID = obj.EntityOid,
                                                            UserID = obj.User.Oid
                                                        };
                return Visible.ToArray();
            }
        }

        //[WebMethod]
        //public TMeasurementUnit[] getMeasurementUnits(long updatedon, string userID)
        //{
        //    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
        //    {
        //        User user = uow.FindObject<User>(new BinaryOperator("Oid", userID, BinaryOperatorType.Equal));
        //        if (user == null || !UserHelper.IsCompanyUser(user))
        //        {
        //            return null;
        //        }

        //        XPCollection<MeasurementUnit> newXPCollection = new XPCollection<MeasurementUnit>(uow);
        //        IEnumerable<TMeasurementUnit> Visible = from obj in newXPCollection.Where(g => g.UpdatedOnTicks >= updatedon).OrderBy(g => g.UpdatedOnTicks)
        //                                                select new TMeasurementUnit
        //                                                {

        //                                                    UpdatedOn = obj.UpdatedOnTicks,
        //                                                    Oid = obj.Oid,
        //                                                    Code = obj.Code,
        //                                                    Description = obj.Description,
        //                                                    SupportDecimal = obj.SupportDecimal
        //                                                };
        //        return Visible.ToArray();
        //    }
        //}

        [WebMethod]
        public TStore[] getStores(long updatedon, string userID)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                User user = uow.FindObject<User>(new BinaryOperator("Oid", userID, BinaryOperatorType.Equal));
                if (user == null || !UserHelper.IsCompanyUser(user))
                {
                    return null;
                }

                XPCollection<Store> newXPCollection = new XPCollection<Store>(uow);
                IEnumerable<TStore> Visible = from obj in newXPCollection.Where(g => g.UpdatedOnTicks >= updatedon).OrderBy(g => g.UpdatedOnTicks)
                                              select new TStore
                                                   {

                                                       UpdatedOn = obj.UpdatedOnTicks,
                                                       Oid = obj.Oid,
                                                       Code = obj.Code,
                                                       IsCentralStore = obj.IsCentralStore,
                                                       Name = obj.Name,
                                                   };
                return Visible.ToArray();
            }
        }

        //[WebMethod]
        //public TVatLC[] getVatLevels(long updatedon, string userID)
        //{
        //    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
        //    {
        //        User user = uow.FindObject<User>(new BinaryOperator("Oid", userID, BinaryOperatorType.Equal));
        //        if (user == null || !UserHelper.IsCompanyUser(user))
        //        {
        //            return null;
        //        }

        //        XPCollection<VatLevel> col = new XPCollection<VatLevel>(uow);
        //        IEnumerable<TVatLC> Visible = from obj in col.Where(g => g.UpdatedOnTicks >= updatedon).OrderBy(g => g.UpdatedOnTicks)
        //                                      select new TVatLC
        //                                          {

        //                                              UpdatedOn = obj.UpdatedOnTicks,
        //                                              Oid = obj.Oid,
        //                                              Description = obj.Description,
        //                                              IsDefault = obj.IsDefault
        //                                          };
        //        return Visible.ToArray();
        //    }
        //}

        //[WebMethod]
        //public TVatLC[] getVatCategories(long updatedon, string userID)
        //{
        //    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
        //    {
        //        User user = uow.FindObject<User>(new BinaryOperator("Oid", userID, BinaryOperatorType.Equal));
        //        if (user == null || !UserHelper.IsCompanyUser(user))
        //        {
        //            return null;
        //        }

        //        XPCollection<VatCategory> newXPCollection = new XPCollection<VatCategory>(uow);
        //        IEnumerable<TVatLC> Visible = from obj in newXPCollection.Where(g => g.UpdatedOnTicks >= updatedon).OrderBy(g => g.UpdatedOnTicks)
        //                                      select new TVatLC
        //                                      {

        //                                          UpdatedOn = obj.UpdatedOnTicks,
        //                                          Oid = obj.Oid,
        //                                          Description = obj.Description,
        //                                          IsDefault = obj.IsDefault
        //                                      };
        //        return Visible.ToArray();
        //    }
        //}

        //[WebMethod]
        //public TItemCategory[] getPriceCatalogs(long updatedon, string userID)
        //{
        //    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
        //    {
        //        User user = uow.FindObject<User>(new BinaryOperator("Oid", userID, BinaryOperatorType.Equal));
        //        if (user == null || !UserHelper.IsCompanyUser(user))
        //        {
        //            return null;
        //        }

        //        XPCollection<PriceCatalog> newXPCollection = new XPCollection<PriceCatalog>(uow);
        //        IEnumerable<TItemCategory> Visible = from obj in newXPCollection.Where(g => g.UpdatedOnTicks >= updatedon).OrderBy(g => g.UpdatedOnTicks)
        //                                             select new TItemCategory
        //                                  {

        //                                      UpdatedOn = obj.UpdatedOnTicks,
        //                                      Oid = obj.Oid,
        //                                      Description = obj.Description,
        //                                      Code = obj.Code,
        //                                      parentOid = (obj.ParentCatalog != null) ? obj.ParentCatalog.Oid : Guid.Empty
        //                                  };
        //        return Visible.ToArray();
        //    }
        //}

        [WebMethod]
        public TAppSettings? getAppSettings(string userID)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                User user = uow.FindObject<User>(new BinaryOperator("Oid", userID, BinaryOperatorType.Equal));
                if (user == null || !UserHelper.IsCompanyUser(user))
                {
                    return null;
                }

                CompanyNew owner = UserHelper.GetCompany(user);

                return new TAppSettings
                {
                    BarcodePad = owner.OwnerApplicationSettings.PadBarcodes,
                    CodePad = owner.OwnerApplicationSettings.PadItemCodes,
                    BarcodePadChar = owner.OwnerApplicationSettings.BarcodePaddingCharacter,
                    BarcodePadLength = owner.OwnerApplicationSettings.BarcodeLength,
                    CodePadChar = owner.OwnerApplicationSettings.ItemCodePaddingCharacter,
                    CodePadLength = owner.OwnerApplicationSettings.ItemCodeLength,
                    ComputeDigits = (int)owner.OwnerApplicationSettings.ComputeDigits,
                    ComputeValueDigits = (int)owner.OwnerApplicationSettings.ComputeValueDigits,
                    DisplayDigits = (int)owner.OwnerApplicationSettings.DisplayDigits,
                    DisplayValueDigits = (int)owner.OwnerApplicationSettings.DisplayValueDigits,
                    DiscountPermited = owner.OwnerApplicationSettings.DiscountPermited

                };
            }

        }

        [WebMethod]
        public bool CleanFile(String filename, String userID)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Guid userGuid;
                if (!Guid.TryParse(userID, out userGuid))
                    return false;
            }

            if (!File.Exists(Server.MapPath("~/Downloads/" + filename)))
                return false;
            bool deleteFile = false;
            using (ZipFile zip = ZipFile.Read(Server.MapPath("~/Downloads/" + filename)))
            {
                //foreach (ZipEntry z in zip)
                //{
                //    if (z.FileName == "Database.txt" && z.Comment == userID)
                //    {
                //        deleteFile = true;
                //        break;
                //    }
                //}
                if (zip.Comment == userID)
                {
                    deleteFile = true;
                }
            }
            if (deleteFile == true)
            {
                File.Delete(Server.MapPath("~/Downloads/" + filename));
                //Cleanup downloads folder
                //CleanupDownloadsFolder();
            }
            return false;
        }
        private void CleanupDownloadsFolder()
        {
            String[] files = Directory.GetFiles(Server.MapPath("~/Downloads/"), "*.zip");
            foreach (String fl in files)
            {
                DateTime dt = File.GetCreationTime(fl);
                TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
                ts = ts.Subtract(new TimeSpan(dt.Ticks));
                if (ts.TotalHours > 36)
                {
                    File.Delete(fl);
                }
            }
        }

        [WebMethod]
        public String GetTotalUpdatesLink(long Customerupdatedon, long Itemupdatedon, long IATupdatedon, long Barcodeupdatedon, long ICupdatedon,
            long PCupdatedon, long PCDupdatedon, long VCupdatedon, long VFupdatedon, long VLupdatedon, long offerUpdatedOn, long offerDetailUpdatedOn,
            long documentStatusUpdatedOn, long storesUpdatedOn, long linkedItemsUpdatedOn, long mmUpdatedOn, long documentTypeUpdatedOn,
            String userID)
        {

            long timeCount = 90 * TimeSpan.TicksPerSecond;
            int recordCount = 0;
            const int recordLimit = 100000;

            long now = DateTime.Now.Ticks;
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Guid userGuid, zipfilenamegd;
                if (!Guid.TryParse(userID, out userGuid))
                    return null;
                User user = uow.GetObjectByKey<User>(userGuid);
                if (user == null || !UserHelper.IsCompanyUser(user))
                    return null;

                CompanyNew company = UserHelper.GetCompany(user);
                zipfilenamegd = Guid.NewGuid();
                String filename = Path.GetTempFileName(), zipFileName = "F" + zipfilenamegd.ToString().Replace("-", "") + ".zip";
                using (StreamWriter txt = new StreamWriter(filename))
                {
                    txt.AutoFlush = true;
                    SortProperty[] sortP = new SortProperty[] { new SortProperty("UpdatedOnTicks", SortingDirection.Ascending) };

                    IEnumerable<Customer> custcol = (new XPCollection<Customer>(uow, RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(
                        new BinaryOperator("IsActive", true),
                        new BinaryOperator("UpdatedOnTicks", Customerupdatedon, BinaryOperatorType.GreaterOrEqual)),typeof(Customer),company), sortP)).Take((recordLimit / 2) - recordCount);
                    //XPCursor custcol = new XPCursor(uow, typeof(Customer), new BinaryOperator("UpdatedOnTicks", Customerupdatedon, BinaryOperatorType.GreaterOrEqual), sortP);
                    //custcol.PageSize = 2000;
                    // Write Description                   
                    txt.WriteLine("Customer" + DELIMITER + "Code" + DELIMITER + "CompanyName" + DELIMITER + "DefaultAddress" + DELIMITER + "DefaultPhone" + DELIMITER + "Oid" + DELIMITER + "PriceListOid" + DELIMITER + "StoreOid" + DELIMITER + "UpdatedOn" + DELIMITER + "TaxCode" + DELIMITER + "VatLevel");
                    txt.WriteLine("Address" + DELIMITER + "Oid" + DELIMITER + "CustomerOid" + DELIMITER + "Address");
                    foreach (Customer ct in custcol)
                    {

                        txt.Write("Customer" + DELIMITER);
                        txt.Write(ct.Code + DELIMITER);
                        txt.Write(ct.CompanyName + DELIMITER);
                        txt.Write((ct.DefaultAddress == null ? "" : ct.DefaultAddress.Description) + DELIMITER);
                        txt.Write((ct.DefaultAddress == null || ct.DefaultAddress.DefaultPhone == null ? "" : ct.DefaultAddress.DefaultPhone.Number) + DELIMITER);
                        txt.Write(ct.Oid + DELIMITER);
                        long t1 = DateTime.Now.Ticks;
                        PriceCatalog custPc = ct.GetDefaultPriceCatalog();
                        long t2 = DateTime.Now.Ticks;
                        txt.Write((custPc == null ? Guid.Empty : custPc.Oid) + DELIMITER);
                        txt.Write((ct.DefaultStore == null ? Guid.Empty : ct.DefaultStore.Oid) + DELIMITER);
                        txt.Write(ct.UpdatedOnTicks + DELIMITER);
                        txt.Write(ct.Trader.TaxCode + DELIMITER);
                        txt.WriteLine(ct.VatLevel.Oid);
                        recordCount++;
                        foreach (Address ad in ct.Trader.Addresses)
                        {
                            txt.Write("Address" + DELIMITER);
                            txt.Write(ad.Oid + DELIMITER);
                            txt.Write(ct.Oid + DELIMITER);
                            txt.WriteLine(ad.Description);
                            recordCount++;
                        }
                        if (DateTime.Now.Ticks - now >= timeCount)
                        {
                            recordCount = recordLimit + 1;
                            break;
                        }
                    }

                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename);
                    }

                    IEnumerable<DocumentStatus> documentStatuses = new XPCollection<DocumentStatus>(uow, RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(
                        new BinaryOperator("IsActive", true),
                        new BinaryOperator("UpdatedOnTicks", documentStatusUpdatedOn, BinaryOperatorType.GreaterOrEqual)),typeof(DocumentStatus),company), sortP).Take(recordLimit - recordCount);
                    txt.WriteLine("DocumentStatus" + DELIMITER + "Oid" + DELIMITER + "Description" + DELIMITER + "IsDefault" + DELIMITER + "UpdatedOn");
                    foreach (DocumentStatus ct in documentStatuses)
                    {
                        txt.Write("DocumentStatus" + DELIMITER);
                        txt.Write(ct.Oid + DELIMITER);
                        txt.Write(ct.Description + DELIMITER);
                        txt.Write((ct.IsDefault ? "1" : "0") + DELIMITER);
                        txt.WriteLine("" + ct.UpdatedOnTicks);
                        recordCount++;
                    }
                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename);
                    }

                    IEnumerable<DocumentType> documentTypes = new XPCollection<DocumentType>(uow, RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(new BinaryOperator("IsActive", true),
                        new BinaryOperator("UpdatedOnTicks", documentTypeUpdatedOn, BinaryOperatorType.GreaterOrEqual)),typeof(DocumentType),company), sortP).Take(recordLimit - recordCount);
                    txt.WriteLine("DocumentType" + DELIMITER + "Oid" + DELIMITER + "Code" + DELIMITER + "Description" + DELIMITER + "IsDefault" + DELIMITER + "UpdatedOn");
                    foreach (DocumentType ct in documentTypes)
                    {
                        txt.Write("DocumentType" + DELIMITER);
                        txt.Write(ct.Oid + DELIMITER);
                        txt.Write(ct.Code + DELIMITER);
                        txt.Write(ct.Description + DELIMITER);
                        txt.Write((ct.IsDefault ? "1" : "0") + DELIMITER);
                        txt.WriteLine("" + ct.UpdatedOnTicks);
                        recordCount++;
                    }
                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename);
                    }


                    IEnumerable<Store> stores = new XPCollection<Store>(uow, 
                        RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(new BinaryOperator("IsActive", true), new BinaryOperator("UpdatedOnTicks", storesUpdatedOn, BinaryOperatorType.GreaterOrEqual)),typeof(Store),company),
                        sortP).Take(recordLimit - recordCount);
                    txt.WriteLine("Store" + DELIMITER + "Oid" + DELIMITER + "Code" + DELIMITER + "Name" + DELIMITER + "IsCentralStore" + DELIMITER + "UpdatedOn");
                    foreach (Store ct in stores)
                    {
                        txt.Write("Store" + DELIMITER);
                        txt.Write(ct.Oid + DELIMITER);
                        txt.Write(ct.Code + DELIMITER);
                        txt.Write(ct.Name + DELIMITER);
                        txt.Write((ct.IsCentralStore ? "1" : "0") + DELIMITER);
                        txt.WriteLine("" + ct.UpdatedOnTicks);
                        recordCount++;
                    }
                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename);
                    }

                    IEnumerable<UserTypeAccess> userTypeAccesses = new XPCollection<UserTypeAccess>(uow, CriteriaOperator.And(
                        new BinaryOperator("IsActive", true),
                        new NotOperator(new NullOperator("EntityOid")),
                        new BinaryOperator("User", user, BinaryOperatorType.Equal),
                        new BinaryOperator("EntityType", "ITS.Retail.Model.Store", BinaryOperatorType.Equal))).Take(recordLimit - recordCount);
                    //userTypeAccesses.PageSize = 2000;
                    txt.WriteLine("UserTypeAccess" + DELIMITER + "Oid" + DELIMITER + "UserID" + DELIMITER + "StoreID" + DELIMITER + "UpdatedOn");
                    foreach (UserTypeAccess ct in userTypeAccesses)
                    {
                        txt.Write("UserTypeAccess" + DELIMITER);
                        txt.Write(ct.Oid + DELIMITER);
                        txt.Write(ct.User.Oid + DELIMITER);
                        txt.Write(ct.EntityOid + DELIMITER);
                        txt.WriteLine("" + ct.UpdatedOnTicks);
                        recordCount++;
                    }
                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename);
                    }

                    CriteriaOperator validDetails = CriteriaOperator.And(new NotOperator(new NullOperator("Item")), new NotOperator(new NullOperator("SubItem")),new BinaryOperator("Item.Owner.Oid",company.Oid));
                    IEnumerable<LinkedItem> lis = new XPCollection<LinkedItem>(uow, CriteriaOperator.And(
                        new BinaryOperator("IsActive", true), validDetails, new BinaryOperator("UpdatedOnTicks", linkedItemsUpdatedOn, BinaryOperatorType.GreaterOrEqual)), sortP).Take(recordLimit - recordCount);

                    txt.WriteLine("LinkedItem" + DELIMITER + "Oid" + DELIMITER + "ItemOid" + DELIMITER + "SubitemOid" + DELIMITER + "UpdatedOn");
                    foreach (LinkedItem ct in lis)
                    {
                        txt.Write("LinkedItem" + DELIMITER);
                        txt.Write(ct.Oid + DELIMITER);
                        txt.Write(ct.Item.Oid + DELIMITER);
                        txt.Write(ct.SubItem.Oid + DELIMITER);
                        txt.WriteLine("" + ct.UpdatedOnTicks);
                        recordCount++;
                    }
                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename);
                    }

                    IEnumerable<MeasurementUnit> mmc = new XPCollection<MeasurementUnit>(uow, RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(
                        new BinaryOperator("IsActive", true), new BinaryOperator("UpdatedOnTicks", mmUpdatedOn, BinaryOperatorType.GreaterOrEqual)),typeof(MeasurementUnit),company), sortP).Take(recordLimit - recordCount);

                    txt.WriteLine("MeasurementUnit" + DELIMITER + "Oid" + DELIMITER + "Description" + DELIMITER + "SupportDecimal" + DELIMITER + "UpdatedOn");
                    foreach (MeasurementUnit ct in mmc)
                    {
                        txt.Write("MeasurementUnit" + DELIMITER);
                        txt.Write(ct.Oid + DELIMITER);
                        //txt.Write(ct.Code + DELIMITER);
                        txt.Write(ct.Description + DELIMITER);
                        txt.Write((ct.SupportDecimal ? "1" : "0") + DELIMITER);
                        txt.WriteLine("" + ct.UpdatedOnTicks);
                        recordCount++;
                    }
                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename);
                    }

                    IEnumerable<VatCategory> vc = new XPCollection<VatCategory>(uow, RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(
                        new BinaryOperator("IsActive", true), new BinaryOperator("UpdatedOnTicks", VCupdatedon, BinaryOperatorType.GreaterOrEqual)),typeof(VatCategory),company), sortP).Take(recordLimit - recordCount);
                    /* */
                    txt.WriteLine("VatCategory" + DELIMITER + "Oid" + DELIMITER + "Description" + DELIMITER + "UpdatedOn");
                    foreach (VatCategory ct in vc)
                    {
                        txt.Write("VatCategory" + DELIMITER);
                        txt.Write(ct.Oid + DELIMITER);
                        txt.Write(ct.Description + DELIMITER);
                        //txt.Write((ct.IsDefault ? "1" : "0") + DELIMITER);
                        txt.WriteLine("" + ct.UpdatedOnTicks);
                        recordCount++;
                    }
                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename);
                    }

                    IEnumerable<VatFactor> vf = new XPCollection<VatFactor>(uow, RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(
                        new BinaryOperator("IsActive", true), new BinaryOperator("UpdatedOnTicks", VFupdatedon, BinaryOperatorType.GreaterOrEqual)),typeof(VatFactor),company), sortP).Take(recordLimit - recordCount);
                    /*
                     */
                    txt.WriteLine("VatFactor" + DELIMITER + "Oid" + DELIMITER + "VatCategoryOid" + DELIMITER + "VatLevelOid" + DELIMITER + "VatFactor" + DELIMITER + "UpdatedOn");
                    foreach (VatFactor ct in vf)
                    {
                        txt.Write("VatFactor" + DELIMITER);
                        txt.Write(ct.Oid + DELIMITER);
                        txt.Write(ct.VatCategory.Oid + DELIMITER);
                        txt.Write(ct.VatLevel.Oid + DELIMITER);
                        txt.Write(ct.Factor + DELIMITER);
                        txt.WriteLine("" + ct.UpdatedOnTicks);
                        recordCount++;
                    }
                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename);
                    }


                    IEnumerable<VatLevel> vl = new XPCollection<VatLevel>(uow, RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(
                        new BinaryOperator("IsActive", true), new BinaryOperator("UpdatedOnTicks", VLupdatedon, BinaryOperatorType.GreaterOrEqual)),typeof(VatLevel),company), sortP).Take(recordLimit - recordCount);
                    /* */
                    txt.WriteLine("VatLevel" + DELIMITER + "Oid" + DELIMITER + "Description" + DELIMITER + "IsDefault" + DELIMITER + "UpdatedOn");
                    foreach (VatLevel ct in vl)
                    {
                        txt.Write("VatLevel" + DELIMITER);
                        txt.Write(ct.Oid + DELIMITER);
                        txt.Write(ct.Description + DELIMITER);
                        txt.Write((ct.IsDefault ? "1" : "0") + DELIMITER);
                        txt.WriteLine("" + ct.UpdatedOnTicks);
                        recordCount++;
                    }
                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename);
                    }

                    IEnumerable<Item> itemCol = new XPCollection<Item>(uow, RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(
                        new BinaryOperator("IsActive", true), new BinaryOperator("UpdatedOnTicks", Itemupdatedon, BinaryOperatorType.GreaterOrEqual)),typeof(Item),company), sortP).Take(recordLimit - recordCount);
                    //itemCol.PageSize = 2000;
                    txt.WriteLine("Item" + DELIMITER + "Code" + DELIMITER + "DefaultBarcodeOid" + DELIMITER + "ImageOid" + DELIMITER + "InsertedDay" + DELIMITER + "isActive" + DELIMITER + "maxOrderQty" + DELIMITER + "Name" + DELIMITER + "Oid" + DELIMITER + "packingQty" + DELIMITER + "UpdatedOn" + DELIMITER + "VatCategory");
                    foreach (Item ct in itemCol)
                    {
                        txt.Write("Item" + DELIMITER);
                        txt.Write(ct.Code + DELIMITER);
                        txt.Write((ct.DefaultBarcode == null ? Guid.Empty : ct.DefaultBarcode.Oid) + DELIMITER);
                        //txt.Write(ct.ItemImageOid + DELIMITER);
                        txt.Write(Guid.Empty + DELIMITER);
                        txt.Write("" + ct.InsertedDate.Ticks + DELIMITER);
                        txt.Write((ct.IsActive ? "1" : "0") + DELIMITER);
                        txt.Write(ct.MaxOrderQty + DELIMITER);
                        txt.Write(ct.Name + DELIMITER);
                        txt.Write(ct.Oid + DELIMITER);
                        txt.Write(ct.PackingQty + DELIMITER);
                        txt.Write(ct.UpdatedOnTicks + DELIMITER);
                        txt.WriteLine(ct.VatCategory.Oid);
                        recordCount++;
                    }
                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename);
                    }

                    IEnumerable<ItemAnalyticTree> itemAT = new XPCollection<ItemAnalyticTree>(uow, CriteriaOperator.And(new BinaryOperator("IsActive", true), 
                        new BinaryOperator("UpdatedOnTicks", IATupdatedon, BinaryOperatorType.GreaterOrEqual), new BinaryOperator("Object.Owner.Oid",company.Oid)), sortP).Take(recordLimit - recordCount);
                    /*
                     */
                    txt.WriteLine("ItemAnalyticTree" + DELIMITER + "Oid" + DELIMITER + "ItemOid" + DELIMITER + "ItemCategoryOid" + DELIMITER + "UpdatedOn");
                    foreach (ItemAnalyticTree ct in itemAT)
                    {
                        txt.Write("ItemAnalyticTree" + DELIMITER);
                        txt.Write(ct.Oid + DELIMITER);
                        txt.Write(ct.Object.Oid + DELIMITER);
                        txt.Write(ct.Node.Oid + DELIMITER);
                        txt.WriteLine("" + ct.UpdatedOnTicks);
                        recordCount++;
                    }
                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename);
                    }


                    IEnumerable<Barcode> bc = new XPCollection<Barcode>(uow, CriteriaOperator.And(
                        new BinaryOperator("IsActive", true), new BinaryOperator("UpdatedOnTicks", Barcodeupdatedon, BinaryOperatorType.GreaterOrEqual)), sortP).Take(recordLimit - recordCount);
                    /*
                     */
                    txt.WriteLine("Barcode" + DELIMITER + "Oid" + DELIMITER + "ItemOid" + DELIMITER + "MeasurementUnitOid" + DELIMITER + "Code" + DELIMITER + "UpdatedOn");
                    foreach (Barcode ct in bc)
                    {
                        Item item = ItemHelper.GetItemOfSupplier(uow, ct, company);
                        if (item != null)
                        {
                            txt.Write("Barcode" + DELIMITER);
                            txt.Write(ct.Oid + DELIMITER);
                            txt.Write(item.Oid + DELIMITER);
                            txt.Write(ct.MeasurementUnit(item.Owner).Oid + DELIMITER);
                            txt.Write(ct.Code + DELIMITER);
                            txt.WriteLine("" + ct.UpdatedOnTicks);
                            recordCount++;
                        }
                    }
                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename);
                    }

                    IEnumerable<ItemCategory> ic = new XPCollection<ItemCategory>(uow, RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(
                        new BinaryOperator("IsActive", true), new BinaryOperator("UpdatedOnTicks", ICupdatedon, BinaryOperatorType.GreaterOrEqual)),typeof(ItemCategory),company), sortP).Take(recordLimit - recordCount);
                    /*
                
                     */
                    txt.WriteLine("ItemCategory" + DELIMITER + "Oid" + DELIMITER + "parentOid" + DELIMITER + "Description" + DELIMITER + "Code" + DELIMITER + "UpdatedOn");
                    foreach (ItemCategory ct in ic)
                    {
                        txt.Write("ItemCategory" + DELIMITER);
                        txt.Write(ct.Oid + DELIMITER);
                        txt.Write((ct.Parent == null ? Guid.Empty : ct.Parent.Oid) + DELIMITER);
                        txt.Write(ct.Description + DELIMITER);
                        txt.Write(ct.Code + DELIMITER);
                        txt.WriteLine("" + ct.UpdatedOnTicks);
                        recordCount++;
                    }
                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename);
                    }


                    IEnumerable<PriceCatalog> pc = new XPCollection<PriceCatalog>(uow, RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(
                        new BinaryOperator("IsActive", true), new BinaryOperator("UpdatedOnTicks", PCupdatedon, BinaryOperatorType.GreaterOrEqual)),typeof(PriceCatalog),company), sortP).Take(recordLimit - recordCount);
                    /*
                
                     */
                    txt.WriteLine("PriceCatalog" + DELIMITER + "Oid" + DELIMITER + "parentOid" + DELIMITER + "Description" + DELIMITER + "Code" + DELIMITER + "UpdatedOn");
                    foreach (PriceCatalog ct in pc)
                    {
                        txt.Write("PriceCatalog" + DELIMITER);
                        txt.Write(ct.Oid + DELIMITER);
                        txt.Write((ct.ParentCatalog == null ? Guid.Empty : ct.ParentCatalog.Oid) + DELIMITER);
                        txt.Write(ct.Description + DELIMITER);
                        txt.Write(ct.Code + DELIMITER);
                        txt.WriteLine("" + ct.UpdatedOnTicks);
                        recordCount++;
                    }
                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename);
                    }

                    //XPCollection<PriceCatalogDetail> pcd = new XPCollection<PriceCatalogDetail>(uow, new BinaryOperator("UpdatedOnTicks", PCDupdatedon, BinaryOperatorType.GreaterOrEqual), sortP);
                    XPCursor pcd = new XPCursor(uow, typeof(PriceCatalogDetail), CriteriaOperator.And(
                        new BinaryOperator("IsActive", true), new BinaryOperator("UpdatedOnTicks", PCDupdatedon, BinaryOperatorType.GreaterOrEqual), new BinaryOperator("PriceCatalog.Owner.Oid", company.Oid)), sortP);
                    pcd.PageSize = 2000;
                    pcd.TopReturnedObjects = (recordLimit - recordCount);
                    /* */
                    txt.WriteLine("PriceCatalogDetail" + DELIMITER + "Oid" + DELIMITER + "BarcodeOid" + DELIMITER + "PriceCatalogOid" + DELIMITER + "ItemOid" + DELIMITER + "value" + DELIMITER + "discount" + DELIMITER + "vatIncluded" + DELIMITER + "UpdatedOn");
                    foreach (PriceCatalogDetail ct in pcd)
                    {
                        txt.Write("PriceCatalogDetail" + DELIMITER);
                        txt.Write(ct.Oid + DELIMITER);
                        txt.Write(ct.Barcode.Oid + DELIMITER);
                        txt.Write(ct.PriceCatalog.Oid + DELIMITER);
                        txt.Write(ct.Item.Oid + DELIMITER);
                        txt.Write(ct.Value + DELIMITER);
                        txt.Write(ct.Discount + DELIMITER);
                        txt.Write(ct.VATIncluded + DELIMITER);
                        txt.WriteLine("" + ct.UpdatedOnTicks);
                        recordCount++;
                    }
                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename); 
                    }


                    IEnumerable<Offer> offers = new XPCollection<Offer>(uow, RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(
                        new BinaryOperator("IsActive", true), new BinaryOperator("UpdatedOnTicks", offerUpdatedOn, BinaryOperatorType.GreaterOrEqual)),typeof(Offer),company), sortP).Take(recordLimit - recordCount);
                    txt.WriteLine("Offer" + DELIMITER + "Oid" + DELIMITER + "PriceCatalogOid" + DELIMITER + "Description" + DELIMITER + "Description2" + DELIMITER + "isActive" + DELIMITER + "StartDate" + DELIMITER + "EndDate" + DELIMITER + "UpdatedOn");
                    foreach (Offer ct in offers)
                    {
                        txt.Write("Offer" + DELIMITER);
                        txt.Write(ct.Oid + DELIMITER);
                        txt.Write(ct.PriceCatalog.Oid + DELIMITER);
                        txt.Write(ct.Description + DELIMITER);
                        txt.Write(ct.Description2 + DELIMITER);
                        txt.Write((ct.IsActive ? "1" : "0") + DELIMITER);
                        txt.Write(ct.StartDate.Ticks + DELIMITER);
                        txt.Write(ct.EndDate.Ticks + DELIMITER);
                        txt.WriteLine("" + ct.UpdatedOnTicks);
                        recordCount++;
                    }
                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename);
                    }

                    IEnumerable<OfferDetail> offerDetails = new XPCollection<OfferDetail>(uow, CriteriaOperator.And(
                        new BinaryOperator("IsActive", true), new BinaryOperator("UpdatedOnTicks", offerDetailUpdatedOn, BinaryOperatorType.GreaterOrEqual), new BinaryOperator("Offer.Owner.Oid",company.Oid)), sortP).Take(recordLimit - recordCount);
                    txt.WriteLine("OfferDetail" + DELIMITER + "Oid" + DELIMITER + "OfferOid" + DELIMITER + "ItemOid" + DELIMITER + "isActive" + DELIMITER + "UpdatedOn");
                    foreach (OfferDetail ct in offerDetails)
                    {
                        txt.Write("OfferDetail" + DELIMITER);
                        txt.Write(ct.Oid + DELIMITER);
                        txt.Write((ct.Offer == null ? Guid.Empty : ct.Offer.Oid) + DELIMITER);
                        txt.Write(ct.Item.Oid + DELIMITER);
                        txt.Write((ct.IsActive ? "1" : "0") + DELIMITER);
                        txt.WriteLine("" + ct.UpdatedOnTicks);
                        recordCount++;
                    }
                    if (recordCount >= recordLimit)
                    {
                        txt.Close();
                        return CreateZipFile(userID, zipFileName, filename);
                    }


                }
                return CreateZipFile(userID, zipFileName, filename);
            }
        }

        private string CreateZipFile(string userID, string zipFileName, string filename)
        {
            using (ZipFile zip = new ZipFile())
            {
                if (!Directory.Exists(Server.MapPath("~/Downloads")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Downloads"));
                }
                ZipEntry z = zip.AddFile(filename, "\\");
                z.FileName = "Database.txt";
                z.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                //z.Comment = userID;
                zip.Comment = userID;
                zip.Save(Server.MapPath("~/Downloads/" + zipFileName));
            }
            return zipFileName;
        }

        public enum CreateCustomerResult
        {
            OTHERERROR = -100,
            GSIS_SERVICE_ERROR = -8,
            CUSTOMERCODENOTEXIST = -7,
            PRICECATALOGNOTINSTORE = -6,
            PRICECATALOGNOTFOUND = -5,
            USERHASNOSTOREACCESS = -4,
            INVALIDUSER = -3,
            TAXCODEFOUNDNOACCESS = -2,
            TAXCODENOTFOUND = -1,
            TAXCODEEXISTS = 0,
            TAXCODECREATED = 1
        }

        [WebMethod]
        public bool CheckValidStorePriceCatalogPair(string store, string priceCatalog)
        {
            Guid pcGuid, storeGuid;
            if (!Guid.TryParse(store, out storeGuid))
            {
                return false;
            }
            if (!Guid.TryParse(priceCatalog, out pcGuid))
            {
                return false;
            }
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Store storeobj = uow.GetObjectByKey<Store>(storeGuid);
                if (storeobj == null) return false;
                PriceCatalog pcobj = uow.GetObjectByKey<PriceCatalog>(pcGuid);
                if (pcobj == null) return false;
                StorePriceList s = uow.FindObject<StorePriceList>(CriteriaOperator.And(new BinaryOperator("Store", store), new BinaryOperator("PriceList", pcobj)));
                return s != null;
            }
        }

        [WebMethod]
        public TDocumentHeader[] GetDocumentHeaders(string userid, string customerid)
        {
            Guid userGuid, customerGuid;
            if (!Guid.TryParse(userid, out userGuid) || !Guid.TryParse(customerid, out customerGuid))
                return null;

            XPCollection<DocumentHeader> headers = GetDocumentHeaders(userGuid, customerGuid);
            IEnumerable<TDocumentHeader> documents = from doc in headers
                                                     select new TDocumentHeader
                                                     {
                                                         discount = (double)doc.TotalDiscountAmount,
                                                         finalizedDateTime = doc.FinalizedDate.Ticks,
                                                         final = (double)doc.GrossTotal,
                                                         netTotal = (double)doc.NetTotal
                                                     };
            return documents.ToArray();
        }

        [WebMethod]
        public string GetDocumentHeadersXML(string userid, string customerid)
        {
            Guid userGuid, customerGuid;
            if (!Guid.TryParse(userid, out userGuid) || !Guid.TryParse(customerid, out customerGuid))
                return null;
            XPCollection<DocumentHeader> headers = GetDocumentHeaders(userGuid, customerGuid);
            XmlDocument xmlDoc = PrepareDocumentsResultXml(headers);
            return xmlDoc.InnerXml;
        }

        [WebMethod]
        public string GetDocumentHeadersXMLWithFilters(string userid, string customerid, long fromTicks, long toTicks, string docStatusID, string docTypeID, string hasBeenChecked, string hasBeenExecuted)
        {
            Guid userGuid, customerGuid;
            if (!Guid.TryParse(userid, out userGuid) || !Guid.TryParse(customerid, out customerGuid))
                return null;


            XPCollection<DocumentHeader> headers = GetDocumentHeaders(userGuid, customerGuid, fromTicks, toTicks, docStatusID, docTypeID, hasBeenChecked, hasBeenExecuted);
            XmlDocument xmlDoc = PrepareDocumentsResultXml(headers);
            return xmlDoc.InnerXml;
        }

        private XPCollection<DocumentHeader> GetDocumentHeaders(Guid userID, Guid customerID)
        {
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            User user = uow.GetObjectByKey<User>(userID);
            Customer customer = uow.GetObjectByKey<Customer>(customerID);
            if (customer == null || customer.DefaultStore == null || user == null || !UserHelper.IsCompanyUser(user))
                return null;

            if (UserHelper.GetStoresThatUserOwns(user).Where(g => g.Oid == customer.DefaultStore.Oid).Count() != 1)
                return null;

            XPCollection<DocumentHeader> headers = new XPCollection<DocumentHeader>(uow,
                CriteriaOperator.And(
                    new BinaryOperator("Customer", customer),
                    CriteriaOperator.Or(
                        new BinaryOperator("DocumentNumber", 0, BinaryOperatorType.Greater),
                        new BinaryOperator("CreatedBy", user)
                     )
                )
            );

            return headers;
        }

        private XPCollection<DocumentHeader> GetDocumentHeaders(Guid userID, Guid customerID, long fromTicks, long toTicks, string docStatusID, string docTypeID, string hasBeenChecked, string hasBeenExecuted)
        {
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            User user = uow.GetObjectByKey<User>(userID);
            Customer customer = uow.GetObjectByKey<Customer>(customerID);
            if (customer == null || customer.DefaultStore == null || user == null || !UserHelper.IsCompanyUser(user))
                return null;

            if (UserHelper.GetStoresThatUserOwns(user).Where(g => g.Oid == customer.DefaultStore.Oid).Count() != 1)
                return null;

            CriteriaOperator fromCriteria = null;
            if (fromTicks > 0)
            {
                fromCriteria = new BinaryOperator("FinalizedDate", new DateTime(fromTicks), BinaryOperatorType.GreaterOrEqual);
            }
            CriteriaOperator toCriteria = null;
            if (toTicks > 0)
            {
                toCriteria = new BinaryOperator("FinalizedDate", new DateTime(toTicks), BinaryOperatorType.LessOrEqual);
            }

            CriteriaOperator docStatusCriteria = null;
            Guid docStatusOid = Guid.Empty;
            if (!String.IsNullOrWhiteSpace(docStatusID) && Guid.TryParse(docStatusID, out docStatusOid))
            {
                docStatusCriteria = new BinaryOperator("Status.Oid", docStatusOid);
            }

            CriteriaOperator docTypeCriteria = null;
            Guid docTypeOid = Guid.Empty;
            if (!String.IsNullOrWhiteSpace(docTypeID) && Guid.TryParse(docTypeID, out docTypeOid))
            {
                docTypeCriteria = new BinaryOperator("DocumentType.Oid", docTypeOid);
            }

            CriteriaOperator hasBeenCheckedCriteria = null;
            bool hasBeenCheckedParsed = false;
            if (!String.IsNullOrWhiteSpace(hasBeenChecked) && Boolean.TryParse(hasBeenChecked, out hasBeenCheckedParsed))
            {
                hasBeenCheckedCriteria = new BinaryOperator("HasBeenChecked", hasBeenCheckedParsed);
            }

            CriteriaOperator hasBeenExecutedCriteria = null;
            bool hasBeenExecutedParsed = false;
            if (!String.IsNullOrWhiteSpace(hasBeenExecuted) && Boolean.TryParse(hasBeenExecuted, out hasBeenExecutedParsed))
            {
                hasBeenExecutedCriteria = new BinaryOperator("HasBeenExecuted", hasBeenExecutedParsed);
            }

            CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("Customer", customer), fromCriteria, toCriteria,
                docStatusCriteria, docTypeCriteria, hasBeenCheckedCriteria, hasBeenExecutedCriteria);


            XPCollection<DocumentHeader> headers = new XPCollection<DocumentHeader>(uow,
                CriteriaOperator.And(criteria, CriteriaOperator.Or(new BinaryOperator("DocumentNumber", 0, BinaryOperatorType.Greater),
                                                                  new BinaryOperator("CreatedBy", user)
                     )
                )
            );

            return headers;
        }

        private XmlDocument PrepareDocumentsResultXml(XPCollection<DocumentHeader> headers)
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (headers.Count <= 0)
            {
                return xmlDoc;
            }
            else
            {
                xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null));
                XmlElement rootNode = xmlDoc.CreateElement("Documents");
                xmlDoc.AppendChild(rootNode);
                foreach (DocumentHeader header in headers)
                {
                    XmlNode documentNode = rootNode.AppendChild(xmlDoc.CreateElement("Document"));
                    documentNode.AppendChild(xmlDoc.CreateElement("Number")).InnerText = header.DocumentNumber.ToString();
                    documentNode.AppendChild(xmlDoc.CreateElement("TotalItems")).InnerText = header.DocumentDetails.Sum(x => x.Qty).ToString();
                    documentNode.AppendChild(xmlDoc.CreateElement("CreatedOn")).InnerText = header.CreatedOn.ToString("d/M/yyyy") + " " + header.CreatedOn.ToString("HH:mm");
                    documentNode.AppendChild(xmlDoc.CreateElement("Status")).InnerText = (header.Status == null || header.Status.Description == null ? "" : header.Status.Description.ToString());
                    documentNode.AppendChild(xmlDoc.CreateElement("Type")).InnerText = (header.DocumentType == null || header.DocumentType.Description == null ? "" : header.DocumentType.Description.ToString());
                    documentNode.AppendChild(xmlDoc.CreateElement("Discount")).InnerText = BusinessLogic.RoundAndStringify(header.TotalDiscountAmount, header.Owner) + " €";
                    documentNode.AppendChild(xmlDoc.CreateElement("NetTotal")).InnerText = BusinessLogic.RoundAndStringify(header.NetTotal, header.Owner) + " €";
                    documentNode.AppendChild(xmlDoc.CreateElement("HasBeenChecked")).InnerText = header.HasBeenChecked ? "1" : "0";
                    documentNode.AppendChild(xmlDoc.CreateElement("HasBeenExecuted")).InnerText = header.HasBeenExecuted ? "1" : "0";
                    documentNode.AppendChild(xmlDoc.CreateElement("FinalizedDateTime")).InnerText = header.FinalizedDate.ToString("d/M/yyyy") + " " + header.FinalizedDate.ToString("HH:mm");
                    documentNode.AppendChild(xmlDoc.CreateElement("Total")).InnerText = BusinessLogic.RoundAndStringify(header.GrossTotal, header.Owner) + " €";
                }
                return xmlDoc;
            }

        }

        [WebMethod]
        public int CreateCustomer(string taxCode, string customer_code, string userid, string store, string priceCatalog)
        {
            try
            {
                Guid userguid;
                if (!Guid.TryParse(userid, out userguid))
                    return (int)CreateCustomerResult.INVALIDUSER;
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    User user = uow.GetObjectByKey<User>(userguid);
                    if (user == null)
                    {
                        return (int)CreateCustomerResult.INVALIDUSER;
                    }

                    XPCollection<Customer> customers = new XPCollection<Customer>(uow, new BinaryOperator("Trader.TaxCode", taxCode));
                    IEnumerable<Store> stores = UserHelper.GetStoresThatUserOwns(user);
                    if (customers.Count > 0)
                    {

                        foreach (Customer cust in customers)
                        {
                            if (stores.Where(g => g.Oid == cust.DefaultStore.Oid).Count() > 0)
                            {
                                return (int)CreateCustomerResult.TAXCODEEXISTS;
                            }
                        }
                        return (int)CreateCustomerResult.TAXCODEFOUNDNOACCESS;
                    }
                    else
                    {
                        //create
                        Guid pcGuid, storeGuid;
                        if (!Guid.TryParse(store, out storeGuid))
                        {
                            return (int)CreateCustomerResult.OTHERERROR;
                        }
                        IEnumerable<Store> foundStores = stores.Where(g => g.Oid == storeGuid);
                        if (foundStores.Count() == 0)
                        {
                            return (int)CreateCustomerResult.OTHERERROR;
                        }

                        Store customerStore = foundStores.FirstOrDefault();
                        //PriceCatalog pc = uow.FindObject<PriceCatalog>(new BinaryOperator("Code", priceCatalog));



                        if (!Guid.TryParse(priceCatalog, out pcGuid))
                        {
                            return (int)CreateCustomerResult.PRICECATALOGNOTFOUND;
                        }
                        PriceCatalog pc = uow.GetObjectByKey<PriceCatalog>(pcGuid);
                        if (pc == null)
                        {
                            return (int)CreateCustomerResult.PRICECATALOGNOTFOUND;
                        }
                        IEnumerable<PriceCatalog> foundPcs = customerStore.StorePriceLists.Where(g => g.PriceList.Oid == pc.Oid).Select(x => x.PriceList);

                        if (foundPcs.Count() == 0)
                        {
                            return (int)CreateCustomerResult.PRICECATALOGNOTINSTORE;
                        }

                        return (int)CreateCustomerResult.GSIS_SERVICE_ERROR;
                        /*using (RgWsBasStoixN service = new RgWsBasStoixN())
                        {
                            service.Timeout = 10000;
                            service.Url = "https://www1.gsis.gr/wsgsis/RgWsBasStoixN/RgWsBasStoixNSoapHttpPort";
                            RgWsBasStoixEpitRtUser infoOut = new RgWsBasStoixEpitRtUser();
                            decimal pCallSeqId_out = 0;
                            GenWsErrorRtUser error = new GenWsErrorRtUser();
                            try
                            {
                                service.rgWsBasStoixEpit(taxCode, ref infoOut, ref pCallSeqId_out, ref error);
                            }
                            catch //(Exception e)
                            {
                                return (int)CreateCustomerResult.GSIS_SERVICE_ERROR;
                            }
                            if (error == null || String.IsNullOrEmpty(error.errorCode))
                            {
                                Trader trader = uow.FindObject<Trader>(new BinaryOperator("TaxCode", taxCode));
                                if (trader == null)
                                {
                                    trader = new Trader(uow);
                                    trader.TaxCode = taxCode;
                                    trader.TaxOffice = infoOut.doyDescr;
                                    trader.IsActive = true;
                                    trader.CreatedBy = user;
                                    trader.Save();
                                }

                                Customer customer = null;
                                if (!String.IsNullOrWhiteSpace(customer_code))
                                {
                                    customer = uow.FindObject<Customer>(new BinaryOperator("Code", customer_code));
                                    if (customer == null)
                                        return (int)CreateCustomerResult.CUSTOMERCODENOTEXIST;
                                }
                                else
                                {
                                    customer = new Customer(uow);
                                    customer.Code = "";
                                }
                                customer.Trader = trader;
                                customer.IsActive = true;
                                customer.CompanyName = infoOut.onomasia;

                                customer.CompanyBrandName = infoOut.commerTitle;
                                customer.Owner = customerStore.Owner;
                                customer.Profession = infoOut.actLongDescr;
                                customer.PaymentMethod = uow.FindObject<PaymentMethod>(new BinaryOperator("IsDefault", true));
                                if (customer.PaymentMethod == null)
                                {
                                    customer.PaymentMethod = new XPCollection<PaymentMethod>(uow).FirstOrDefault();
                                }
                                customer.VatLevel = uow.FindObject<VatLevel>(new BinaryOperator("IsDefault", true));
                                if (customer.VatLevel == null)
                                {
                                    customer.VatLevel = new XPCollection<VatLevel>(uow).FirstOrDefault();
                                }

                                if (!string.IsNullOrEmpty(infoOut.postalAddress))
                                {
                                    Address address = trader.Addresses.Where(x => x.Street == infoOut.postalAddress).FirstOrDefault();
                                    if (address == null)
                                    {
                                        address = new Address(uow);
                                        address.Street = infoOut.postalAddress;
                                        address.City = infoOut.parDescription;
                                        address.Trader = trader;
                                        address.PostCode = infoOut.postalZipCode;
                                        address.Save();
                                    }
                                }
                                StorePriceList spl;
                                spl = customerStore.StorePriceLists.Where(g => g.PriceList.Oid == pc.Oid).First();
                                CustomerStorePriceList cspl = new CustomerStorePriceList(uow);
                                cspl.StorePriceList = spl;
                                cspl.Customer = customer;
                                customer.Save();
                                cspl.Save();
                                XpoHelper.CommitChanges(uow);
                                return (int)CreateCustomerResult.TAXCODECREATED;
                            }
                            else
                            {
                                return (int)CreateCustomerResult.TAXCODENOTFOUND;
                            }
                        }*/
                    }
                }
            }
            catch// (Exception e)
            {

                return (int)CreateCustomerResult.OTHERERROR;
            }

        }

        [WebMethod]
        public String GetImagesLink(String userID, params String[] imageGuids)
        {
            String zipFileName = "";

            Guid userGuid, zipfilenamegd;
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {

                if (!Guid.TryParse(userID, out userGuid))
                    return null;
                User user = uow.GetObjectByKey<User>(userGuid);
                if (user == null || !UserHelper.IsCompanyUser(user))
                    return null;

                zipfilenamegd = Guid.NewGuid();
                String filename = Path.GetTempFileName();
                zipFileName = "I" + zipfilenamegd.ToString().Replace("-", "") + ".zip";
                int[] sizes = { 32, 64, 128 };
                using (ZipFile zip = new ZipFile())
                {
                    if (!Directory.Exists(Server.MapPath("~/Downloads")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Downloads"));
                    }

                    foreach (String str in imageGuids)
                    {
                        Guid itemImageOid;
                        if (Guid.TryParse(str, out itemImageOid) && itemImageOid != Guid.Empty)
                        {

                            //ItemImage itm = uow.GetObjectByKey<ItemImage>(itemImageOid);
                            Item itm = uow.GetObjectByKey<Item>(itemImageOid);

                            if (itm != null)
                            {
                                String imgFilename = "img_" + itm.Oid.ToString().Replace("-", "") + ".png";
                                foreach (int sz in sizes)
                                {
                                    Size imgSize = new Size() { Height = sz, Width = sz };
                                    Image toSave = RetailHelper.ResizeImage(itm.ImageSmall, imgSize);
                                    byte[] result = null;
                                    using (MemoryStream stream = new MemoryStream())
                                    {
                                        toSave.Save(stream, ImageFormat.Png);
                                        result = stream.ToArray();
                                    }
                                    ZipEntry z = zip.AddEntry("\\" + sz + "\\" + imgFilename.ToLower(), result);
                                    z.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                                    //z.Comment = userID;
                                    //                                    toSave.Save("c:\\test.png",GetEncoderInfo("image/jpeg");)
                                }
                            }
                        }
                    }

                    /*ZipEntry z = zip.AddFile(filename, "\\");
                    
                    z.FileName = "Database.txt";
                    z.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                    z.Comment = userID;*/
                    zip.Comment = userID;
                    zip.Save(Server.MapPath("~/Downloads/" + zipFileName));
                }
            }
            return zipFileName;
        }

        [WebMethod]
        public List<String> DoesExistsAndIsActive(String[] objectIds, String objectType, String userid)
        {
            Guid userguid;
            if (!Guid.TryParse(userid, out userguid))
                return null;
            Type type = Assembly.GetAssembly(typeof(Customer)).GetType("ITS.Retail.Model." + objectType);
            List<String> results = new List<String>();
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Guid objGuid;
                foreach (String obid in objectIds)//
                {
                    if (Guid.TryParse(obid, out objGuid))
                    {
                        BaseObj obj = (BaseObj)uow.GetObjectByKey(type, objGuid);
                        if (obj == null || !obj.IsActive)
                        {
                            results.Add(obid);
                        }
                    }
                }
            }
            return results;
        }

        [WebMethod]
        public List<String> GetInactiveObjects(long mobileUpdatedOn, String objectType, String userid)
        {
            Guid userguid;
            //const int recordLimit = 100000;
            if (!Guid.TryParse(userid, out userguid))
                return null;
            Type type = Assembly.GetAssembly(typeof(Customer)).GetType("ITS.Retail.Model." + objectType);
            List<String> results = new List<String>();
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                User user = uow.GetObjectByKey<User>(userguid);
                if (user == null)
                {
                    return null;
                }
                SortProperty[] sortP = new SortProperty[] { new SortProperty("UpdatedOnTicks", SortingDirection.Ascending) };
                XPCursor objectList = (new XPCursor(uow, type, CriteriaOperator.And(new BinaryOperator("IsActive", false),
                                                                                             new BinaryOperator("UpdatedOnTicks", mobileUpdatedOn, BinaryOperatorType.GreaterOrEqual))
                                                                                            , sortP));
                //objectList.TopReturnedObjects = recordLimit;
                objectList.PageSize = 2000;
                foreach (BaseObj obj in objectList)//
                {
                    results.Add(obj.Oid.ToString());
                }
            }
            return results;
        }

        [WebMethod]
        public Guid GetStoreControllerGuid(string storeControllerId, string username, string password)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                User user = (User)XpoHelper.GetNewUnitOfWork().FindObject<User>(CriteriaOperator.Parse("UserName='" + username + "' and Password='" + UserHelper.EncodePassword(password) + "'", ""));
                if (user == null)
                {
                    return Guid.Empty;
                }
                CompanyNew supplier = UserHelper.GetCompany(user);
                if (supplier == null)
                {
                    return Guid.Empty;
                }

                StoreControllerSettings settings = uow.FindObject<StoreControllerSettings>(
                    CriteriaOperator.And(new BinaryOperator("ID", storeControllerId), new BinaryOperator("Owner.Oid", supplier.Oid)));
                if (settings == null)
                {
                    return Guid.Empty;
                }

                return settings.Oid;

            }
        }

        [WebMethod]
        public string GetOwner(string storeControllerID, string username, string password)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                User user = (User)XpoHelper.GetNewUnitOfWork().FindObject<User>(CriteriaOperator.Parse("UserName='" + username + "' and Password='" + UserHelper.EncodePassword(password) + "'", ""));
                if (user == null)
                {
                    return "{}";
                }
                CompanyNew supplier = UserHelper.GetCompany(user);
                if (supplier == null)
                {
                    return "{}";
                }

                StoreControllerSettings settings = uow.FindObject<StoreControllerSettings>(
                    CriteriaOperator.And(new BinaryOperator("ID", storeControllerID), new BinaryOperator("Owner.Oid", supplier.Oid)));
                if (settings == null)
                {
                    return "{}";
                }

                return settings.Owner.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
            }
        }

        [WebMethod]
        public string GetStoreControllerSettings(string storeControllerID, string username, string password)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                User user = (User)XpoHelper.GetNewUnitOfWork().FindObject<User>(CriteriaOperator.Parse("UserName='" + username + "' and Password='" + UserHelper.EncodePassword(password) + "'", ""));
                if (user == null)
                {
                    return "{}";
                }
                CompanyNew supplier = UserHelper.GetCompany(user);
                if (supplier == null)
                {
                    return "{}";
                }

                StoreControllerSettings settings = uow.FindObject<StoreControllerSettings>(
                    CriteriaOperator.And(new BinaryOperator("ID", storeControllerID), new BinaryOperator("Owner.Oid", supplier.Oid)));
                if (settings == null)
                {
                    return "{}";
                }

                return settings.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
            }
        }

        [WebMethod]
        public Guid GetStoreOfStoreController(string storeControllerId, string username, string password)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                User user = (User)XpoHelper.GetNewUnitOfWork().FindObject<User>(CriteriaOperator.Parse("UserName='" + username + "' and Password='" + UserHelper.EncodePassword(password) + "'", ""));
                if (user == null)
                {
                    return Guid.Empty;
                }
                CompanyNew supplier = UserHelper.GetCompany(user);
                if (supplier == null)
                {
                    return Guid.Empty;
                }

                StoreControllerSettings settings = uow.FindObject<StoreControllerSettings>(
                    CriteriaOperator.And(new BinaryOperator("ID", storeControllerId), new BinaryOperator("Owner.Oid", supplier.Oid)));
                if (settings == null)
                {
                    return Guid.Empty;
                }

                return settings.Store == null ? Guid.Empty : settings.Store.Oid;

            }
        }

        [WebMethod]
        public Guid GetDefaultCustomerOfStoreController(string storeControllerId, string username, string password)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                User user = (User)XpoHelper.GetNewUnitOfWork().FindObject<User>(CriteriaOperator.Parse("UserName='" + username + "' and Password='" + UserHelper.EncodePassword(password) + "'", ""));
                if (user == null)
                {
                    return Guid.Empty;
                }
                CompanyNew supplier = UserHelper.GetCompany(user);
                if (supplier == null)
                {
                    return Guid.Empty;
                }

                StoreControllerSettings settings = uow.FindObject<StoreControllerSettings>(
                    CriteriaOperator.And(new BinaryOperator("ID", storeControllerId), new BinaryOperator("Owner.Oid", supplier.Oid)));
                if (settings == null)
                {
                    return Guid.Empty;
                }

                return settings.DefaultCustomer == null ? Guid.Empty : settings.DefaultCustomer.Oid;

            }
        }

        [WebMethod(Description = "Get Mobile Filelist")]
        public String[][] GetMobileFilelist()
        {
            return GetFileList("Mobile");
        }

        protected String[][] GetFileList(string contentfolder)
        {
            DirectoryInfo df = new DirectoryInfo(Server.MapPath("~/RetailMobile/" + contentfolder));
            FileInfo[] fi = df.GetFiles();
            List<String[]> lst = new List<String[]>();
            foreach (FileInfo ft in fi)
            {
                String[] toAdd = new String[3];
                toAdd[0] = ft.Name;
                toAdd[1] = GetFileHash(Server.MapPath("~/RetailMobile/" + contentfolder) + "\\" + ft.Name);
                toAdd[2] = HttpContext.Current.Request.Url.ToString().Substring(0, HttpContext.Current.Request.Url.ToString().ToUpper().IndexOf("/RetailService.asmx".ToUpper()))
                    + "/RetailMobile/" + contentfolder + "/" + ft.Name;
                lst.Add(toAdd);

            }
            return lst.ToArray();
        }

        protected String GetFileHash(String file)
        {
            String strResult = "";

            byte[] arrbytHashValue;
            System.IO.FileStream oFileStream = null;

            using (MD5CryptoServiceProvider oMD5Hasher = new MD5CryptoServiceProvider())
            {
                try
                {
                    oFileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    arrbytHashValue = oMD5Hasher.ComputeHash(oFileStream);
                    oFileStream.Close();
                    strResult = System.BitConverter.ToString(arrbytHashValue);
                }
                catch (System.Exception ex)
                {
                    return "-1";
                }
            }

            return (strResult);
        }
    }
}
#endif
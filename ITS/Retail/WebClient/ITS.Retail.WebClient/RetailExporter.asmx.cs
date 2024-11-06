using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using ITS.Retail.Model;
using ITS.Retail.Common;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Common.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient
{
    /// <summary>
    /// Summary description for RetailExporter
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class RetailExporter : WebService
    {

        //private int MAX_ITEMS = 1000;
        //private readonly String DELIMITER = "\t&\t";
		ApplicationSettings application_settings;
        public RetailExporter()
		{

			this.application_settings = XpoHelper.GetNewUnitOfWork().FindObject<ApplicationSettings>(null);

		}

        [WebMethod]
        public String ImportDocument(String Username, String Password, String DocumentCommaDelimeted)
        {
            String message;
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                try
                {
                    User user = uow.FindObject<User>(CriteriaOperator.And(new BinaryOperator("UserName", Username), new BinaryOperator("Password",  UserHelper.EncodePassword(Password))));
                    if (user == null)
                    {
                        return Resources.Login_Failed ;
                    }
                    if (UserHelper.IsCustomer(user))
                    {
                        XPCollection<DocumentType> documentTypes = new XPCollection<DocumentType>(uow, new BinaryOperator("IsDefault", true));
                        if (documentTypes.Count == 0)
                        {
                            documentTypes = new XPCollection<DocumentType>(uow);
                        }

                        XPCollection<DocumentStatus> documentStatus = new XPCollection<DocumentStatus>(uow, new BinaryOperator("IsDefault", true));
                        if (documentStatus.Count == 0)
                        {
                            documentStatus = new XPCollection<DocumentStatus>(uow);
                        }

                        DocumentHeader document = new DocumentHeader(uow);
                        document.CreatedBy = user;
                        document.Customer = UserHelper.GetCustomer(user);
                        document.DeliveryAddress = document.Customer.DefaultAddress == null ? "" : document.Customer.DefaultAddress.Description;
                        document.Division = eDivision.Purchase;
                        document.DocumentType = documentTypes.First();
                        document.FinalizedDate = DateTime.Now;
                        document.FiscalDate = DateTime.Now;
                        document.Status = documentStatus.First();
                        CustomerStorePriceList cspl = document.Customer.CustomerStorePriceLists.First(g=>g.IsDefault);
                        if (cspl == null)
                        {
                            cspl = document.Customer.CustomerStorePriceLists.First();
                        }
                        if (cspl != null)
                        {
                            document.Store = cspl.StorePriceList.Store;
                        }
                        else
                        {
                            return "Invalid Customer Settings";
                        }

                        IEnumerable<StoreDocumentSeriesType> sdst = document.DocumentType.StoreDocumentSeriesTypes.Where(g => g.DocumentSeries.Store == document.Store);
                        if (sdst.Count() > 0)
                        {
                            document.DocumentSeries = sdst.First().DocumentSeries;
                        }
                        PriceCatalog currentPC = document.Customer.GetDefaultPriceCatalog();
                        List<string> unknownItems = new List<string>();
                        List<string> unpricedItems = new List<string>();
                        List<string> documentLines = DocumentCommaDelimeted.Split(' ').ToList();

                        foreach (String line in documentLines)
                        {
                            String[] elements = line.Split(';');
                            String Code = elements[0];
                            String sQty = elements[1];
                            decimal qty;
                            if (!decimal.TryParse(sQty, out qty))
                            {
                                qty = 1;
                            }
                            string searchBarcode = Code;
                            string searchItem = Code;

                            if (document.Customer.Owner.OwnerApplicationSettings.PadItemCodes)
                            {
                                searchItem = searchItem.PadLeft(document.Customer.Owner.OwnerApplicationSettings.ItemCodeLength, document.Customer.Owner.OwnerApplicationSettings.ItemCodePaddingCharacter[0]);
                            }

                            if (document.Customer.Owner.OwnerApplicationSettings.PadBarcodes)
                            {
                                searchBarcode = searchBarcode.PadLeft(document.Customer.Owner.OwnerApplicationSettings.BarcodeLength, document.Customer.Owner.OwnerApplicationSettings.BarcodePaddingCharacter[0]);
                            }

                            PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetail(document.Store, searchBarcode, document.Customer);
                            PriceCatalogDetail pricecatalogdetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                            if (pricecatalogdetail != null)
                            {
                                DocumentDetail document_detail = DocumentHelper.ComputeDocumentLine(
                                    ref document, pricecatalogdetail.Item, priceCatalogPolicyPriceResult.SearchBarcode,
                                    qty, false, pricecatalogdetail.Value, false,"",
                                    null);
                                DocumentHelper.AddItem(ref document, document_detail);
                            }
                            else
                            {
                                Barcode barcode = uow.FindObject<Barcode>(new BinaryOperator("Code", searchBarcode, BinaryOperatorType.Equal));
                                Item item = uow.FindObject<Item>(new BinaryOperator("Code", searchItem, BinaryOperatorType.Equal));

                                if (barcode == null && item == null)
                                {
                                    unknownItems.Add(Code);
                                }
                                else
                                {
                                    unpricedItems.Add(Code);
                                }
                                
                            }

                        }
                        if (document.DocumentDetails.Count > 0)
                        {
                            document.Save();
                            uow.CommitChanges();
                            message = Resources.SavedSuccesfully;
                        }
                        else
                        {
                            message = "No document saved.";
                        }
                        if (unknownItems.Count > 0)
                        {
                            message += Environment.NewLine;
                            foreach (String code in unknownItems)
                            {
                                message += Resources.ItemNotFound + " : " + code + Environment.NewLine;
                            }
                        }
                        if (unpricedItems.Count > 0)
                        {
                            message += Environment.NewLine;
                            foreach (String code in unpricedItems)
                            {
                                message += "No price found for : " + code + Environment.NewLine;
                            }
                        }
                        return message;
                        
                    }
                }
                catch (Exception ex)
                {
                    return Resources.AnErrorOccurred+" "+ex.Message;
                }
            }
            return Resources.AnErrorOccurred;
        }
        
    }
}

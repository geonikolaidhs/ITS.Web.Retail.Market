using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Platform.Common.AuxilliaryClasses;
using ITS.Retail.Platform.Common.Helpers;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Extensions;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.WebClient.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace ITS.Retail.WebClient.Controllers
{
    [AllowAnonymous]
    public class PriceCheckerController : BaseController
    {
        //
        // GET: /PriceChecker/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MK1200()
        {
            return View();
        }

        public ActionResult SG15()
        {
            return View();
        }

        public ActionResult ScanCode()
        {
            return PartialView();
        }


        public ActionResult CheckPrice(string searchcode)
        {
            PriceCheckerViewModel priceCheckerViewModel = GetPriceCheckerViewModel(searchcode);
            return View(priceCheckerViewModel);
        }

        public ActionResult MK1200CheckPrice(string searchcode)
        {
            PriceCheckerViewModel priceCheckerViewModel = GetPriceCheckerViewModel(searchcode);
            return View(priceCheckerViewModel);
        }

        public ActionResult SG15CheckPrice(string searchcode)
        {
            PriceCheckerViewModel priceCheckerViewModel = GetPriceCheckerViewModel(searchcode);
            return View(priceCheckerViewModel);
        }

        [AcceptVerbs("GET", "POST")]
        public string SG15CheckPriceTcp(string searchcode)
        {

            JsonSerializerSettings sets = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                NullValueHandling = NullValueHandling.Ignore
            };

            PriceCheckerViewModel priceCheckerViewModel = GetPriceCheckerViewModel(searchcode);
            SG15Result result = new SG15Result();
            string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = new CultureInfo("el");
            if (priceCheckerViewModel != null)
            {
                if (priceCheckerViewModel.customer != null)
                {
                    result.GeneralInfo = Resources.Customer;
                    result.GeneralResult = priceCheckerViewModel.customer.Description;
                    result.MainInfo = Resources.Points;
                    Int32 points = Convert.ToInt32(priceCheckerViewModel.customer.CollectedPoints); 
                    result.mainResult = (points).ToString();
                }
                else if (priceCheckerViewModel.weightedBarcodeInfo != null)
                {
                    string weightedValue = BusinessLogic.RoundAndStringify(priceCheckerViewModel.priceCatalogDetail.RetailValue, priceCheckerViewModel.Owner);
                    result.GeneralInfo = Resources.Item;
                    result.GeneralResult = priceCheckerViewModel.itemBarcode.Item.Name;
                    result.MainInfo = "Ποσ" + ":" + Decimal.Round(priceCheckerViewModel.weightedBarcodeInfo.Quantity,2).ToString();
                    result.mainResult = "  Τιμ "   + Decimal.Round(priceCheckerViewModel.weightedBarcodeInfo.Value,2).ToString(); 
                }
                else if (priceCheckerViewModel.itemBarcode == null)
                {
                    result.error = Resources.ItemNotFound;
                }
                else if (priceCheckerViewModel.priceCatalogDetail == null)
                {
                    result.error = Resources.PriceNotFound;
                }
                else
                {
                    result.GeneralInfo = Resources.Item;
                    result.GeneralResult = priceCheckerViewModel.itemBarcode.Item.Name;
                    result.MainInfo = "Τιμή Μον ";
                    result.mainResult = BusinessLogic.RoundAndStringify(priceCheckerViewModel.priceCatalogDetail.RetailValue, priceCheckerViewModel.Owner) + "€";
                }
            }
            var ser = JsonSerializer.Create(sets);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
            return JsonConvert.SerializeObject(result, sets);
        }




        private PriceCheckerViewModel GetPriceCheckerViewModel(string searchcode)
        {
            PriceCatalogDetail priceCatalogDetail = null;
            ItemBarcode itemBarcode = null;
            BarcodeParseResult barcodeParseResult = null;
            Store store = GetList<StoreControllerSettings>(XpoSession).FirstOrDefault().Store;
            WeightedBarcodeInfo weightedBarcodeInfo = null;
            CompanyNew owner = store.Owner;
            Customer customer = null;

            if (String.IsNullOrEmpty(searchcode) == false)
            {
                string barcodeString = owner.OwnerApplicationSettings.PadBarcodes
                                       ? searchcode.PadLeft(owner.OwnerApplicationSettings.BarcodeLength, owner.OwnerApplicationSettings.BarcodePaddingCharacter[0])
                                       : searchcode;

                string itemcodeString = owner.OwnerApplicationSettings.PadItemCodes
                                       ? searchcode.PadLeft(owner.OwnerApplicationSettings.ItemCodeLength, owner.OwnerApplicationSettings.ItemCodePaddingCharacter[0])
                                       : searchcode;

                Barcode barcode = XpoSession.FindObject<Barcode>(new BinaryOperator("Code", barcodeString));
                Item item = null;

                if (barcode == null)//No barcode found search for item
                {
                    item = XpoSession.FindObject<Item>(new BinaryOperator("Code", itemcodeString));
                    if (item != null && item.DefaultBarcode != null)
                    {
                        CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("Item", item.Oid),
                                                                         new BinaryOperator("Barcode", item.DefaultBarcode.Oid));

                        criteria = ApplyOwnerCriteria(criteria, typeof(ItemBarcode), owner);

                        itemBarcode = XpoSession.FindObject<ItemBarcode>(criteria);
                    }
                }
                else//barcode found
                {
                    itemBarcode = barcode.ItemBarcode(owner);
                }

                if (itemBarcode != null)
                {
                    PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetail(store, itemBarcode.Barcode.Code);
                    priceCatalogDetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                }
                else//no item found. check for weighted item
                {
                    barcodeParseResult = CustomBarcodeHelper.ParseCustomBarcode(GetList<BarcodeType>(XpoSession).ToList(),
                                                                         searchcode,
                                                                         owner.OwnerApplicationSettings.PadBarcodes,
                                                                         owner.OwnerApplicationSettings.BarcodeLength,
                                                                         owner.OwnerApplicationSettings.BarcodePaddingCharacter.First()
                                                                         );
                    PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(XpoSession as UnitOfWork,
                                                                                                               new EffectivePriceCatalogPolicy(store, store.DefaultPriceCatalogPolicy),
                                                                                                               barcodeParseResult.DecodedCode);
                    priceCatalogDetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                    if (priceCatalogDetail != null
                        && (barcodeParseResult.BarcodeParsingResult == BarcodeParsingResult.ITEM_CODE_QUANTITY
                           || barcodeParseResult.BarcodeParsingResult == BarcodeParsingResult.ITEM_CODE_VALUE
                          )
                       )
                    {
                        weightedBarcodeInfo = new WeightedBarcodeInfo(barcodeParseResult);
                        weightedBarcodeInfo.PriceCatalogDetail = priceCatalogDetail.Oid;
                        if (weightedBarcodeInfo.BarcodeParsingResult == BarcodeParsingResult.ITEM_CODE_VALUE && priceCatalogDetail.RetailValue != 0)
                        {
                            weightedBarcodeInfo.Quantity = weightedBarcodeInfo.Value / priceCatalogDetail.RetailValue;
                        }
                        if (weightedBarcodeInfo.BarcodeParsingResult == BarcodeParsingResult.ITEM_CODE_QUANTITY)
                        {
                            weightedBarcodeInfo.Value = weightedBarcodeInfo.Quantity * priceCatalogDetail.RetailValue;
                        }

                        Barcode salesBarcode = XpoSession.FindObject<Barcode>(
                                                                        RetailHelper.ApplyOwnerCriteria(
                                                                                        new BinaryOperator("Code", weightedBarcodeInfo.DecodedCode),
                                                                                        typeof(Barcode),
                                                                                        owner
                                                                          ));
                        if (salesBarcode != null)
                        {
                            weightedBarcodeInfo.Barcode = salesBarcode.Oid;
                            CriteriaOperator itemBarcodeCriteria = RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(
                                                                                                                        new BinaryOperator("Item", priceCatalogDetail.Item),
                                                                                                                        new BinaryOperator("Barcode", salesBarcode.Oid)
                                                                                                                       ),
                                                                                                   typeof(ItemBarcode),
                                                                                                   owner
                                                                                                  );
                            ItemBarcode salesItemBarcode = XpoSession.FindObject<ItemBarcode>(itemBarcodeCriteria);
                            if (salesItemBarcode != null)
                            {
                                weightedBarcodeInfo.ItemBarcode = salesItemBarcode.Oid;
                                itemBarcode = salesItemBarcode;
                            }
                        }
                    }
                    else//no barcode/item or weighted item found.check for customer
                    {
                        CriteriaOperator customerCriteria = CriteriaOperator.And(
                                                                      CriteriaOperator.Or(new BinaryOperator("CardID", searchcode),
                                                                        new BinaryOperator("Code", searchcode),
                                                                        new BinaryOperator("Trader.TaxCode", searchcode)
                                                                       ),
                                                                       new BinaryOperator("Owner", owner.Oid)
                                                                      );
                        customer = XpoSession.FindObject<Customer>(customerCriteria);
                    }
                }

            }


            PriceCheckerViewModel priceCheckerViewModel = new PriceCheckerViewModel()
            {
                itemBarcode = itemBarcode,
                priceCatalogDetail = priceCatalogDetail,
                Owner = owner,
                weightedBarcodeInfo = weightedBarcodeInfo,
                customer = customer
            };
            return priceCheckerViewModel;
        }

    }

    public class SG15Result
    {
        public string GeneralInfo { get; set; }
        public string GeneralResult { get; set; }
        public string MainInfo { get; set; }
        public string mainResult { get; set; }
        public string error { get; set; }
    }
}

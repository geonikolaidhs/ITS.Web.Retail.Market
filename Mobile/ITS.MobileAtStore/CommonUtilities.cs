using System;
using System.Collections.Generic;
using System.Text;
using ITS.MobileAtStore.ObjectModel;
using DevExpress.Xpo;
using System.Drawing;
using ITS.Common.Utilities.Compact;
using System.Windows.Forms;
using System.Data;
using System.ComponentModel;
using DevExpress.Data.Filtering;
//using ITS.MobileAtStore.AuxilliaryClasses;


namespace ITS.MobileAtStore
{
    public static class CommonUtilities
    {
        /// <summary>
        /// Converts a given web service typed product to its equivalent objectmodel type.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="dataLayer"></param>
        /// <returns></returns>
        public static Product ConvertProduct(WRMMobileAtStore.Product product, IDataLayer dataLayer)
        {
            if (product == null)
            {
                return null;
            }
            Product convertedProduct = new Product(new Session(dataLayer));
            convertedProduct.Code = product.Code;
            convertedProduct.Barcode = product.Barcode;
            convertedProduct.ErrorMessage = product.ErrorMessage;
            convertedProduct.ExtraCode = product.ExtraCode;
            convertedProduct.Stock = product.Stock;            
            if (product.ProductQuantities != null && product.ProductQuantities.Length > 0)
            {
                convertedProduct.ProductQuantities = new List<ProductQuantity>();
                foreach (WRMMobileAtStore.ProductQuantity pq in product.ProductQuantities)
                {
                    ProductQuantity cpq = new ProductQuantity();
                    cpq.Code = pq.Code;
                    cpq.ExtraCode = pq.ExtraCode;
                    cpq.Quantity = pq.Quantity;
                    convertedProduct.ProductQuantities.Add(cpq);
                }
            }
            convertedProduct.RequiredQuantity = product.RequiredQuantity;
            convertedProduct.RestInvQty = product.RestInvQty;
            convertedProduct.VatCat = product.VatCat;
            convertedProduct.Description = product.Description.Trim();
            convertedProduct.Price = product.Price;
            convertedProduct.Supplier = product.Supplier == null ? string.Empty : product.Supplier.Trim();
            convertedProduct.IsActive = product.IsActive;
            convertedProduct.IsActiveOnSupplier = product.IsActiveOnSupplier;
            convertedProduct.ExtraCodeIsActive = product.ExtraCodeIsActive;
            convertedProduct.AverageMonthSales = product.AverageMonthSales;
            convertedProduct.OrderMM = product.OrderMM;
            convertedProduct.extrainfo = product.extrainfo;
            convertedProduct.BasicSupplier = product.BasicSupplier;
            convertedProduct.BasicSupplierColor = product.BasicSupplierColor;
            convertedProduct.CalculatedTotalPrice = product.CalculatedTotalPrice;
            convertedProduct.BarcodeParsingResult = product.BarcodeParsingResult;
            convertedProduct.Quantity = product.Quantity;
            convertedProduct.WeightedDecodedBarcode = product.WeightedDecodedBarcode;
            convertedProduct.SupportsDecimalQuantities = product.SupportsDecimalQuantities;
            return convertedProduct;
        }

        public static void SetExtraInfoText(Product prod, System.Windows.Forms.Label lblExtraInfo)
        {
            //Initialize ExtraInfo
            lblExtraInfo.Text = "";
            lblExtraInfo.ForeColor = Color.DarkBlue;

            //Figure out what to display in the ExtrInfo label text..
            if (prod.IsActive == false)
            {
                //MessageForm.Execute("Πληροφορία", "Το προϊόν δεν είναι ενεργό για παραγγελία");
                lblExtraInfo.Text = "Μη ενεργό για παραγγελία";
                lblExtraInfo.ForeColor = Color.Red;
            }
            else
                if (string.IsNullOrEmpty(prod.Supplier))
                {
                    //MessageForm.Execute("Πληροφορία", "Το προϊόν δεν έχει βασικό προμηθευτή");
                    lblExtraInfo.Text = "Χωρίς προμηθευτή";
                    lblExtraInfo.ForeColor = Color.Red;
                }
                else
                    if (prod.IsActiveOnSupplier == false)
                    {
                        lblExtraInfo.Text = "Ανενεργό σε προμηθευτή";
                        lblExtraInfo.ForeColor = Color.Red;
                    }
                    else
                    {
                        lblExtraInfo.Text = prod.Supplier;
                    }
        }

        /// <summary>
        /// Takes for granted that you have already open the data layers and deletes the given document type if it has 0 lines
        /// </summary>
        /// <param name="docType">Given docType to check</param>
        /// <returns>true if we deleted it</returns>
        public static bool DeleteEmptyDocument(DOC_TYPES docType, bool forcedOffline)
        {
            using (UnitOfWork uow = new UnitOfWork(MobileAtStore.TransactionsDL))
            {
                Header header = null;/* = uow.FindObject<Header>(CriteriaOperator.And(
                                                           CriteriaOperator.Parse("[DocType] = ?", (int)docType), new BinaryOperator("ForcedOffline", 
                                                           forcedOffline)));*/
                XPCollection<Header> allHeaders = new XPCollection<Header>(uow, CriteriaOperator.Parse(string.Format("[DocType] = {0}", (int)docType)));

                //backwards compartible
                foreach (Header head in allHeaders)
                {
                    if (head.ForcedOffline == forcedOffline)
                    {
                        header = head;
                        break;
                    }
                }
                if (header == null)
                    return false;
                else
                {
                    if (header.CountLines() <= 0)
                    {
                        Header.CascadeDelete(header.Oid, uow);
                        return true;
                    }
                }
                return false;//doc has more than 0 lines
            }
        }

        /// <summary>
        /// Trims the start of the string from zeros, if the provided string is null or empty it returns an empty string
        /// </summary>
        /// <param name="s">The string to trimstart its zeros</param>
        /// <returns>The string without the zeros</returns>
        public static string TrimStartFromZeros(string s)
        {
            if (!string.IsNullOrEmpty(s))
                return s.TrimStart('0');
            else
                return string.Empty;
        }

        ///// <summary>
        ///// Takes a listbox and fills it with the required output path entries for the given doctype, if we give DOC_TYPES.ALL_TYPES it adds all 
        ///// output paths.
        ///// </summary>
        ///// <param name="listBox"></param>
        ///// <param name="docType"></param>
        //public static void FillOutputPathsList(ListBox listBox, DOC_TYPES docType)
        //{
        //    ITS.MobileAtStore.MobileWebService.MobileWebService webService = null;
        //    try
        //    {
        //        using (webService = MobileAtStore.GetWebService(10000))
        //        {
        //            ITS.MobileAtStore.MobileWebService.FileExportLocation[] fileExportSettings = webService.GetFileExportSettings();
        //            //They forced me do this.... Microsoft that is and their stupid ListBox
        //            listBox.Items.Clear();
        //            if (fileExportSettings.Length > 0)
        //            {
        //                foreach (ITS.MobileAtStore.MobileWebService.FileExportLocation export in fileExportSettings)
        //                {
        //                    FileExport fExport = new ITS.MobileAtStore.AuxilliaryClasses.FileExport(export);
        //                    if (fExport.ApplicableDocTypes.Contains(docType))
        //                    {
        //                        listBox.Items.Add(fExport);
        //                    }
        //                }
        //            }                    
        //        }
        //        if (listBox.Items.Count == 0)
        //        {
        //            listBox.Items.Add(FileExport.Database());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageForm.Execute("Σφάλμα", "Η ανάκτηση των τοποθεσιών εξαγωγής από το web service δεν ήταν επιτυχής\r\n" + ex.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
        //    }

        //}

        /// <summary>
        /// Checks if the given doctype is included in the provided outputPath
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="docType"></param>
        /// <returns></returns>
        //public static bool OutputPathIncludesDocType(FileExport outputPath, DOC_TYPES docType)
        //{
        //    return outputPath.ApplicableDocTypes.Contains(docType);
        //}

        public static bool ReplaceFNCChar(KeyPressEventArgs e, TextBox text)
        {
            if (AppSettings.B128Settings == null || e.KeyChar != 29)
                return false;
            int currentPos = text.SelectionStart;
            text.Text = text.Text.Insert(text.SelectionStart, AppSettings.B128Settings.ContentTerminator.ToString());
            text.SelectionStart = currentPos + 1;
            e.Handled = true;
            return e.Handled;
        }

        /// <summary>
        /// returns product info from web service, throws exception if timed out
        /// </summary>
        /// <param name="code"></param>
        /// <param name="supplierCode"></param>
        /// <returns></returns>
        public static Product GetProductFromWebService(string code, string supplierCode,ITS.MobileAtStore.WRMMobileAtStore.eDocumentType eDocumentType)
        {
            Product mobileProduct = null;
            WRMMobileAtStore.Product givenProduct = null;
            try
            {
                using (WRMMobileAtStore.WRMMobileAtStore service = MobileAtStore.GetWebService(AppSettings.Timeout))
                {
                    givenProduct = service.GetProduct(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, code, supplierCode, null, "",eDocumentType,true);

                    if (givenProduct != null && givenProduct.BarcodeParsingResult != ITS.MobileAtStore.WRMMobileAtStore.BarcodeParsingResult.NONE)
                    {
                        if (!string.IsNullOrEmpty(givenProduct.ErrorMessage))
                        {
                            MessageForm.Execute("Σφάλμα", givenProduct.ErrorMessage, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                            return mobileProduct;
                        }
                        else if (!string.IsNullOrEmpty(givenProduct.Code))
                        {
                            mobileProduct = CommonUtilities.ConvertProduct(givenProduct, MobileAtStore.ItemsDL);
                        }
                    }
                }
            }
            catch (System.Net.WebException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                MessageForm.Execute("Σφάλμα", "Υπήρξε ένα σφάλμα κατά την ανάκτηση πληροφοριών προϊόντος/r/n" + ex.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
            }
            return mobileProduct;
        }

        /// <summary>
        /// returns product info from web service, throws exception if timed out
        /// </summary>
        /// <param name="code"></param>
        /// <param name="supplierCode"></param>
        /// <returns></returns>
        public static WRMMobileAtStore.Warehouse[] GetWarehousesFromWebService()
        {
            WRMMobileAtStore.Warehouse[] warehouses = null;
            // = null;
            try
            {
                using (WRMMobileAtStore.WRMMobileAtStore service = MobileAtStore.GetWebService(AppSettings.Timeout))
                {
                    warehouses = service.GetWarehouses(AppSettings.Terminal.ID,true,AppSettings.Terminal.IP);
                }
            }
            catch (Exception ex)
            {
                MessageForm.Execute("Σφάλμα", "Υπήρξε ένα σφάλμα κατά την ανάκτηση των αποθηκευτικών χώρων/r/n" + ex.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                throw ex;
            }
            return warehouses;
        }

        /// <summary>
        /// returns product info from web service, throws exception if timed out
        /// </summary>
        /// <param name="code"></param>
        /// <param name="supplierCode"></param>
        /// <returns></returns>
        public static WRMMobileAtStore.PriceCatalogPolicy[] GetPriceCatalogPoliciesFromWebService()
        {
            WRMMobileAtStore.PriceCatalogPolicy[] priceLists = null;
            
            try
            {
                using (WRMMobileAtStore.WRMMobileAtStore service = MobileAtStore.GetWebService(AppSettings.Timeout))
                {
                    priceLists = service.GetPriceCatalogPolicies(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP);
                }
            }
            catch (Exception ex)
            {
                MessageForm.Execute("Σφάλμα", "Υπήρξε ένα σφάλμα κατά την ανάκτηση των τιμοκαταλόγων/r/n" + ex.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                throw ex;
            }
            return priceLists;
        }


        public static DateTime GetTimeServerFromWebService (int timeout)
        {
            DateTime remote = DateTime.Now;
            bool remoteSetted;
            try
            {
                using (WRMMobileAtStore.WRMMobileAtStore service = MobileAtStore.GetWebService(timeout))
                {
                    service.GetNow(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, out remote, out remoteSetted);
                }               
            }
            catch (Exception ex)
            {
            }
            return remote;
        }


        public static List<Warehouse> ConvertWarehouses(WRMMobileAtStore.Warehouse[] webWares)
        {
            List<Warehouse> wares = new List<Warehouse>();
            if (webWares == null)
                return wares;
            foreach (WRMMobileAtStore.Warehouse webWare in webWares)
            {
                wares.Add(new Warehouse()
                {
                    CompCode = webWare.CompCode,
                    Description = webWare.Description,
                    ErrorMessage = webWare.ErrorMessage
                });
            }
            return wares;
        }

        public static List<PriceCatalogPolicy> ConvertPriceCatalogPolicy(WRMMobileAtStore.PriceCatalogPolicy[] webPLs)
        {
            List<PriceCatalogPolicy> priceCatalogPolicies = new List<PriceCatalogPolicy>();
            if (webPLs == null)
            {
                return priceCatalogPolicies;
            }
            foreach (WRMMobileAtStore.PriceCatalogPolicy webPL in webPLs)
            {
                priceCatalogPolicies.Add(new PriceCatalogPolicy()
                {
                    ID = webPL.ID,
                    Description = webPL.Description,
                    ErrorMessage = webPL.ErrorMessage
                });
            }
            return priceCatalogPolicies;
        }

        #region ShowFormSpecifics
        public static T ShowForm<T>(T res, Form form)
        {
            if (form != null)
                form.Show();
            return res;
        }
        #endregion

        internal static void WaitingCursor()
        {
            Cursor.Current = Cursors.WaitCursor;
        }

        internal static void NormalCursor()
        {
            Cursor.Current = Cursors.Default;
        }
    }
}

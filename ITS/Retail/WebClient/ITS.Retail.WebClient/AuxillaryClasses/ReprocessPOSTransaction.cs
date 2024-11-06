using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.AuxillaryClasses
{
    public class ReprocessPOSTransaction
    {
        public string file { get; set; }

        public string oids
        {
            set
            {
                this.Documents = new List<Guid>();
                this.ParseErrors = string.Empty;
                if (value != null && !string.IsNullOrEmpty(value))
                {
                    List<string> guids = value.Split(',').ToList();
                    foreach (string guid in guids)
                    {
                        Guid docGuid;
                        if (Guid.TryParse(guid, out docGuid))
                        {
                            this.Documents.Add(docGuid);
                        }
                        else
                        {
                            this.ParseErrors += guid;
                        }
                    }
                }
            }
        }

        [JsonIgnore]
        public string ParseErrors { get; set; }
        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(this.ParseErrors);
            }
        }


        [JsonIgnore]
        public List<Guid> Documents { get; set; }

        public bool ProcessFile(out string processInfo)
        {
            processInfo = this.file + ":";
            if (this.Documents.Count == 0)
            {
                processInfo += Resources.PleaseSelectARecord;
                return false;
            }

            bool allDocumentsSucceeded = true;

            using (UnitOfWork posUow = TransactionConnectionHelper.OpenFile(file))
            {
                foreach (Guid guid in Documents)
                {
                    try
                    {
                        using (UnitOfWork webUow = XpoHelper.GetNewUnitOfWork())
                        {
                            POS.Model.Transactions.DocumentHeader posTransaction = posUow.GetObjectByKey<POS.Model.Transactions.DocumentHeader>(guid);
                            string documentJsonString = posTransaction.ToJson(Platform.PlatformConstants.JSON_SERIALIZER_SETTINGS);
                            ITS.Retail.Model.DocumentHeader documentHeader = webUow.GetObjectByKey<ITS.Retail.Model.DocumentHeader>(guid);
                            bool updateCustomerPoints = false;
                            bool updateDocumentCoupons = false;
                            bool applyPricesFromPOS = false;//disable runing code for now
                            bool createCustomerFromPOSDocument = false;//disable running code for now
                            if (documentHeader == null)
                            {
                                documentHeader = new Model.DocumentHeader(webUow);
                                updateCustomerPoints = true;
                                updateDocumentCoupons = true;
                            }
                            string documentProcessError = string.Empty;
                            bool jsonParseResult = documentHeader.FromJson(documentJsonString, Platform.PlatformConstants.JSON_SERIALIZER_SETTINGS, true, true, out documentProcessError);
                            processInfo += documentProcessError;
                            if (jsonParseResult == false || String.IsNullOrWhiteSpace(documentProcessError) == false)
                            {
                                allDocumentsSucceeded = false;
                                continue;
                            }
                            if (updateCustomerPoints)
                            {
                                DocumentHelper.UpdateCustomerPoints(documentHeader, MvcApplication.ApplicationInstance);
                            }
                            if (applyPricesFromPOS)
                            {
                                DocumentHelper.ApplyPricesFromPOS(documentHeader, MvcApplication.ApplicationInstance);
                            }
                            if (updateDocumentCoupons)
                            {
                                DocumentHelper.UpdateDocumentCoupons(documentHeader, MvcApplication.ApplicationInstance);
                            }
                            if (createCustomerFromPOSDocument)
                            {
                                DocumentHelper.CreateCustomerFromPOSDocument(documentHeader, MvcApplication.ApplicationInstance);
                            }
                            documentHeader.Save();
                            XpoHelper.CommitChanges(webUow);
                        }
                    }
                    catch (Exception exception)
                    {
                        string message = this.file + " for Document with Oid = " + guid.ToString() + exception.GetFullMessage() + Environment.NewLine + exception.GetFullStackTrace();
                        string actionAndController = "ReprocessPOSTransaction.ProcessFile " + this.file + " for Document with Oid = " + guid.ToString();
                        processInfo += message;
                        allDocumentsSucceeded = false;
                        LogErrorMessage errorMessage = new LogErrorMessage()
                        {
                            Result = message,
                            Error = message,
                            Action = actionAndController,
                            Controller = actionAndController
                        };
                        ITSLogHelper.Log(errorMessage);

                        MvcApplication.WRMLogModule.Log(exception, actionAndController, KernelLogLevel.Error);
                    }
                }
            }

            if (allDocumentsSucceeded)
            {
                processInfo += Resources.SavedSuccesfully + Environment.NewLine;
            }
            TransactionConnectionHelper.ReleaseResources();
            return allDocumentsSucceeded;
        }
    }
}
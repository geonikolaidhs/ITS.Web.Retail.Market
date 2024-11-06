using System;
using System.Collections.Generic;
using System.Text;
using ITS.MobileAtStore.ObjectModel;
using System.Xml;
using ITS.MobileAtStore.Common.WRMService;
using ITS.MobileAtStore.Common.Helpers;

namespace ITS.MobileAtStore.Common.ApplicationExportSettings
{
    public class WRMExportSettings : ApplicationExportSettings
    {
        private const string SERVICE_NAME = "MobileAtStoreService.asmx";
        private const int QUANTITY_MULTIPLIER = 1000;
        private int TIMEOUT = 30000;


        //Specific Application Settings
        public string webServiceURL { get; set; }

        public WRMExportSettings()
            : base()
        {

        }

        public WRMExportSettings(string serverMapPathRoot, ConnectionSettings connectionSettings, Dictionary<string, object> applicationSpecificSettings) :
            base(serverMapPathRoot, connectionSettings,applicationSpecificSettings)
        {
        }


         public override void Initialise()
         {
             this.Application = eApplication.WRM;
             this.ConnectionSettings.ConnectionMode = ConnectionMode.SQL_SERVER;
         }

        public override bool PerformExport(Header document, string remoteIP, out string logMessage)
        {
            logMessage = "Performing Export";            
            string exportResult = "";
            bool result = ExportDocument(document, out exportResult);
            logMessage += exportResult;
            return result;
        }

        private bool PostDocument(string documentXml, out string result)
        {
            using (WRMService.MobileAtStoreService service = GetService(TIMEOUT))
            {
                string answer = service.PostDocument(documentXml);
                if (answer != "1")
                {
                    XmlDocument responseDocument = new XmlDocument();
                    responseDocument.LoadXml(answer);
                    XmlNode erroNode = responseDocument.SelectSingleNode("request");
                    string errorMessage = erroNode.Attributes["errordescr"].Value;

                    result = errorMessage;
                    return false;
                }
                else
                {
                    result = "Success";
                    return true;
                }
            }
        }

        private bool ExportDocument(Header document,out string result)
        {
            try
            {
                string documentXml = CreateXMLString(document);

                

                    using (WRMService.MobileAtStoreService service = GetService(TIMEOUT))
                    {

                        InvalidItem[] invalidItems = service.ValidateOrderItems(documentXml);
                        if (invalidItems == null || invalidItems.Length == 0)
                        {
                            return PostDocument(documentXml,out result);
                        }
                        else
                        {
                            result = "Found " + invalidItems.Length + " invalid items";
                            foreach (InvalidItem invalidItem in invalidItems)
                            {

                                Line invalidLine = null;
                                foreach(Line line in document.Lines)
                                {
                                    if(line.ProdCode == invalidItem.ItemCode
                                       || line.ProdBarcode == invalidItem.Barcode )
                                    {
                                        invalidLine = line;
                                        break;
                                    }
                                }

                                if (invalidLine == null)
                                {
                                    result = "Δεν ήταν εφικτή η διαγραφή του μη έγκυρου είδους με κωδικό "
                                               + invalidItem.ItemCode + " και Barcode " + invalidItem.Barcode
                                               + " γιατί δε βρέθηκε στο παραστατικό του φορητού.";
                                    return false;
                                }

                                document.Lines.Remove(invalidLine);
                            }
                            if(document.Lines.Count == 0)
                            {
                                document.Delete();
                                result += "Το παραστατικό δεν έχει είδη.";
                                return true;
                            }
                            documentXml = CreateXMLString(document);
                            return PostDocument(documentXml,out result);
                        }
                    }
            }
            catch (Exception ex)
            {
                result = ex.Message;
                if(ex.InnerException!=null && String.IsNullOrEmpty(ex.InnerException.Message)==false)
                {
                    result+="\r\n";
                    result += ex.InnerException.Message;
                    if (String.IsNullOrEmpty(ex.StackTrace) == false)
                    {
                        result += "\r\n";
                        result += ex.StackTrace;
                    }
                }
                return false;
            }
        }

        public WRMService.MobileAtStoreService GetService(int time)
        {
            WRMService.MobileAtStoreService myservice = new WRMService.MobileAtStoreService();
            myservice.Timeout = time;
            myservice.Url = this.ServiceURL;
            myservice.UserAgent = "Datalogger";//"Retail Mobile Client";
            return myservice;
        }

        private string ServiceURL
        {
            get
            {
                string serviceURL = this.webServiceURL;
                if(serviceURL.EndsWith("\\")==false)
                {
                    serviceURL += "\\";
                }
                serviceURL += SERVICE_NAME;
                return serviceURL;
            }
        }

        private string CreateXMLString(Header document)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.AppendChild( xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));
            XmlElement root = xmlDoc.CreateElement("Document");

            XmlElement status = xmlDoc.CreateElement("Status");
            status.InnerText = document.DocStatus.ToString();//TODO Get remote Oid
            root.AppendChild(status);

            XmlElement type = xmlDoc.CreateElement("DocumentType");
            type.InnerText = document.DocType.ToString();//TODO Get remote Oid
            root.AppendChild(type);

            string defaultTaxCode = "999999999";

            XmlElement traderTaxCode = xmlDoc.CreateElement("TraderTaxCode");
            traderTaxCode.InnerText = string.IsNullOrEmpty(document.CustomerAFM) ? defaultTaxCode : document.CustomerAFM;
            root.AppendChild(traderTaxCode);

            XmlElement traderCode = xmlDoc.CreateElement("TraderCode");
            traderCode.InnerText = string.IsNullOrEmpty(document.CustomerCode) ? defaultTaxCode : document.CustomerCode;
            root.AppendChild(traderCode);


            //XmlElement companyid = xmlDoc.CreateElement("Company");
            //companyid.InnerText = document.CustomerAFM;//TODO Get remote Company Oid
            //root.AppendChild(companyid);

            //XmlElement storeid = xmlDoc.CreateElement("Store");
            //storeid.InnerText = document.CustomerCode;//TODO Get remote Store Oid
            //root.AppendChild(storeid);

            XmlElement RemoteDeviceDocumentHeaderGuid = xmlDoc.CreateElement("RemoteDeviceDocumentHeaderGuid");
            RemoteDeviceDocumentHeaderGuid.InnerText = document.Oid.ToString();//TODO Get remote Customer Oid
            root.AppendChild(RemoteDeviceDocumentHeaderGuid);

            XmlElement Division = xmlDoc.CreateElement("Division");
            Division.InnerText = WRMMappingHelper.GetDivisionForDocumentType(document.DocType).ToString();//TODO Get remote Customer Oid
            root.AppendChild(Division);


            XmlElement User = xmlDoc.CreateElement("User");
            User.InnerText = "User";//TODO Get current user Oid
            root.AppendChild(User);


            //Add Document Details
            XmlElement documentDetails = xmlDoc.CreateElement("DocumentDetails");

            foreach (Line line in document.Lines)
            {
                XmlElement documentDetail = xmlDoc.CreateElement("DocumentDetail");
                XmlElement item = xmlDoc.CreateElement("Code");
                item.InnerText = String.IsNullOrEmpty(line.ProdBarcode) ? line.ProdCode : line.ProdBarcode;
                documentDetail.AppendChild(item);

                XmlElement quantity = xmlDoc.CreateElement("Qty");
                quantity.InnerText = (line.Qty1*QUANTITY_MULTIPLIER).ToString();
                documentDetail.AppendChild(quantity);

                documentDetails.AppendChild(documentDetail);
            }

            root.AppendChild(documentDetails);
            xmlDoc.AppendChild(root);
            return xmlDoc.InnerXml;
        }



    }
}

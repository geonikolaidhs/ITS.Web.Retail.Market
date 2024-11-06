using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace POSLoader.Helpers
{
    public static class ConfigHelper
    {
        public static string WebServiceUrl(string configFile){
            return GetValue(configFile,"StoreControllerURL");
        }

        public static int DeviceId(string configFile)
        {
            string dvid = GetValue(configFile, "TerminalID");
            //return GetValue(config_file, "TerminalID");
            int deviceid;
            if (int.TryParse(dvid, out deviceid))
                return deviceid;
            else return int.MinValue;
        }

        private static string GetValue(string configFile,string property)
        {
            //if (File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\Configuration\\Globals.xml"))
            if (File.Exists(configFile))
            {
                using (XmlTextReader xmlReader = new XmlTextReader(configFile))
                {
                    //read through all the nodes
                    while (xmlReader.Read())
                    {
                        //the headlines we want are in the item nodes
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            if(xmlReader.Name == property){
                                xmlReader.Read();
                                if (xmlReader.Value.EndsWith("/"))
                                    return xmlReader.Value.Substring(0, xmlReader.Value.Length - 1);
                                return xmlReader.Value;
                            }
                            //switch (xmlReader.Name)
                            //{
                            //    case "CurrentStore":
                            //        xmlReader.Read();
                            //        CurrentStore = SessionHelper.Master.GetObjectByKey<Store>(xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim()));
                            //        break;
                            //    case "CurrentTerminal":
                            //        xmlReader.Read();
                            //        CurrentTerminal = SessionHelper.Settings.GetObjectByKey<Terminal>(xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim()));
                            //        break;
                            //    case "TerminalID":
                            //        xmlReader.Read();
                            //        TerminalID = Int32.Parse(xmlReader.Value);
                            //        break;
                            //    case "DefaultCustomer":
                            //        xmlReader.Read();
                            //        DefaultCustomer = SessionHelper.Master.GetObjectByKey<Customer>(xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim()));
                            //        break;
                            //    case "DefaultDocumentType":
                            //        xmlReader.Read();
                            //        DefaultDocumentType = SessionHelper.Settings.GetObjectByKey<DocumentType>(xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim()));
                            //        break;
                            //    case "DefaultDocumentStatus":
                            //        xmlReader.Read();
                            //        DefaultDocumentStatus = SessionHelper.Settings.GetObjectByKey<DocumentStatus>(xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim()));
                            //        break;
                            //    case "DefaultDocumentSeries":
                            //        xmlReader.Read();
                            //        DefaultDocumentSeries = SessionHelper.Settings.GetObjectByKey<DocumentSeries>(xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim()));
                            //        break;
                            //    case "DefaultPaymentMethod":
                            //        xmlReader.Read();
                            //        DefaultPaymentMethod = SessionHelper.Settings.GetObjectByKey<PaymentMethod>(xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim()));
                            //        break;
                            //    case "FiscalMethod":
                            //        xmlReader.Read();
                            //        try
                            //        {
                            //            FiscalMethod = (eFiscalMethod)Enum.Parse(typeof(eFiscalMethod), xmlReader.Value.ToUpper());
                            //        }
                            //        catch
                            //        {
                            //            throw new Exception("Invalid Fiscal Method");
                            //        }
                            //        break;
                            //    case "FiscalDevice":
                            //        xmlReader.Read();
                            //        try
                            //        {
                            //            FiscalDevice = (eFiscalDevice)Enum.Parse(typeof(eFiscalDevice), xmlReader.Value.ToUpper());
                            //        }
                            //        catch
                            //        {
                            //            throw new Exception("Invalid Fiscal Device");
                            //        }
                            //        break;
                            //    case "ReceiptVariableIdentifier":
                            //        xmlReader.Read();
                            //        ReceiptVariableIdentifier = xmlReader.Value;
                            //        break;
                            //    case "ABCDirectory":
                            //        xmlReader.Read();
                            //        ABCDirectory = xmlReader.Value;
                            //        break;
                            //    case "Locale":
                            //        xmlReader.Read();
                            //        Locale = LocaleHelper.GetLanguage(xmlReader.Value);
                            //        LocaleHelper.SetLocale(xmlReader.Value);
                            //        break;
                            //    case "StoreControllerURL":
                            //        xmlReader.Read();
                            //        StoreControllerWebServiceURL = xmlReader.Value.TrimEnd('/') + "/POSUpdateService.asmx";
                            //        break;
                            //    default:
                            //        break;
                            //}
                        }
                    }                    
                }
                return "";
            }
            else
            {
                throw new Exception("File " + configFile + " not found!");
            }
        }
    }
}

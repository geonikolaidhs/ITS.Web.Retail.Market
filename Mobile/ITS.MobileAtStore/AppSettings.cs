using System;

using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Windows.Forms;
using ITS.Common.Utilities.EAN128BarcodeNS;
using ITS.Common.Utilities.Compact;
using ITS.MobileAtStore.ObjectModel;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using ITS.MobileAtStore.AuxilliaryClasses;
using ITS.MobileAtStore.Helpers;
using System.Net;
using System.Security.Cryptography;

namespace ITS.MobileAtStore
{

    public static class AppSettings
    {
        static string localItemsDBLocation = ""; //The location of the items database
        static volatile string serverIP = ""; //The web service url
        static OPERATION_MODE operationMode = 0; //The operation mode
        public static EAN128IdentifierSettings B128Settings;
        static volatile int _Timeout = 40000;

        const int DEFAULT_QUANTITY_NUMBER_OF_INTEGRAL_DIGITS = 4;
        const int DEFAULT_QUANTITY_NUMBER_OF_DECIMAL_DIGITS = 3;

        public static int Timeout
        {
            get
            {
                return _Timeout;
            }
            set
            {
                _Timeout = value;
            }
        }

        public static bool QueueEnable
        {
            get;
            set;
        }

        public static QueueQRPrintingFormat Format { get; private set; }

        public static TerminalSettings Terminal { get; set; }

        static bool advancedPriceCheckingActive = false;
        static string compcode = "-1";
        static string priceCatalogPolicy = "";


        public static Dictionary<DOC_TYPES, bool> PerformAverageQuantityCheck;

        public static Dictionary<DOC_TYPES, bool> PerformActiveItemCheck;

        public static Dictionary<DOC_TYPES, bool> ReplaceInActiveItemWithActiveMaternal;

        public static Dictionary<DOC_TYPES, bool> ShowAvgForm;

        public static Dictionary<DOC_TYPES, bool> DisableOffine;

        public static bool UseSales;

        public static decimal MaximumAllowedQuantity { get; set; }
        public static decimal DEFAULT_MAXIMUM_ALLOWED_QUANTITTY = 100;
        public static int QuantityNumberOfIntegralDigits = DEFAULT_QUANTITY_NUMBER_OF_INTEGRAL_DIGITS;
        public static int QuantityNumberOfDecimalDigits = DEFAULT_QUANTITY_NUMBER_OF_DECIMAL_DIGITS;
        public static int QuantityNumberOfTotalDigits
        {
            get
            {
                return QuantityNumberOfIntegralDigits + QuantityNumberOfDecimalDigits;
            }
        }

#if (DEBUG)
        static string configurationFile = "dataLoggerConfigDebug.xml"; //The debug configuration file
#else
        static string configurationFile = "dataLoggerConfig.xml"; //The configuration file
#endif
        /// <summary>
        /// Reads teh settings from the configuration file
        /// </summary>
        public static void ReadSettings()
        {
            XmlDocument xml;
            XmlElement element;

            //Step 1 Read Terminal settings
            try
            {
                string terminalFile = OpenNETCF.Windows.Forms.Application2.StartupPath + "\\TerminalSettings.xml";
                XmlSerializer ser = new XmlSerializer(typeof(TerminalSettings));
                using (StreamReader stream = new StreamReader(terminalFile))
                {
                    Terminal = ser.Deserialize(stream) as TerminalSettings;
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message + "\r\n" + ex.StackTrace;
                Terminal = new TerminalSettings();
                Terminal.ID = 0;
            }

            try
            {
                //Try to get from service
                string config = OpenNETCF.Windows.Forms.Application2.StartupPath + "\\" + configurationFile;
                xml = new XmlDocument();
                xml.Load(config);
                element = xml["Settings"]["ServerIP"];
                serverIP = element.InnerXml;

                using (var service = MobileAtStore.GetWebService(AppSettings.Timeout))
                {
                    string fileContent;
                    bool result, resultSpecified;
                    service.GetMobileConfig(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, out result, out resultSpecified, out fileContent);
                    if (result && resultSpecified)
                    {
                        xml.InnerXml = fileContent;
                        xml.Save(config);
                    }
                    //so far web service GetMobileConfig always fails and there is already a bug in this using section that can be resolved ONLY
                    //when the web service GetMobileConfig is fixed
                    //ITS.MobileAtStore.WRMMobileAtStore.MobileAtStoreSettings mobileAtStoreSettings = service.GetSettings();
                    //UpdateSettings(mobileAtStoreSettings);
                }
            }
            catch(Exception ex)
            {
                string exceptionMessage = ex.Message + "\r\n" + ex.StackTrace;
            }

            try
            {
                string config = OpenNETCF.Windows.Forms.Application2.StartupPath + "\\" + configurationFile;
                xml = new XmlDocument();
                xml.Load(config);
                element = xml["Settings"]["LocalItemsDBLocation"];
                localItemsDBLocation = element.InnerXml;

                element = xml["Settings"]["ServerIP"];
                serverIP = element.InnerXml;

                element = xml["Settings"]["OperationMode"];
                operationMode = (OPERATION_MODE)int.Parse(element.InnerXml);

                element = xml["Settings"]["AdvancedPriceChecking"];
                advancedPriceCheckingActive = element.Attributes["Active"].Value == "True" ? true : false;

                element = xml["Settings"]["QuantityNumberOfDecimalDigits"];
                int numberOfDigits = DEFAULT_QUANTITY_NUMBER_OF_DECIMAL_DIGITS;
                try
                {
                    numberOfDigits = int.Parse(element.InnerXml);
                }
                catch(Exception e)
                {
                    string error = e.Message;
                }
                QuantityNumberOfDecimalDigits = numberOfDigits;

                element = xml["Settings"]["QuantityNumberOfIntegralDigits"];
                numberOfDigits = DEFAULT_QUANTITY_NUMBER_OF_INTEGRAL_DIGITS;
                try
                {
                    numberOfDigits = int.Parse(element.InnerXml);
                }
                catch (Exception e)
                {
                    string error = e.Message;
                }
                QuantityNumberOfIntegralDigits = numberOfDigits;
                compcode = xml["Settings"]["AdvancedPriceChecking"]["CompCode"].InnerXml;
                priceCatalogPolicy = xml["Settings"]["AdvancedPriceChecking"]["PriceCatalogPolicy"].InnerXml;
                element = xml["Settings"]["OperationMode"];
                operationMode = (OPERATION_MODE)int.Parse(element.InnerXml);

                EAN128BarcodeSettingsLoader bsl = new EAN128BarcodeSettingsLoader();
                B128Settings = bsl.LoadFromFile(config, "Settings");
                if (bsl.Errors.Count > 0)
                {
                    string errorMessage = "Λάθος στην παραμετροποίηση των EAN128\r\n";
                    foreach (string s in bsl.Errors)
                    {
                        errorMessage += s + "\r\n";
                    }
                    MessageForm.Execute("Σφάλμα", errorMessage);
                    throw new Exception("Η παραμετροποίηση των EAN128 είναι λανθασμένη");
                }
                if (xml["Settings"]["Timeout"] == null)
                {
                    try
                    {
                        int f = Int32.Parse(xml["Settings"]["Timeout"].InnerText);
                        Timeout = f;
                    }
                    catch
                    {
                    }
                }

                string performAverageQuantityCheck = xml["Settings"]["PerformAverageQuantityCheck"] != null
                                                    && !String.IsNullOrEmpty(xml["Settings"]["PerformAverageQuantityCheck"].InnerText)
                                                    ? xml["Settings"]["PerformAverageQuantityCheck"].InnerText.ToUpper()
                                                    : "";
                string performActiveItemCheck = xml["Settings"]["PerformActiveItemCheck"] != null
                                                    && !String.IsNullOrEmpty(xml["Settings"]["PerformActiveItemCheck"].InnerText)
                                                    ? xml["Settings"]["PerformActiveItemCheck"].InnerText.ToUpper()
                                                    : "";
                string replaceInActiveItemWithActiveMaternal = xml["Settings"]["ReplaceInActiveItemWithActiveMaternal"] != null
                                                    && !String.IsNullOrEmpty(xml["Settings"]["ReplaceInActiveItemWithActiveMaternal"].InnerText)
                                                    ? xml["Settings"]["ReplaceInActiveItemWithActiveMaternal"].InnerText.ToUpper()
                                                    : "";

                string showAvgForm = xml["Settings"]["ShowAvgForm"] != null
                                    && !String.IsNullOrEmpty(xml["Settings"]["ShowAvgForm"].InnerText)
                                    ? xml["Settings"]["ShowAvgForm"].InnerText.ToUpper()
                                    : "";

                if (xml["Settings"]["Queue"] != null)
                {
                    ReadQueueFormatSettings(xml["Settings"]["Queue"].InnerText.Trim());
                }
                if (xml["Settings"]["ExtraSettings"] != null)
                {
                    ReadExtraSettings(xml["Settings"]["ExtraSettings"].InnerText.Trim());
                }

                try
                {
                    AppSettings.UseSales = bool.Parse(xml["Settings"]["UseSales"].InnerText.TrimStart().TrimEnd());
                }
                catch
                {
                    AppSettings.UseSales = false;
                }

                try
                {
                    AppSettings.MaximumAllowedQuantity = decimal.Parse(xml["Settings"]["MaximumAllowedQuantity"].InnerText.TrimStart().TrimEnd());
                }
                catch
                {
                    AppSettings.MaximumAllowedQuantity = DEFAULT_MAXIMUM_ALLOWED_QUANTITTY;
                }

                AppSettings.PerformAverageQuantityCheck = new Dictionary<DOC_TYPES, bool>();
                AppSettings.PerformActiveItemCheck = new Dictionary<DOC_TYPES, bool>();
                AppSettings.ReplaceInActiveItemWithActiveMaternal = new Dictionary<DOC_TYPES, bool>();
                AppSettings.ShowAvgForm = new Dictionary<DOC_TYPES, bool>();
                AppSettings.DisableOffine = new Dictionary<DOC_TYPES, bool>()
                    {
                        {DOC_TYPES.COMPETITION, false},
                        {DOC_TYPES.ESL_INV, true},
                        {DOC_TYPES.INVENTORY, true},
                        {DOC_TYPES.INVOICE, true},
                        {DOC_TYPES.INVOICE_SALES, true},
                        {DOC_TYPES.MATCHING, false},
                        {DOC_TYPES.ORDER, false},
                        {DOC_TYPES.PICKING, false},
                        {DOC_TYPES.PRICE_CHECK, false},
                        {DOC_TYPES.RECEPTION, false},
                        {DOC_TYPES.TAG, false},
                        {DOC_TYPES.QUEUE_QR, true},
                        {DOC_TYPES.TRANSFER, false}
                    };



                foreach (DOC_TYPES docType in GetValues(DOC_TYPES.ALL_TYPES))
                {
                    string dcTypeStr = docType.ToString().ToUpper();
                    bool includeDocType = performAverageQuantityCheck.IndexOf(dcTypeStr) >= 0;
                    AppSettings.PerformAverageQuantityCheck.Add(docType, includeDocType);

                    includeDocType = performActiveItemCheck.IndexOf(dcTypeStr) >= 0;
                    AppSettings.PerformActiveItemCheck.Add(docType, includeDocType);

                    includeDocType = replaceInActiveItemWithActiveMaternal.IndexOf(dcTypeStr) >= 0;
                    AppSettings.ReplaceInActiveItemWithActiveMaternal.Add(docType, includeDocType);

                    includeDocType = showAvgForm.IndexOf(dcTypeStr) >= 0;
                    AppSettings.ShowAvgForm.Add(docType, includeDocType);
                }
                /*
                 *  <PerformAverageQuantityCheck>
                    INVOICE,ORDER,PICKING,TAG
                    </PerformAverageQuantityCheck>
                    <PerformActiveItemCheck>
                    INVOICE,ORDER,PICKING
                    </PerformActiveItemCheck>
                    <ReplaceInActiveItemWithActiveMaternal>
                    INVOICE,ORDER,PICKING,RECEPTION
                    </ReplaceInActiveItemWithActiveMaternal>
                 */
         
                try
                {
                    using (var service = MobileAtStore.GetWebService(AppSettings.Timeout))
                    {
                        ITS.MobileAtStore.WRMMobileAtStore.MobileAtStoreSettings mobileAtStoreSettings = service.GetSettings();
                        UpdateSettings(mobileAtStoreSettings);
                    }
               }
                catch( Exception exception )
                {
                    string expetionError = exception.Message + "\r\n" + exception.StackTrace;
                }
                try
                {
                    QueueQRPrinterSettings.Load();
                }
                catch (Exception exception)
                {
                    string exceptionError =  exception.Message;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Λάθος κατά την ανάγνωση του <" + configurationFile + ">.\r\n" + e.Message);
            }
        }

        public static void UpdateSettings(ITS.MobileAtStore.WRMMobileAtStore.MobileAtStoreSettings mobileAtStoreSettings)
        {
            AppSettings.QuantityNumberOfDecimalDigits = mobileAtStoreSettings.QuantityNumberOfDecimalDigits;
            AppSettings.QuantityNumberOfIntegralDigits = mobileAtStoreSettings.QuantityNumberOfIntegralDigits;
            SaveSettings();
        }

        private static void ReadQueueFormatSettings(string data)
        {
            try
            {
                try
                {
                    XmlDocument xml = CryptoService.DecryptXmlFromBase64String(data);
                    string file = OpenNETCF.Windows.Forms.Application2.StartupPath + "\\" + "PrintFormat.xml";
                    xml = new XmlDocument();
                    xml.Load(file);

                    if (xml["Settings"]["QueueFormat"] != null && xml["Settings"]["QueueMicroFormats"] != null)
                    {
                        Format = new QueueQRPrintingFormat();
                        var queueFormatElement = xml["Settings"]["QueueFormat"];
                        var microFormatElement = xml["Settings"]["QueueMicroFormats"];

                        Format.LargeText = microFormatElement["LargeText"].InnerText;
                        Format.LargestText = microFormatElement["LargestText"].InnerText;
                        Format.NormalText = microFormatElement["NormalText"].InnerText;
                        Format.QrCode = microFormatElement["QrCode"].InnerText;
                        Format.Barcode = microFormatElement["Barcode"].InnerText;
                        Format.OrientationFix = microFormatElement["OrientationFix"].InnerText;
                        Format.AdaptiveText = microFormatElement["AdaptiveText"].InnerText;

                        Format.QRBlockSize = int.Parse(queueFormatElement["QRBlockSize"].InnerText);
                        Format.PrintFormat = new List<QueueQRPrintingFormatElement>();
                        foreach (XmlNode node in queueFormatElement["PrintingFormat"].ChildNodes)
                        {
                            QueueQRPrintingFormatElement felement = QueueQRPrintingFormatElement.GetElement(node);
                            if (felement != null)
                            {
                                Format.PrintFormat.Add(felement);
                            }
                        }
                        Format.PrintFormat.Add(new NormalTextQueueQRPrintingFormatElement() { Text = " " });
                        Format.PrintFormat.Add(new AdaptiveTextQueueQRPrintingFormatElement("40", "A,20") { Text = "Powered by ITS S.A. (http://www.itservices.gr)" });
                        Format.PrintFormat.Add(new NormalTextQueueQRPrintingFormatElement() { Text = " " });
                        Format.PrintFormat.Add(new NormalTextQueueQRPrintingFormatElement() { Text = " " });
                        Format.PrintFormat.Add(new NormalTextQueueQRPrintingFormatElement() { Text = " " });
                        Format.PrintFormat.Add(new NormalTextQueueQRPrintingFormatElement() { Text = " " });

                        XmlNodeList ignorePrefixesNode = queueFormatElement["IgnoringPrefixes"].ChildNodes;
                        Format.IgnorePrefixes = new List<string>(ignorePrefixesNode.Count);
                        foreach (XmlNode node in ignorePrefixesNode)
                        {
                            if (node.Name == "Prefix")
                            {
                                Format.IgnorePrefixes.Add(node.InnerText);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    string exceptionError = e.Message;
                    MessageForm.Execute("Σφάλμα", "Λάθος κατά την ανάγνωση των δεδομένων μορφοποίησης εκτύπωσης ουράς", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                }
             
            }
            catch (Exception e)
            {
                string exceptionError = e.Message;
                MessageForm.Execute("Σφάλμα", "Λάθος κατά την ανάγνωση των δεδομένων μορφοποίησης εκτύπωσης ουράς", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
            }
        }

        private static void ReadExtraSettings(string data)
        {
            try
            {
                try
                {
                    XmlDocument xml = CryptoService.DecryptXmlFromBase64String(data);
                                  
                    if (xml["Settings"]["QueueEnable"] != null)
                    {
                        try
                        {
                            AppSettings.QueueEnable = bool.Parse(xml["Settings"]["QueueEnable"].InnerText.Trim());
                        }
                        catch (Exception ex)
                        {
                            string exceptionError = ex.Message;
                            AppSettings.QueueEnable = false;
                        }
                    }                
                }
                catch (Exception e)
                {
                    string exceptionError = e.Message;
                    MessageForm.Execute("Σφάλμα", "Λάθος κατά την ανάγνωση των δεδομένων μορφοποίησης εκτύπωσης ουράς", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                }             
            }
            catch (Exception e)
            {
                string exceptionError = e.Message;
                //MessageForm.Execute("Σφάλμα", "Λάθος κατά την ανάγνωση των δεδομένων μορφοποίησης εκτύπωσης ουράς", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
            }
        }

        public static IEnumerable<Enum> GetValues(Enum enumeration)
        {
            List<Enum> enumerations = new List<Enum>();
            FieldInfo[] fields = enumeration.GetType().GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo fieldInfo in fields)
            {
                Enum item = (Enum)fieldInfo.GetValue(enumeration);
                if (item != null)
                {
                    enumerations.Add(item);
                }
            }
            return enumerations;
        }

        /// <summary>
        /// Saves the settings in the xml file
        /// </summary>
        /// <returns></returns>
        public static bool SaveSettings()
        {
            try
            {
                string terminalFile = OpenNETCF.Windows.Forms.Application2.StartupPath + "\\TerminalSettings.xml";
                XmlSerializer ser = new XmlSerializer(typeof(TerminalSettings));
                using (StreamWriter stream = new StreamWriter(terminalFile))
                {
                    ser.Serialize(stream, Terminal);
                    stream.Flush();
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message + "\r\n" + ex.StackTrace;
                Terminal = new TerminalSettings();
                Terminal.ID = 0;
            }
            DataSet ds = new DataSet();
            DataRow dr;
            DataRow drAdvPrice;
            try
            {
                ds.ReadXml(OpenNETCF.Windows.Forms.Application2.StartupPath + "//" + configurationFile);
                dr = ds.Tables["Settings"].Select()[0];
                drAdvPrice = ds.Tables["AdvancedPriceChecking"].Select()[0];
                dr["LocalItemsDBLocation"] = localItemsDBLocation;
                dr["ServerIP"] = serverIP;
                dr["OperationMode"] = (int)operationMode;
                List<string> newColumns = new List<string>()
                {
                    "QuantityNumberOfDecimalDigits",
                    "QuantityNumberOfIntegralDigits"
                };
                foreach (string columnName in newColumns)
                {
                    if (!dr.Table.Columns.Contains(columnName))
                    {
                        dr.Table.Columns.Add(columnName);
                    }
                }
                dr["QuantityNumberOfDecimalDigits"] = QuantityNumberOfDecimalDigits;
                dr["QuantityNumberOfIntegralDigits"] = QuantityNumberOfIntegralDigits;
                drAdvPrice["PriceCatalogPolicy"] = priceCatalogPolicy.ToString();
                drAdvPrice["CompCode"] = compcode.ToString();
            }
            catch (Exception e)
            {
                MessageForm.Execute("Σφάλμα", "Λάθος κατά την αποθήκευση του <" + configurationFile + ">." + e.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                ds = null;
                dr = null;
                drAdvPrice = null;
            }

            if (ds != null && dr != null)
            {
                dr.AcceptChanges();
                drAdvPrice.AcceptChanges();
                ds.AcceptChanges();
                ds.WriteXml(OpenNETCF.Windows.Forms.Application2.StartupPath + "//" + configurationFile);

                dr = null;
                drAdvPrice = null;
                ds.Dispose();
                return true;
            }
            return false;
        }

        #region Enumerations
        public enum OPERATION_MODE
        {
            BATCH = 1,
            ONLINE = 2
        }
        #endregion

        #region Properties
        public static string LocalItemsDBLocation
        {
            get
            {
                return localItemsDBLocation;
            }
            set
            {
                localItemsDBLocation = value;
            }
        }

        public static OPERATION_MODE OperationMode
        {
            get
            {
                return operationMode;
            }

            set
            {
                operationMode = value;
            }
        }

        public static string ServerIP
        {
            get
            {
                return serverIP;
            }
            set
            {
                serverIP = value;
            }
        }

        public static bool AdvancedPriceCheckingActive
        {
            get
            {
                return advancedPriceCheckingActive;
            }
            set
            {
                advancedPriceCheckingActive = value;
            }
        }

        public static string CompCode
        {
            get
            {
                return compcode;
            }
            set
            {
                compcode = value;
            }
        }

        public static string PriceList
        {
            get
            {
                return priceCatalogPolicy;
            }
            set
            {
                priceCatalogPolicy = value;
            }
        }

        private static volatile bool _connectedToServiceService = false;
        public static bool ConnectedToWebService
        {
            get
            {
                return _connectedToServiceService;
            }
            set
            {
                _connectedToServiceService = value;
            }
        }
        #endregion



        
    }
}

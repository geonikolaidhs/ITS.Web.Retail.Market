using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Helpers.POSReports;
using ITS.POS.Client.Receipt;
using ITS.POS.Hardware;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using Microsoft.CSharp;
using NLog;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Container for all the application's configuration.
    /// Reads and checks settings at initialization.
    /// </summary>
    public class ConfigurationManager : IConfigurationManager
    {
        public Guid CurrentStoreOid { get; set; }
        public Guid CurrentTerminalOid { get; set; }
        public int TerminalID { get; set; }
        public Guid DefaultCustomerOid { get; set; }
        public Guid DefaultDocumentTypeOid { get; set; }
        public Guid ProFormaInvoiceDocumentTypeOid { get; set; }
        public Guid ProFormaInvoiceDocumentSeriesOid { get; set; }
        public Guid SpecialProformaDocumentTypeOid { get; set; }
        public Guid SpecialProformaDocumentSeriesOid { get; set; }
        public Guid DepositDocumentTypeOid { get; set; }
        public Guid DepositDocumentSeriesOid { get; set; }
        public Guid DepositItemOid { get; set; }
        public Guid WithdrawalDocumentTypeOid { get; set; }
        public Guid WithdrawalDocumentSeriesOid { get; set; }
        public Guid WithdrawalItemOid { get; set; }
        public Guid DefaultDocumentStatusOid { get; set; }
        public Guid DefaultPaymentMethodOid { set; get; }
        public Guid DefaultDocumentSeriesOid { get; set; }
        public bool UsesTouchScreen { get; set; }
        public bool UsesKeyLock { get; set; }
        public bool AutoFocus { get; set; }
        public bool AsksForStartingAmount { get; set; }
        public bool AsksForFinalAmount { get; set; }
        public bool POSSellsInactiveItems { get; set; }
        public bool AutoIssueZEAFDSS { get; set; }
        public bool EnableLowEndMode { get; set; }
        public bool DemoMode { get; set; }
        public bool UseSliderPauseForm { get; set; }
        public bool UseCashCounter { get; set; }
        public eForcedWithdrawMode ForcedWithdrawMode { get; set; }
        public eDocumentDetailPrintDescription DocumentDetailPrintDescription { get; set; }
        public decimal ForcedWithdrawCashAmountLimit { get; set; }
        public bool PrintDiscountAnalysis { get; set; }
        public ReceiptSchema ReceiptSchema { get; set; }
        public ReceiptSchema ΧReportSchema { get; set; }
        public ReceiptSchema ZReportSchema { get; set; }
        public string ReceiptVariableIdentifier { get; set; }
        public string CurrencySymbol { get; set; }
        public eCurrencyPattern CurrencyPattern { get; set; }
        public int CurrencyLocation { get; set; }
        public string ABCDirectory { get; set; }

        public List<Image> PauseFormImages { get; set; }

        public eFiscalMethod FiscalMethod
        {
            get
            {
                return this.FiscalDevice.GetFiscalMethod();
            }
        }
        public eFiscalDevice FiscalDevice { get; set; }
        public eLocale Locale { get; set; }
        public string StoreControllerWebServiceURL { get; set; }

        private ISessionManager SessionManager { get; set; }
        private IPlatformRoundingHandler PlatformRoundingHandler { get; set; }

        private IPosKernel Kernel { get; set; }

        public List<POSDocumentReportSettings> DocumentReports { get; set; }

        public List<DatabaseCommand> DbCommands { get; set; }

        public ConfigurationManager(ISessionManager sessionManager, IPlatformRoundingHandler platformRoundingHandler, IPosKernel kernel)
        {
            Kernel = kernel;
            SessionManager = sessionManager;
            PlatformRoundingHandler = platformRoundingHandler;
            this.DocumentReports = new List<POSDocumentReportSettings>();
            this.DbCommands = new List<DatabaseCommand>();
        }


        private void LoadExtraSettings(string extraSettingsXmlPath)
        {
            if (this.DbCommands != null)
            {
                this.DbCommands.Clear();
                this.DbCommands = null;
            }

            this.DbCommands = new List<DatabaseCommand>();

            try
            {
                if (File.Exists(extraSettingsXmlPath))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(extraSettingsXmlPath);


                    foreach (XmlNode dbCommndNode in xmlDocument.GetElementsByTagName("DbCommand"))
                    {
                        try
                        {
                            XmlDocument settingsDocument = new XmlDocument();
                            settingsDocument.LoadXml(dbCommndNode.OuterXml);

                            DatabaseCommand DbCommand = new DatabaseCommand();
                            DbCommand.DbType = ((DBType)Enum.Parse(typeof(DBType), settingsDocument.GetElementsByTagName("DbType")[0].InnerText));

                            if (String.IsNullOrEmpty(DbCommand.DbType.ToString()))
                            {
                                throw new Exception(String.Format("Error loading {0}. DBType value '{1}' is invalid", extraSettingsXmlPath, DbCommand.DbType.ToString()));
                            }

                            DbCommand.ConnectionString = settingsDocument.GetElementsByTagName("ConnectionString")[0].InnerText;
                            if (String.IsNullOrEmpty(DbCommand.ConnectionString))
                            {
                                throw new Exception(String.Format("Error loading {0}. ConnectionString value '{1}' is invalid", extraSettingsXmlPath, DbCommand.ConnectionString));
                            }

                            DbCommand.Command = settingsDocument.GetElementsByTagName("Command")[0].InnerText;
                            if (String.IsNullOrEmpty(DbCommand.Command))
                            {
                                throw new Exception(String.Format("Error loading {0}. Command Text '{1}' is invalid", extraSettingsXmlPath, DbCommand.Command));
                            }

                            DbCommand.ApplyTime = settingsDocument.GetElementsByTagName("ApplyTime")[0].InnerText;
                            if (String.IsNullOrEmpty(DbCommand.ApplyTime))
                            {
                                throw new Exception(String.Format("Error loading {0}. ApplyTime value '{1}' is invalid", extraSettingsXmlPath, DbCommand.ApplyTime));
                            }

                            DbCommand.ApplyOn = settingsDocument.GetElementsByTagName("ApplyOn")[0].InnerText;
                            if (String.IsNullOrEmpty(DbCommand.ApplyOn))
                            {
                                throw new Exception(String.Format("Error loading {0}. ApplyOn value '{1}' is invalid", extraSettingsXmlPath, DbCommand.ApplyOn));
                            }

                            DbCommand.SelectColumn = settingsDocument.GetElementsByTagName("SelectColumn")[0].InnerText;
                            if (String.IsNullOrEmpty(DbCommand.SelectColumn))
                            {
                                throw new Exception(String.Format("Error loading {0}. SelectColumn value '{1}' is invalid", extraSettingsXmlPath, DbCommand.SelectColumn));
                            }

                            foreach (XmlNode ParameterNode in settingsDocument.GetElementsByTagName("Parameter"))
                            {
                                XmlDocument parametersDocument = new XmlDocument();
                                parametersDocument.LoadXml(dbCommndNode.OuterXml);

                                if (!String.IsNullOrEmpty(parametersDocument.GetElementsByTagName("Param")[0].InnerText) && !String.IsNullOrEmpty(parametersDocument.GetElementsByTagName("ParamValue")[0].InnerText))
                                {
                                    DbCommand.Parameters.Add(parametersDocument.GetElementsByTagName("Param")[0].InnerText, parametersDocument.GetElementsByTagName("ParamValue")[0].InnerText);
                                }
                            }
                            this.DbCommands.Add(DbCommand);
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetFullMessage();
            }
        }

        private void LoadPOSDocumentReportSettings(string reportSettingsXmlPath)
        {
            if (this.DocumentReports != null)
            {
                this.DocumentReports.Clear();
                this.DocumentReports = null;
            }

            this.DocumentReports = new List<POSDocumentReportSettings>();

            if (File.Exists(reportSettingsXmlPath))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(reportSettingsXmlPath);
                List<String> references = new List<String>();
                List<String> usings = new List<String>();
                List<String> properties = new List<String>();
                Dictionary<String, List<DynamicFunction>> assembliesDict = new Dictionary<string, List<DynamicFunction>>();
                foreach (XmlNode reportNode in xmlDocument.GetElementsByTagName("ReportSetting"))
                {

                    try
                    {

                        XmlDocument settingsDocument = new XmlDocument();
                        settingsDocument.LoadXml(reportNode.OuterXml);

                        POSDocumentReportSettings posDocumentReportSettings = new POSDocumentReportSettings();

                        if (settingsDocument.GetElementsByTagName("Printer").Count > 0)
                        {
                            posDocumentReportSettings.Printer = settingsDocument.GetElementsByTagName("Printer")[0].InnerText;
                        }

                        if (String.IsNullOrEmpty(posDocumentReportSettings.Printer))
                        {
                            throw new Exception(String.Format("Error loading {0}. Printer value '{1}' is invalid", reportSettingsXmlPath, posDocumentReportSettings.Printer));
                        }

                        Guid documentTypeOid;
                        if (Guid.TryParse(settingsDocument.GetElementsByTagName("DocumentType")[0].InnerText, out documentTypeOid) == false)
                        {
                            throw new Exception(String.Format("Error loading {0}. DocumentType value '{1}' is invalid", reportSettingsXmlPath, settingsDocument.GetElementById("DocumentType").InnerText));
                        }

                        posDocumentReportSettings.DocumentTypeOid = documentTypeOid;
                        if (this.DocumentReports.Where(report => report.DocumentTypeOid == posDocumentReportSettings.DocumentTypeOid).Count() > 1)
                        {
                            throw new Exception(String.Format("Error loading {0}. DocumentType value '{1}' already found in settings", reportSettingsXmlPath, posDocumentReportSettings.DocumentTypeOid.ToString()));
                        }

                        //  WINDOWS PRINTER SETTINGS // 

                        Guid customReportOid;
                        if (settingsDocument.GetElementsByTagName("CustomReport").Count > 0)
                        {
                            if (Guid.TryParse(settingsDocument.GetElementsByTagName("CustomReport")[0].InnerText, out customReportOid) == false)
                            {
                                throw new Exception(String.Format("Error loading {0}. CustomReport value '{1}' is invalid", reportSettingsXmlPath, settingsDocument.GetElementById("CustomReport").InnerText));
                            }
                            if (customReportOid != Guid.Empty)
                            {
                                posDocumentReportSettings.CustomReportOid = customReportOid;
                            }
                        }


                        //  OPOS PRINTER SETTINGS // 

                        Guid printFormatOid;
                        if (settingsDocument.GetElementsByTagName("XMLPrintFormat").Count > 0)
                        {
                            if (Guid.TryParse(settingsDocument.GetElementsByTagName("XMLPrintFormat")[0].InnerText, out printFormatOid) == false)
                            {
                                throw new Exception(String.Format("Error loading {0}. XMLPrintFormat value '{1}' is invalid", reportSettingsXmlPath, settingsDocument.GetElementById("XMLPrintFormat").InnerText));
                            }
                            posDocumentReportSettings.PrintFormatOid = printFormatOid;
                            posDocumentReportSettings.XMLPrintFormat = new ReceiptSchema();


                            POSPrintFormat pf = this.SessionManager.GetSession<POSPrintFormat>().GetObjectByKey<POSPrintFormat>(printFormatOid);
                            if (pf != null)
                            {
                                posDocumentReportSettings.XMLPrintFormat.LoadFromXmlString(pf.Format);
                                List<DynamicFunction> dynamicFunctions = new List<DynamicFunction>();
                                assembliesDict.Add(posDocumentReportSettings.DocumentTypeOid.ToString(), dynamicFunctions);
                            }
                        }

                        this.DocumentReports.Add(posDocumentReportSettings);
                    }
                    catch (Exception ex)
                    {
                        Kernel.LogFile.Error("Error on Reading ReportSetting ", ex.Message);
                    }
                }


                List<POSDocumentReportSettings> xmlreports = this.DocumentReports.Where(x => x.XMLPrintFormat != null).ToList();
                foreach (POSDocumentReportSettings xmlReport in xmlreports)
                {
                    try
                    {
                        references = new List<String>();
                        usings = new List<String>();
                        properties = new List<String>();
                        String key = assembliesDict.Where(x => x.Key == xmlReport.DocumentTypeOid.ToString()).FirstOrDefault().Key;
                        List<DynamicFunction> functions = assembliesDict.Where(x => x.Key == xmlReport.DocumentTypeOid.ToString()).FirstOrDefault().Value;
                        if (!String.IsNullOrEmpty(key))
                        {
                            XmlDocument funcDoc = new XmlDocument();
                            POSPrintFormat pf = this.SessionManager.GetSession<POSPrintFormat>().GetObjectByKey<POSPrintFormat>(xmlReport.PrintFormatOid);
                            funcDoc.LoadXml(pf.Format);

                            foreach (XmlNode function in funcDoc.GetElementsByTagName("Function"))
                            {
                                String Name = String.Empty;
                                String Code = String.Empty;
                                String ReturnType = String.Empty;
                                String Parameters = String.Empty;

                                foreach (XmlAttribute attr in function.Attributes)
                                {
                                    switch (attr.Name.ToUpper())
                                    {
                                        case "NAME":
                                            Name = attr.Value;
                                            break;
                                        case "RETURNTYPE":
                                            ReturnType = attr.Value;
                                            break;
                                        case "PARAMETERS":
                                            Parameters = attr.Value;
                                            break;
                                    }
                                }

                                Code = function.InnerText;
                                if (!String.IsNullOrEmpty(Name) && !String.IsNullOrEmpty(Code))
                                {
                                    if (String.IsNullOrEmpty(ReturnType))
                                    {
                                        ReturnType = "void";
                                    }
                                    DynamicFunction func = new DynamicFunction(Name, Code, xmlReport.DocumentTypeOid, ReturnType, Parameters);
                                    functions.Add(func);
                                }
                            }

                            assembliesDict[key] = functions;
                            foreach (XmlNode reference in funcDoc.GetElementsByTagName("Reference"))
                                if (!String.IsNullOrEmpty(reference.InnerXml))
                                    references.Add(reference.InnerXml);


                            foreach (XmlNode use in funcDoc.GetElementsByTagName("Using"))
                                if (!String.IsNullOrEmpty(use.InnerXml))
                                    usings.Add(use.InnerXml);

                            foreach (XmlNode property in funcDoc.GetElementsByTagName("Property"))
                                if (!String.IsNullOrEmpty(property.InnerXml))
                                    properties.Add(property.InnerXml);

                        }
                    }
                    catch (Exception ex)
                    {
                        Kernel.LogFile.Error(ex.Message + " " + "At Loading Xml Reports ");
                        continue;
                    }
                }
                CreateAssemblies(assembliesDict, usings, references, properties);
            }
        }

        private void CreatePosOposReports()
        {
            List<PosReport> posReports = new List<PosReport>();
            posReports = new XPCollection<PosReport>(SessionManager.GetSession<PosReport>(), CriteriaOperator.And(new BinaryOperator("IsActive", true))).ToList();
            foreach (PosReport report in posReports)
            {
                CreatePosOposReportsAssemblies(report);
            }

        }

        private void CreatePosOposReportsAssemblies(PosReport report)
        {

            List<String> references = new List<String>() { "System.Core.dll", "DevExpress.Data.v15.2.dll", "DevExpress.Xpo.v15.2.dll", "NLog.dll" };
            CompileCode(report.Format, report.Oid.ToString(), references);
        }



        private void CreateAssemblies(Dictionary<String, List<DynamicFunction>> dictionary, List<String> usings, List<String> references, List<String> classProperties)
        {
            foreach (var dict in dictionary)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(CreateUsingStatements(usings));
                sb.Append(Environment.NewLine);
                sb.Append(CreateClassCode(dict.Value, classProperties));
                CompileCode(sb.ToString(), dict.Key, references);
            }
        }


        private String CreateClassCode(List<DynamicFunction> functions, List<String> properties)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(System.Environment.NewLine);
            sb.Append("namespace ITS.POS.Client" + System.Environment.NewLine);
            sb.Append("{" + System.Environment.NewLine);
            sb.Append("	public class " + "DynamicClass" + " " + System.Environment.NewLine);
            sb.Append(" {" + System.Environment.NewLine);
            sb.Append(System.Environment.NewLine);
            sb.Append("ISessionManager sessionManager { get; set; }" + Environment.NewLine);
            sb.Append("IConfigurationManager config { get; set; }" + Environment.NewLine);
            sb.Append("DocumentHeader obj { get; set; }" + Environment.NewLine);
            sb.Append("IPosKernel kernel { get; set; }" + Environment.NewLine);
            sb.Append("public DynamicClass(ISessionManager sessionManager, IConfigurationManager config, DocumentHeader obj, IPosKernel kernel)" + Environment.NewLine);
            sb.Append("{" + Environment.NewLine);
            sb.Append("this.sessionManager = sessionManager;");
            sb.Append(Environment.NewLine);
            sb.Append("this.config = config;");
            sb.Append(Environment.NewLine);
            sb.Append("this.obj = obj;");
            sb.Append(Environment.NewLine);
            sb.Append("this.kernel = kernel;");
            sb.Append("}" + Environment.NewLine);
            sb.Append(Environment.NewLine);

            foreach (String property in properties)
            {
                sb.Append(property + System.Environment.NewLine);
            }
            sb.Append(System.Environment.NewLine);

            foreach (DynamicFunction function in functions)
            {
                sb.Append("public " + function.ReturnType + " " + function.Name + " (" + function.Parameters + ") {" + System.Environment.NewLine);
                sb.Append(function.Code + System.Environment.NewLine);
                sb.Append("}");
                sb.Append(System.Environment.NewLine);
            }
            sb.Append(" }" + System.Environment.NewLine);
            sb.Append("}" + System.Environment.NewLine);
            return sb.ToString();
        }

        private String CreateUsingStatements(List<String> usings)
        {
            StringBuilder sb = new StringBuilder();
            foreach (String use in usings)
            {
                sb.Append("using " + use + ";" + System.Environment.NewLine);
            }
            return sb.ToString();
        }

        private void CompileCode(String code, String assemblyName, List<String> references)
        {
            try
            {

                Kernel.LogFile.Info(DateTime.Now + " Class To Create");
                Kernel.LogFile.Info(code);
                Kernel.LogFile.Info(DateTime.Now + " " + Environment.NewLine);
                String compileCode = String.Empty;
                CSharpCodeProvider csp = new CSharpCodeProvider();
                ICodeCompiler cc = csp.CreateCompiler();
                CompilerParameters cp = new CompilerParameters();
                cp.OutputAssembly = Application.StartupPath + "\\Modules\\" + assemblyName + ".dll";
                AddReferencesToAssembly(ref cp, references);
                cp.CompilerOptions = "/target:library /optimize";
                cp.GenerateExecutable = false;
                cp.GenerateInMemory = false;

                System.CodeDom.Compiler.TempFileCollection tfc = new TempFileCollection(Application.StartupPath, false);
                CompilerResults cr = new CompilerResults(tfc);
                cr = cc.CompileAssemblyFromSource(cp, code);
                if (cr.Errors.Count > 0)
                {
                    foreach (CompilerError ce in cr.Errors)
                    {
                        Kernel.LogFile.Error("Error at line " + ce.Line.ToString() + "  at column " + ce.Column.ToString());
                        Kernel.LogFile.Error(ce.ErrorNumber + ": " + ce.ErrorText);

                    }
                }
                System.Collections.Specialized.StringCollection sc = cr.Output;
                foreach (string s in sc)
                {
                    Kernel.LogFile.Error(s + " At CompileCode");
                }
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Error(ex.GetFullMessage() + " At CompileCode");
            }
        }

        private void AddReferencesToAssembly(ref CompilerParameters parameters, List<String> references)
        {
            string[] itsFiles = Directory.GetFiles(Application.StartupPath + "\\", "ITS.*.dll");
            string[] systemFiles = Directory.GetFiles(Application.StartupPath + "\\", "System.*");
            string[] files = itsFiles.ToList().Concat(systemFiles.ToList()).ToArray();

            for (int i = 0; i < files.Count(); i++)
            {
                parameters.ReferencedAssemblies.Add(ExtractFilename(files[i]));
            }

            foreach (String reference in references)
            {
                parameters.ReferencedAssemblies.Add(reference);
            }
            parameters.ReferencedAssemblies.Add("ITS.POS.Client.exe");
        }

        private static string ExtractFilename(string filepath)
        {
            if (filepath.Trim().EndsWith(@"\"))
                return String.Empty;


            int position = filepath.LastIndexOf('\\');
            if (position == -1)
            {
                if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + filepath))
                    return filepath;
                else
                    return String.Empty;
            }
            else
            {
                if (File.Exists(filepath))
                    return filepath.Substring(position + 1);
                else
                    return String.Empty;
            }
        }

        private static String ReplaceString(String originalString, String removeString, String replaceString)
        {
            String result = String.Empty;
            result = originalString.Replace(removeString, replaceString);
            return result;
        }

        /// <summary>
        /// Loads the application's configuration files.
        /// </summary>
        /// <param name="globalsXmlPath"></param>
        /// <param name="receiptFormatXmlPath"></param>
        /// <param name="xReportFormatXmlPath"></param>
        /// <param name="zReportFormatXmlPath"></param>
        /// <param name="logger"></param>
        public void LoadConfiguration(string globalsXmlPath, string receiptFormatXmlPath, string xReportFormatXmlPath, string zReportFormatXmlPath, string reportSettingsXmlPath, Logger logger, string extraSettingsXmlPath)
        {
            //MaximumQuantity = 9999.9m;
            //MaximumValue = 9999.99m;
            //MaximumTotal = 9999.99m;

            ReceiptVariableIdentifier = "@";
            CurrencySymbol = "€";
            CurrencyPattern = eCurrencyPattern.AFTER_NUMBER_WITH_SPACE;

            if (File.Exists(globalsXmlPath))
            {
                using (XmlTextReader xmlReader = new XmlTextReader(globalsXmlPath))
                {
                    //read through all the nodes
                    while (xmlReader.Read())
                    {
                        //the headlines we want are in the item nodes
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            switch (xmlReader.Name)
                            {
                                case "CurrentStore":
                                    xmlReader.Read();
                                    CurrentStoreOid = xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim());
                                    break;
                                case "CurrentTerminal":
                                    xmlReader.Read();
                                    CurrentTerminalOid = xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim());
                                    break;
                                case "TerminalID":
                                    xmlReader.Read();
                                    TerminalID = Int32.Parse(xmlReader.Value);
                                    break;
                                case "DefaultCustomer":
                                    xmlReader.Read();
                                    DefaultCustomerOid = xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim());
                                    break;
                                case "DefaultDocumentType":
                                    xmlReader.Read();
                                    DefaultDocumentTypeOid = xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim());
                                    break;
                                case "ProFormaInvoiceDocumentType":
                                    xmlReader.Read();
                                    ProFormaInvoiceDocumentTypeOid = xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim());
                                    break;
                                case "ProFormaInvoiceDocumentSeries":
                                    xmlReader.Read();
                                    ProFormaInvoiceDocumentSeriesOid = xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim());
                                    break;

                                case "SpecialProformaDocumentType":
                                    xmlReader.Read();
                                    SpecialProformaDocumentTypeOid = xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim());
                                    break;
                                case "SpecialProformaDocumentSeries":
                                    xmlReader.Read();
                                    SpecialProformaDocumentSeriesOid = xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim());
                                    break;

                                case "WithdrawalDocumentType":
                                    xmlReader.Read();
                                    WithdrawalDocumentTypeOid = xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim());
                                    break;
                                case "WithdrawalDocumentSeries":
                                    xmlReader.Read();
                                    WithdrawalDocumentSeriesOid = xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim());
                                    break;
                                case "WithdrawalItem":
                                    xmlReader.Read();
                                    WithdrawalItemOid = xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim());
                                    break;
                                case "DepositDocumentType":
                                    xmlReader.Read();
                                    DepositDocumentTypeOid = xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim());
                                    break;
                                case "DepositDocumentSeries":
                                    xmlReader.Read();
                                    DepositDocumentSeriesOid = xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim());
                                    break;
                                case "DepositItem":
                                    xmlReader.Read();
                                    DepositItemOid = xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim());
                                    break;
                                case "DefaultDocumentStatus":
                                    xmlReader.Read();
                                    DefaultDocumentStatusOid = xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim());
                                    break;
                                case "DefaultDocumentSeries":
                                    xmlReader.Read();
                                    DefaultDocumentSeriesOid = xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim());
                                    break;
                                case "DefaultPaymentMethod":
                                    xmlReader.Read();
                                    DefaultPaymentMethodOid = xmlReader.Value.Trim() == "" ? Guid.Empty : new Guid(xmlReader.Value.Trim());
                                    break;
                                case "FiscalDevice":
                                    xmlReader.Read();
                                    try
                                    {
                                        FiscalDevice = (eFiscalDevice)Enum.Parse(typeof(eFiscalDevice), xmlReader.Value.ToUpper());
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Info(ex, "GlobalContext:InitGlobals,Exception catched");
                                        throw new Exception(POSClientResources.INVALID_FISCAL_DEVICE);
                                    }
                                    break;
                                case "ReceiptVariableIdentifier":
                                    xmlReader.Read();
                                    ReceiptVariableIdentifier = xmlReader.Value;
                                    break;
                                case "CurrencySymbol":
                                    xmlReader.Read();
                                    CurrencySymbol = xmlReader.Value;
                                    break;
                                case "CurrencyPattern":
                                    xmlReader.Read();
                                    try
                                    {
                                        CurrencyPattern = (eCurrencyPattern)Enum.Parse(typeof(eCurrencyPattern), xmlReader.Value.ToUpper());
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Info(ex, "GlobalContext:InitGlobals,Exception catched");
                                        throw new Exception(POSClientResources.INVALID_PROPERTY + " Currency Pattern");
                                    }
                                    break;
                                case "ABCDirectory":
                                    xmlReader.Read();
                                    ABCDirectory = xmlReader.Value;
                                    break;
                                case "UsesTouchScreen":
                                    xmlReader.Read();
                                    bool usesTouchScreen;
                                    if (Boolean.TryParse(xmlReader.Value, out usesTouchScreen))
                                    {
                                        UsesTouchScreen = usesTouchScreen;
                                    }
                                    break;
                                case "POSSellsInactiveItems":
                                    xmlReader.Read();
                                    bool posSellsInactiveItems;
                                    if (Boolean.TryParse(xmlReader.Value, out posSellsInactiveItems))
                                    {
                                        POSSellsInactiveItems = posSellsInactiveItems;
                                    }
                                    break;
                                case "UsesKeyLock":
                                    xmlReader.Read();
                                    bool usesKeyLock;
                                    if (Boolean.TryParse(xmlReader.Value, out usesKeyLock))
                                    {
                                        UsesKeyLock = usesKeyLock;
                                    }
                                    break;
                                case "AutoFocus":
                                    xmlReader.Read();
                                    bool autoFocus;
                                    if (Boolean.TryParse(xmlReader.Value, out autoFocus))
                                    {
                                        AutoFocus = autoFocus;
                                    }
                                    break;
                                case "AsksForStartingAmount":
                                    xmlReader.Read();
                                    bool asksForStartingAmount;
                                    if (Boolean.TryParse(xmlReader.Value, out asksForStartingAmount))
                                    {
                                        AsksForStartingAmount = asksForStartingAmount;
                                    }
                                    break;
                                case "AsksForFinalAmount":
                                    xmlReader.Read();
                                    bool asksForFinalAmount;
                                    if (Boolean.TryParse(xmlReader.Value, out asksForFinalAmount))
                                    {
                                        AsksForFinalAmount = asksForFinalAmount;
                                    }
                                    break;
                                case "PrintDiscountAnalysis":
                                    xmlReader.Read();
                                    bool printDiscountAnalysis;
                                    if (Boolean.TryParse(xmlReader.Value, out printDiscountAnalysis))
                                    {
                                        PrintDiscountAnalysis = printDiscountAnalysis;
                                    }
                                    break;
                                case "AutoIssueZEAFDSS":
                                    xmlReader.Read();
                                    bool autoIssueZEAFDSS;
                                    if (Boolean.TryParse(xmlReader.Value, out autoIssueZEAFDSS))
                                    {
                                        AutoIssueZEAFDSS = autoIssueZEAFDSS;
                                    }
                                    break;
                                case "Locale":
                                    xmlReader.Read();
                                    Locale = LocaleHelper.GetLanguage(xmlReader.Value);
                                    break;
                                case "StoreControllerURL":
                                    xmlReader.Read();
                                    StoreControllerWebServiceURL = xmlReader.Value.TrimEnd('/') + "/POSUpdateService.asmx";
                                    break;
                                case "EnableLowEndMode":
                                    xmlReader.Read();
                                    bool enableLowEndMode;
                                    if (Boolean.TryParse(xmlReader.Value, out enableLowEndMode))
                                    {
                                        EnableLowEndMode = enableLowEndMode;
                                    }
                                    break;
                                case "DemoMode":
                                    xmlReader.Read();
                                    bool demoMode;
                                    if (Boolean.TryParse(xmlReader.Value, out demoMode))
                                    {
                                        DemoMode = demoMode;
                                    }
                                    break;
                                case "ForcedWithdrawMode":
                                    xmlReader.Read();
                                    try
                                    {
                                        ForcedWithdrawMode = (eForcedWithdrawMode)Enum.Parse(typeof(eForcedWithdrawMode), xmlReader.Value.ToUpper());
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Info(ex, "GlobalContext:InitGlobals,Exception catched");
                                        throw new Exception(POSClientResources.INVALID_SETTINGS + " eForcedWithdrawMode '" + xmlReader.Value.ToUpper() + "' does not exist");
                                    }
                                    break;
                                case "ForcedWithdrawCashAmountLimit":
                                    xmlReader.Read();
                                    decimal forcedWithdrawCashAmountLimit;
                                    if (decimal.TryParse(xmlReader.Value, out forcedWithdrawCashAmountLimit))
                                    {
                                        ForcedWithdrawCashAmountLimit = forcedWithdrawCashAmountLimit / 100;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    LocaleHelper.SetLocale(LocaleHelper.GetLanguageCode(Locale), CurrencySymbol, CurrencyPattern);
                }
            }
            else
            {
                throw new Exception(String.Format(POSClientResources.FILE_0_NOT_FOUND, globalsXmlPath));
            }

            if (File.Exists(receiptFormatXmlPath))
            {
                ReceiptSchema = new Receipt.ReceiptSchema();
                ReceiptSchema.LoadFromXml(receiptFormatXmlPath);

                try
                {
                    DocumentDetailPrintDescription = eDocumentDetailPrintDescription.ItemName;
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(receiptFormatXmlPath);
                    foreach (XmlNode dtlNode in xmlDocument.GetElementsByTagName("Cell"))
                    {
                        if (dtlNode.InnerText.Contains("CustomDescription"))
                        {
                            DocumentDetailPrintDescription = eDocumentDetailPrintDescription.CustomDescription;
                            break;
                        }
                        else if (dtlNode.InnerText.Contains("ItemName"))
                        {
                            DocumentDetailPrintDescription = eDocumentDetailPrintDescription.ItemName;
                            break;
                        }
                        else if (dtlNode.InnerText.Contains("ItemExtraInfoDescription"))
                        {
                            DocumentDetailPrintDescription = eDocumentDetailPrintDescription.ItemExtraInfoName;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {

                }

            }
            else
            {
                throw new Exception(String.Format(POSClientResources.FILE_0_NOT_FOUND, receiptFormatXmlPath));
            }

            if (File.Exists(xReportFormatXmlPath))
            {
                ΧReportSchema = new Receipt.ReceiptSchema();
                ΧReportSchema.LoadFromXml(xReportFormatXmlPath);
            }
            else
            {
                throw new Exception(String.Format(POSClientResources.FILE_0_NOT_FOUND, xReportFormatXmlPath));
            }

            if (File.Exists(zReportFormatXmlPath))
            {
                ZReportSchema = new Receipt.ReceiptSchema();
                ZReportSchema.LoadFromXml(zReportFormatXmlPath);
            }
            else
            {
                throw new Exception(String.Format(POSClientResources.FILE_0_NOT_FOUND, zReportFormatXmlPath));
            }

            LoadPOSDocumentReportSettings(reportSettingsXmlPath);

            LoadExtraSettings(extraSettingsXmlPath);

            CreatePosOposReports();
            string pauseFormMediaPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Media\\Images";
            try
            {
                UseSliderPauseForm = SessionManager.GetSession<PosReport>().GetObjectByKey<ITS.POS.Model.Settings.POS>(this.CurrentTerminalOid).UseSliderPauseForm;
                UseCashCounter = SessionManager.GetSession<PosReport>().GetObjectByKey<ITS.POS.Model.Settings.POS>(this.CurrentTerminalOid).UseCashCounter;
                PauseFormImages = LoadImages(pauseFormMediaPath);
            }
            catch (Exception ex) { }


        }

        public List<Image> LoadImages(string MediaFolder)
        {
            List<Image> images = new List<Image>();
            try
            {

                DirectoryInfo directory = new DirectoryInfo(MediaFolder);
                string supportedExtensions = "*.jpg,*.gif,*.png,*.bmp,*.jpe,*.jpeg,*.wmf,*.emf,*.xbm,*.ico,*.eps,*.tif,*.tiff";
                foreach (FileInfo imageFile in directory.GetFiles("*.*", SearchOption.AllDirectories).Where(s => supportedExtensions.Contains(Path.GetExtension(s.Name).ToLower())))
                {
                    Image img = Image.FromFile(@MediaFolder + "/" + imageFile.Name);
                    images.Add(img);
                }
            }
            catch (Exception ex) { }

            return images;
        }

        private OwnerApplicationSettings appSettings;
        private volatile bool reloadAppSettings;

        /// <summary>
        /// Gets the OwnerApplicationSettings for this POS.
        /// </summary>
        /// <returns></returns>
        public OwnerApplicationSettings GetAppSettings()
        {
            if (appSettings == null || reloadAppSettings)
            {
                appSettings = SessionManager.FindObject<OwnerApplicationSettings>(null);
                reloadAppSettings = false;
                PlatformRoundingHandler.SetOwnerApplicationSettings(appSettings);
            }
            return appSettings;
        }

        /// <summary>
        /// Issues a reload of the application settings from the database. Call GetAppSettings() to get the new settings.
        /// </summary>
        public void ReloadApplicationSettings()
        {
            ////This will will only flag the settings for reload, to avoid cross-thread violations of uow.
            reloadAppSettings = true;
        }

        /// <summary>
        /// Checks if the loaded settings are valid.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool CheckIfSettingsAreValid(out string message)
        {
            bool areValid = true;
            message = "";

            if (SessionManager.FindObject<OwnerApplicationSettings>(null) == null)
            {
                message += "Application Settings are null. ";
                areValid = false;
            }

            if (SessionManager.GetObjectByKey<Store>(this.CurrentStoreOid) == null)
            {
                message += "CurrentStore is null. ";
                areValid = false;
            }

            if (SessionManager.GetObjectByKey<POS.Model.Settings.POS>(this.CurrentTerminalOid) == null)
            {
                message += "CurrentTerminal is null. ";
                areValid = false;
            }

            if (SessionManager.GetObjectByKey<Customer>(DefaultCustomerOid) == null)
            {
                message += "DefaultCustomer is null. ";
                areValid = false;
            }

            if (SessionManager.GetObjectByKey<DocumentType>(DefaultDocumentTypeOid) == null)
            {
                message += "DefaultDocumentType is null. ";
                areValid = false;
            }

            if (SessionManager.GetObjectByKey<DocumentStatus>(DefaultDocumentStatusOid) == null)
            {
                message += "DefaultDocumentStatus is null. ";
                areValid = false;
            }

            if (SessionManager.GetObjectByKey<DocumentSeries>(DefaultDocumentSeriesOid) == null)
            {
                message += "DefaultDocumentSeries is null. ";
                areValid = false;
            }

            if (SessionManager.GetObjectByKey<PaymentMethod>(DefaultPaymentMethodOid) == null)
            {
                message += "DefaultPaymentMethod is null. ";
                areValid = false;
            }

            return areValid;
        }

        public Receipt.ReceiptSchema GetReceiptSchema(DocumentHeader documentHeader)
        {
            POSDocumentReportSettings posDocumentReportSettings = this.DocumentReports.Where(report => report.DocumentTypeOid == documentHeader.DocumentType).FirstOrDefault();
            if (posDocumentReportSettings != null && posDocumentReportSettings.XMLPrintFormat != null)
            {
                return posDocumentReportSettings.XMLPrintFormat;
            }

            return this.ReceiptSchema;
        }
    }
}

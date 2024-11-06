using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Ionic.Zip;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.AuxillaryClasses;
using ITS.Retail.WebClient.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Xml;
using ITS.Retail.Platform.Common.ViewModel;
using POSCommandsLibrary;
using ITS.Retail.Platform.Common.AuxilliaryClasses;
using ITS.Retail.PrintServer.Common;

namespace ITS.Retail.WebClient
{
    /// <summary>
    /// Summary description for POSUpdateService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
    // [System.Web.Script.Services.ScriptService]

    public class POSUpdateService : WebService
    {
        private static Queue<Tuple<GenericThread, object>> threadsToRun;
        private static GenericThread currentThread;
        private static Timer runner;
        private static TimerCallback runnerCallback;

        public static void Initialize()
        {
            threadsToRun = new Queue<Tuple<GenericThread, object>>();
            currentThread = null;
            runnerCallback = new TimerCallback(ThreadRunner);
            runner = new Timer(runnerCallback, null, 1000, 200);
        }

        protected CriteriaOperator GetRestrictions<T>(string deviceID, eIdentifier identifier, eUpdateDirection direction) where T : BasicObj
        {
            return SynchronizationInfoHelper.GetUpdaterRestrictionsForDevice(MvcApplication.ApplicationInstance, typeof(T), deviceID, identifier, direction);
        }

        public string GetDBObjects<T>(DateTime version, string deviceID, eIdentifier identifier, eUpdateDirection direction, out int totalChanges) where T : BasicObj
        {
            try
            {
                string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                List<string> list = new List<string>(MvcApplication.UpdaterBatchSize);
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    CriteriaOperator versionCriteria = new BinaryOperator("UpdatedOnTicks", version.Ticks, BinaryOperatorType.GreaterOrEqual);
                    if (direction == eUpdateDirection.STORECONTROLLER_TO_POS)
                    {
                        string typeName = typeof(T).Name.Replace(typeof(T).Namespace + ".", "");
                        UpdaterMode manualUpdates = uow.FindObject<UpdaterMode>(CriteriaOperator.And(new BinaryOperator("Mode", eUpdaterMode.MANUAL), new BinaryOperator("EntityName", typeName)));
                        if (manualUpdates != null)
                        {
                            Guid deviceGuid;
                            long maxVersion = 0;
                            if (Guid.TryParse(deviceID, out deviceGuid))
                            {
                                POSUpdaterManualVersion maxVersionObject = uow.FindObject<POSUpdaterManualVersion>(
                                    CriteriaOperator.And(
                                        new BinaryOperator("EntityName", typeName),
                                        new BinaryOperator("POS.Oid", deviceGuid)
                                    ));
                                maxVersion = maxVersionObject == null ? 0 : maxVersionObject.MaxUpdatedOnTicks;
                            }
                            versionCriteria = CriteriaOperator.And(versionCriteria, new BinaryOperator("UpdatedOnTicks", maxVersion, BinaryOperatorType.LessOrEqual));
                        }
                    }

                    CriteriaOperator crop = CriteriaOperator.And(
                                             CriteriaOperator.Or(
                                                        CriteriaOperator.And(GetRestrictions<T>(deviceID, identifier, direction), new OperandProperty("GCRecord").IsNull()),
                                                        new OperandProperty("GCRecord").IsNotNull()),
                                             versionCriteria,
                                            CriteriaOperator.Parse("IsExactType(This,?)", typeof(T).FullName));


                    int topReturnedObjects = MvcApplication.UpdaterBatchSize;
                    int maxBytesToReturn = MvcApplication.UpdaterBatchSize * 1000;
                    XPCursor clItems = new XPCursor(uow, typeof(T), crop, new SortProperty("UpdatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Ascending));
                    clItems.SelectDeleted = true;
                    clItems.TopReturnedObjects = topReturnedObjects;

                    totalChanges = (int)uow.Evaluate<T>(CriteriaOperator.Parse("Count"), crop);
                    int byteCount = 0;
                    int sizeOfChar = sizeof(char);
                    foreach (T item in clItems)
                    {
                        string jsonString = item.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS, direction);
                        list.Add(jsonString);
                        byteCount += (jsonString.Length * sizeOfChar);
                        if (byteCount >= maxBytesToReturn)
                        {
                            break;
                        }
                    }
                }
                string result = ZipLZMA.CompressLZMA(JsonConvert.SerializeObject(list, PlatformConstants.JSON_SERIALIZER_SETTINGS));
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                return result;
            }
            catch (Exception e)
            {
                MvcApplication.WRMLogModule.Log(e, "POSUpdateService - GetDBObjects", KernelLogLevel.Error);
                totalChanges = 0;
                return e.GetFullMessage() + Environment.NewLine + "StackTrace:" + e.GetFullStackTrace();
            }
        }

        protected bool CanDownloadType(Type type, string deviceID, eIdentifier identifier)
        {
            object[] attributes = type.GetCustomAttributes(typeof(UpdaterAttribute), false);
            if (attributes.Count() == 0)
            {
                return false;
            }
            UpdaterAttribute attribute = attributes.First() as UpdaterAttribute;

            if (ApplicationHelper.IsMasterInstance())//#if _RETAIL_WEBCLIENT
            {
                if (identifier == eIdentifier.POS || identifier == eIdentifier.MASTER)
                {
                    return false;
                }
                if (!attribute.Permissions.HasFlag(eUpdateDirection.MASTER_TO_STORECONTROLLER))
                {
                    return false;
                }
            }//#endif
            if (ApplicationHelper.IsStoreControllerInstance())//#if _RETAIL_STORECONTROLLER
            {
                if (identifier == eIdentifier.STORECONTROLLER)
                {
                    return false;
                }
                if (identifier == eIdentifier.POS && !attribute.Permissions.HasFlag(eUpdateDirection.STORECONTROLLER_TO_POS))
                {
                    return false;
                }
                if (identifier == eIdentifier.MASTER && !attribute.Permissions.HasFlag(eUpdateDirection.STORECONTROLLER_TO_MASTER))
                {
                    return false;
                }
            }//#endif

            if (ApplicationHelper.IsDualInstance())//#if _RETAIL_DUAL
            {
                if (identifier != eIdentifier.POS)
                {
                    return false;
                }
                if (!attribute.Permissions.HasFlag(eUpdateDirection.STORECONTROLLER_TO_POS))
                {
                    return false;
                }
            }//#endif

            return true;
        }

        protected bool CanPostType(Type type, string deviceID, eIdentifier identifier)
        {
            object[] attributes = type.GetCustomAttributes(typeof(UpdaterAttribute), false);
            if (attributes.Count() == 0)
            {
                return false;
            }
            UpdaterAttribute attribute = attributes.First() as UpdaterAttribute;
            if (ApplicationHelper.IsMasterInstance())//#if _RETAIL_WEBCLIENT
            {
                if (identifier == eIdentifier.POS || identifier == eIdentifier.MASTER)
                {
                    return false;
                }
                if (!attribute.Permissions.HasFlag(eUpdateDirection.STORECONTROLLER_TO_MASTER))
                {
                    return false;
                }
            }//#endif
            else if (ApplicationHelper.IsStoreControllerInstance())//#if _RETAIL_STORECONTROLLER
            {
                if (identifier == eIdentifier.STORECONTROLLER)
                {
                    return false;
                }
                if (identifier == eIdentifier.POS && !attribute.Permissions.HasFlag(eUpdateDirection.POS_TO_STORECONTROLLER))
                {
                    return false;
                }
                if (identifier == eIdentifier.MASTER && !attribute.Permissions.HasFlag(eUpdateDirection.MASTER_TO_STORECONTROLLER))
                {
                    return false;
                }
            }//#endif
            else if (ApplicationHelper.IsDualInstance())//#if _RETAIL_DUAL
            {
                if (identifier != eIdentifier.POS)
                {
                    return false;
                }
                if (!attribute.Permissions.HasFlag(eUpdateDirection.POS_TO_STORECONTROLLER))
                {
                    return false;
                }
            }//#endif
            return true;
        }

        [WebMethod]
        [LogExtension]
        public string GetChanges(string TypeName, DateTime version, string deviceID, eIdentifier identifier, out int totalChanges)
        {
            totalChanges = 0;
            try
            {
                string fullTypeName = typeof(Item).AssemblyQualifiedName.ToString().Replace("Item", TypeName);
                MethodInfo callMethod = this.GetType().GetMethod("GetDBObjects");
                if (callMethod == null) return "";
                Type argsType = Type.GetType(fullTypeName);
                if (!CanDownloadType(argsType, deviceID, identifier))
                {
                    return null;
                }

                eUpdateDirection currentDirection = eUpdateDirection.STORECONTROLLER_TO_POS;
                if (ApplicationHelper.IsMasterInstance())//#if _RETAIL_WEBCLIENT
                {
                    currentDirection = eUpdateDirection.MASTER_TO_STORECONTROLLER;
                    if (MvcApplication.IsImportRunning)
                    {
                        return MvcApplication.IMPORT_IS_RUNNING;
                    }
                }
                else if (ApplicationHelper.IsStoreControllerInstance())//#if _RETAIL_STORECONTROLLER
                {
                    if (identifier == eIdentifier.MASTER)
                    {
                        currentDirection = eUpdateDirection.STORECONTROLLER_TO_MASTER;
                    }
                    else
                    {
                        currentDirection = eUpdateDirection.STORECONTROLLER_TO_POS;
                    }
                }

                Type[] args = new Type[] { argsType };
                MethodInfo generic = callMethod.MakeGenericMethod(args);
                object[] parameters = new object[] { version, deviceID, identifier, currentDirection, totalChanges };

                string result = generic.Invoke(this, parameters).ToString();
                totalChanges = (int)parameters[4];

                return result;
            }
            catch
            {
                return null;
            }
        }

        [WebMethod]
        [LogExtension]
        public string GetStoreControllerCommands(Guid storeControllerOid, bool isOnline)
        {
            if (MvcApplication.ApplicationInstance != eApplicationInstance.RETAIL)
            {
                return null;
            }
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                StoreControllerSettings settings = uow.GetObjectByKey<StoreControllerSettings>(storeControllerOid);
                if (settings != null)
                {
                    settings.IsOnline = isOnline;
                    settings.Save();
                    uow.CommitChanges();

                    CriteriaOperator crop = new BinaryOperator("StoreController.Oid", storeControllerOid);
                    XPCollection<StoreControllerCommand> cmd = new XPCollection<StoreControllerCommand>(uow, crop);
                    if (cmd.Count == 0)
                    {
                        return null;
                    }
                    string result;
                    try
                    {
                        result = JsonConvert.SerializeObject(cmd.Select(x => x.ToJson()).ToArray());
                        uow.Delete(cmd);
                        uow.CommitChanges();
                    }
                    catch
                    {
                        result = null;
                    }
                    return result;
                }
                return null;
            }
        }


#if _RETAIL_STORECONTROLLER || _RETAIL_DUAL

        [WebMethod]
        [LogExtension]
        public POSCommandSet SendPosStatus(Guid posOid, eMachineStatus status)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {

                //Αναζητούμε τη μηχανή
                Model.POS pos = uow.GetObjectByKey<Model.POS>(posOid);
                POSCommandSet commandSet = new POSCommandSet();
                commandSet.Commands = new List<SerializableTuple<ePosCommand, string>>();
                //newCommand.Command = ePosCommand.NONE;
                if (pos != null)
                {
                    //Αλλάζουμε το status στο μοντέλο κ την ημερομηνία κ ώρα
                    pos.Status.MachineStatus = status;
                    pos.Status.MachineStatusTicks = DateTime.Now.Ticks;
                    pos.Status.LastKnownIP = HttpContext.Current.Request.UserHostAddress;
                    pos.Status.Save();
                    try
                    {
                        XpoHelper.CommitTransaction(uow);
                    }
                    catch (Exception)
                    {
                        uow.RollbackTransaction();
                    }

                    CriteriaOperator commandCriteria = CriteriaOperator.And(new BinaryOperator("POS.Oid", pos.Oid));
                    XPCollection<POSCommand> commands = new XPCollection<POSCommand>(uow, commandCriteria,
                                                    new SortProperty("CreatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Ascending));
                    commands.TopReturnedObjects = 1;
                    if (commands.Count > 0)
                    {
                        POSCommand currentCommand = commands.FirstOrDefault();
                        POSCommandSet newSet = new POSCommandSet();
                        newSet.Commands = new List<SerializableTuple<ePosCommand, string>>() {
                            new SerializableTuple<ePosCommand, string>() { Item1 = currentCommand.Command, Item2 = currentCommand.CommandParameters }
                        };
                        currentCommand.Delete();
                        uow.CommitChanges();
                        return newSet;
                    }
                    return commandSet;
                }
                return commandSet;
            }
        }


        [WebMethod]
        [LogExtension]
        public void CreateCommandResult(Guid posOid, string commandType, string command, string commandResult)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Model.Terminal terminal = uow.GetObjectByKey<Model.Terminal>(posOid);
                if (terminal != null)
                {
                    PosCommandResult posCommandResult = new PosCommandResult(uow);
                    posCommandResult.PosOid = posOid;
                    posCommandResult.CommandType = commandType;
                    posCommandResult.Command = command;
                    posCommandResult.CommandResult = commandResult;
                    posCommandResult.PosCode = terminal.Name;
                    posCommandResult.ResultTime = DateTime.Now;
                    posCommandResult.Save();
                    try
                    {
                        XpoHelper.CommitTransaction(uow);
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;
                        uow.RollbackTransaction();
                    }
                }
            }
        }


#endif

        [WebMethod]
        [LogExtension]
        public string GetGlobalsXml(int posid)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));
                String toReturn = "";
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Model.POS pos = uow.FindObject<Model.POS>(new BinaryOperator("ID", posid));
                    if (pos == null)
                    {
                        XmlNode errorNode = xml.CreateElement("Error");

                        errorNode.InnerText = "To POS με τον συγκεκριμένο ID δεν υπάρχει: '" + posid + "'";
                        xml.AppendChild(errorNode);
                        toReturn = xml.InnerXml;
                        return toReturn;
                    }
                    XmlNode globals = xml.CreateElement("Globals");

                    XmlNode currentStore = xml.CreateElement("CurrentStore");
                    currentStore.InnerText = pos.Store.Oid.ToString();
                    globals.AppendChild(currentStore);

                    XmlNode currentTerminal = xml.CreateElement("CurrentTerminal");
                    currentTerminal.InnerText = pos.Oid.ToString();
                    globals.AppendChild(currentTerminal);

                    XmlNode currentTerminalID = xml.CreateElement("TerminalID");
                    currentTerminalID.InnerText = pos.ID.ToString();
                    globals.AppendChild(currentTerminalID);

                    XmlNode defaultCustomer = xml.CreateElement("DefaultCustomer");
                    defaultCustomer.InnerText = pos.DefaultCustomer.Oid.ToString();
                    globals.AppendChild(defaultCustomer);

                    XmlNode defaultDocumentType = xml.CreateElement("DefaultDocumentType");
                    defaultDocumentType.InnerText = pos.DefaultDocumentType.Oid.ToString();
                    globals.AppendChild(defaultDocumentType);

                    XmlNode proFormaInvoiceDocumentType = xml.CreateElement("ProFormaInvoiceDocumentType");
                    proFormaInvoiceDocumentType.InnerText = pos.ProFormaInvoiceDocumentType.Oid.ToString();
                    globals.AppendChild(proFormaInvoiceDocumentType);

                    XmlNode proFormaInvoiceDocumentSeries = xml.CreateElement("ProFormaInvoiceDocumentSeries");
                    proFormaInvoiceDocumentSeries.InnerText = pos.ProFormaInvoiceDocumentSeries.Oid.ToString();
                    globals.AppendChild(proFormaInvoiceDocumentSeries);

                    XmlNode specialProformaDocumentType = xml.CreateElement("SpecialProformaDocumentType");
                    specialProformaDocumentType.InnerText = pos.SpecialProformaDocumentType != null ? pos.SpecialProformaDocumentType.Oid.ToString() : Guid.Empty.ToString();
                    globals.AppendChild(specialProformaDocumentType);

                    XmlNode specialProformaDocumentSeries = xml.CreateElement("SpecialProformaDocumentSeries");
                    specialProformaDocumentSeries.InnerText = pos.SpecialProformaDocumentSeries != null ? pos.SpecialProformaDocumentSeries.Oid.ToString() : Guid.Empty.ToString();
                    globals.AppendChild(specialProformaDocumentSeries);

                    XmlNode withdrawalDocumentType = xml.CreateElement("WithdrawalDocumentType");
                    withdrawalDocumentType.InnerText = pos.WithdrawalDocumentType.Oid.ToString();
                    globals.AppendChild(withdrawalDocumentType);

                    XmlNode withdrawalDocumentSeries = xml.CreateElement("WithdrawalDocumentSeries");
                    withdrawalDocumentSeries.InnerText = pos.WithdrawalDocumentSeries.Oid.ToString();
                    globals.AppendChild(withdrawalDocumentSeries);

                    XmlNode withdrawalItem = xml.CreateElement("WithdrawalItem");
                    withdrawalItem.InnerText = pos.WithdrawalItem.Oid.ToString();
                    globals.AppendChild(withdrawalItem);

                    XmlNode depositDocumentType = xml.CreateElement("DepositDocumentType");
                    depositDocumentType.InnerText = pos.DepositDocumentType.Oid.ToString();
                    globals.AppendChild(depositDocumentType);

                    XmlNode depositDocumentSeries = xml.CreateElement("DepositDocumentSeries");
                    depositDocumentSeries.InnerText = pos.DepositDocumentSeries.Oid.ToString();
                    globals.AppendChild(depositDocumentSeries);

                    XmlNode depositItem = xml.CreateElement("DepositItem");
                    depositItem.InnerText = pos.DepositItem.Oid.ToString();
                    globals.AppendChild(depositItem);

                    XmlNode defaultDocumentStatus = xml.CreateElement("DefaultDocumentStatus");
                    defaultDocumentStatus.InnerText = pos.DefaultDocumentStatus.Oid.ToString();
                    globals.AppendChild(defaultDocumentStatus);

                    XmlNode defaultDocumentSeries = xml.CreateElement("DefaultDocumentSeries");
                    defaultDocumentSeries.InnerText = pos.DefaultDocumentSeries.Oid.ToString();
                    globals.AppendChild(defaultDocumentSeries);

                    XmlNode defaultPaymentMethod = xml.CreateElement("DefaultPaymentMethod");
                    defaultPaymentMethod.InnerText = pos.DefaultPaymentMethod.Oid.ToString();
                    globals.AppendChild(defaultPaymentMethod);

                    XmlNode usesTouchScreen = xml.CreateElement("UsesTouchScreen");
                    usesTouchScreen.InnerText = pos.UsesTouchScreen.ToString();
                    globals.AppendChild(usesTouchScreen);

                    XmlNode usesKeyLock = xml.CreateElement("UsesKeyLock");
                    usesKeyLock.InnerText = pos.UsesKeyLock.ToString();
                    globals.AppendChild(usesKeyLock);

                    XmlNode autoFocus = xml.CreateElement("AutoFocus");
                    autoFocus.InnerText = pos.AutoFocus.ToString();
                    globals.AppendChild(autoFocus);

                    XmlNode asksForStartingAmount = xml.CreateElement("AsksForStartingAmount");
                    asksForStartingAmount.InnerText = pos.AsksForStartingAmount.ToString();
                    globals.AppendChild(asksForStartingAmount);

                    XmlNode asksForFinalAmount = xml.CreateElement("AsksForFinalAmount");
                    asksForFinalAmount.InnerText = pos.AsksForFinalAmount.ToString();
                    globals.AppendChild(asksForFinalAmount);

                    XmlNode printDiscountAnalysis = xml.CreateElement("PrintDiscountAnalysis");
                    printDiscountAnalysis.InnerText = pos.PrintDiscountAnalysis.ToString();
                    globals.AppendChild(printDiscountAnalysis);

                    XmlNode autoIssueZEAFDSS = xml.CreateElement("AutoIssueZEAFDSS");
                    autoIssueZEAFDSS.InnerText = pos.AutoIssueZEAFDSS.ToString();
                    globals.AppendChild(autoIssueZEAFDSS);

                    XmlNode fiscalDevice = xml.CreateElement("FiscalDevice");
                    fiscalDevice.InnerText = pos.FiscalDevice.ToString();
                    globals.AppendChild(fiscalDevice);

                    XmlNode receiptVariableIdentifier = xml.CreateElement("ReceiptVariableIdentifier");
                    receiptVariableIdentifier.InnerText = pos.ReceiptVariableIdentifier.ToString();
                    globals.AppendChild(receiptVariableIdentifier);

                    XmlNode currencySymbol = xml.CreateElement("CurrencySymbol");
                    currencySymbol.InnerText = pos.CurrencySymbol.ToString();
                    globals.AppendChild(currencySymbol);

                    XmlNode currencyPattern = xml.CreateElement("CurrencyPattern");
                    currencyPattern.InnerText = pos.CurrencyPattern.ToString();
                    globals.AppendChild(currencyPattern);

                    XmlNode ABCDirectory = xml.CreateElement("ABCDirectory");
                    ABCDirectory.InnerText = pos.ABCDirectory;
                    globals.AppendChild(ABCDirectory);

                    XmlNode Locale = xml.CreateElement("Locale");
                    Locale.InnerText = pos.CultureInfo.GetDescription();
                    globals.AppendChild(Locale);

                    XmlNode EnableLowEndMode = xml.CreateElement("EnableLowEndMode");
                    EnableLowEndMode.InnerText = pos.EnableLowEndMode.ToString();
                    globals.AppendChild(EnableLowEndMode);

                    XmlNode DemoMode = xml.CreateElement("DemoMode");
                    DemoMode.InnerText = pos.DemoMode.ToString();
                    globals.AppendChild(DemoMode);

                    XmlNode forcedWithdrawMode = xml.CreateElement("ForcedWithdrawMode");
                    forcedWithdrawMode.InnerText = pos.ForcedWithdrawMode.ToString();
                    globals.AppendChild(forcedWithdrawMode);

                    XmlNode StandaloneFiscalOnErrorMessage = xml.CreateElement("StandaloneFiscalOnErrorMessage");
                    StandaloneFiscalOnErrorMessage.InnerText = pos.StandaloneFiscalOnErrorMessage;
                    globals.AppendChild(StandaloneFiscalOnErrorMessage);

                    XmlNode forcedWithdrawCashAmountLimit = xml.CreateElement("ForcedWithdrawCashAmountLimit");
                    forcedWithdrawCashAmountLimit.InnerText = ((int)(pos.ForcedWithdrawCashAmountLimit * 100)).ToString();
                    globals.AppendChild(forcedWithdrawCashAmountLimit);

                    if (pos.Store.StoreControllerSettings == null)
                    {
                        throw new Exception("StoreControllerSettings not found. Store Oid:'" + pos.Store.Oid + "'");
                    }

                    XmlNode POSSellsInactiveItems = xml.CreateElement("POSSellsInactiveItems");
                    POSSellsInactiveItems.InnerText = pos.Store.StoreControllerSettings.POSSellsInactiveItems.ToString();
                    globals.AppendChild(POSSellsInactiveItems);

                    XmlNode StoreControllerURL = xml.CreateElement("StoreControllerURL");
                    StoreControllerURL.InnerText = HttpContext.Current.Request.Url.ToString()
                        .Substring(0, HttpContext.Current.Request.Url.ToString().IndexOf(HttpContext.Current.Request.RawUrl))
                        + HttpContext.Current.Request.ApplicationPath;
                    globals.AppendChild(StoreControllerURL);

                    xml.AppendChild(globals);

                    return xml.InnerXml;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod]
        [LogExtension]
        public string GetUpdaterModesXml(int posid)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));
                string toReturn = "";
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Model.POS pos = uow.FindObject<Model.POS>(new BinaryOperator("ID", posid));
                    if (pos == null)
                    {
                        XmlNode errorNode = xml.CreateElement("Error");

                        errorNode.InnerText = "To POS με τον συγκεκριμένο κωδικό δεν υπάρχει";
                        xml.AppendChild(errorNode);
                        toReturn = xml.InnerXml;
                        return toReturn;
                    }
                    XmlNode updaterModesNode = xml.CreateElement("EntityUpdaterModes");
                    foreach (UpdaterMode mode in new XPCollection<UpdaterMode>(uow))
                    {
                        XmlNode modeNode = xml.CreateElement("UpdaterMode");

                        XmlNode EntityNode = xml.CreateElement("Entity");
                        EntityNode.InnerText = mode.EntityName;
                        modeNode.AppendChild(EntityNode);

                        XmlNode ModeNode = xml.CreateElement("Mode");
                        ModeNode.InnerText = mode.Mode.ToString();
                        modeNode.AppendChild(ModeNode);

                        updaterModesNode.AppendChild(modeNode);
                    }
                    xml.AppendChild(updaterModesNode);
                    return xml.InnerXml;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod]
        [LogExtension]
        public String GetCustomActionCodesXml(int posid)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));
                String toReturn = "";
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Model.POS pos = uow.FindObject<Model.POS>(new BinaryOperator("ID", posid));
                    if (pos == null)
                    {
                        XmlNode errorNode = xml.CreateElement("Error");

                        errorNode.InnerText = "To POS με τον συγκεκριμένο κωδικό δεν υπάρχει";
                        xml.AppendChild(errorNode);
                        toReturn = xml.InnerXml;
                        return toReturn;
                    }
                    XmlNode rootNode = xml.CreateElement("CustomActionCodes");
                    foreach (CustomActionCode cac in new XPCollection<CustomActionCode>(uow))
                    {
                        XmlNode customActionCodeNode = xml.CreateElement("CustomActionCode");

                        XmlNode codeNode = xml.CreateElement("Code");
                        codeNode.InnerText = cac.Code;
                        customActionCodeNode.AppendChild(codeNode);

                        XmlNode actionNode = xml.CreateElement("Action");
                        actionNode.InnerText = cac.Action.ToString();
                        customActionCodeNode.AppendChild(actionNode);

                        rootNode.AppendChild(customActionCodeNode);
                    }
                    xml.AppendChild(rootNode);
                    return xml.InnerXml;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private XmlDocument CreateDevicesXmlFile(int posid)
        {
            XmlDocument xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                ITS.Retail.Model.POS pos = uow.FindObject<ITS.Retail.Model.POS>(new BinaryOperator("ID", posid));
                if (pos == null)
                {
                    XmlNode errorNode = xml.CreateElement("Error");

                    errorNode.InnerText = Resources.POSIdNotFound; //"To POS με τον συγκεκριμένο κωδικό δεν υπάρχει";
                    xml.AppendChild(errorNode);
                    return xml;
                }

                XmlNode devicesNodes = xml.CreateElement("Devices");
                //pos.TerminalDeviceAssociations[0].TerminalDevice
                foreach (TerminalDeviceAssociation terminalDeviceAssociation in pos.TerminalDeviceAssociations)
                {
                    POSDevice pdevice = terminalDeviceAssociation.TerminalDevice as POSDevice;
                    XmlNode deviceNode = xml.CreateElement((terminalDeviceAssociation.TerminalDevice as POSDevice).DeviceSettings.DeviceType.ToString());
                    if (pdevice.UsesSpecificDeviceLibrary && pdevice.DeviceSpecificType != eDeviceSpecificType.None)
                    {
                        XmlAttribute deviceType = xml.CreateAttribute("DeviceType");
                        deviceType.Value = pdevice.DeviceSpecificType.GetDescription();
                        deviceNode.Attributes.Append(deviceType);
                    }

                    XmlNode deviceNameNode = xml.CreateElement("Name");
                    deviceNameNode.InnerText = pdevice.Name;
                    deviceNode.AppendChild(deviceNameNode);

                    XmlNode connectionTypeNode = xml.CreateElement("ConnectionType");
                    connectionTypeNode.InnerText = pdevice.ConnectionType.ToString();
                    deviceNode.AppendChild(connectionTypeNode);

                    XmlNode isPrimaryNode = xml.CreateElement("IsPrimary");
                    isPrimaryNode.InnerText = terminalDeviceAssociation.IsPrimary.ToString();
                    deviceNode.AppendChild(isPrimaryNode);

                    XmlNode priorityNode = xml.CreateElement("Priority");
                    priorityNode.InnerText = terminalDeviceAssociation.Priority.ToString();
                    deviceNode.AppendChild(priorityNode);

                    DeviceSettings settings = pdevice.DeviceSettings;
                    foreach (PropertyInfo property in settings.GetType().GetProperties().Where(p => p.DeclaringType.IsSubclassOf(typeof(BaseObj))))
                    {
                        if (property.Name == "DeviceType") continue;

                        XmlNode deviceSetting = xml.CreateElement(property.Name);
                        deviceSetting.InnerText = property.GetValue(settings, null) == null ? "" : property.GetValue(settings, null).ToString();
                        deviceNode.AppendChild(deviceSetting);
                    }

                    devicesNodes.AppendChild(deviceNode);
                }

                xml.AppendChild(devicesNodes);
                return xml;
            }
        }

        [WebMethod]
        [LogExtension]
        public byte[] GetDevicesByteXml(int posid)
        {
            try
            {
                XmlDocument xml = CreateDevicesXmlFile(posid);
                using (MemoryStream stream = new MemoryStream())
                {
                    xml.Save(stream);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                return Encoding.UTF8.GetBytes(ex.Message);
            }
        }

        [WebMethod]
        [LogExtension]
        public string GetDevicesXml(int posid)
        {
            try
            {
                XmlDocument xml = CreateDevicesXmlFile(posid);
                return xml.InnerXml;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod]
        [LogExtension]
        public string GetActionLevelsXml(int posid)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));
                string toReturn = "";
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Model.POS pos = uow.FindObject<Model.POS>(new BinaryOperator("ID", posid));
                    if (pos == null)
                    {
                        XmlNode errorNode = xml.CreateElement("Error");

                        errorNode.InnerText = "To POS με τον συγκεκριμένο κωδικό δεν υπάρχει";
                        xml.AppendChild(errorNode);
                        toReturn = xml.InnerXml;
                        return toReturn;
                    }

                    XmlNode actionLevels = xml.CreateElement("ActionLevels");
                    xml.AppendChild(actionLevels);

                    if (pos.POSActionLevelsSet == null)
                    {
                        return xml.InnerXml;
                    }

                    foreach (POSActionLevel actionLevel in pos.POSActionLevelsSet.POSActionLevels)
                    {
                        XmlNode actionLevelNode = xml.CreateElement("ActionLevel");
                        XmlNode actionNode = xml.CreateElement("Action");
                        XmlNode levelNode = xml.CreateElement("Level");
                        actionNode.InnerText = actionLevel.ActionCode.ToString();
                        levelNode.InnerText = actionLevel.KeyLevel.ToString();

                        actionLevelNode.AppendChild(actionNode);
                        actionLevelNode.AppendChild(levelNode);

                        actionLevels.AppendChild(actionLevelNode);
                    }

                    xml.AppendChild(actionLevels);
                    return xml.InnerXml;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod]
        [LogExtension]
        public string GetPOSVersion()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            return v.Major + "." + v.Minor + "." + v.Build + "." + v.Revision;
        }

        [WebMethod]
        [LogExtension]
        public string GetPOSLoaderLink(int posid)
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            Version loaderVersion = AssemblyName.GetAssemblyName(Server.MapPath("~/pos/version")).Version;
            if (v != loaderVersion)
            {
                return "Version Mismatch Error: Server's POS files to be downloaded are in different version that the Server itself.\nServer Version: " + v.ToString() + "\nFiles version: " + loaderVersion.ToString();
            }
            else
            {
                return HttpContext.Current.Request.Url.ToString().Substring(0, HttpContext.Current.Request.Url.ToString().ToLower().IndexOf("/posupdateservice.asmx")) + "/pos/posloader.zip";
            }
        }

#if _RETAIL_WEBCLIENT

        [WebMethod]
        public String GetAllPosDevices(Guid StoreControllerGuid)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                StoreControllerSettings scs = uow.GetObjectByKey<StoreControllerSettings>(StoreControllerGuid);
                if (scs == null)
                    return "{}";
                XPCollection<POSDevice> posDevices = new XPCollection<POSDevice>(uow);
                List<String> list = posDevices.Select(p => p.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)).ToList();
                string result = /*CompressLZMA*/(JsonConvert.SerializeObject(list, PlatformConstants.JSON_SERIALIZER_SETTINGS));
                return result;
            }
        }

#endif

        [WebMethod]
        [LogExtension]
        public string GetPOSClientLink(int posid)
        {
            return HttpContext.Current.Request.Url.ToString().Substring(0, HttpContext.Current.Request.Url.ToString().ToLower().IndexOf("/posupdateservice.asmx")) + "/pos/pos.zip";
        }

        private bool IsZipValid(string path)
        {
            try
            {
                using (ZipFile zip = ZipFile.Read(path))
                {
                    return zip.Comment == GetPOSVersion();
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        [WebMethod]
        [LogExtension]
        public bool PostData(string typeName, string data, string deviceID, eIdentifier identifier)
        {
            string fullTypeName = typeof(Item).AssemblyQualifiedName.ToString().Replace("Item", typeName);
            List<string> parced = JsonConvert.DeserializeObject<List<string>>(Convert.ToString(ZipLZMA.DecompressLZMA(data.ToString())));
            MethodInfo callMethod = null;
            if (parced.Count > 20 && identifier != eIdentifier.POS)
            {
                callMethod = this.GetType().GetMethod("QueuePostDataThread", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            }
            else
            {
                callMethod = this.GetType().GetMethod("SaveDBObjectsThread", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            }

            Type[] args = new Type[] { Type.GetType(fullTypeName) };
            if (!CanPostType(args[0], deviceID, identifier))
            {
                return false;
            }
            MethodInfo generic = callMethod.MakeGenericMethod(args);
            object parameters = new object[] { data, deviceID };
            object result = generic.Invoke(this, new object[] { parameters });
            if (result is bool)
            {
                return (bool)result;
            }

            return true;
        }

        /// <summary>
        /// Returns the latest version of the data type provided in ticks
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        [WebMethod]
        [LogExtension]
        public long GetVersion(string typeName, string deviceID)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    string fullTypeName = typeof(Item).FullName.ToString().Replace("Item", typeName);
                    TableVersion latestObj = uow.Query<TableVersion>()
                                                        .Where(version => version.EntityName == fullTypeName && version.CreatedByDevice == deviceID)
                                                        .OrderByDescending(version => version.UpdatedOnTicks)
                                                        .FirstOrDefault();
                    return latestObj == null ? DateTime.MinValue.Ticks : latestObj.Number;
                }
            }
            catch
            {
                return DateTime.MinValue.Ticks;
            }
        }

        /// <summary>
        /// Returns the variable values requested for current object
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        [WebMethod]
        [LogExtension]
        public string GetVariableValues(string customDataViewOid, string gridOids, string paramValues)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Guid customDataViewGuid;
                    if (Guid.TryParse(customDataViewOid, out customDataViewGuid))
                    {
                        CustomDataView dataView = uow.GetObjectByKey<CustomDataView>(customDataViewOid);
                        if (dataView != null)
                        {
                            JArray Jparameters = JsonConvert.DeserializeObject(paramValues) as JArray;
                            Dictionary<string, string> parameters = new Dictionary<string, string>();
                            foreach (JToken param in Jparameters)
                            {
                                parameters.Add(string.Format("{{{0}}}", param.First.First), param.Last.First.ToString());
                            }
                            return ActionTypeHelper.ShowDataViewDataWeb(dataView.CreateDataView(dataView.Session, gridOids, parameters)).ToString();
                        }
                    }
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        private static bool GetForceReload<T>(string deviceID)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                TableVersion obj = uow.FindObject<TableVersion>(CriteriaOperator.And(new BinaryOperator("EntityName", typeof(T).ToString()), new BinaryOperator("CreatedByDevice", deviceID)));
                if (obj != null)
                {
                    return obj.ForceReload;
                }
                return true;
            }
        }

        private static void SetForceReload<T>(bool reload, string deviceID)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                TableVersion obj = uow.FindObject<TableVersion>(CriteriaOperator.And(new BinaryOperator("EntityName", typeof(T).ToString()), new BinaryOperator("CreatedByDevice", deviceID)));
                if (obj != null)
                {
                    obj.ForceReload = reload;
                    obj.Save();
                    XpoHelper.CommitChanges(uow);
                }
            }
        }

        public void SaveDBObjectsThread<T>(object parameters) where T : BasicObj
        {
            string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            string jsonData = (parameters as object[])[0] as string;
            string createdByDevice = (parameters as object[])[1] as string;
            int lines = 0;
            JObject jsonObj;
            T currentItem;
            //bool noErrors = true;
            DateTime verUpdate = new DateTime(GetVersion(typeof(T).ToString(), createdByDevice));
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                List<string> data = JsonConvert.DeserializeObject<List<string>>(Convert.ToString(ZipLZMA.DecompressLZMA(jsonData.ToString())));
                if (data.Count == 0)
                {
                    return;
                }
                bool forceReload = GetForceReload<T>(createdByDevice);
                foreach (string obj in data)
                {
                    lines++;
                    jsonObj = JObject.Parse(obj);
                    IEnumerable<JProperty> objValues = jsonObj.Properties();
                    // Find if object  exist
                    string key = jsonObj.Property("Oid").Value.ToString();

                    try
                    {
                        currentItem = uow.GetObjectByKey<T>(new Guid(key));
                        if (currentItem == null)
                        {
                            string gcRecord = jsonObj.Property("GCRecord").Value.ToString();
                            if (gcRecord == "null")
                            {
                                currentItem = (T)Activator.CreateInstance(typeof(T), new object[] { uow });
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            long value = jsonObj.Property("UpdatedOnTicks").Value.Value<long?>() ?? 0;
                            if (value <= currentItem.UpdatedOnTicks && forceReload == false)
                            {
                                verUpdate = currentItem.UpdatedOn;
                                continue;
                            }
                        }
                        string error;
                        currentItem.FromJson(jsonObj, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out error);
                        OnUpdaterSaving(currentItem);
                        currentItem.Save();
                        verUpdate = currentItem.UpdatedOn;
                        if (lines % 1000 == 0)
                        {
                            XpoHelper.CommitChanges(uow);
                            SetVersion<T>(currentItem.UpdatedOn, createdByDevice);
                        }
                    }
                    catch (Exception ex)
                    {
                        //noErrors = false;
                        //MvcApplication.Log.Error(ex, "SaveDBObjectsThread<" + typeof(T).Name + ">");
                        MvcApplication.WRMLogModule.Log(ex, "SaveDBObjectsThread<" + typeof(T).Name + ">", KernelLogLevel.Error);
                        break;
                    }
                }
                try
                {
                    XpoHelper.CommitChanges(uow);
                    SetVersion<T>(verUpdate, createdByDevice);
                }
                catch (Exception ex)
                {
                    //noErrors = false;
                    MvcApplication.WRMLogModule.Log(ex, "SaveDBObjectsThread<" + typeof(T).Name + ">", KernelLogLevel.Error);
                }
            }

            SetForceReload<T>(false, createdByDevice);

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
            //return noErrors;
        }

        /// <summary>
        /// Implement extra OnSaving logic that must not be in the Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="currentItem"></param>
        private static void OnUpdaterSaving<T>(T currentItem)
        {
            if (typeof(T) == typeof(DocumentHeader))
            {
                OnUpdaterSavingDocumentHeader(currentItem);
            }
        }

        private static void OnUpdaterSavingDocumentHeader<T>(T currentItem)
        {
            DocumentHeader documentHeader = currentItem as DocumentHeader;
            DocumentHelper.UpdateCustomerPoints(documentHeader, MvcApplication.ApplicationInstance);
            DocumentHelper.ApplyPricesFromPOS(documentHeader, MvcApplication.ApplicationInstance);
            DocumentHelper.UpdateDocumentCoupons(documentHeader, MvcApplication.ApplicationInstance);
            DocumentHelper.CreateCustomerFromPOSDocument(documentHeader, MvcApplication.ApplicationInstance);
        }

        private static void SetVersion<T>(DateTime ver, string createdByDevice)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                TableVersion obj = uow.FindObject<TableVersion>(CriteriaOperator.And(new BinaryOperator("EntityName", typeof(T).ToString(), BinaryOperatorType.Equal),
                    new BinaryOperator("CreatedByDevice", createdByDevice, BinaryOperatorType.Equal)));
                if (obj == null)
                {
                    obj = new TableVersion(uow) { EntityName = typeof(T).ToString() };
                }
                obj.Number = ver.Ticks;
                obj.CreatedByDevice = createdByDevice;
                obj.Save();
                XpoHelper.CommitChanges(uow);
            }
        }

        protected static void ThreadRunner(object state)
        {
            lock (threadsToRun)
            {
                if ((currentThread == null || !currentThread.IsAlive) && threadsToRun.Count > 0)
                {
                    Tuple<GenericThread, object> threadPair = null;

                    threadPair = threadsToRun.Dequeue();

                    currentThread = threadPair.Item1;
                    currentThread.Start(threadPair.Item2);
                }
            }
        }

        protected bool QueuePostDataThread<T>(object parameters) where T : BasicObj
        {
            try
            {
                GenericThread startImportThread = new GenericThreadT<T>(SaveDBObjectsThread<T>);
                Tuple<GenericThread, object> temp = new Tuple<GenericThread, object>(startImportThread, parameters);
                lock (threadsToRun)
                {
                    threadsToRun.Enqueue(temp);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        [WebMethod]
        [LogExtension]
        public string GetXFormat(int posid)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    ITS.Retail.Model.POS pos = uow.FindObject<ITS.Retail.Model.POS>(new BinaryOperator("ID", posid));
                    if (pos == null)
                    {
                        return "To POS με τον συγκεκριμένο ID δεν υπάρχει: '" + posid + "'";//ErrorXML("To POS με τον συγκεκριμένο ID δεν υπάρχει: '" + posid + "'");
                    }

                    if (pos.XFormat != null && !String.IsNullOrWhiteSpace(pos.XFormat.Format))
                    {
                        return pos.XFormat.Format;
                    }
                    return "To POS με ID '" + posid + "' δεν έχει X Format XML file!";//ErrorXML("To POS με ID '" + posid + "' δεν έχει X Format XML file!");
                }
            }
            catch
            {
                return "To POS με τον συγκεκριμένο ID δεν υπάρχει: '" + posid + "'";//ErrorXML("To POS με τον συγκεκριμένο ID δεν υπάρχει: '" + posid + "'");
            }
        }

        [WebMethod]
        [LogExtension]
        public string GetZFormat(int posid)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    ITS.Retail.Model.POS pos = uow.FindObject<ITS.Retail.Model.POS>(new BinaryOperator("ID", posid));
                    if (pos == null)
                    {
                        return "To POS με τον συγκεκριμένο ID δεν υπάρχει: '" + posid + "'";//ErrorXML("To POS με τον συγκεκριμένο ID δεν υπάρχει: '" + posid + "'");
                    }

                    if (pos.ZFormat != null && !String.IsNullOrWhiteSpace(pos.ZFormat.Format))
                    {
                        return pos.ZFormat.Format;
                    }
                    return "To POS με ID '" + posid + "' δεν έχει Z Format XML file!";//ErrorXML("To POS με ID '" + posid + "' δεν έχει Z Format XML file!");
                }
            }
            catch
            {
                return "To POS με τον συγκεκριμένο ID δεν υπάρχει: '" + posid + "'";//ErrorXML("To POS με τον συγκεκριμένο ID δεν υπάρχει: '" + posid + "'");
            }
        }

        [WebMethod]
        [LogExtension]
        public string GetReceiptFormat(int posid)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    ITS.Retail.Model.POS pos = uow.FindObject<ITS.Retail.Model.POS>(new BinaryOperator("ID", posid));
                    if (pos == null)
                    {
                        return "To POS με τον συγκεκριμένο ID δεν υπάρχει: '" + posid + "'";//ErrorXML("To POS με τον συγκεκριμένο ID δεν υπάρχει: '" + posid + "'");
                    }
                    if (pos.ReceiptFormat != null && !String.IsNullOrWhiteSpace(pos.ReceiptFormat.Format))
                    {
                        return pos.ReceiptFormat.Format;
                    }
                    return "To POS με ID '" + posid + "' δεν έχει Receipt Format XML file!";//ErrorXML("To POS με ID '" + posid + "' δεν έχει Receipt Format XML file!");
                }
            }
            catch
            {
                return "To POS με τον συγκεκριμένο ID δεν υπάρχει: '" + posid + "'";//ErrorXML("To POS με τον συγκεκριμένο ID δεν υπάρχει: '" + posid + "'");
            }
        }

        [WebMethod]
        [LogExtension]
        public Byte[] GetCustomLayout(int posid)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    ITS.Retail.Model.POS pos = uow.FindObject<ITS.Retail.Model.POS>(new BinaryOperator("ID", posid));
                    if (pos == null)
                    {
                        return GetEnumBytes(eServiceResponce.INVALID_INPUT);
                        //return null;//ErrorXML("To POS με τον συγκεκριμένο ID δεν υπάρχει: '" + posid + "'");
                    }
                    if (pos.POSLayout != null)
                    {
                        List<string> tempFiles = new List<string>();

                        if (!Directory.Exists(MvcApplication.TEMP_FOLDER))
                        {
                            Directory.CreateDirectory(MvcApplication.TEMP_FOLDER);
                        }
                        string tempMainFormFile = null;
                        string tempSecondaryFormFile = null;

                        if (pos.POSLayout.MainLayout != null)
                        {
                            tempMainFormFile = MvcApplication.TEMP_FOLDER + "\\" + Guid.NewGuid().ToString().Replace("-", "") + ".itsform";
                            File.WriteAllBytes(tempMainFormFile, pos.POSLayout.MainLayout);
                        }

                        if (pos.POSLayout.SecondaryLayout != null)
                        {
                            tempSecondaryFormFile = MvcApplication.TEMP_FOLDER + "\\" + Guid.NewGuid().ToString().Replace("-", "") + ".itssform";
                            File.WriteAllBytes(tempSecondaryFormFile, pos.POSLayout.SecondaryLayout);
                        }

                        string tempDLL = POSHelper.BuildForms(tempMainFormFile, tempSecondaryFormFile, MvcApplication.TEMP_FOLDER, System.Web.HttpContext.Current.Server);
                        byte[] tempDllBytes = File.ReadAllBytes(tempDLL);

                        tempFiles.Add(tempMainFormFile);
                        tempFiles.Add(tempSecondaryFormFile);
                        foreach (string file in tempFiles)
                        {
                            if (File.Exists(file))
                            {
                                File.Delete(file);
                            }
                        }
                        return tempDllBytes;
                    }
                    return GetEnumBytes(eServiceResponce.EMPTY_RESPONCE);
                    //return null;//ErrorXML("To POS με ID '" + posid + "' δεν έχει Receipt Format XML file!");
                }
            }
            catch (Exception exception)
            {
                MvcApplication.WRMLogModule.Log(exception, kernelLogLevel: KernelLogLevel.Error);
                return GetEnumBytes(eServiceResponce.EXCEPTION_HAS_BEEN_THROWN);
                //return null;//ErrorXML("To POS με τον συγκεκριμένο ID δεν υπάρχει: '" + posid + "'");
            }
        }

        private Byte[] GetEnumBytes(eServiceResponce enumValue)
        {
            byte responce = (byte)enumValue;
            return new Byte[1] { responce };
        }

        [WebMethod]
        [LogExtension]
        public String GetZReportSequence(Guid PosOid)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                ZReportSequence sq = uow.FindObject<ZReportSequence>(new BinaryOperator("POS.Oid", PosOid));
                if (sq == null)
                {
                    ITS.Retail.Model.POS pos = uow.GetObjectByKey<ITS.Retail.Model.POS>(PosOid);
                    if (pos == null)
                    {
                        return null;
                    }

                    sq = new ZReportSequence(uow);
                    sq.POS = pos;
                    sq.ZReportNumber = 0;
                    sq.CreatedByDevice = PosOid.ToString();
                    sq.Save();
                    XpoHelper.CommitTransaction(uow);
                }
                return sq.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
            }
        }

        [WebMethod]
        [LogExtension]
        public string GetDocumentSequence(Guid DocumentSeriesOid)
        {
            try
            {
                return SearchDocumentSequence(DocumentSeriesOid);

            }
            catch (Exception exception)
            {
                MvcApplication.WRMLogModule.Log(exception,
                            "An error has occurred trying to get Sequence for Series '" + DocumentSeriesOid + "' ",
                             "POSUpdateService.asmx", "GetDocumentSequenceForStoreControllerClient",
                             this.Context.Request.UserAgent, this.Context.Request.UserHostAddress, "",
                            null, KernelLogLevel.Info);

                return string.Empty;
            }
        }

        private static string SearchDocumentSequence(Guid DocumentSeriesOid)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                DocumentSequence sq = uow.FindObject<DocumentSequence>(new BinaryOperator("DocumentSeries.Oid", DocumentSeriesOid));
                if (sq == null)
                {
                    DocumentSeries series = uow.GetObjectByKey<DocumentSeries>(DocumentSeriesOid);
                    if (series == null)
                    {
                        return null;
                    }

                    sq = new DocumentSequence(uow);
                    sq.DocumentSeries = series;
                    sq.DocumentNumber = 0;
                    sq.Save();
                    XpoHelper.CommitTransaction(uow);
                }
                return sq.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
            }
        }

        [WebMethod]
        [LogExtension]
        public string[] GetSpecificDeviceDLLLinks(int posid)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    ITS.Retail.Model.POS pos = uow.FindObject<ITS.Retail.Model.POS>(new BinaryOperator("ID", posid));
                    if (pos == null)
                    {
                        return null;
                    }
                    List<string> dllNames = new List<string>();
                    foreach (TerminalDeviceAssociation tda in pos.TerminalDeviceAssociations.Where(x => (x.TerminalDevice as POSDevice).DeviceSpecificType != eDeviceSpecificType.None))
                    {
                        string filename = (tda.TerminalDevice as POSDevice).DeviceSpecificType.GetLibraryFilename();
                        if (!String.IsNullOrWhiteSpace(filename))
                        {
                            string fileUrl = HttpContext.Current.Request.Url.ToString().Substring(0, HttpContext.Current.Request.Url.ToString().ToLower().IndexOf("/posupdateservice.asmx")) + "/pos/SpecificDeviceImplementations/" + filename;
                            dllNames.Add(fileUrl);
                        }
                    }
                    if (dllNames.Count > 0)
                    {
                        return dllNames.ToArray();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        [WebMethod]
        public bool InsertOrUpdateRecord(Guid storeControllerOid, String typeName, String jsonObject, out string message)
        {
            string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            message = "";
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    StoreControllerSettings settings = uow.GetObjectByKey<StoreControllerSettings>(storeControllerOid);
                    if (settings == null)
                    {
                        message = "Not Supported: Error 1";
                        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                        return false;
                    }
                    string fullTypeName = typeof(Item).FullName.ToString().Replace("Item", typeName);
                    Type type = typeof(Item).Assembly.GetType(fullTypeName);
                    if (type == null || typeof(BasicObj).IsAssignableFrom(type) == false)
                    {
                        message = "Not Supported: Error 2";
                        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                        return false;
                    }
                    JObject jsonObj = JObject.Parse(jsonObject);
                    IEnumerable<JProperty> objValues = jsonObj.Properties();
                    // Find if object  exist
                    string key = jsonObj.Property("Oid").Value.ToString();
                    Guid gKey;
                    BasicObj obj = null;
                    if (Guid.TryParse(key, out gKey))
                    {
                        obj = uow.GetObjectByKey(type, gKey) as BasicObj;
                    }

                    if (obj == null)
                    {
                        obj = (BasicObj)Activator.CreateInstance(type, new object[] { uow });
                    }
                    if (obj.FromJson(jsonObject, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out message) == false)
                    {
                        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                        return false;
                    }
                    XpoHelper.CommitChanges(uow);
                }
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message + ex.StackTrace;
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                return false;
            }
        }

        [WebMethod]
        [LogExtension]
        public bool PostSynchronizationInfo(Guid deviceOid, string deviceName, long databaseCount, long version, string entityName,
            eSyncInfoEntityDirection direction, long minVersion, out long serverVersion, out long serverEntityCount, out string message)
        {
            message = null;
            serverVersion = -1;
            serverEntityCount = -1;
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    SynchronizationInfo entityDetail = uow.FindObject<SynchronizationInfo>(CriteriaOperator.And(new BinaryOperator("EntityName", entityName),
                                                                                                                  new BinaryOperator("DeviceOid", deviceOid)));
                    eIdentifier remoteDeviceType = MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL ? eIdentifier.STORECONTROLLER : eIdentifier.POS;
                    serverEntityCount = SynchronizationInfoHelper.GetEntityExpectedCountOfRemoteDevice(MvcApplication.ApplicationInstance, entityName, 0, deviceOid, remoteDeviceType, direction);
                    serverVersion = SynchronizationInfoHelper.GetEntityExpectedVersionOfRemoteDevice(MvcApplication.ApplicationInstance, entityName, deviceOid, remoteDeviceType, direction);
                    if (entityDetail == null)
                    {
                        entityDetail = new SynchronizationInfo(uow);
                        entityDetail.EntityName = entityName;
                        entityDetail.DeviceOid = deviceOid;
                        entityDetail.DeviceType = remoteDeviceType;
                    }

                    entityDetail.DeviceName = deviceName;
                    entityDetail.DeviceEntityCount = databaseCount;
                    entityDetail.DeviceVersion = version;
                    entityDetail.DeviceMinVersion = minVersion;
                    entityDetail.SyncInfoEntityDirection = direction;
                    entityDetail.LastUpdate = DateTime.Now;

                    XpoHelper.CommitChanges(uow);

                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message + ex.StackTrace;
                MvcApplication.WRMLogModule.Log(ex, "", KernelLogLevel.Error);
                return false;
            }
        }

        [WebMethod]
        public bool DeleteRecord(Guid storeControllerOid, String typeName, Guid oid, out string message)
        {
            message = "";
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    StoreControllerSettings settings = uow.GetObjectByKey<StoreControllerSettings>(storeControllerOid);
                    if (settings == null)
                    {
                        message = "Not Supported: Error 1";
                        return false;
                    }
                    string fullTypeName = typeof(Item).FullName.ToString().Replace("Item", typeName);
                    Type type = typeof(Item).Assembly.GetType(fullTypeName);
                    if (type == null || typeof(BasicObj).IsAssignableFrom(type) == false)
                    {
                        message = "Not Supported: Error 2";
                        return false;
                    }

                    BasicObj obj = uow.GetObjectByKey(type, oid) as BasicObj;
                    if (obj == null)
                    {
                        message = "Object not found";
                        return false;
                    }
                    obj.Delete();
                    XpoHelper.CommitChanges(uow);
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message + ex.StackTrace;
                return false;
            }
        }

        [WebMethod]
        public bool Ping()
        {
            return true;
        }

        /// <summary>
        /// Searches for a usable coupon with the given code.
        /// </summary>
        /// <param name="code">The code to search with</param>
        /// <param name="storeGuid">The current store in order to filter with its owner</param>
        /// <returns>Null if no coupon found or found but is not possible to use it, otherwise it returns the coupon that has been found as a view model.</returns>
        [WebMethod]
        public CouponViewModel GetExistingUsableCoupon(string deviceID, string code, Guid storeGuid)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Store store = uow.GetObjectByKey<Store>(storeGuid);
                CompanyNew owner = store == null ? null : store.Owner;
                if (string.IsNullOrEmpty(code) || owner == null)
                {
                    return null;
                }
                long nowTicks = DateTime.Now.Ticks;
                CriteriaOperator criteria = CriteriaOperator.And(
                                                                    new BinaryOperator("Code", code),
                                                                    new BinaryOperator("IsActiveFrom", nowTicks, BinaryOperatorType.LessOrEqual),
                                                                    new BinaryOperator("IsActiveUntil", nowTicks, BinaryOperatorType.GreaterOrEqual)
                                                                 );
                criteria = RetailHelper.ApplyOwnerCriteria(criteria, typeof(Coupon), owner);
                Coupon coupon = uow.FindObject<Coupon>(criteria);
                if (coupon == null)
                {
                    return this.GetExistingUsableCouponByMask(deviceID, code, store, uow);
                }
                if (coupon.IsUnique && coupon.NumberOfTimesUsed > 0)
                {
                    return null;
                }

                return CreateCouponViewModel(coupon);
            }
        }

        private CouponViewModel CreateCouponViewModel(Coupon coupon)
        {
            CouponViewModel couponViewModel = new CouponViewModel()
            {
                Oid = coupon.Oid,
                Code = coupon.Code,
                Owner = coupon.Owner.Oid,
                Description = coupon.Description,
                IsActiveUntilDate = coupon.IsActiveUntilDate,
                IsActiveFromDate = coupon.IsActiveFromDate,
                NumberOfTimesUsed = coupon.NumberOfTimesUsed,
                IsUnique = coupon.IsUnique,
                CouponAppliesOn = coupon.CouponAppliesOn,
                CouponAmountType = coupon.CouponAmountType,
                CouponAmountIsAppliedAs = coupon.CouponAmountIsAppliedAs,
                Amount = coupon.Amount,
                DiscountType = coupon.DiscountType == null ? Guid.Empty : coupon.DiscountType.Oid,
                PaymentMethod = coupon.PaymentMethod == null ? Guid.Empty : coupon.PaymentMethod.Oid,
                CouponCategory = coupon.CouponCategory == null ? Guid.Empty : coupon.CouponCategory.Oid,
                CouponCategoryDescription = coupon.CouponCategory == null ? string.Empty : coupon.CouponCategory.Description,
                CouponMaskOid = coupon.CouponMask != null ? coupon.CouponMask.Oid : Guid.Empty
            };
            return couponViewModel;
        }

        private CouponViewModel CreateCouponViewModel(GeneratedCoupon generatedCoupon)
        {
            CouponViewModel couponViewModel = new CouponViewModel()
            {
                Oid = Guid.Empty,
                Code = generatedCoupon.Code,
                Owner = generatedCoupon.Owner.Oid,
                Description = generatedCoupon.Description,
                IsActiveUntilDate = generatedCoupon.CouponMask.IsActiveUntilDate,
                IsActiveFromDate = generatedCoupon.CouponMask.IsActiveFromDate,
                NumberOfTimesUsed = generatedCoupon.Status == GeneratedCouponStatus.Used ? 1 : 0,
                IsUnique = generatedCoupon.CouponMask.IsUnique,
                CouponAppliesOn = generatedCoupon.CouponMask.CouponAppliesOn,
                CouponAmountType = generatedCoupon.CouponMask.CouponAmountType,
                CouponAmountIsAppliedAs = generatedCoupon.CouponMask.CouponAmountIsAppliedAs,
                Amount = CouponHelper.GetMaskAmount(generatedCoupon.Code, generatedCoupon.CouponMask),
                DiscountType = generatedCoupon.CouponMask.DiscountType == null ? Guid.Empty : generatedCoupon.CouponMask.DiscountType.Oid,
                PaymentMethod = generatedCoupon.CouponMask.PaymentMethod == null ? Guid.Empty : generatedCoupon.CouponMask.PaymentMethod.Oid,
                CouponCategory = generatedCoupon.CouponMask.CouponCategory == null ? Guid.Empty : generatedCoupon.CouponMask.CouponCategory.Oid,
                CouponCategoryDescription = generatedCoupon.CouponMask.CouponCategory == null ? string.Empty : generatedCoupon.CouponMask.CouponCategory.Description,
                CouponMaskOid = generatedCoupon.CouponMask != null ? generatedCoupon.CouponMask.Oid : Guid.Empty
            };
            return couponViewModel;
        }

        private CouponViewModel GetExistingUsableCouponByMask(string deviceID, string code, Store store, UnitOfWork uow)
        {
            long nowTicks = DateTime.Now.Ticks;
            XPCollection<CouponMask> masks = new XPCollection<CouponMask>(uow, RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(
                                                                                    new BinaryOperator("IsActiveFrom", nowTicks, BinaryOperatorType.LessOrEqual),
                                                                                    new BinaryOperator("IsActiveUntil", nowTicks, BinaryOperatorType.GreaterOrEqual)
                ), typeof(CouponMask), store.Owner));
            CouponMask couponMask = masks.FirstOrDefault(mask => code.StartsWith(mask.Prefix) && code.Length == mask.Prefix.Length + mask.Mask.Length);
            if (couponMask == null)
            {
                return null;
            }

            decimal amount = CouponHelper.GetMaskAmount(code, couponMask);

            //In any case Create Coupon BUT save ONLY IF Necessary depending on the Application instance
            Coupon coupon = new Coupon(uow)
            {
                Owner = uow.GetObjectByKey<CompanyNew>(StoreControllerAppiSettings.Owner.Oid),
                CouponCategory = uow.GetObjectByKey<CouponCategory>(couponMask.CouponCategory.Oid),
                CouponMask = uow.GetObjectByKey<CouponMask>(couponMask.Oid),
                Amount = amount,
                Code = code,
                CouponAmountIsAppliedAs = couponMask.CouponAmountIsAppliedAs,
                CouponAmountType = couponMask.CouponAmountType,
                CouponAppliesOn = couponMask.CouponAppliesOn,
                Description = couponMask.Description,
                DiscountType = couponMask.DiscountType,
                IsActiveFrom = couponMask.IsActiveFrom,
                IsActiveUntil = couponMask.IsActiveUntil,
                IsUnique = true,
                NumberOfTimesUsed = 0,
                PaymentMethod = couponMask.PaymentMethod
            };
            switch (MvcApplication.ApplicationInstance)
            {
                case eApplicationInstance.DUAL_MODE:
                    coupon.Save();
                    uow.CommitChanges();
                    return CreateCouponViewModel(coupon);
                case eApplicationInstance.RETAIL:
                    coupon.Save();
                    uow.CommitChanges();
                    return CreateCouponViewModel(coupon);
                case eApplicationInstance.STORE_CONTROLER:

                    using (UnitOfWork unitOfWork = XpoHelper.GetNewUnitOfWork())
                    {

                        GeneratedCoupon generatedCoupon = unitOfWork.FindObject<GeneratedCoupon>(new BinaryOperator("Code", coupon.Code));

                        if (generatedCoupon == null)
                        {
                            Model.POS pos = unitOfWork.FindObject<Model.POS>(new BinaryOperator("ID", deviceID));
                            Guid device = StoreControllerAppiSettings.StoreControllerOid;//From Web
                            if (pos != null)//From POS
                            {
                                device = pos.Oid;
                            }
                            else//From SCDC
                            {
                                Guid.TryParse(deviceID, out device);
                            }

                            generatedCoupon = new GeneratedCoupon(unitOfWork)
                            {
                                Owner = unitOfWork.GetObjectByKey<CompanyNew>(StoreControllerAppiSettings.Owner.Oid),
                                Code = coupon.Code,
                                CouponMask = unitOfWork.GetObjectByKey<CouponMask>(coupon.CouponMask.Oid),
                                Status = GeneratedCouponStatus.Requested,
                                Device = device
                            };
                            generatedCoupon.Save();
                            XpoHelper.CommitChanges(unitOfWork);
                        }
                        return CreateCouponViewModel(generatedCoupon);
                    }
                default:
                    throw new NotSupportedException("Method GetExistingUsableCouponByMask");
            }
        }

        /// <summary>
        /// Updates the number of times a coupon has been used
        /// </summary>
        /// <param name="transactionCoupons">A list of used coupons</param>
        /// <param name="errorMessage">Is string.Empty if everything is ok, otherwise it contains an error.</param>
        /// <returns>True if coupons have been successfully updated, otherwise false. If false out parameter errorMessage contains the failure explanation.</returns>
        [WebMethod]
        public bool UpdateCoupons(List<TransactionCouponViewModel> transactionCoupons, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    foreach (TransactionCouponViewModel transactionCouponViewModel in transactionCoupons)
                    {
                        Coupon coupon = uow.GetObjectByKey<Coupon>(transactionCouponViewModel.Coupon);
                        coupon.NumberOfTimesUsed++;
                        coupon.Save();
                    }
                    XpoHelper.CommitChanges(uow);

                    //if (MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER)
                    //{
                    //    try
                    //    {

                    //    }
                    //    catch (Exception exception)
                    //    {
                    //        //NOP
                    //    }
                    //}

                    return true;
                }
            }
            catch (Exception exception)
            {
                errorMessage = exception.GetFullMessage();
                return false;
            }
        }


        [WebMethod]
        public string DirectPostNewDocument(string document, Guid posOid, eIdentifier identifier, Guid userId, out string errorMessage)
        {
            if (identifier == eIdentifier.POS)
            {
                bool headerIsSaved = false;
                string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
                DocumentHeader header = null;
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    try
                    {
                        User user = uow.GetObjectByKey<User>(userId);
                        if (user == null)
                        {
                            errorMessage = "Invalid User";
                            return null;
                        }
                        ITS.Retail.Model.POS pos = uow.GetObjectByKey<ITS.Retail.Model.POS>(posOid);
                        if (pos == null)
                        {
                            errorMessage = "Wrong POS";
                            return null;
                        }

                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                        header = new DocumentHeader(uow);

                        bool result = header.FromJson(document, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out errorMessage);


                        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);

                        if (result == false)
                        {
                            return null;
                        }
                        header.UpdateByDevice = header.CreatedByDevice;
                        header.CreatedByDevice = StoreControllerAppiSettings.StoreControllerOid.ToString();
                        if (header.DocumentType.TakesDigitalSignature)
                        {
                            header.Save();
                            XpoHelper.CommitChanges(uow);
                            headerIsSaved = true;

                            StoreControllerSettings settings = uow.GetObjectByKey<StoreControllerSettings>(StoreControllerAppiSettings.CurrentStore.StoreControllerSettings.Oid);
                            List<POSDevice> posDevices = settings.StoreControllerTerminalDeviceAssociations.
                                Where(x =>
                                        x.DocumentSeries.Any(y => y.DocumentSeries.Oid == header.DocumentSeries.Oid)
                                     && x.TerminalDevice is POSDevice
                                     && (x.TerminalDevice as POSDevice).DeviceSettings.DeviceType == DeviceType.DiSign
                                ).Select(x => x.TerminalDevice).Cast<POSDevice>().ToList();
                            string signature = DocumentHelper.SignDocument(header, user, header.Owner, MvcApplication.OLAPConnectionString, posDevices);
                            if (string.IsNullOrWhiteSpace(signature))
                            {
                                errorMessage = "Document Saved. Digital Signature is missing";
                            }
                            header.Signature = signature;
                            header.Save();
                        }

                        XpoHelper.CommitChanges(uow);
                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                        string headerToJson = header.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
                        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);

                        StoreDocumentSeriesType storeDocumentSeriesType = header.DocumentType
                                                                          .StoreDocumentSeriesTypes
                                                                          .FirstOrDefault(storeDocSeriesType => storeDocSeriesType.DocumentSeries.Oid == header.DocumentSeries.Oid);

                        POSDevice remotePrinterService = storeDocumentSeriesType.PrintServiceSettings == null ? null : storeDocumentSeriesType.PrintServiceSettings.RemotePrinterService;
                        if (remotePrinterService == null || remotePrinterService.IsActive == false)
                        {
                            errorMessage = String.Format("{0}: {1}", ResourcesLib.Resources.InvalidValue, ResourcesLib.Resources.RemotePrinterService);
                            return headerToJson;
                        }
                        else
                        {
                            PrintServerPrintDocumentResponse response = PrinterServiceHelper.PrintDocument(remotePrinterService, userId, header.Oid, header.POSID, storeDocumentSeriesType.PrintServiceSettings.PrinterNickName);
                            if (response == null)
                            {
                                errorMessage = Resources.CouldNotEstablishConnection + " Remote Print Service :" + remotePrinterService.Name;
                                return headerToJson;
                            }
                            switch (response.Result)
                            {
                                case ePrintServerResponseType.FAILURE:
                                    errorMessage = response.Explanation;
                                    return headerToJson;
                                case ePrintServerResponseType.SUCCESS:
                                    return headerToJson;
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                        string headerToJson = header.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
                        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                        return headerIsSaved ? headerToJson : null;
                    }
                }
            }
            else
            {
                errorMessage = "Wrong Source";
            }
            return null;
        }

        /// <summary>
        /// Returns a string that is a zip List<Guid> 
        /// of objects of type T that have NOT been transfered         
        /// from device (POS or SC) that calls this method
        /// to SC or HQ running this web service
        /// </summary>
        /// <param name="expectedTransferedObjects">The ids (in zipped fromat) of objects of type 'type' that should have been transfered and need verification</param>
        /// <param name="type">The type of objects.It should be a subclass of BasicObj</param>
        /// <returns>a string that is a zip List<Guid> of objects of type 'type'
        /// that have NOT been transfered from device (POS or SC) that calls this method
        /// to SC or HQ running this web service.</returns>
        [WebMethod]
        public string GetNotTransferedObjects(string expectedTransferedObjects, string type)
        {
            Type basicObjectType = Assembly.GetAssembly(typeof(BasicObj)).GetType(type);
            if (string.IsNullOrEmpty(expectedTransferedObjects))
            {
                throw new Exception(Resources.NoObjectsHaveBeenProvidedToCheckTheirTransferStatus);
            }
            List<ObjectSignature> expectedTransferedObjectsOids = JsonConvert.DeserializeObject<List<ObjectSignature>>(ZipLZMA.DecompressLZMA(expectedTransferedObjects) as string);
            List<Guid> dataToRetransfer = new List<Guid>();
            if (expectedTransferedObjectsOids.Count > 0)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    expectedTransferedObjectsOids.ForEach(expectedObject =>
                    {
                        BasicObj basicObject = uow.GetObjectByKey(basicObjectType, expectedObject.Oid) as BasicObj;
                        if (basicObject == null || (basicObject.ObjectSignature != expectedObject.Hash && basicObject.UpdatedOnTicks < expectedObject.UpdatedOnTicks))
                        {
                            dataToRetransfer.Add(expectedObject.Oid);
                        }
                    });
                }
            }
            return ZipLZMA.CompressLZMA(JsonConvert.SerializeObject(dataToRetransfer, PlatformConstants.JSON_SERIALIZER_SETTINGS)).ToString();
        }

        [WebMethod]
        [LogExtension]
        public string GetReportSettingsXml(int posid)
        {
            try
            {
                XmlDocument xml = CreateReportSettingsXmlFile(posid);
                return xml.InnerXml;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private XmlDocument CreateReportSettingsXmlFile(int posid)
        {
            XmlDocument xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                ITS.Retail.Model.POS pos = uow.FindObject<ITS.Retail.Model.POS>(new BinaryOperator("ID", posid));
                if (pos == null)
                {
                    XmlNode errorNode = xml.CreateElement("Error");
                    errorNode.InnerText = Resources.POSIdNotFound;
                    xml.AppendChild(errorNode);
                    return xml;
                }
                XmlNode settingsNode = xml.CreateElement("Settings");
                pos.POSReportSettings.ToList().ForEach(reportSettings =>
                {
                    XmlNode reportSettingsNode = xml.CreateElement("ReportSetting");
                    XmlNode documentTypeNode = xml.CreateElement("DocumentType");
                    documentTypeNode.InnerText = reportSettings.DocumentType.Oid.ToString();
                    reportSettingsNode.AppendChild(documentTypeNode);
                    XmlNode printerNode = xml.CreateElement("Printer");
                    printerNode.InnerText = reportSettings.Printer.Name;
                    reportSettingsNode.AppendChild(printerNode);
                    XmlNode xmlCustomReportNode = xml.CreateElement("CustomReport");
                    xmlCustomReportNode.InnerText = reportSettings.Report == null ? Guid.Empty.ToString() : reportSettings.Report.Oid.ToString();
                    reportSettingsNode.AppendChild(xmlCustomReportNode);
                    XmlNode xmlPrintFormatNode = xml.CreateElement("XMLPrintFormat");
                    if (reportSettings.PrintFormat != null)
                    {
                        xmlPrintFormatNode.InnerXml = reportSettings.PrintFormat.Oid.ToString();
                        reportSettingsNode.AppendChild(xmlPrintFormatNode);
                        // XmlDocument printFormatXml = new XmlDocument();
                        //printFormatXml.LoadXml(reportSettings.PrintFormat.Format);
                        // XmlNode format = printFormatXml.GetElementsByTagName("ReceiptFormat").Item(0);
                        //xml.ImportNode(format, true);
                        //xmlPrintFormatNode.AppendChild(format);
                        //xmlPrintFormatNode.InnerXml = format.OuterXml;
                    }

                    settingsNode.AppendChild(reportSettingsNode);
                });
                xml.AppendChild(settingsNode);
            }
            return xml;
        }


        [WebMethod]
        [LogExtension]
        public Byte[] GetPOSReportLibrary(int posid)
        {
            return GetDllLibrary(posid, Server.MapPath("~/bin/ITS.POS.Reports.dll"));
        }

        private byte[] GetDllLibrary(int posid, string dllPath)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    ITS.Retail.Model.POS pos = uow.FindObject<ITS.Retail.Model.POS>(new BinaryOperator("ID", posid));
                    if (pos == null)
                    {
                        return GetEnumBytes(eServiceResponce.INVALID_INPUT);
                    }
                    if (pos.POSReportSettings.Count > 0)
                    {
                        byte[] tempDllBytes = File.ReadAllBytes(dllPath);
                        return tempDllBytes;
                    }
                    return GetEnumBytes(eServiceResponce.EMPTY_RESPONCE);
                }
            }
            catch (Exception exception)
            {
                MvcApplication.WRMLogModule.Log(exception, kernelLogLevel: KernelLogLevel.Error);
                return GetEnumBytes(eServiceResponce.EXCEPTION_HAS_BEEN_THROWN);
            }
        }

        [WebMethod]
        [LogExtension]
        public Byte[] GetITSRetailCommonLibrary(int posid)
        {
            return GetDllLibrary(posid, Server.MapPath("~/bin/ITS.Retail.Common.dll"));
        }

#if _RETAIL_WEBCLIENT
        /// <summary>
        /// Available ONLY on HQ. Called by SC to retrieve objects for transfer verification
        /// </summary>
        /// <param name="type">The fullname of obejct type (BasicObj)</param>
        /// <param name="storeController"></param>
        /// <returns></returns>
        [WebMethod]
        public string GetObjectsForVerification(string type,Guid storeController)
        {
            List<ObjectSignature> objectsToBeVerified = new List<ObjectSignature>();


            return ZipLZMA.CompressLZMA(JsonConvert.SerializeObject(objectsToBeVerified, PlatformConstants.JSON_SERIALIZER_SETTINGS)).ToString();
        }
#endif
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Hardware;
using System.IO;
using ITS.POS.Model.Transactions;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Helpers;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware.Common;
using System.Reflection;
using ITS.POS.Client.Exceptions;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    ///  For ADHME only. Signs (if required) and prints the current document to the primary Printer
    /// </summary>
    public class ActionPrintReceipt : Action
    {
        public ActionPrintReceipt(IPosKernel kernel)
            : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.PRINT_RECEIPT; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT | eMachineStatus.DAYSTARTED | eMachineStatus.CLOSED | eMachineStatus.SALE; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions = false)
        {
            Printer printer = (parameters as ActionPrintReceiptParams).Printer;
            bool cutPaper = (parameters as ActionPrintReceiptParams).CutPaper;
            DocumentHeader documentHeader = (parameters as ActionPrintReceiptParams).DocumentHeader;
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IReceiptBuilder receiptBuilder = Kernel.GetModule<IReceiptBuilder>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IDocumentService documentService = Kernel.GetModule<IDocumentService>();
            IActionManager actionManager = Kernel.GetModule<IActionManager>();

            if (printer == null)
            {
                throw new Exception(POSClientResources.ERROR_PRINT_RECEIPT + ". " + POSClientResources.PRINTER_NULL);
                //"Error Printing Receipt. Printer is null.");
            }

            if (printer.ConType != ConnectionType.NONE)
            {

                DeviceResult beginTranResult = printer.BeginTransaction();
                Kernel.LogFile.Debug(DateTime.Now + "  Print Receipt: End BeginTransaction");
                if (beginTranResult != DeviceResult.SUCCESS)
                {
                    string message = Resources.POSClientResources.PRINTER_FAILURE + " : " + beginTranResult.ToLocalizedString();//Enum.GetName(typeof(DeviceResult), beginTranResult);
                    printer.EndTransaction();
                    throw new Exception(message);
                }




                Kernel.LogFile.Debug(DateTime.Now + "  Print Receipt: End Test Printer");
                List<string> allTheLines = new List<string>();
                Receipt.Receipt receipt = new Receipt.Receipt();
                Receipt.ReceiptSchema receiptSchema = config.GetReceiptSchema(documentHeader);
                receipt.Header = receiptBuilder.CreateReceiptHeader(receiptSchema, documentHeader, printer.Settings.LineChars, printer.ConType);
                if (documentHeader.InEmulationMode && receipt.Header.Count > 0)
                {
                    ////Inject illegal warning
                    receipt.Header.InsertRange(0, receiptBuilder.CreateSimplePrinterLines(eAlignment.CENTER, printer, false,
                                                            POSClientResources.ILLEGAL_RECEIPT_TRAINING_MODE_MESSAGE,
                                                            POSClientResources.PUNISHABLE_BY_LAW_MESSAGE,
                                                            POSClientResources.IS_NOT_VALID_SALES_DOCUMENT_MESSAGE));
                }

                receipt.Body = receiptBuilder.CreateReceiptBody(receiptSchema, documentHeader, printer.Settings.LineChars, printer.ConType);

                if (documentHeader.InEmulationMode)
                {
                    ////Inject illegal warning
                    List<string> injectionMessage = receiptBuilder.CreateSimplePrinterLines(eAlignment.CENTER, printer, false,
                                                                    POSClientResources.ILLEGAL_RECEIPT_TRAINING_MODE_MESSAGE,
                                                                    POSClientResources.PUNISHABLE_BY_LAW_MESSAGE);
                    if (receipt.Body.Count > 4)
                    {
                        List<string> copyOfBody = receipt.Body.ToList();
                        int injectionCounter = 0;

                        int injectionOffset = injectionMessage.Count;
                        for (int i = 4; i < receipt.Body.Count; i += 5)
                        {
                            copyOfBody.InsertRange(i + (injectionCounter * injectionOffset), injectionMessage);
                            injectionCounter++;
                        }
                        receipt.Body = copyOfBody;
                    }
                    else
                    {
                        receipt.Body.InsertRange(receipt.Body.Count, injectionMessage);
                    }
                }

                receipt.Footer = receiptBuilder.CreateReceiptFooter(receiptSchema, documentHeader, printer.Settings.LineChars, printer.ConType);
                if (documentHeader.InEmulationMode)
                {
                    ////Inject illegal warning
                    receipt.Footer.AddRange(receiptBuilder.CreateSimplePrinterLines(eAlignment.CENTER, printer, false,
                                                            POSClientResources.ILLEGAL_RECEIPT_TRAINING_MODE_MESSAGE,
                                                            POSClientResources.PUNISHABLE_BY_LAW_MESSAGE,
                                                            POSClientResources.IS_NOT_VALID_SALES_DOCUMENT_MESSAGE));
                }

                allTheLines = receipt.GetReceiptLines();

                Kernel.LogFile.Debug(DateTime.Now + "  Print Receipt: End GetReceiptLines");
                string signature = "";
                //Give Signature
                DocumentType documentType = sessionManager.GetObjectByKey<DocumentType>(documentHeader.DocumentType);
                if (documentType.TakesDigitalSignature && string.IsNullOrWhiteSpace(documentHeader.Signature))
                {

                    if (eFiscalMethod.EAFDSS == config.FiscalMethod)
                    {

                        Model.Settings.POS currentTerminal = sessionManager.GetSession<Model.Settings.POS>().GetObjectByKey<Model.Settings.POS>(config.CurrentTerminalOid);
                        if (currentTerminal == null)
                        {
                            throw new POSException(Resources.POSClientResources.INVALID_TERMINAL_OID);
                        }

                        if (currentTerminal.IsStandaloneFiscalOnError)
                        {
                            if (String.IsNullOrWhiteSpace(currentTerminal.StandaloneFiscalOnErrorMessage))
                            {
                                throw new POSException(Resources.POSClientResources.STANDALONE_FISCAL_ON_ERROR_MESSAGE_IS_EMPTY);
                            }
                            signature = currentTerminal.StandaloneFiscalOnErrorMessage;
                        }
                        else
                        {
                            string tempFile = Path.GetTempFileName();
                            using (StreamWriter wr = new StreamWriter(tempFile, false, Encoding.GetEncoding(1253)))
                            {
                                foreach (string line in allTheLines)
                                {
                                    wr.WriteLine(line);
                                }
                            }

                            Kernel.LogFile.Debug(String.Format("Start get signature of receipt {0} - {1}", documentHeader.DocumentSeriesCode, documentHeader.DocumentNumber));
                            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();

                            //Device fiscalDevice = deviceManager.GetEAFDSSDevice(config.FiscalDevice);
                            List<Device> fiscalDevices = deviceManager.GetEAFDSSDevicesByPriority(config.FiscalDevice);
                            if (fiscalDevices == null || fiscalDevices.Count == 0)
                            {
                                throw new POSException(POSClientResources.NO_ELECTRONIC_SIGNATURE_DEVICE_FOUND);
                            }
                            List<string> exceptionsThrown = new List<string>();
                            foreach (Device fiscalDevice in fiscalDevices)
                            {
                                Kernel.LogFile.Trace("Trying to sign using " + fiscalDevice.DeviceName);
                                try
                                {
                                    switch (config.FiscalDevice)
                                    {
                                        case eFiscalDevice.DATASIGN:
                                            DataSignESD defaultDataSign = fiscalDevice as DataSignESD;
                                            DataSignESD.DataSignResult datasignResult = defaultDataSign.SignDocument(config.ABCDirectory, tempFile, ref signature);
                                            if (datasignResult != DataSignESD.DataSignResult.ERR_SUCCESS)
                                            {
                                                string message = "Error Signing Receipt: " + datasignResult.ToString();
                                                throw new Exception(message);
                                            }
                                            break;
                                        case eFiscalDevice.ALGOBOX_NET:
                                            AlgoboxNetESD defaultAlgobox = fiscalDevice as AlgoboxNetESD;
                                            AlgoboxNetESD.AlgoboxNetResult algoboxResult = defaultAlgobox.SignDocument(tempFile, documentService.CreateFiscalInfoLine(documentHeader, Kernel.LogFile), ref signature);
                                            if (algoboxResult != AlgoboxNetESD.AlgoboxNetResult.SUCCESS)
                                            {
                                                string message = "Error Signing Receipt: " + signature.ToString();
                                                throw new Exception(message);
                                            }
                                            break;
                                        case eFiscalDevice.DISIGN:
                                            DiSign defaultDisign = fiscalDevice as DiSign;
                                            int result;
                                            int maxNumberOfRetries = 3;
                                            int currentTry = 0;
                                            do
                                            {
                                                currentTry++;
                                                result = defaultDisign.SignDocument(allTheLines.Aggregate((f, s) => f + Environment.NewLine + s), documentService.CreateFiscalInfoLine(documentHeader, Kernel.LogFile), ref signature);
                                            } while( result !=0 && currentTry < maxNumberOfRetries );

                                            if (result == -4)//eFiscalResponseType.FISCAL_MUST_ISSUE_Z
                                            {
                                                actionManager.GetAction(eActions.SHOW_BLINKING_ERROR).Execute(new ActionShowBlinkingErrorParams(false));
                                                throw new POSException(Resources.POSClientResources.YOU_MUST_ISSUE_Z);
                                            }
                                            if (result == -2)//eFiscalResponseType.FISCAL_IS_ON_ERROR
                                            {
                                                actionManager.GetAction(eActions.SHOW_BLINKING_ERROR).Execute(new ActionShowBlinkingErrorParams(true));
                                            }
                                            else if (result != 0)
                                            {   
                                                string message = "Error Signing Receipt. Error Code: " + result + " Message: " + signature.ToString();
                                                throw new Exception(message);
                                            }
                                            else
                                            {
                                                actionManager.GetAction(eActions.SHOW_BLINKING_ERROR).Execute(new ActionShowBlinkingErrorParams(false));
                                            }
                                            break;
                                        default:
                                            throw new POSException(POSClientResources.INVALID_FISCAL_DEVICE);
                                    }
                                    break;
                                }
                                catch (POSException posException)
                                {
                                    Kernel.LogFile.Error(posException, "Unable to sign using " + fiscalDevice.DeviceName);
                                    throw;
                                }
                                catch(Exception ex)
                                {
                                    fiscalDevice.FailureCount++;
                                    Kernel.LogFile.Info(ex, "Unable to sign using " + fiscalDevice.DeviceName);
                                    exceptionsThrown.Add( fiscalDevice.DeviceName  + " : " + ex.GetFullMessage());
                                }                               
                            }
                            if (File.Exists(tempFile))
                            {
                                File.Delete(tempFile);
                            }
                            if(exceptionsThrown.Count >= fiscalDevices.Count)
                            {
                                string fiscalDevicesErrors = string.Empty;
                                exceptionsThrown.ForEach(exception => {
                                    fiscalDevicesErrors += Environment.NewLine + exception + Environment.NewLine;
                                });
                                throw new POSException( string.Format( "Cannot sign the receipt after trying with all fiscal devices. {0}", fiscalDevicesErrors));
                            }
                            Kernel.LogFile.Debug(String.Format("End get signature of receipt {0} - {1}", documentHeader.DocumentSeriesCode, documentHeader.DocumentNumber));
                        }
                    }
                    documentHeader.Signature = signature;
                }
                else
                {
                    signature = documentHeader.Signature;
                }

                if (documentHeader.InEmulationMode)
                {
                    allTheLines.Add(Environment.NewLine + Environment.NewLine + receiptBuilder.CreateSimplePrinterLines(eAlignment.CENTER, printer, false, signature).FirstOrDefault()
                                   + Environment.NewLine);
                }
                else
                {
                    allTheLines.Add(Environment.NewLine + Environment.NewLine + signature + Environment.NewLine);
                }

                if (cutPaper && printer.ConType == ConnectionType.OPOS && printer.SupportsCutter)
                {
                    allTheLines.Add(ITS.POS.Hardware.OPOSDriverCommands.PrinterCommands.OneShotCommands.FeedAndPaperCut());
                }
                Kernel.LogFile.Debug(DateTime.Now + " Print Receipt: Start Send Print");
                DeviceResult printResult = printer.PrintLines(allTheLines.ToArray());
                Kernel.LogFile.Debug(DateTime.Now + " Print Receipt: End Send Print");

                if (printResult != DeviceResult.SUCCESS)
                {
                    string message = POSClientResources.PRINTER_FAILURE + ": " + printResult.ToLocalizedString();//Enum.GetName(typeof(DeviceResult), printResult);
                    printer.EndTransaction();
                    throw new Exception(message);
                }
                Kernel.LogFile.Debug(DateTime.Now + " Print Receipt: Start EndTransaction");
                printer.EndTransaction();
                Kernel.LogFile.Debug(DateTime.Now + " Print Receipt: End EndTransaction");
            }
        }

    }
}

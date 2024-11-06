using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Receipt;
using ITS.POS.Hardware;
using ITS.POS.Hardware.Common;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Service for generating the text of non fiscal prints
    /// </summary>
    public class ReceiptBuilder : IReceiptBuilder
    {
        ISessionManager SessionManager { get; set; }
        IConfigurationManager ConfigurationManager { get; set; }
        Logger LogFile { get; set; }
        IDocumentService DocumentService { get; set; }
        IPlatformRoundingHandler PlatformRoundingHandler { get; set; }

        IPosKernel Kernel { get; set; }


        public ReceiptBuilder(ISessionManager sessionManager, IConfigurationManager config, IDocumentService documentService, IPlatformRoundingHandler platformRoundingHandler, Logger logFile, IPosKernel kernel)
        {
            this.SessionManager = sessionManager;
            this.ConfigurationManager = config;
            this.DocumentService = documentService;
            this.PlatformRoundingHandler = platformRoundingHandler;
            this.LogFile = logFile;
            this.Kernel = kernel;
        }

        public List<string> CreateReceiptHeader(ReceiptSchema receiptSchema, object source, int lineChars, ConnectionType printerConnectionType)
        {
            return CreateReceiptPart(receiptSchema.ReceiptHeader, source, lineChars, printerConnectionType);
        }

        public List<string> CreateReceiptFooter(ReceiptSchema receiptSchema, object source, int lineChars, ConnectionType printerConnectionType)
        {
            return CreateReceiptPart(receiptSchema.ReceiptFooter, source, lineChars, printerConnectionType);
        }

        public List<string> CreateReceiptBody(ReceiptSchema receiptSchema, object source, int lineChars, ConnectionType printerConnectionType)
        {
            return CreateReceiptPart(receiptSchema.ReceiptBody, source, lineChars, printerConnectionType);
        }

        public List<string> CreateDiscountPrintingLines(DocumentHeader header, int lineChars)
        {
            ReceiptSchema schema = new ReceiptSchema();
            ReceiptPart body = new ReceiptPart();
            bool nothingToShow = true;
            List<DocumentDetail> detailsToShow = header.DocumentDetails.Where(x => x.TotalNonDocumentDiscount > 0).ToList();
            if (detailsToShow.Count > 0)
            {
                nothingToShow = false;

                string title = "-" + Resources.POSClientResources.LINE_DISCOUNTS + "-";
                string horizontalLine = this.BuildHorizontalLine(title.Length);

                body.Elements.Add(new SimpleLine(horizontalLine) { LineAlignment = eAlignment.CENTER });
                body.Elements.Add(new SimpleLine(title) { LineAlignment = eAlignment.CENTER });
                body.Elements.Add(new SimpleLine(horizontalLine) { LineAlignment = eAlignment.CENTER });
                foreach (DocumentDetail detail in detailsToShow)
                {
                    bool itemPrinted = false;
                    foreach (DocumentDetailDiscount discount in detail.DocumentDetailDiscounts
                                                                .Where(x =>
                                                                    x.DiscountSource != eDiscountSource.DOCUMENT
                                                                    && x.DiscountSource != eDiscountSource.PRICE_CATALOG
                                                                     && x.DiscountSource != eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT))
                    {

                        switch (discount.DiscountSource)
                        {
                            case eDiscountSource.PROMOTION_LINE_DISCOUNT:
                                {
                                    if (!itemPrinted)
                                    {
                                        DynamicLine dynamicLine1 = new DynamicLine();
                                        dynamicLine1.Cells.Add(new DynamicLineCell()
                                        {
                                            CellAlignment = eAlignment.LEFT,
                                            Content = discount.DocumentDetail.ItemName
                                        });
                                        body.Elements.Add(dynamicLine1);
                                        itemPrinted = true;
                                    }
                                    DynamicLine dynamicLine = new DynamicLine();
                                    dynamicLine.Cells.Add(new DynamicLineCell()
                                    {
                                        CellAlignment = eAlignment.LEFT,
                                        Content = discount.DisplayName.ToUpperGR()
                                    });
                                    dynamicLine.Cells.Add(new DynamicLineCell()
                                    {
                                        CellAlignment = eAlignment.RIGHT,
                                        Content = discount.Value.ToString("0.00 €")
                                    });
                                    body.Elements.Add(dynamicLine);
                                    break;
                                }
                            case eDiscountSource.CUSTOM:
                                {
                                    if (!itemPrinted)
                                    {
                                        DynamicLine dynamicLine1 = new DynamicLine();
                                        dynamicLine1.Cells.Add(new DynamicLineCell()
                                        {
                                            CellAlignment = eAlignment.LEFT,
                                            Content = discount.DocumentDetail.ItemName
                                        });
                                        body.Elements.Add(dynamicLine1); itemPrinted = true;
                                    }
                                    DynamicLine dynamicLine = new DynamicLine();
                                    dynamicLine.Cells.Add(new DynamicLineCell()
                                    {
                                        CellAlignment = eAlignment.LEFT,
                                        Content = POSClientResources.CUSTOM_DISCOUNT.ToUpperGR()
                                    });
                                    dynamicLine.Cells.Add(new DynamicLineCell()
                                    {
                                        CellAlignment = eAlignment.RIGHT,
                                        Content = discount.Value.ToString("0.00 €")
                                    });
                                    body.Elements.Add(dynamicLine);
                                    break;
                                }
                        }
                    }
                }

            }
            if (header.DocumentDiscountAmount > 0 || header.PointsDiscountAmount > 0 || header.PromotionsDiscountAmount > 0
                                                                                     || header.CustomerDiscountAmount > 0 || header.DefaultDocumentDiscountAmount > 0)
            {
                nothingToShow = false;

                string title = "-" + Resources.POSClientResources.DOCUMENT_DISCOUNTS + "-";
                string horizontalLine = this.BuildHorizontalLine(title.Length);
                body.Elements.Add(new SimpleLine(horizontalLine) { LineAlignment = eAlignment.CENTER });
                body.Elements.Add(new SimpleLine(title) { LineAlignment = eAlignment.CENTER });
                body.Elements.Add(new SimpleLine(horizontalLine) { LineAlignment = eAlignment.CENTER });
                if (header.DocumentDiscountAmount > 0)
                {
                    DynamicLine dynamicLine = new DynamicLine();
                    dynamicLine.Cells.Add(new DynamicLineCell()
                    {
                        CellAlignment = eAlignment.LEFT,
                        Content = POSClientResources.CUSTOM_DISCOUNT.ToUpperGR()
                    });
                    dynamicLine.Cells.Add(new DynamicLineCell()
                    {
                        CellAlignment = eAlignment.RIGHT,
                        Content = header.DocumentDiscountAmount.ToString("0.00 €")
                    });
                    body.Elements.Add(dynamicLine);
                }
                if (header.PointsDiscountAmount > 0)
                {
                    DynamicLine dynamicLine = new DynamicLine();
                    dynamicLine.Cells.Add(new DynamicLineCell()
                    {
                        CellAlignment = eAlignment.LEFT,
                        Content = POSClientResources.POINTS_DISCOUNT.ToUpperGR()
                    });
                    dynamicLine.Cells.Add(new DynamicLineCell()
                    {
                        CellAlignment = eAlignment.RIGHT,
                        Content = header.PointsDiscountAmount.ToString("0.00 €")
                    });
                    body.Elements.Add(dynamicLine);
                }
                if (header.PromotionsDiscountAmount > 0)
                {
                    DynamicLine dynamicLine = new DynamicLine();
                    dynamicLine.Cells.Add(new DynamicLineCell()
                    {
                        CellAlignment = eAlignment.LEFT,
                        Content = POSClientResources.PROMOTION_DISCOUNT.ToUpperGR()
                    });
                    dynamicLine.Cells.Add(new DynamicLineCell()
                    {
                        CellAlignment = eAlignment.RIGHT,
                        Content = header.PromotionsDiscountAmount.ToString("0.00 €")
                    });
                    body.Elements.Add(dynamicLine);
                }
                if (header.CustomerDiscountAmount > 0)
                {
                    DynamicLine dynamicLine = new DynamicLine();
                    dynamicLine.Cells.Add(new DynamicLineCell()
                    {
                        CellAlignment = eAlignment.LEFT,
                        Content = POSClientResources.CUSTOMER.ToUpperGR()
                    });
                    dynamicLine.Cells.Add(new DynamicLineCell()
                    {
                        CellAlignment = eAlignment.RIGHT,
                        Content = header.CustomerDiscountAmount.ToString("0.00 €")
                    });
                    body.Elements.Add(dynamicLine);
                }
                if (header.DefaultDocumentDiscountAmount > 0)
                {
                    DynamicLine dynamicLine = new DynamicLine();
                    dynamicLine.Cells.Add(new DynamicLineCell()
                    {
                        CellAlignment = eAlignment.LEFT,
                        Content = POSClientResources.DEFAULT_DOCUMENT_DISCOUNT.ToUpperGR()
                    });
                    dynamicLine.Cells.Add(new DynamicLineCell()
                    {
                        CellAlignment = eAlignment.RIGHT,
                        Content = header.DefaultDocumentDiscountAmount.ToString("0.00 €")
                    });
                    body.Elements.Add(dynamicLine);
                }
            }

            schema.ReceiptHeader = body;

            if (nothingToShow)
            {
                return new List<string>();
            }
            else
            {
                List<string> result = this.CreateReceiptHeader(schema, null, lineChars, ConnectionType.COM);
                return result;

            }
        }

        public List<string> CreatePointsPrintingLines(bool showTotals, int newPoints, int previousTotalPoints,
            int subtractedPoints, int lineChars, params string[] prefixLines)
        {
            ReceiptSchema schema = new ReceiptSchema();
            ReceiptPart header = new ReceiptPart();
            bool nothingToShow = true;
            if (prefixLines != null)
            {
                foreach (string line in prefixLines)
                {
                    SimpleLine prefixSimpleLine = new SimpleLine(line);
                    prefixSimpleLine.LineAlignment = eAlignment.CENTER;
                    header.Elements.Add(prefixSimpleLine);
                }
            }


            int currentTotalPointsBeforeSubstraction = previousTotalPoints + newPoints;
            int currentTotalPoints = currentTotalPointsBeforeSubstraction - subtractedPoints;

            string horizontalLine = this.BuildHorizontalLine(lineChars);

            header.Elements.Add(new SimpleLine(horizontalLine));

            if (showTotals && previousTotalPoints > 0)
            {
                nothingToShow = false;
                DynamicLine dynamicLine = new DynamicLine();
                dynamicLine.Cells.Add(new DynamicLineCell() { CellAlignment = eAlignment.LEFT, Content = POSClientResources.PREVIOUS_TOTAL_POINTS.ToUpperGR() });
                dynamicLine.Cells.Add(new DynamicLineCell() { CellAlignment = eAlignment.RIGHT, Content = previousTotalPoints.ToString() });
                dynamicLine.LineAlignment = eAlignment.CENTER;
                header.Elements.Add(dynamicLine);
            }

            if (newPoints > 0)
            {
                nothingToShow = false;
                DynamicLine dynamicLine = new DynamicLine();
                dynamicLine.Cells.Add(new DynamicLineCell() { CellAlignment = eAlignment.LEFT, Content = POSClientResources.POINTS_OF_TRANSACTION.ToUpperGR() });
                dynamicLine.Cells.Add(new DynamicLineCell() { CellAlignment = eAlignment.RIGHT, Content = newPoints.ToString() });
                dynamicLine.LineAlignment = eAlignment.CENTER;
                header.Elements.Add(dynamicLine);
            }

            if (subtractedPoints > 0)
            {
                nothingToShow = false;
                DynamicLine dynamicLine = new DynamicLine();
                dynamicLine.Cells.Add(new DynamicLineCell() { CellAlignment = eAlignment.LEFT, Content = POSClientResources.CONSUMED_POINTS.ToUpperGR() });
                dynamicLine.Cells.Add(new DynamicLineCell() { CellAlignment = eAlignment.RIGHT, Content = "-" + subtractedPoints.ToString() });
                dynamicLine.LineAlignment = eAlignment.CENTER;
                header.Elements.Add(dynamicLine);
            }

            if (showTotals && currentTotalPoints > 0)
            {
                nothingToShow = false;
                header.Elements.Add(new SimpleLine(horizontalLine));
                DynamicLine dynamicLine = new DynamicLine();
                dynamicLine.Cells.Add(new DynamicLineCell() { CellAlignment = eAlignment.LEFT, Content = POSClientResources.CURRENT_TOTAL.ToUpperGR() });
                dynamicLine.Cells.Add(new DynamicLineCell() { CellAlignment = eAlignment.RIGHT, Content = currentTotalPoints.ToString() });
                dynamicLine.LineAlignment = eAlignment.CENTER;
                header.Elements.Add(dynamicLine);
                header.Elements.Add(new SimpleLine(horizontalLine));
            }

            schema.ReceiptHeader = header;


            if (nothingToShow)
            {
                return new List<string>();
            }
            else
            {
                List<string> result = this.CreateReceiptHeader(schema, null, lineChars, ConnectionType.COM);
                return result;

            }
        }

        private string BuildHorizontalLine(int lineChars)
        {
            StringBuilder horizontalLineBuilder = new StringBuilder();
            if (lineChars > 0)
            {
                for (int i = 0; i < lineChars; i++)
                {
                    horizontalLineBuilder.Append("-");
                }
            }
            else
            {
                horizontalLineBuilder.Append("----------");
            }
            return horizontalLineBuilder.ToString();
        }

        public List<string> CreateSimplePrinterLines(eAlignment allignment, Device printer, bool addCutPaperCommand, params string[] lines)
        {
            ReceiptSchema schema = new ReceiptSchema();
            ReceiptPart header = new ReceiptPart();
            foreach (string line in lines)
            {
                SimpleLine simleLine = new SimpleLine(line);
                simleLine.LineAlignment = allignment;
                header.Elements.Add(simleLine);
            }

            schema.ReceiptHeader = header;
            List<string> result = this.CreateReceiptHeader(schema, null, printer.Settings.LineChars, printer.ConType);
            if (addCutPaperCommand && printer.ConType == ConnectionType.OPOS && printer is Printer && (printer as Printer).SupportsCutter)
            {
                result.Add(Hardware.OPOSDriverCommands.PrinterCommands.OneShotCommands.FeedAndPaperCut());
            }

            return result;
        }

        public List<string> CreateFiscalVersionLines(string fiscalVersion, int lineChars, params string[] prefixLines)
        {
            ReceiptSchema schema = new ReceiptSchema();
            ReceiptPart header = new ReceiptPart();
            if (prefixLines != null)
            {
                foreach (string line in prefixLines)
                {
                    SimpleLine prefixSimpleLine = new SimpleLine(line);
                    prefixSimpleLine.LineAlignment = eAlignment.CENTER;
                    header.Elements.Add(prefixSimpleLine);
                }
            }

            SimpleLine versionLine = new SimpleLine("v" + fiscalVersion);
            versionLine.LineAlignment = eAlignment.CENTER;
            header.Elements.Add(versionLine);
            schema.ReceiptHeader = header;

            List<string> result = this.CreateReceiptHeader(schema, null, lineChars, ConnectionType.COM);

            return result;
        }

        public List<string> CreateWithdrawOrDepositLines(Device printer, User currentUser, string headerLine, decimal value, string store, IReceiptBuilder receiptBuilder, IConfigurationManager config, Reason reason = null, string comments = null, string userCode = null)
        {
            List<string> lines = new List<string>();

            Receipt.Receipt receipt = new Receipt.Receipt();
            receipt.Header = receiptBuilder.CreateReceiptHeader(config.ΧReportSchema, null, printer.Settings.LineChars, printer.ConType);
            lines.AddRange(receipt.Header);
            lines.RemoveAll(x => x.Contains("INVALID_PROPERTY"));
            lines.RemoveRange(lines.IndexOf(lines.Last()) - 1, 1);
            lines.AddRange(this.CreateSimplePrinterLines(eAlignment.CENTER, printer, false, headerLine));

            List<string> middleLines = new List<string>();
            middleLines.Add(POSClientResources.DATE.ToUpperGR() + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
            middleLines.Add(POSClientResources.Store.ToUpperGR() + ": " + store);
            middleLines.Add(POSClientResources.CASHIER.ToUpperGR() + ": " + currentUser.POSUserName);
            middleLines.Add(POSClientResources.POS.ToUpperGR() + ": " + ConfigurationManager.TerminalID);
            lines.AddRange(this.CreateSimplePrinterLines(eAlignment.LEFT, printer, false, middleLines.ToArray()));

            List<string> footerLines = new List<string>();
            footerLines.Add(POSClientResources.AMOUNT.ToUpperGR() + ": " + value.ToString("c"));
            if (reason != null)
            {
                footerLines.Add(POSClientResources.REASON.ToUpperGR() + ": " + reason.Description);
            }
            footerLines.Add(POSClientResources.COMMENTS.ToUpperGR() + ": " + comments);
            footerLines.Add(POSClientResources.TAX_CODE_OR_CODE.ToUpperGR() + ": " + userCode);

            lines.AddRange(this.CreateSimplePrinterLines(eAlignment.CENTER, printer, true, footerLines.ToArray()));
            return lines;
        }

        private List<string> CreateReceiptElement(ReceiptElement element, object source, int lineChars, ConnectionType printerConnectionType)
        {
            List<string> elementStrings = new List<string>();
            string variableIdentifier = ConfigurationManager.ReceiptVariableIdentifier;
            OwnerApplicationSettings appSettings = ConfigurationManager.GetAppSettings();
            if (element is SimpleLine)
            {
                elementStrings.Add(CreateReceiptLine(element as ReceiptLine, source, lineChars, printerConnectionType, variableIdentifier));
            }
            else if (element is DynamicLine)
            {
                if (((element as DynamicLine).Condition == eCondition.PROFORMA && source is DocumentHeader && (source as DocumentHeader).DocumentType == ConfigurationManager.ProFormaInvoiceDocumentTypeOid)
                    || ((element as DynamicLine).Condition == eCondition.RECEIPT && source is DocumentHeader && (source as DocumentHeader).InEmulationMode == false && (source as DocumentHeader).DocumentType != ConfigurationManager.ProFormaInvoiceDocumentTypeOid)
                    || ((element as DynamicLine).Condition == eCondition.NONZERODOCUMENTDISCOUNT && source is DocumentHeader && PlatformRoundingHandler.RoundDisplayValue((source as DocumentHeader).GrossTotalBeforeDocumentDiscount - (source as DocumentHeader).GrossTotal) != 0)
                    || ((element as DynamicLine).Condition == eCondition.HASCHANGE && source is DocumentHeader && DocumentService.CheckIfShouldGiveChange((source as DocumentHeader)) == true && (source as DocumentHeader).Change > 0)
                    || ((element as DynamicLine).Condition == eCondition.NONDEFAULTCUSTOMER && source is DocumentHeader && ((source as DocumentHeader).Customer != ConfigurationManager.DefaultCustomerOid || (source as DocumentHeader).CustomerNotFound))
                    || ((element as DynamicLine).Condition == eCondition.SINGLEQUANTITY && source is DocumentDetail && (source as DocumentDetail).Qty == 1)
                    || ((element as DynamicLine).Condition == eCondition.MULTIQUANTITY && source is DocumentDetail && (source as DocumentDetail).Qty != 1)
                    || ((element as DynamicLine).Condition == eCondition.NONZEROLINEDISCOUNT && source is DocumentDetail && PlatformRoundingHandler.RoundDisplayValue((source as DocumentDetail).TotalNonDocumentDiscount) != 0)
                    || ((element as DynamicLine).Condition == eCondition.DOESNOTINCREASEDRAWERAMOUNT && source is ReportPaymentMethod && (source as ReportPaymentMethod).IncreasesDrawerAmount == false)
                    || ((element as DynamicLine).Condition == eCondition.INCREASESDRAWERAMOUNT && source is ReportPaymentMethod && (source as ReportPaymentMethod).IncreasesDrawerAmount == true)
                    || ((element as DynamicLine).Condition == eCondition.NONE))
                {
                    elementStrings.Add(CreateReceiptLine(element as ReceiptLine, source, lineChars, printerConnectionType, variableIdentifier));
                }
            }
            else if (element is DetailGroup)
            {
                if (element.Source == eSource.DOCUMENTDETAIL)
                {
                    if (!(source is DocumentHeader))
                    {
                        throw new Exception(String.Format(POSClientResources.RECEIPT_SOURCE_INVALID, source.GetType().ToString(), typeof(DocumentDetail).ToString(), typeof(DocumentHeader).ToString()));
                    }
                    foreach (DocumentDetail detail in (source as DocumentHeader).DocumentDetails.Where(x => x.IsCanceled == false))
                    {
                        foreach (ReceiptElement line in (element as DetailGroup).Lines)
                        {
                            elementStrings.AddRange(CreateReceiptElement(line, detail, lineChars, printerConnectionType));
                        }
                    }
                }
                else if (element.Source == eSource.DOCUMENTPAYMENT)
                {
                    if (!(source is DocumentHeader))
                    {
                        throw new Exception(String.Format(POSClientResources.RECEIPT_SOURCE_INVALID, source.GetType().ToString(), typeof(DocumentPayment).ToString(), typeof(DocumentHeader).ToString()));
                    }
                    foreach (DocumentPayment payment in (source as DocumentHeader).DocumentPayments.Where(x => x.Amount >= 0))
                    {
                        foreach (ReceiptElement line in (element as DetailGroup).Lines)
                        {
                            elementStrings.AddRange(CreateReceiptElement(line, payment, lineChars, printerConnectionType));
                        }
                    }
                }
                else if (element.Source == eSource.DOCUMENTDETAILDISCOUNT)
                {
                    if (!(source is DocumentDetail))
                    {
                        throw new Exception(String.Format(POSClientResources.RECEIPT_SOURCE_INVALID, source.GetType().ToString(), typeof(DocumentDetailDiscount).ToString(), typeof(DocumentDetail).ToString()));
                    }
                    foreach (DocumentDetailDiscount discount in (source as DocumentDetail).NonHeaderDocumentDetailDiscounts)
                    {
                        foreach (ReceiptElement line in (element as DetailGroup).Lines)
                        {
                            elementStrings.AddRange(CreateReceiptElement(line, discount, lineChars, printerConnectionType));
                        }
                    }
                }
                else if (element.Source == eSource.PAYMENTMETHODS)
                {
                    if (!(source is Report))
                    {
                        throw new Exception(String.Format(POSClientResources.RECEIPT_SOURCE_INVALID, source.GetType().ToString(), typeof(ReportPaymentMethod).ToString(), typeof(Report).ToString()));
                    }
                    foreach (ReportPaymentMethod paymentMethod in (source as Report).PaymentMethods)
                    {
                        foreach (ReceiptElement line in (element as DetailGroup).Lines)
                        {
                            elementStrings.AddRange(CreateReceiptElement(line, paymentMethod, lineChars, printerConnectionType));
                        }
                    }
                }
            }

            return elementStrings;
        }

        private List<string> CreateReceiptPart(ReceiptPart receiptPart, object source, int lineChars, ConnectionType printerConnectionType)
        {
            List<string> lines = new List<string>();
            foreach (ReceiptElement element in receiptPart.Elements)
            {
                lines.AddRange(CreateReceiptElement(element, source, lineChars, printerConnectionType));
            }
            return lines;
        }

        private string CreateReceiptLine(ReceiptLine line, object dynamicLineSource, int lineChars, ConnectionType printerConnectionType, string variableIdentifier)
        {
            if (line is SimpleLine)
            {
                return BuildSimpleLine(line as SimpleLine, lineChars);
            }
            else if (line is DynamicLine)
            {
                return BuildDynamicLine(line as DynamicLine, lineChars, variableIdentifier, dynamicLineSource);
            }
            return "";
        }

        private object GetPropertyValue(object src, string propName)
        {
            try
            {
                if (src == null) throw new ArgumentException("Value cannot be null.", "src");
                if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

                if (propName.Contains("."))
                {
                    var temp = propName.Split(new char[] { '.' }, 2);
                    return GetPropertyValue(GetPropertyValue(src, temp[0]), temp[1]);
                }
                else
                {
                    Guid key = Guid.Empty; ;
                    PropertyInfo prop = src.GetType().GetProperty(propName);
                    if (prop.PropertyType == typeof(Guid) || prop.PropertyType == typeof(BasicObj))
                    {

                        if (prop.PropertyType == typeof(Guid))
                        {
                            var val = prop.GetValue(src, null);
                            Guid.TryParse(val.ToString(), out key);
                        }

                        if (prop.PropertyType == typeof(BasicObj))
                        {
                            var val = prop.GetValue(src, null);
                            Guid.TryParse(val.ToString(), out key);
                        }

                        switch (propName)
                        {
                            case "Customer":
                                return SessionManager.GetObjectByKey<Customer>(key);
                            case "Address":
                                return SessionManager.GetObjectByKey<Address>(key);
                            case "Trader":
                                return SessionManager.GetObjectByKey<Trader>(key);
                            case "DefaultAddress":
                                return SessionManager.GetObjectByKey<Address>(key);
                            case "DefaultPhone":
                                return SessionManager.GetObjectByKey<Phone>(key);
                            case "Store":
                                return SessionManager.GetObjectByKey<Store>(key);
                            case "TaxOfficeLookUp":
                                return SessionManager.GetObjectByKey<TaxOffice>(key);
                            case "DocumentType":
                                return SessionManager.GetObjectByKey<DocumentType>(key);
                            case "DocumentSeries":
                                return SessionManager.GetObjectByKey<DocumentSeries>(key);
                            case "ReferenceCompany":
                                return SessionManager.GetObjectByKey<CompanyNew>(key);
                            case "MainCompany":
                                return SessionManager.GetObjectByKey<CompanyNew>(key);
                            case "UserDailyTotals":
                                return SessionManager.GetObjectByKey<UserDailyTotals>(key);
                            case "User":
                                return SessionManager.GetObjectByKey<User>(key);
                            case "Owner":
                                return SessionManager.GetObjectByKey<CompanyNew>(key);
                            case "BillingAddress":
                                return SessionManager.GetObjectByKey<Address>(key);

                        }
                    }
                    return prop != null ? prop.GetValue(src, null) : null;
                }
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Error(ex.Message);
                return "";
            }
        }

        private string ParceDynamicString(string variableIdentifier, string dynamicString, object source)
        {
            Regex regularExpr = new Regex(variableIdentifier + "[^" + variableIdentifier + "]*" + variableIdentifier);
            MatchCollection matches = regularExpr.Matches(dynamicString);
            List<string> properties = new List<string>();

            DocumentHeader header = source as DocumentHeader;


            foreach (Match match in matches)
            {
                properties.Add(match.Value.Trim(variableIdentifier.ToCharArray()));
            }

            foreach (string property in properties)
            {
                object value;
                if (property.Contains("Function"))
                {
                    value = ExecuteDynamicFunction(property, header.DocumentType.ToString() + ".dll", "DynamicClass", header);
                    dynamicString = dynamicString.Replace(variableIdentifier + property + variableIdentifier, value.ToString());
                }
                else if (property.Contains(".") == false)
                {
                    try
                    {
                        PropertyInfo prop = source.GetType().GetProperty(property);
                        if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(double))
                        {
                            if (prop.Name == "Qty" && source.GetType() == typeof(DocumentDetail))
                            {
                                value = String.Format("{0:0.000}", prop.GetValue(source, null));
                            }
                            else
                            {
                                value = String.Format("{0:0.00}", prop.GetValue(source, null));
                            }
                        }
                        else
                        {
                            value = prop.GetValue(source, null) == null ? "" : prop.GetValue(source, null).ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogFile.Error("ReceipteHelper:ParceDynamicString,Exception catched Property : " + property);
                        LogFile.Info(ex, "ReceipteHelper:ParceDynamicString,Exception catched");
                        value = "INVALID_PROPERTY";
                    }
                    dynamicString = dynamicString.Replace(variableIdentifier + property + variableIdentifier, value.ToString());
                }
                else
                {
                    value = GetPropertyValue(source, property).ToString();
                    dynamicString = dynamicString.Replace(variableIdentifier + property + variableIdentifier, value.ToString());
                }
            }
            return dynamicString;
        }

        private string BuildSimpleLine(SimpleLine line, int lineChars)
        {
            string finalLine = "";
            if (line.MaxCharacters == 0)
            {
                finalLine = line.Content;
            }
            else
            {
                int contentLength = (line as SimpleLine).Content.Length;
                finalLine = new string(line.Content.Take(line.MaxCharacters > contentLength ? contentLength : (int)line.MaxCharacters).ToArray());
            }
            switch (line.LineAlignment)
            {
                case eAlignment.CENTER:
                    return PadCenter(finalLine, lineChars, ' ');
                case eAlignment.LEFT:
                    return finalLine;
                case eAlignment.RIGHT:
                    return finalLine.PadLeft(lineChars - finalLine.Length, ' ');
            }
            return "";
        }

        private string BuildDynamicLine(DynamicLine line, int lineChars, string variableIdentifier, object dynamicLineSource)
        {
            if (line.Cells.Count == 0)
            {
                throw new POSException("Invalid file format. No cells found in Dynamic Line.");
            }

            int cellLength = lineChars / line.Cells.Count;
            int remainingCellsCount = line.Cells.Count;
            int currentCellLength = cellLength;

            string finalLine = "";
            foreach (DynamicLineCell cell in line.Cells)
            {
                string cellString = "";

                cellString = ParceDynamicString(variableIdentifier, cell.Content, dynamicLineSource);
                if (cell.MaxCharacters != 0)
                {
                    int contentLength = cellString.Length;
                    cellString = new string(cellString.Take(cell.MaxCharacters > contentLength ? contentLength : (int)cell.MaxCharacters).ToArray());
                    currentCellLength = (int)cell.MaxCharacters;
                }

                switch (cell.CellAlignment)
                {
                    case eAlignment.CENTER:
                        cellString = PadCenter(cellString, currentCellLength, ' ');
                        break;
                    case eAlignment.RIGHT:
                        cellString = cellString.PadLeft(currentCellLength, ' ');
                        break;
                    case eAlignment.LEFT:
                        cellString = cellString.PadRight(currentCellLength);
                        break;
                }
                finalLine += cellString;
                remainingCellsCount--;
                int remainingSpaceFromCellOverflow = (lineChars - finalLine.Length);

                if (cellString.Length > currentCellLength && remainingSpaceFromCellOverflow > 0 && remainingCellsCount > 0)
                {
                    currentCellLength = remainingSpaceFromCellOverflow / remainingCellsCount;
                }
                else
                {
                    currentCellLength = cellLength;
                }

            }
            return finalLine;
        }

        private string PadCenter(string s, int width, char c)
        {
            if (s == null || width <= s.Length) return s;
            int center = width / 2;
            int posRight = center + (s.Length / 2) + 1;
            string finalStr = s.PadLeft(posRight, c); //first half of centered string
            for (int i = 0; i < width - posRight; i++)
            {
                finalStr += c;
            }
            return finalStr;
        }


        private byte[] loadFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            byte[] buffer = new byte[(int)fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            fs = null;
            return buffer;
        }

        private object ExecuteDynamicFunction(String FunctionName, String assemblyName, String ClassName, DocumentHeader obj)
        {
            object result;
            try
            {
                string assemblyPath = Path.GetDirectoryName(Application.ExecutablePath) + "//Modules//" + assemblyName;
                Assembly assembly = Assembly.LoadFrom(assemblyPath);
                Type T = assembly.GetType("ITS.POS.Client." + ClassName);
                BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
                object[] constructorParams = new object[] { this.SessionManager, this.ConfigurationManager, obj, Kernel };
                object instance = Activator.CreateInstance(T, constructorParams);
                MethodInfo callMethod = T.GetMethod(FunctionName, flags);
                Type returnType = callMethod.ReturnType;
                if (returnType == typeof(string) || returnType.Name == "void" || string.IsNullOrEmpty(returnType.Name))
                {
                    result = "";
                }
                else
                {
                    result = Activator.CreateInstance(returnType);
                }
                if (callMethod != null)
                {
                    result = callMethod.Invoke(instance, null).ToString();
                }
                assembly = null;
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Error(ex.Message);
                return " ";
            }
            if (result == null)
            {
                result = "";
            }
            return result;
        }
    }
}

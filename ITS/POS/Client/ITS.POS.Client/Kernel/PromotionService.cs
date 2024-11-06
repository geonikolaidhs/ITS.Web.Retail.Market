using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Client.Exceptions;
using ITS.POS.Hardware;
using ITS.POS.Hardware.Common;
using ITS.POS.Model.Master;
using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using ITS.Retail.Platform.Kernel.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// POS specific implementation of the platform promotion service. 
    /// Handles the promotions' execution logic
    /// </summary>
    public class PromotionService : IPromotionService
    {
        ISessionManager SessionManager { get; set; }
        IConfigurationManager ConfigurationManager { get; set; }
        IDocumentService DocumentService { get; set; }
        IReceiptBuilder ReceiptBuilder { get; set; }
        IPlatformPromotionService PlatformPromotionService { get; set; }

     

        public PromotionService(ISessionManager sessionManager, IConfigurationManager config, IDocumentService documentService,
            IReceiptBuilder receiptBuilder, IPlatformPromotionService platformPromotionservice)
        {
            this.SessionManager = sessionManager;
            this.ConfigurationManager = config;
            this.DocumentService = documentService;
            this.ReceiptBuilder = receiptBuilder;
            this.PlatformPromotionService = platformPromotionservice;
        }

        public void ClearDocumentPromotions(IDocumentHeader header)
        {
            PlatformPromotionService.ClearDocumentPromotions(header);
        }

        public void ExecutePromotions(IDocumentHeader header, IPriceCatalogPolicy priceCatalogPolicy, IOwnerApplicationSettings ownerApplicationSettings, DateTime activeAtDate, bool includeTestPromotions)
        {
            
            PlatformPromotionService.ExecutePromotions(header, priceCatalogPolicy, ownerApplicationSettings, activeAtDate, includeTestPromotions);
        }

        public void ExecutePromotionResults(DocumentHeader header, ePromotionResultExecutionPlan executionPlan, Device printer, IFormManager formManager, Logger logFile)
        {
            if (header.DocumentPromotions.Count > 0)
            {
                XPCollection<PromotionResult> promotionsResults = new XPCollection<PromotionResult>(SessionManager.GetSession<PromotionResult>(),
                                                                    CriteriaOperator.And(new InOperator("Promotion", header.DocumentPromotions.Select(x => x.Promotion)),
                                                                                         new BinaryOperator("ExecutionPlan", executionPlan)));
                foreach (PromotionResult result in promotionsResults)
                {
                    if (result is PromotionDisplayResult)
                    {
                        string message = (result as PromotionDisplayResult).Message;
                        formManager.ShowCancelOnlyMessageBox(message);
                    }
                    else if (result is PromotionPrintResult)
                    {
                        string message = (result as PromotionPrintResult).Message;
                        if (printer is Printer)
                        {
                            List<string> lines = ReceiptBuilder.CreateSimplePrinterLines(eAlignment.CENTER, printer as Printer, true, message);
                            (printer as Printer).PrintLines(lines.ToArray());
                        }
                        else if (printer is FiscalPrinter)
                        {
                            FiscalLine line = new FiscalLine() { Type = ePrintType.NORMAL, Value = message };
                            (printer as FiscalPrinter).PrintIllegal(line);
                        }
                        else
                        {
                            throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                        }
                    }
                }
            }
        }

     

    }
}

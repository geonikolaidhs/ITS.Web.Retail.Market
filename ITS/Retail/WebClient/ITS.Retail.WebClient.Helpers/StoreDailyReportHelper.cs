using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.Retail.WebClient.Helpers
{
    public class StoreDailyReportHelper
    {
        public static int QUANTITY_MULTIPLIER
        {
            get
            {
                return 10000;
            }

        }

        public static void GetDailyTotals(StoreDailyReport report, Session session, DateTime date)
        {
            IEnumerable<DailyTotals> dailyTotals = new XPCollection<DailyTotals>(session).Where(total => total.FiscalDate.ToShortDateString() == date.ToShortDateString() &&
                                                                                                !total.FiscalDateOpen ).AsEnumerable();
            int linenumber = 0;
            foreach (DailyTotals dailytotal in dailyTotals)
            {
                linenumber++;
                StoreDailyReportDailyTotal dailyReportDailyTotal = new StoreDailyReportDailyTotal(session);
                dailyReportDailyTotal.DailyTotal = dailytotal;
                dailyReportDailyTotal.StoreDailyReport = report;
                dailyReportDailyTotal.Description = "Z " + dailytotal.FiscalDeviceSerialNumber;
                dailyReportDailyTotal.Value = dailytotal.Sum;
                dailyReportDailyTotal.LineNumber = linenumber;
                report.DailyTotals.Add(dailyReportDailyTotal);
                report.DailyTotalsTotal += dailyReportDailyTotal.Value;
            }
            report.CollectionsTotal = report.DailyTotalsTotal;
        }

        public static IEnumerable<StoreDailyReportPayment> GetCredits(StoreDailyReport report, Session session)
        {
            IEnumerable<IGrouping<PaymentMethod, DailyTotalsDetail>> dailyTotalsPayments = report.DailyTotals.SelectMany(storedailytotal =>
                storedailytotal.DailyTotal.DailyTotalsDetails.AsEnumerable()).Where(detail =>
                detail.Payment != null && !detail.Payment.IncreasesDrawerAmount).GroupBy(detail => detail.Payment);

            IEnumerable<IGrouping<PaymentMethod, DocumentPayment>> docHeaderPayments = report.DocumentHeaders.Where(doc =>
                doc.Type == eStoreDailyReportDocHeaderType.Proforma).SelectMany(storedocheader =>
                storedocheader.DocumentHeader.DocumentPayments.AsEnumerable()).Where(detail =>
                detail.PaymentMethod != null && !detail.PaymentMethod.IncreasesDrawerAmount).GroupBy(detail => detail.PaymentMethod);

            int linenumber = 0;

            foreach (IGrouping<PaymentMethod, DailyTotalsDetail> detailGroup in dailyTotalsPayments)
            {
                linenumber++;
                StoreDailyReportPayment storeDailyReportPayment = new StoreDailyReportPayment(session);
                StoreDailyReportPayment storeDailyReportPaymentStats = new StoreDailyReportPayment(session);
                storeDailyReportPayment.DailyReport = storeDailyReportPaymentStats.DailyReport = report;
                storeDailyReportPayment.LineNumber = storeDailyReportPaymentStats.LineNumber =  linenumber;
                storeDailyReportPayment.Type = eStoreDailyReportDocHeaderType.Credit;
                storeDailyReportPaymentStats.Type = eStoreDailyReportDocHeaderType.Statistic;
                storeDailyReportPayment.Description = Resources.DailyTotals+" : "+ detailGroup.Key.Description;
                storeDailyReportPaymentStats.Description = detailGroup.Key.Description;
                foreach(DailyTotalsDetail detail in detailGroup)
                {
                    storeDailyReportPayment.UserValue = storeDailyReportPaymentStats.UserValue += detail.Amount;    
                }
                report.Lines.Add(storeDailyReportPayment);
                report.Lines.Add(storeDailyReportPaymentStats);
                report.CreditsGridTotal += storeDailyReportPayment.UserValue;
            }

            foreach (IGrouping<PaymentMethod, DocumentPayment> detailGroup in docHeaderPayments)
            {
                linenumber++;
                StoreDailyReportPayment storeDailyReportPayment = new StoreDailyReportPayment(session);
                StoreDailyReportPayment storeDailyReportPaymentStats = report.Lines.Where(line => line.Description.Split(':').Last().Trim() == detailGroup.Key.Description).FirstOrDefault();
                storeDailyReportPayment.DailyReport = report;
                storeDailyReportPayment.LineNumber = linenumber;
                storeDailyReportPayment.Type = eStoreDailyReportDocHeaderType.Credit;
                storeDailyReportPayment.Description = Resources.Documents + " : " + detailGroup.Key.Description;
                if (storeDailyReportPaymentStats == null)
                {
                    storeDailyReportPaymentStats = new StoreDailyReportPayment(session);
                    storeDailyReportPaymentStats.LineNumber = report.Lines.Where(line => line.Type == eStoreDailyReportDocHeaderType.Statistic).Select(line => line.LineNumber).Max() + 1;
                    storeDailyReportPaymentStats.DailyReport = report;
                    storeDailyReportPaymentStats.Type = eStoreDailyReportDocHeaderType.Statistic;
                    storeDailyReportPaymentStats.Description = detailGroup.Key.Description;
                }
                foreach (DocumentPayment detail in detailGroup)
                {
                    storeDailyReportPayment.UserValue += detail.Amount;
                    storeDailyReportPaymentStats.UserValue += detail.Amount;
                }
                report.Lines.Add(storeDailyReportPayment);
                report.CreditsGridTotal += storeDailyReportPayment.UserValue;
            }


            return report.Lines.Where(payment => payment.Type == eStoreDailyReportDocHeaderType.Credit);
        }

        public static void GetDocumentHeaders(StoreDailyReport report, Session session, string docTypeCode, DateTime date)
        {   
            IEnumerable<DocumentHeader> documents = null;
            int linenumber = 0;

            
            documents = new XPCollection<DocumentHeader>(session).Where(doc => 
                doc.DocumentType.Code == docTypeCode &&
                !doc.IsCanceled &&
                doc.FiscalDate.ToShortDateString() == date.ToShortDateString()).AsEnumerable();

            foreach (DocumentHeader document in documents)
            {
                linenumber++;
                StoreDailyReportDocumentHeader dailyReportDocHeader = new StoreDailyReportDocumentHeader(session);
                dailyReportDocHeader.Description = document.Customer.CompanyName;
                dailyReportDocHeader.DocumentHeader = document;
                dailyReportDocHeader.StoreDailyReport = report;
                switch(docTypeCode)
                {
                    case "101":
                        dailyReportDocHeader.Type = eStoreDailyReportDocHeaderType.Proforma;
                        report.InvoicesTotalCash += document.DocumentPayments.Where(payment => payment.PaymentMethod!= null && payment.PaymentMethod.IncreasesDrawerAmount).Sum(docpayment => docpayment.Amount);
                        report.InvoicesTotal += document.GrossTotal;
                        break;
                    case "927":
                        dailyReportDocHeader.Type = eStoreDailyReportDocHeaderType.AutoDelivery;
                        report.AutoDeliveriesTotal += document.GrossTotal;
                        break;
                    default:
                        dailyReportDocHeader.Type = eStoreDailyReportDocHeaderType.Other;
                        break;
                }
                dailyReportDocHeader.LineNumber = linenumber;
         
            }
        }

        public static void GetStatistics(StoreDailyReport report, Session session, DateTime date)
        {
            IEnumerable<DocumentHeader> proformas = new XPCollection<DocumentHeader>(session).Where(doc => doc.DocumentType.Code == "5" && !doc.IsCanceled &&
                doc.FiscalDate.ToShortDateString() == date.ToShortDateString()).AsEnumerable();

            //Proformas
            StoreDailyReportPayment storeDailyReportPayment1 = new StoreDailyReportPayment(session);
            storeDailyReportPayment1.DailyReport = report;
            storeDailyReportPayment1.Description = Resources.ProformaInvoices;
            storeDailyReportPayment1.Type = eStoreDailyReportDocHeaderType.Statistic;
            storeDailyReportPayment1.UserValue = proformas.Sum(doc => doc.GrossTotal);
            storeDailyReportPayment1.LineNumber = 0;
            report.Lines.Add(storeDailyReportPayment1);

            //CustomersCount
            StoreDailyReportPayment storeDailyReportPayment2 = new StoreDailyReportPayment(session);
            storeDailyReportPayment2.DailyReport = report;
            storeDailyReportPayment2.Description = Resources.CustomersCount;
            storeDailyReportPayment2.Type = eStoreDailyReportDocHeaderType.Statistic;
            storeDailyReportPayment2.UserValue = proformas.Count();
            storeDailyReportPayment2.LineNumber = report.Lines.Where(line => line.Type == eStoreDailyReportDocHeaderType.Statistic).Select(line => line.LineNumber).Max() + 1; 
            report.Lines.Add(storeDailyReportPayment2);

            //CustomersCount
            StoreDailyReportPayment storeDailyReportPayment3 = new StoreDailyReportPayment(session);
            storeDailyReportPayment3.DailyReport = report;
            storeDailyReportPayment3.Description = Resources.DailyReportAvg;
            storeDailyReportPayment3.Type = eStoreDailyReportDocHeaderType.Statistic;
            storeDailyReportPayment3.UserValue = storeDailyReportPayment2.UserValue > 0 ? storeDailyReportPayment1.UserValue / storeDailyReportPayment2.UserValue : 0;
            storeDailyReportPayment3.LineNumber = storeDailyReportPayment2.LineNumber + 1;
            report.Lines.Add(storeDailyReportPayment3);
        }

        public static void RecalculateReportTotals(StoreDailyReport report)
        {
            report.CollectionsTotal =  report.InvoicesTotalCash + report.AutoDeliveriesTotal + report.DailyTotalsTotal + report.CollectionComplement;
            report.PaymentsWithDrawsTotal = report.MainPOSWithdraws + report.PaymentsTotal;
            report.CreditsTotal = report.OtherExpanses + report.CreditsGridTotal;
            report.CreditsPaymentsWithDrawsTotal = report.PaymentsWithDrawsTotal + report.CreditsTotal;
            report.CashDelivery = report.PaperMoney + report.Coins;
            report.ReportTotal = report.CreditsPaymentsWithDrawsTotal + report.CashDelivery;
            report.POSDifference = report.CollectionsTotal - report.ReportTotal;
        }
    }
}

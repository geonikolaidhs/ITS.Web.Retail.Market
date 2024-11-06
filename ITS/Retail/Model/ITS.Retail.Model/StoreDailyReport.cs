//-----------------------------------------------------------------------
// <copyright file="StoreDailyReport.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System.Linq;
using System.Collections.Generic;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;

namespace ITS.Retail.Model
{
    [Updater(Order = 995, Permissions = eUpdateDirection.STORECONTROLLER_TO_MASTER)]
    public class StoreDailyReport : BaseObj
    {
        private Store _Store;
        private decimal _CollectionsTotal;
        private decimal _MainPOSWithdraws;
        private decimal _Coins;
        private decimal _PaperMoney;
        private decimal _OtherExpanses;
        private decimal _DailyTotalsTotal;
        private decimal _InvoicesTotal;
        private decimal _AutoDeliveriesTotal;
        private decimal _PaymentsTotal;
        private decimal _PaymentsWithDrawsTotal;
        private decimal _CreditsTotal;
        private decimal _CreditsPaymentsWithDrawsTotal;
        private decimal _CashDelivery;
        private decimal _InvoicesTotalCash;
        private decimal _ReportTotal;
        private decimal _POSDifference;
        private int? _Code;
        private DateTime _ReportDate;
        private decimal _CreditsGridTotal;
        private CompanyNew _Owner;
        private decimal _CollectionComplement;
        private string _CollectionComplementText;
        private string _OtherExpansesText;


        public StoreDailyReport()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public StoreDailyReport(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        [Indexed("Owner;GCRecord")]
        [DisplayOrder(Order = 1)]
        public int? Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }

        [DisplayOrder(Order = 2)]
        public DateTime ReportDate
        {
            get
            {
                return _ReportDate;
            }
            set
            {
                SetPropertyValue("ReportDate", ref _ReportDate, value);
            }
        }

        public Store Store
        {
            get
            {
                return _Store;
            }
            set
            {
                SetPropertyValue("Store", ref _Store, value);
            }
        }

        public CompanyNew Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                SetPropertyValue("Owner", ref _Owner, value);
            }
        }
        [DisplayOrder(Order = 3)]
        public decimal DailyTotalsTotal
        {
            get
            {
                return _DailyTotalsTotal;
            }
            set
            {
                SetPropertyValue("DailyTotalsTotal", ref _DailyTotalsTotal, value);
            }
        }

        [DisplayOrder(Order = 4)]
        public decimal InvoicesTotalCash
        {
            get
            {
                return _InvoicesTotalCash;
            }
            set
            {
                SetPropertyValue("InvoicesTotalCash", ref _InvoicesTotalCash, value);
            }
        }

        [DisplayOrder(Order = 5)]
        public decimal AutoDeliveriesTotal
        {
            get
            {
                return _AutoDeliveriesTotal;
            }
            set
            {
                SetPropertyValue("AutoDeliveriesTotal", ref _AutoDeliveriesTotal, value);
            }
        }

        [DisplayOrder(Order = 6)]
        public decimal CollectionComplement
        {
            get
            {
                return _CollectionComplement;
            }
            set
            {
                SetPropertyValue("CollectionComplement", ref _CollectionComplement, value);
            }
        }

        [DisplayOrder(Order = 7)]
        public string CollectionComplementText
        {
            get
            {
                return _CollectionComplementText;
            }
            set
            {
                SetPropertyValue("CollectionComplementText", ref _CollectionComplementText, value);
            }
        }
        [DisplayOrder(Order = 8)]
        public decimal CollectionsTotal
        {
            get
            {
                return _CollectionsTotal;
            }
            set
            {
                SetPropertyValue("CollectionsTotal", ref _CollectionsTotal, value);
            }
        }

        public decimal InvoicesTotal
        {
            get
            {
                return _InvoicesTotal;
            }
            set
            {
                SetPropertyValue("InvoicesTotal", ref _InvoicesTotal, value);
            }
        }

        [DisplayOrder(Order = 9)]
        public decimal MainPOSWithdraws
        {
            get
            {
                return _MainPOSWithdraws;
            }
            set
            {
                SetPropertyValue("MainPOSDeposits", ref _MainPOSWithdraws, value);
            }
        }


        [DisplayOrder(Order = 10)]
        public decimal PaymentsTotal
        {
            get
            {
                return _PaymentsTotal;
            }
            set
            {
                SetPropertyValue("PaymentsTotal", ref _PaymentsTotal, value);
            }
        }

        [DisplayOrder(Order = 11)]
        public decimal PaymentsWithDrawsTotal
        {
            get
            {
                return _PaymentsWithDrawsTotal;
            }
            set
            {
                SetPropertyValue("PaymentsWithDrawsTotal", ref _PaymentsWithDrawsTotal, value);
            }
        }

        [DisplayOrder(Order = 12)]
        public decimal CreditsGridTotal
        {
            get
            {
                return _CreditsGridTotal;
            }
            set
            {
                SetPropertyValue("CreditsGridTotal", ref _CreditsGridTotal, value);
            }
        }

        public decimal CreditsTotal
        {
            get
            {
                return _CreditsTotal;
            }
            set
            {
                SetPropertyValue("CreditsTotal", ref _CreditsTotal, value);
            }
        }

        [DisplayOrder(Order = 13)]
        public string OtherExpansesText
        {
            get
            {
                return _OtherExpansesText;
            }
            set
            {
                SetPropertyValue("OtherExpansesText", ref _OtherExpansesText, value);
            }
        }

        [DisplayOrder(Order = 14)]
        public decimal OtherExpanses
        {
            get
            {
                return _OtherExpanses;
            }
            set
            {
                SetPropertyValue("OtherExpanses", ref _OtherExpanses, value);
            }
        }

        [DisplayOrder(Order = 15)]
        public decimal CreditsPaymentsWithDrawsTotal
        {
            get
            {
                return _CreditsPaymentsWithDrawsTotal;
            }
            set
            {
                SetPropertyValue("CreditsPaymentsWithDrawsTotal", ref _CreditsPaymentsWithDrawsTotal, value);
            }
        }

        [DisplayOrder(Order = 16)]
        public decimal PaperMoney
        {
            get
            {
                return _PaperMoney;
            }
            set
            {
                SetPropertyValue("PaperMoney", ref _PaperMoney, value);
            }
        }

        [DisplayOrder(Order = 17)]
        public decimal Coins
        {
            get
            {
                return _Coins;
            }
            set
            {
                SetPropertyValue("Coins", ref _Coins, value);
            }
        }

        [DisplayOrder(Order = 18)]
        public decimal CashDelivery
        {
            get
            {
                return _CashDelivery;
            }
            set
            {
                SetPropertyValue("CashDelivery", ref _CashDelivery, value);
            }
        }

        [DisplayOrder(Order = 19)]
        public decimal ReportTotal
        {
            get
            {
                return _ReportTotal;
            }
            set
            {
                SetPropertyValue("ReportTotal", ref _ReportTotal, value);
            }
        }
        [DisplayOrder(Order = 20)]
        public decimal POSDifference
        {
            get
            {
                return _POSDifference;
            }
            set
            {
                SetPropertyValue("POSDifference", ref _POSDifference, value);
            }
        }

        public override Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType, bool includeDetails, eUpdateDirection direction = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.POS_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.STORECONTROLLER_TO_POS)
        {
            Dictionary<string, object> dictionary = base.GetDict(settings, includeType, includeDetails);

            if (includeDetails)
            {
                dictionary.Add("DailyTotals", DailyTotals.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
                dictionary.Add("DocumentHeaders", DocumentHeaders.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
                dictionary.Add("Lines", Lines.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
            }
            return dictionary;
        }

        [Aggregated, Association("StoreDailyReport-StoreDailyReportDailyTotals")]
        public XPCollection<StoreDailyReportDailyTotal> DailyTotals
        {
            get
            {
                return GetCollection<StoreDailyReportDailyTotal>("DailyTotals");
            }
        }

        [Aggregated, Association("StoreDailyReport-StoreDailyReportDocumentHeaders")]
        public XPCollection<StoreDailyReportDocumentHeader> DocumentHeaders
        {
            get
            {
                return GetCollection<StoreDailyReportDocumentHeader>("DocumentHeaders");
            }
        }

        [Aggregated, Association("StoreDailyReport-StoreDailyReportPayments")]
        public XPCollection<StoreDailyReportPayment> Lines
        {
            get
            {
                return GetCollection<StoreDailyReportPayment>("Lines");
            }
        }

        protected override void OnSaving()
        {
            if (this.SkipOnSavingProcess)
            {
                base.OnSaving();
                return;
            }

            if (this.IsDeleted)
            {
                this.Code = null;
            }
            else if (this.Code == null)
            {
                int? maxCode = (int?)Session.EvaluateInTransaction(this.Session.GetClassInfo(this),
                                                    CriteriaOperator.Parse("Max(Code)"),
                                                    new BinaryOperator("Store.Oid", this.Store.Oid));
                if (maxCode == null)
                {
                    maxCode = 0;
                }

                maxCode++;
                this.Code = maxCode;
            }

            base.OnSaving();
        }
    }
}

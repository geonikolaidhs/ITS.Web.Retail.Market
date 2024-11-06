//-----------------------------------------------------------------------
// <copyright file="StoreDailyReportDailyTotal.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class StoreDailyReportDailyTotal : StoreDailyReportLine
    {
        private DailyTotals _DailyTotal;
        private StoreDailyReport _StoreDailyReport;
        private decimal _Value;
        public StoreDailyReportDailyTotal()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public StoreDailyReportDailyTotal(Session session)
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

        public DailyTotals DailyTotal
        {
            get
            {
                return _DailyTotal;
            }
            set
            {
                SetPropertyValue("DailyTotal", ref _DailyTotal, value);
            }
        }

        [Association("StoreDailyReport-StoreDailyReportDailyTotals")]
        public StoreDailyReport StoreDailyReport
        {
            get
            {
                return _StoreDailyReport;
            }
            set
            {
                SetPropertyValue("StoreDailyReport", ref _StoreDailyReport, value);
            }
        }

        public decimal Value
        {
            get
            {
                return _Value;
            }
            set
            {
                SetPropertyValue("Value", ref _Value, value);
            }
        }
    }
}

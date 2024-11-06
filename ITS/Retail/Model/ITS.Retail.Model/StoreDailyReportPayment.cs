//-----------------------------------------------------------------------
// <copyright file="StoreDailyReportPayment.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System.Linq;
using System.Collections.Generic;
using ITS.Retail.Platform.Enumerations;


namespace ITS.Retail.Model
{
    public class StoreDailyReportPayment : StoreDailyReportLine
    {
        private StoreDailyReport _DailyReport;
         public StoreDailyReportPayment()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

         public StoreDailyReportPayment(Session session)
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

        [Association("StoreDailyReport-StoreDailyReportPayments")]
        public StoreDailyReport DailyReport
        {
            get
            {
                return _DailyReport;
            }
            set
            {
                SetPropertyValue("DailyReport", ref _DailyReport, value);
            }
        }
    }
}

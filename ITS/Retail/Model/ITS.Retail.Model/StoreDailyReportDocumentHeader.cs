//-----------------------------------------------------------------------
// <copyright file="StoreDailyReportDocumentHeader.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class StoreDailyReportDocumentHeader : StoreDailyReportLine
    {
        private DocumentHeader _DocumentHeader;
        private StoreDailyReport _StoreDailyReport;
        public StoreDailyReportDocumentHeader()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public StoreDailyReportDocumentHeader(Session session)
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

        public DocumentHeader DocumentHeader
        {
            get
            {
                return _DocumentHeader;
            }
            set
            {
                SetPropertyValue("DocumentHeader", ref _DocumentHeader, value);
            }
        }

        [Association("StoreDailyReport-StoreDailyReportDocumentHeaders")]
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

        
    }
}

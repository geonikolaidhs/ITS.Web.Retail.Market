//-----------------------------------------------------------------------
// <copyright file="StoreDailyReportLine.cs" company="ITS">
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
    [NonPersistent]
    public class StoreDailyReportLine : BaseObj
    {
        private decimal _UserValue;
        private string _Description;
        private int _LineNumber;
        private bool _UserSetsValue;
        private eStoreDailyReportDocHeaderType _Type;

        public StoreDailyReportLine()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public StoreDailyReportLine(Session session)
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

        public decimal UserValue
        {
            get
            {
                return _UserValue;
            }
            set
            {
                SetPropertyValue("UserValue", ref _UserValue, value);
            }
        }

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                SetPropertyValue("Description", ref _Description, value);
            }
        }

        public int LineNumber
        {
            get
            {
                return _LineNumber;
            }
            set
            {
                SetPropertyValue("LineNumber", ref _LineNumber, value);
            }
        }

        public bool UserSetsValue
        {
            get
            {
                return _UserSetsValue;
            }
            set
            {
                SetPropertyValue("UserSetsValue", ref _UserSetsValue, value);
            }
        }

        public eStoreDailyReportDocHeaderType Type
        {
            get
            {
                return _Type;
            }
            set
            {
                SetPropertyValue("Type", ref _Type, value);
            }
        }
    }
}

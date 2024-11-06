//-----------------------------------------------------------------------
// <copyright file="DataFileRecordDetail.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Xpo.Metadata;
using Newtonsoft.Json;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Model.NonPersistant;

namespace ITS.Retail.Model
{
    public class DataFileRecordDetail : ImportFileRecordField
    {
        public DataFileRecordDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DataFileRecordDetail(Session session)
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

        // Fields...

        private DataFileRecordHeader _Header;

        [Association("DataFileRecordHeader-DataFileRecordDetails")]
        public DataFileRecordHeader Header
        {
            get
            {
                return _Header;
            }
            set
            {
                SetPropertyValue("Header", ref _Header, value);
            }
        }
        
    }
}

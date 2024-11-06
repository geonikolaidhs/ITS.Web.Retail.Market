//-----------------------------------------------------------------------
// <copyright file="DataFileRecordHeader.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Xpo.Metadata;
using Newtonsoft.Json;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    public class DataFileRecordHeader : ImportFileRecordHeader, IRequiredOwner
    {
        public DataFileRecordHeader()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DataFileRecordHeader(Session session)
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

        // Fields...
        private DataFileRecordDetail _KeyProperty;

        private CompanyNew _Owner;
        private DataFileRecordDetail _ReferenceProperty;

        [DescriptionField]
        public String Description
        {
            get
            {
                return EntityName;
            }
        }

        [DisplayOrder(Order = 1)]
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

        //[Association("DataFileRecordDetail-DataFileRecordHeaders")]
        [DisplayOrder(Order = 7)]
        public DataFileRecordDetail KeyProperty
        {
            get
            {
                return _KeyProperty;
            }
            set
            {
                SetPropertyValue("KeyProperty", ref _KeyProperty, value);
            }
        }

        [DisplayOrder(Order = 8)]
        public DataFileRecordDetail ReferenceProperty
        {
            get
            {
                return _ReferenceProperty;
            }
            set
            {
                SetPropertyValue("ReferenceProperty", ref _ReferenceProperty, value);
            }
        }

        [Aggregated, Association("DataFileRecordHeader-DataFileRecordDetails")]
        public XPCollection<DataFileRecordDetail> DataFileRecordDetails
        {
            get
            {
                return GetCollection<DataFileRecordDetail>("DataFileRecordDetails");
            }
        }

        [Association("DataFileRecordHeader-DecocedData")]
        public XPCollection<DecodedRawData> DecocedData
        {
            get
            {
                return GetCollection<DecodedRawData>("DecocedData");
            }
        }

        public override Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType, bool includeDetails, eUpdateDirection direction = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.POS_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.STORECONTROLLER_TO_POS)
        {
            Dictionary<string, object> dictionary = base.GetDict(settings, includeType, includeDetails);
            if(includeDetails)
            {
                dictionary.Add("DataFileRecordDetails", this.DataFileRecordDetails.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
            }

            return dictionary;
        }
    }
}

//-----------------------------------------------------------------------
// <copyright file="Categories.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;

namespace ITS.Retail.Model
{
    [NonPersistent]
    public class Category : LookupField, IOwner
    {
        public Category()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Category(Session session)
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


        private CompanyNew _Owner;
        private string _Code;
        [Indexed("GCRecord;Owner;ObjectType", Unique = true)]
        [DisplayOrder(Order = 1)]
        public string Code
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

        private string _ReferenceCode;
        [Indexed("GCRecord;Owner;ObjectType")]        
        public string ReferenceCode
        {
            get
            {
                return _ReferenceCode;
            }
            set
            {
                SetPropertyValue("ReferenceCode", ref _ReferenceCode, value);
            }
        }

        [Indexed("GCRecord;IsDefault", Unique = false)]
        [DisplayOrder(Order = 3)]
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
    }

}
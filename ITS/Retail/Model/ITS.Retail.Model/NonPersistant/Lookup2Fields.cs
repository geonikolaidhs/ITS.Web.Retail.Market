//-----------------------------------------------------------------------
// <copyright file="Lookup2Fields.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using System.ComponentModel.DataAnnotations;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{
    [NonPersistent]
    //[IsDefaultUniqueFields(UniqueFields= new string []{"Code"})]
    public class Lookup2Fields : LookupField, IOwner
    {
        public Lookup2Fields()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Lookup2Fields(Session session)
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

        public Lookup2Fields(string code, string description)
            : base(description)
        {
            _Code = code;
        }
        public Lookup2Fields(Session session, string code, string description)
            : base(session, description)
        {
            _Code = code;
        }
        
        private CompanyNew _Owner;
        private string _Code;
        [Indexed("GCRecord;Owner", Unique = true)]
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
         [Indexed("GCRecord;Owner")]
        //[DisplayOrder(Order = 1)]
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
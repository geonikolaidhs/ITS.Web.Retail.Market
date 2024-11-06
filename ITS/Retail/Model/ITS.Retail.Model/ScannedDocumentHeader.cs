//-----------------------------------------------------------------------
// <copyright file="DocumentHeader.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ITS.Retail.Platform.Enumerations;
using System.Data;
using System.Drawing;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{
   
    public class ScannedDocumentHeader : BaseObj ,IOwner
    {
        public ScannedDocumentHeader()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ScannedDocumentHeader(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
        }

        // Fields...
        private long? _InsertedOn;
        private long _ScannedOn;
        private bool _Inserted;
        private string _SupplierTaxCode;
        private String _DocumentNumber;
        private User _EditingUser;
        private DateTime _DocumentIssueDate;
        private double _DocumentAmount;

        private CompanyNew _Owner;

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

        [DescriptionField]
        public String Description
        {
            get
            {
                return this.Owner.CompanyName + " Scanned " + ScannedOnDateTime.ToShortDateString();
            }
        }

        [ValueConverter(typeof(DevExpress.Xpo.Metadata.ImageValueConverter)), Delayed]
        public Image ScannedImage
        {
            get { return GetDelayedPropertyValue<Image>("ScannedImage"); }
            set { SetDelayedPropertyValue<Image>("ScannedImage", value); }
        }

        public DateTime DocumentIssueDate
        {
            get
            {
                return _DocumentIssueDate;
            }
            set
            {
                SetPropertyValue("DocumentIssueDate", ref _DocumentIssueDate, value);
            }
        }

        public double DocumentAmount
        {
            get
            {
                return _DocumentAmount;
            }
            set
            {
                SetPropertyValue("DocumentAmount", ref _DocumentAmount, value);
            }
        }


        public String DocumentNumber
        {
            get
            {
                return _DocumentNumber;
            }
            set
            {
                SetPropertyValue("DocumentNumber", ref _DocumentNumber, value);
            }
        }


        public string SupplierTaxCode
        {
            get
            {
                return _SupplierTaxCode;
            }
            set
            {
                SetPropertyValue("SupplierTaxCode", ref _SupplierTaxCode, value);
            }
        }

        public User EditingUser
        {
            get
            {
                return _EditingUser;
            }
            set
            {
                SetPropertyValue("EditingUser", ref _EditingUser, value);
            }
        }


        public long ScannedOn
        {
            get
            {
                return _ScannedOn;
            }
            set
            {
                SetPropertyValue("ScannedOn", ref _ScannedOn, value);
            }
        }

        public DateTime ScannedOnDateTime
        {
            get
            {
                return new DateTime(ScannedOn);
            }
        }

        public bool Inserted
        {
            get
            {
                return _Inserted;
            }
            set
            {
                SetPropertyValue("Inserted", ref _Inserted, value);
            }
        }


        public long? InsertedOn
        {
            get
            {
                return _InsertedOn;
            }
            set
            {
                SetPropertyValue("InsertedOn", ref _InsertedOn, value);
            }
        }

        public DateTime? InsertedOnDateTime
        {
            get
            {
                if (InsertedOn == null)
                {
                    return null;
                }
                return new DateTime((long)InsertedOn);
            }
        }

    }
}

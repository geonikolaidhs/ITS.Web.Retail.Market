﻿using System;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using DevExpress.Data.Filtering;

namespace ITS.POS.Model.Master
{
    public class Trader : BaseObj
    {
        public Trader()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Trader(Session session)
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

        private string _Code;
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

        private string _FirstName;
        public string FirstName
        {
            get
            {
                return _FirstName;
            }
            set
            {
                SetPropertyValue("FirstName", ref _FirstName, value);
            }
        }

        private string _LastName;
        public string LastName
        {
            get
            {
                return _LastName;
            }
            set
            {
                SetPropertyValue("LastName", ref _LastName, value);
            }
        }

        private string _TaxCode;
        [Indexed(Unique = false)]
        public string TaxCode
        {
            get
            {
                return _TaxCode;
            }
            set
            {
                SetPropertyValue("TaxCode", ref _TaxCode, value);
            }
        }
        private DateTime? _GDPRRegistrationDate;
        private string _GDPRProtocolNumber;

        public DateTime? GDPRRegistrationDate
        {
            get
            {
                return _GDPRRegistrationDate;
            }
            set
            {
                SetPropertyValue("GDPRRegistrationDate", ref _GDPRRegistrationDate, value);
            }
        }

        public string GDPRProtocolNumber
        {
            get
            {
                return _GDPRProtocolNumber;
            }
            set
            {
                SetPropertyValue("GDPRProtocolNumber", ref _GDPRProtocolNumber, value);
            }
        }
        private Guid? _TaxOfficeLookUpOid;
        [Persistent("TaxOfficeLookUp")]
        public Guid? TaxOfficeLookUpOid
        {
            get
            {
                return _TaxOfficeLookUpOid;
            }
            set
            {
                SetPropertyValue("TaxOfficeLookUpOid", ref _TaxOfficeLookUpOid, value);
            }
        }

        [NonPersistent]
        public TaxOffice TaxOfficeLookUp
        {
            get
            {
                return this.Session.FindObject<TaxOffice>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.TaxOfficeLookUpOid));
            }
            set
            {
                if (value == null)
                {
                    this.TaxOfficeLookUpOid = null;
                }
                else
                {
                    this.TaxOfficeLookUpOid = value.Oid;
                }
            }
        }
    }
}

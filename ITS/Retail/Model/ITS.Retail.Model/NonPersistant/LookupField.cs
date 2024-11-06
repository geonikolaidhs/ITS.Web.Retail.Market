//-----------------------------------------------------------------------
// <copyright file="LookupField.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using System.Collections.Generic;
using DevExpress.Xpo.DB;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using DevExpress.Data.Filtering;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{
    [NonPersistent]
    public class LookupField : BaseObj
    {
        public LookupField()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public LookupField(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            IsDefault = false;
        }
        public LookupField(string description) : base()
        {
            _Description = description;
        }
        public LookupField(Session session, string description)
            : base(session)
        {
            _Description = description;
            _IsDefault = false;
        }
        private string _Description;
        [Indexed, Size(1024)]
        [DescriptionField]
        [DisplayOrder(Order = 2)]
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

        public bool Update(LookupField lf)
        {
            try
            {
                Description = lf.Description;
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool _IsDefault;
        public bool IsDefault
        {
            get
            {
                return _IsDefault;
            }
            set
            {
                SetPropertyValue("IsDefault", ref _IsDefault, value);
            }
        }
        protected override void OnSaving()
        {
            if (this.SkipOnSavingProcess)
            {
                base.OnSaving();
                return;
            }
            if (IsDefault)
            {
                CriteriaOperator crop = CriteriaOperator.And(
                    new BinaryOperator("IsDefault", true, BinaryOperatorType.Equal),
                    new NotOperator(new BinaryOperator("Oid", Oid))
                    );
                if (typeof(IOwner).IsAssignableFrom(this.GetType()))
                {
                    crop = CriteriaOperator.And(crop,
                        CriteriaOperator.Or(
                            new BinaryOperator("Owner", this.GetPropertyValue("Owner")),
                            new NullOperator("Owner")
                        )
                    );
                }
                dynamic obj = this.Session.FindObject(this.GetType(), crop);
                if (obj != null && obj.Oid != Oid)
                {
                    throw new Exception("Is Default already exist");
                }
            }
            base.OnSaving();
        }

    }
}
//-----------------------------------------------------------------------
// <copyright file="MLString.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using DevExpress.Data.Filtering;
using System.Linq;

namespace ITS.Retail.Model
{
    [NonPersistent]
    public class MLString<T> : BaseObj where T : BaseObj
    {
        public MLString()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public MLString(Session session)
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


        public virtual object Parent
        {
            get
            {
                return null;
            }
        }

        // Fields...
        private string _Locale;

        [Indexed(Unique = false), Size(10)]
        public string Locale
        {
            get
            {
                return _Locale;
            }
            set
            {
                SetPropertyValue("Locale", ref _Locale, value);
            }
        }

        private string _Description;
        //[Indexed, Size(2047)]
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
            if (IsDefault && !IsDeleted)
            {
                CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("IsDefault", true, BinaryOperatorType.Equal),
                                                             new BinaryOperator("Locale", this.Locale, BinaryOperatorType.Equal)
                                                            );

                XPCollection objs = new XPCollection(this.Session, this.GetType(), crop);
                foreach (dynamic obj in objs)
                {
                    if (obj.Oid != this.Oid && obj.Parent == this.Parent)
                    {
                        throw new Exception("Is Default already exist");
                    }
                }
            }
            base.OnSaving();
        }


        //public override void GetData(Session myses, object item) {
        //    base.GetData(myses, item);
        //    MLString<T> mls = item as MLString<T>;
        //    Locale = mls.Locale;
        //}

    }
}

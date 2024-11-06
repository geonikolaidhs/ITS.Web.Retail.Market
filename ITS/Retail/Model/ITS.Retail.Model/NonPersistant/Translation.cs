//-----------------------------------------------------------------------
// <copyright file="ItemTranslation.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using System.Threading;
using System.Linq;
using System.Collections.Generic;


namespace ITS.Retail.Model {

    public abstract class Translation<T> : BaseObj where T : BaseObj {
        public Translation()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public Translation(Session session)
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


        private string _ObjectName;
        private string _FieldName;

        public string ObjectName
        {
            get
            {
                return _ObjectName;
            }
            set
            {
                SetPropertyValue("ObjectName", ref _ObjectName, value);
            }
        }

        public string FieldName
        {
            get
            {
                return _FieldName;
            }
            set
            {
                SetPropertyValue("FieldName", ref _FieldName, value);
            }
        }
        //[Association("ItemTranslation-FieldTranslations")]
        protected abstract List<MLString<T>> GetItemFieldTranslations();

        public override string ToString()
        {
            string locale = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            return ToString(locale);
        }

        public string ToString(string locale)
        {
            IEnumerable<MLString<T>> ift = GetItemFieldTranslations().Where(g => g.Locale == locale);
            if (ift.Count() == 1)
            {
                return ift.First().Description;
            }
            else if (GetItemFieldTranslations().Where(g => g.IsDefault).Count() == 1)
            {
                return GetItemFieldTranslations().Where(g => g.IsDefault).First().Description;
            }
            return "";
        }

    }
}

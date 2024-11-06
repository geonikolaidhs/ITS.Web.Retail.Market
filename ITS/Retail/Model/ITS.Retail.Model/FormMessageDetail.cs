//-----------------------------------------------------------------------
// <copyright file="ItemMLString.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;

namespace ITS.Retail.Model {

    public class FormMessageDetail : MLString<BaseObj> {
        public FormMessageDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public FormMessageDetail(Session session)
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
        private FormMessage _FormMessage;

		public override object Parent
		{
			get
			{
				return FormMessage;
			}
		}

        [Association("FormMessage-FormMessageDetails")]
        [Indexed("Locale;GCRecord", Unique = true)]
        public FormMessage FormMessage {
            get {
                return _FormMessage;
            }
            set {
                SetPropertyValue("FormMessage", ref _FormMessage, value);
            }
        }
    }
}

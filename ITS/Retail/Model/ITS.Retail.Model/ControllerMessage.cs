//-----------------------------------------------------------------------
// <copyright file="ControllerMessage.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using System.Linq;
using System.Collections.Generic;


namespace ITS.Retail.Model {

    public class ControllerMessage : LookupField, IOwner {
        public ControllerMessage()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public ControllerMessage(Session session)
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
        private CompanyNew _Owner;

        [Indexed("GCRecord", Unique = false)]
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

        [Association("FormMessage-ControllerMessages")]
        [Indexed("GCRecord", Unique=false)]
        public FormMessage FormMessage {
            get {
                return _FormMessage;
            }
            set {
                SetPropertyValue("FormMessage", ref _FormMessage, value);
            }
        }

        //public override void GetData(Session myses, object item) {
        //    base.GetData(myses, item);
        //    ControllerMessage fm = item as ControllerMessage;
        //    FormMessage = GetLookupObject<FormMessage>(myses, fm.FormMessage) as FormMessage;
        //    Owner = GetLookupObject<CompanyNew>(myses, fm.Owner);

        //}
    }
}

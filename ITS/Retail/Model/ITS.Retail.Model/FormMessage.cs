//-----------------------------------------------------------------------
// <copyright file="FormMessage.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using System.Linq;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;
using System.Collections.Generic;

namespace ITS.Retail.Model
{
    // ITS(c)


    [Serializable]
    public class FormMessage : Translation<BaseObj>, IOwner
    {
        public FormMessage() : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public FormMessage(Session session) : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        [DescriptionField]
        public string Description
        {
            get
            {
                return FieldName;
            }
        }

        // Fields...
        private eOwnershipScope _OwnershipScope;
        private eMessageType _MessagePlace;
        public eMessageType MessagePlace
        {
            get
            {
                return _MessagePlace;
            }
            set
            {
                SetPropertyValue("MessagePlace", ref _MessagePlace, value);
            }
        }


        private CompanyNew _Owner;
        [Association("Supplier-FormMessages")]
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


        public eOwnershipScope OwnershipScope
        {
            get
            {
                return _OwnershipScope;
            }
            set
            {
                SetPropertyValue("OwnershipScope", ref _OwnershipScope, value);
            }
        }

        [Aggregated, Association("FormMessage-ControllerMessages")]
        [DisplayOrder(Order = 2)]
        public XPCollection<ControllerMessage> ControllerMessages
        {
            get
            {
                return GetCollection<ControllerMessage>("ControllerMessages");
            }
        }

        [Aggregated, Association("FormMessage-FormMessageDetails")]
        [DisplayOrder(Order = 1)]
        public XPCollection<FormMessageDetail> FormMessageDetails
        {
            get
            {
                return GetCollection<FormMessageDetail>("FormMessageDetails");
            }
        }

        protected override List<MLString<BaseObj>> GetItemFieldTranslations()
        {
            return FormMessageDetails.Cast<MLString<BaseObj>>().ToList();       
        }
    }
}

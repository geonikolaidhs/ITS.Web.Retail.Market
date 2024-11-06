using DevExpress.Xpo;

using ITS.Retail.Platform.Enumerations;


using ITS.WRM.Model.Interface;

using SFA_Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 260, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class BarcodeType : LookUp2Fields, IBarcodeType, ITS.WRM.Model.Interface.Model.SupportingClasses.IRequiredOwner
    {
        public BarcodeType()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public BarcodeType(Session session)
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

        public string EntityType { get; set; }
        
        public bool HasMixInformation { get; set; }

        public bool IsWeighed { get; set; }

        public string Mask { get; set; }

        public bool NonSpecialCharactersIncluded { get; set; }

        public string Prefix { get; set; }

        public bool PrefixIncluded { get; set; }


    }
}
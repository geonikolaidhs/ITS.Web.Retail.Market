using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;

namespace ITS.Retail.Model
{
    /// <summary>
    /// A many to many relationship ONLY for weighted BarcodeTypes
    /// </summary>
    [Updater(Order = 274, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class DocumentTypeBarcodeType : BaseObj
    {
        private DocumentType _DocumentType;
        private BarcodeType _BarcodeType;

        public DocumentTypeBarcodeType()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentTypeBarcodeType(Session session)
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


        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    
                    if (owner == null)
                    {
                        throw new Exception("DocumentTypeBarcodeType.GetUpdaterCriteria(); Owner: Owner is null");
                    }

                    crop = new BinaryOperator("DocumentType.Owner.Oid", owner.Oid);
                    break;
                
            }
            return crop;
        }

        [Association("DocumentType-DocumentTypeBarcodeTypes")]
        public DocumentType DocumentType
        {
            get
            {
                return _DocumentType;
            }
            set
            {
                SetPropertyValue("DocumentType", ref _DocumentType, value);
            }
        }

        [Association("BarcodeType-BarcodeTypeBarcodeTypes")]
        public BarcodeType BarcodeType
        {
            get
            {
                return _BarcodeType;
            }
            set
            {
                SetPropertyValue("BarcodeType", ref _BarcodeType, value);
            }
        }
    }
}

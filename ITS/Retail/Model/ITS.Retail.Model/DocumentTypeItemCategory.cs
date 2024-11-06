using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    [Updater(Order = 445, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class DocumentTypeItemCategory : BaseObj
    {
        private DocumentType _DocumentType;
        private ItemCategory _ItemCategory;
        public DocumentTypeItemCategory()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentTypeItemCategory(Session session)
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

        [Association("DocumentType-DocumentTypeItemCategories")]
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

        public ItemCategory ItemCategory
        {
            get
            {
                return _ItemCategory;
            }
            set
            {
                SetPropertyValue("ItemCategory", ref _ItemCategory, value);
            }
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (store == null)
                    {
                        throw new Exception("DocTypeCustomerCategory.GetUpdaterCriteria(); Error: Store is null");
                    }
                    crop = new BinaryOperator("DocumentType.Owner.Oid", supplier.Oid);
                    break;
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    crop =
                        new ContainsOperator("DocumentType.StoreDocumentSeriesTypes",
                        CriteriaOperator.Or(
                            new BinaryOperator("DocumentSeries.eModule", eModule.POS),
                            new ContainsOperator("POSStoreDocumentSeriesTypes", new BinaryOperator("POS.Oid", deviceID))
                            ));
                    break;
            }
            return crop;
        }
    }
}

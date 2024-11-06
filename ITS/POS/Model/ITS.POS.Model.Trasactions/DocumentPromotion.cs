using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Master;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.POS.Model.Transactions
{
    [SyncInfoIgnoreAttribute]
    public class DocumentPromotion : BaseObj, IDocumentPromotion
    {
        public DocumentPromotion()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentPromotion(Session session)
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

        private Guid _Promotion;
        private DocumentHeader _DocumentHeader;
        private string _PromotionCode;
        private string _PromotionDescription;
        private decimal _TotalGain;
        private Guid _DocumentHeaderOid;

        [Association("DocumentHeader-DocumentPromotions")]
        public DocumentHeader DocumentHeader
        {
            get
            {
                return _DocumentHeader;
            }
            set
            {
                SetPropertyValue("DocumentHeader", ref _DocumentHeader, value);
                if (_DocumentHeader != null)
                {
                    this.DocumentHeaderOid = _DocumentHeader.Oid;
                }
            }
        }


        public Guid DocumentHeaderOid
        {
            get
            {
                return _DocumentHeaderOid;
            }
            set
            {
                SetPropertyValue("DocumentHeaderOid", ref _DocumentHeaderOid, value);
            }
        }

        [DenormalizedField("PromotionCode", typeof(Promotion), "ITS.Retail.Model.Promotion", "Code")]
        public Guid Promotion
        {
            get
            {
                return _Promotion;
            }
            set
            {
                SetPropertyValue("Promotion", ref _Promotion, value);
            }
        }

        public string PromotionCode
        {
            get
            {
                return _PromotionCode;
            }
            set
            {
                SetPropertyValue("PromotionCode", ref _PromotionCode, value);
            }
        }

        public string PromotionDescription
        {
            get
            {
                return _PromotionDescription;
            }
            set
            {
                SetPropertyValue("PromotionDescription", ref _PromotionDescription, value);
            }
        }

        public decimal TotalGain
        {
            get
            {
                return _TotalGain;
            }
            set
            {
                SetPropertyValue("TotalGain", ref _TotalGain, value);
            }
        }

        IDocumentHeader IDocumentPromotion.DocumentHeader
        {
            get
            {
                return DocumentHeader;
            }
            set
            {
                DocumentHeader = (DocumentHeader)value;
            }
        }

        [NonPersistent]
        public Guid PromotionOid
        {
            get
            {
                return Promotion;
            }
            set
            {
                Promotion = value;
            }
        }
       
    }
}

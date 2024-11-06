using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.Retail.Model
{
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

        private Promotion _Promotion;
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

        public Promotion Promotion
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

        [Size(SizeAttribute.Unlimited)]
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

        [NonPersistent]
        public Guid PromotionOid
        {
            get
            {
                if (this.Promotion == null)
                {
                    return Guid.Empty;
                }
                return this.Promotion.Oid;
            }
            set
            {
                if (value == null || value == Guid.Empty)
                {
                    this.Promotion = null;
                }
                Guid? guid = value as Guid?;
                if (!guid.HasValue)
                {
                    this.Promotion = null;
                }
                this.Promotion = this.Session.GetObjectByKey<Promotion>(guid.Value);
            }
        }

        [NonPersistent]
        IDocumentHeader IDocumentPromotion.DocumentHeader
        {
            get
            {
                return this.DocumentHeader;
            }
            set
            {
                this.DocumentHeader = value as DocumentHeader;
            }
        }


        decimal IDocumentPromotion.TotalGain
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        Guid IDocumentPromotion.PromotionOid
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string IDocumentPromotion.PromotionCode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string IDocumentPromotion.PromotionDescription
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        Guid IPersistentObject.Oid
        {
            get { throw new NotImplementedException(); }
        }

        Session IPersistentObject.Session
        {
            get { throw new NotImplementedException(); }
        }

        void IPersistentObject.Save()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Common.ViewModel;


namespace ITS.Retail.WebClient.ViewModel
{
    public class DocumentPromotionViewModel: IPersistableViewModel
    {
        public Guid Oid { get; set; }

        public Type PersistedType
        {
            get { return typeof(DocumentPromotion); }
        }

        public bool IsDeleted { get; set; }

        public void UpdateModel(Session uow)
        {
            this.UpdateProperties(uow);
        }

        public bool Validate(out string message)
        {
            throw new NotImplementedException();
        }

        public Guid Promotion { get; set; }
        public Guid DocumentHeader { get; set; }
        public string PromotionCode { get; set; }
        public string PromotionDescription { get; set; }
        public decimal TotalGain { get; set; }
    }
}

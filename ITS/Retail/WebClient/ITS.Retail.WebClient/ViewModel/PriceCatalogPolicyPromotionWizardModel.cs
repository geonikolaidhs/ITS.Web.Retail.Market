using DevExpress.Xpo;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{
    public class PriceCatalogPolicyPromotionWizardModel : IPersistableViewModel
    {
        public Guid Promotion { get; set; }
        public Guid PriceCatalogPolicy { get; set; }
        public Guid Oid { get; set; }
        public String PriceCatalogPolicyDescription { get; set; }

        public PriceCatalogPolicyPromotionWizardModel()
        {
            this.Oid = Guid.NewGuid();
        }

        public Type PersistedType
        {
            get
            {
                return typeof(PriceCatalogPolicyPromotion);
            }
        }

        [PersistableViewModel(NotPersistant = true)]
        public bool IsDeleted { get; set; }

        public void UpdateModel(Session uow)
        {
            PriceCatalogPolicy policy = uow.GetObjectByKey<PriceCatalogPolicy>(this.PriceCatalogPolicy);
            if (policy != null)
            {
                this.PriceCatalogPolicyDescription = policy.Description;
            }
        }

        public bool Validate(out string message)
        {
            message = "";
            return true;
        }
    }
}
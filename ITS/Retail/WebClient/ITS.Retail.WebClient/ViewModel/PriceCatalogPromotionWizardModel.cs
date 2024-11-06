using DevExpress.Xpo;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{
    public class PriceCatalogPromotionWizardModel : IPersistableViewModel
    {

        public Guid Promotion { get; set; }
        public Guid PriceCatalog { get; set; }
        public Guid Oid { get; set; }
        public String PriceCatalogDescription { get; set; }

        public PriceCatalogPromotionWizardModel()
        {
            this.Oid = Guid.NewGuid();
        }

        public Type PersistedType
        {
            get { return typeof(PriceCatalogPromotion); }
        }

        [PersistableViewModel(NotPersistant = true)]
        public bool IsDeleted { get; set; }

        public void UpdateModel(Session uow)
        {
            PriceCatalog pc = uow.GetObjectByKey<PriceCatalog>(this.PriceCatalog);
            if(pc != null)
            {
                this.PriceCatalogDescription = pc.Description;
            }
        }

        public bool Validate(out string message)
        {
            message = "";
            return true;
        }
    }
}
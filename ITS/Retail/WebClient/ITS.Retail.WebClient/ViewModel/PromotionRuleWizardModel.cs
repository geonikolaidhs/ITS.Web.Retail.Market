using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Web;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.Model;
using ITS.Retail.WebClient.AuxillaryClasses;
using System.Reflection;
using System.Collections;
using ITS.Retail.Common.ViewModel;

namespace ITS.Retail.WebClient.ViewModel
{
    public abstract class PromotionRuleWizardModel : IPersistableViewModel
    {
        public abstract bool Validate(out string message);

        public abstract IEnumerable<PromotionRuleWizardModel> AllChilds { get; }

        public abstract Type PersistedType { get; }

        public Guid Oid { get; set; }

        public Guid? ParentOid { get; set; }

        [PersistableViewModel(NotPersistant=true)]
        public abstract String Description { get; }

        [PersistableViewModel(NotPersistant = true)]
        public PromotionRuleWizardModel This { get { return this; } }

        [PersistableViewModel(NotPersistant = true)]
        public bool IsDeleted { get; set; }

        public PromotionRuleWizardModel()
        {
            Oid = Guid.NewGuid();
        }

        public virtual void UpdateModel(Session uow)
        {

        }

        public object ShallowCopy()
        {
            return MemberwiseClone();
        }
    }
}

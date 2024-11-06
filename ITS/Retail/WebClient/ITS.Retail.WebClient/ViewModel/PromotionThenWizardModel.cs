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
    public abstract class PromotionThenWizardModel : IPersistableViewModel
    {
        public virtual bool Validate(out string message)
        {
            message = "";
            return true;
        }

        public abstract Type PersistedType { get; }
        public Guid Oid { get; set; }

        [PersistableViewModel(NotPersistant = true)]
        public bool IsDeleted { get; set; }

        [PersistableViewModel(NotPersistant = true)]
        public PromotionThenWizardModel This { get { return this; } }

        [PersistableViewModel(NotPersistant = true)]
        public string Type
        {
            get
            {
                return this.GetType().GetProperty("PersistedType").GetDisplayName();
            }
        }

        public PromotionThenWizardModel()
        {
            Oid = Guid.NewGuid();
        }

        public virtual void UpdateModel(Session uow)
        {

        }

        public T ShallowCopy<T>() where T : PromotionThenWizardModel
        {
            return (T)(MemberwiseClone());
        }

        public object ShallowCopy()
        {
            return MemberwiseClone();
        }
    }
}

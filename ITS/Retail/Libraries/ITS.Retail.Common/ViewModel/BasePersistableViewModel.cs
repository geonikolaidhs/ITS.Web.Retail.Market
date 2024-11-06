using DevExpress.Xpo;
using System;

namespace ITS.Retail.Common.ViewModel
{
    public abstract class BasePersistableViewModel : BasicViewModel, IPersistableViewModel
    {
        // Fields...
        private bool _IsDeleted;
        private Guid _Oid;
        private DateTime _CreatedOn;
        private DateTime _UpdatedOn;

        public BasePersistableViewModel()
        {
            this.Oid = Guid.NewGuid();
            this.CreatedOn = DateTime.Now;
            this.UpdatedOn = DateTime.Now;
        }

        public Guid Oid
        {
            get
            {
                return _Oid;
            }
            set
            {
                SetPropertyValue("Oid", ref _Oid, value);
            }
        }

        public abstract Type PersistedType { get; }


        [PersistableViewModel(NotPersistant = true)]
        public object This
        {
            get
            {
                return this;
            }
        }

        [PersistableViewModel(NotPersistant = true)]
        public bool IsNew { get; set; }

        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                SetPropertyValue("IsDeleted", ref _IsDeleted, value);
            }
        }

        public DateTime CreatedOn
        {
            get
            {
                return _CreatedOn;
            }
            set
            {
                SetPropertyValue("CreatedOn", ref _CreatedOn, value);
            }
        }

        public DateTime UpdatedOn
        {
            get
            {
                return _UpdatedOn;
            }
            set
            {
                SetPropertyValue("UpdatedOn", ref _UpdatedOn, value);
            }
        }

        public virtual void UpdateModel(Session uow)
        {
            //throw new NotImplementedException();
        }

        public virtual bool Validate(out string message)
        {
            message = "";
            //throw new NotImplementedException();
            return true;
        }
    }
}

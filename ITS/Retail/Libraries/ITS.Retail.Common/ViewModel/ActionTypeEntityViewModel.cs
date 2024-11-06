using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common.ViewModel
{
    public class ActionTypeEntityViewModel : BasePersistableViewModel
    {
        private Guid _EntityOid;
        private string _EntityType;
        private Guid _ActionType;

        public override Type PersistedType
        {
            get 
            {
                return typeof(ActionTypeEntity);
            }
        }

        public Guid EntityOid
        {
            get
            {
                return _EntityOid;
            }
            set
            {
                SetPropertyValue("EntityOid", ref _EntityOid, value);
            }
        }

        public string EntityType
        {
            get
            {
                return _EntityType;
            }
            set
            {
                SetPropertyValue("EntityType", ref _EntityType, value);
            }
        }

        [Binding("ActionType_VI")]
        public Guid ActionType
        {
            get
            {
                return _ActionType;
            }
            set
            {
                SetPropertyValue("ActionType", ref _ActionType, value);
            }
        }
    }
}

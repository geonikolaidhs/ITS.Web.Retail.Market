using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    public class WRMApplicationSettings : BaseObj
    {
        public WRMApplicationSettings(Session session) : base(session)
        {
        }


        private eApplicationInstance _ApplicationInstance;

        public eApplicationInstance ApplicationInstance
        {
            get
            {
                return _ApplicationInstance;
            }
            set
            {
                SetPropertyValue("ApplicationInstance", ref _ApplicationInstance, value);
            }
        }

        [NonPersistent]
        public int ApplicationInstanceInteger
        {
            get
            {
                return (int)this.ApplicationInstance;
            }
            set
            {
                this.ApplicationInstance = (eApplicationInstance)value;
            }
        }
    }
}

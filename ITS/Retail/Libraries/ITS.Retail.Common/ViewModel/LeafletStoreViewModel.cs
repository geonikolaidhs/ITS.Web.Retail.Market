using ITS.Retail.Model;
using System;

namespace ITS.Retail.Common.ViewModel
{
    public class LeafletStoreViewModel : BasePersistableViewModel
    {
        private Guid _Leaflet;
        private Guid _Store;

        public override Type PersistedType { get { return typeof(LeafletStore); } }

        public Guid Leaflet
        {
            get
            {
                return _Leaflet;
            }
            set
            {
                SetPropertyValue("Leaflet", ref _Leaflet, value);
            }
        }

        public Guid Store
        {
            get
            {
                return _Store;
            }
            set
            {
                SetPropertyValue("Store", ref _Store, value);
            }
        }
    }
}

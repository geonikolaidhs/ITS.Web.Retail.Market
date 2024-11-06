using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using System;

namespace ITS.Retail.WebClient.ViewModel
{
    public class LicenseUserDistributionViewModel : BasePersistableViewModel, IPersistableViewModel
    {
        public int MaxConnectedUsers { get; set; }
        public int MaxPeripheralsUsers { get; set; }
        public int MaxTabletSmartPhoneUsers { get; set; }
        public Guid Server { get; set; }
        public LicenseServerInstance LicenseServerInstance { get; set; }
        public string Description { get; set; }

        public override Type PersistedType
        {
            get {
                return typeof(LicenseUserDistribution);
            }
        }
    }
}
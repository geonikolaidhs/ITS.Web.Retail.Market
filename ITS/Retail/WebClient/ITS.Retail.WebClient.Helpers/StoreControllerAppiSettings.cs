using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.Retail.Model;
using ITS.Retail.Common;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;

namespace ITS.Retail.WebClient.Helpers
{
	public static class StoreControllerAppiSettings
    {
		public static Guid CurrentStoreOid { get; set; }
		public static int ID = int.MinValue;
		public static string MasterURL { get; set; }
        public static Guid DefaultCustomerOid { get; set; }
        public static Guid StoreControllerOid { get; set; }
		const string ServiceName = "POSUpdateService.asmx";
        public static string TransactionFilesFolder
        {
            get
            {
                if (CurrentStore == null || CurrentStore.StoreControllerSettings == null )
                {
                    return string.Empty;
                }
                return CurrentStore.StoreControllerSettings.TransactionFilesFolder;
            }
        }
                
        public static Customer DefaultCustomer
        {
            get
            {
                return XpoHelper.GetNewUnitOfWork().GetObjectByKey<Customer>(DefaultCustomerOid);
            }
        }
                
		public static Store CurrentStore
		{
			get
			{
                return XpoHelper.GetNewUnitOfWork().GetObjectByKey<Store>(CurrentStoreOid);
			}
		}

		public static string MasterServiceURL
		{
			get
			{
				return MasterURL == null ? null : MasterURL.TrimEnd('/') + "/" + ServiceName;
			}
		}

        public static CompanyNew Owner
        {
            get
            {
                return CurrentStore.Owner;
            }
        }

        public static OwnerApplicationSettings OwnerApplicationSettings
        {
            get
            {
                if (CurrentStore.Owner.OwnerApplicationSettings != null)
                {
                    return CurrentStore.Owner.OwnerApplicationSettings;
                }
                return CurrentStore.Session.FindObject<OwnerApplicationSettings>(new NullOperator("Owner"));
            }
        }
    }
}
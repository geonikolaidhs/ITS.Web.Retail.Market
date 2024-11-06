using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using System;

namespace ITS.Retail.WebClient.Helpers
{
    public static class LicenseUserDistributionHelper
    {
        public static int GetMaximumAllowedUsers(LicenseServerInstance licenseServerInstance, Guid server, eApplicationInstance applicationInstance , int licenseManagerMaxUsers)
        {
            if (applicationInstance == eApplicationInstance.RETAIL)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    CriteriaOperator masterLicenceCriteria = GetHeadquartersDistributionCriteria();//RetailHelper.ApplyOwnerCriteria
                    LicenseUserDistribution licenseUserDistribution = uow.FindObject<LicenseUserDistribution>(masterLicenceCriteria);
                    if (licenseUserDistribution != null)
                    {
                        return licenseUserDistribution.MaxConnectedUsers;
                    }
                }
                return licenseManagerMaxUsers;
            }

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("LicenseServerInstance", licenseServerInstance),
                                                                 new BinaryOperator("Server", server));
                LicenseUserDistribution licenseUserDistribution = uow.FindObject<LicenseUserDistribution>(criteria);
                if (licenseUserDistribution != null)
                {
                    return licenseUserDistribution.MaxConnectedUsers;
                }
            }
            return 0;
        }

        public static CriteriaOperator GetHeadquartersDistributionCriteria()
        {
            return CriteriaOperator.And(new BinaryOperator("Description", Platform.PlatformConstants.HEADQUARTERS),
                                                                       new BinaryOperator("Server", Guid.Empty)
                                                                     );
        }
    }
}

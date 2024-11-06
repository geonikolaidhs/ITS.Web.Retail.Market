using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.WebClient.Helpers
{
    public static class DocumentSeriesHelper
    {
        public static bool DocumentSeriesNumberCanBeEdited(DocumentSeries documentSeries, User user,eApplicationInstance applicationInstance)
        {
            if (!UserHelper.IsAdmin(user))
            {
                return false;
            }

            switch (applicationInstance)
            {
                case eApplicationInstance.DUAL_MODE:
                    return true;
                case eApplicationInstance.RETAIL:
                    return documentSeries.eModule == eModule.HEADQUARTERS || documentSeries.eModule == eModule.NONE;
                case eApplicationInstance.STORE_CONTROLER:
                    return documentSeries.eModule== eModule.STORECONTROLLER || documentSeries.eModule == eModule.POS;
                default:
                    return false;
            }
        }
    }
}

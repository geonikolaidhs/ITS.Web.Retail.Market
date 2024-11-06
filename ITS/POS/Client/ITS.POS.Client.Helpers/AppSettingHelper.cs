using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using DevExpress.Xpo;

namespace ITS.POS.Client.Helpers
{
    public class AppSettingHelper
    {
        private static OwnerApplicationSettings AppSettings;
        public static bool UpdateApplicationSettings { get; set; }
        //public AppSettingHelper()
        //{
        //}

        public static OwnerApplicationSettings GetAppSettings()
        {
            if (AppSettings == null || UpdateApplicationSettings)
            {
                AppSettings = SessionHelper.FindObject<OwnerApplicationSettings>(null);
                UpdateApplicationSettings = false;
            }
            return AppSettings;
        }
    }
}

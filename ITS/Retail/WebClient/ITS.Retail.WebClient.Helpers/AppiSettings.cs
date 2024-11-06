using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.Retail.Model;
using System.Drawing;
using ITS.Retail.Platform.Enumerations;


namespace ITS.Retail.WebClient.Helpers
{
    public static class AppiSettings
    {
        public static Image MainScreenImage;
        public static Image MenuLogo;     
        public static LogLevel LoggingLevel;
        public static int LogSQLSelectEntriesLimit;

        /// <summary>
        /// Sleep time for totaliZer Tasks in msec
        /// </summary>
        public static int ActionTypeTriggerSleepTime;
        //public static int ActionTypeTriggerReloadObjectTries;

        public static void ReadSettings(ApplicationSettings application_settings)
        {
            if (application_settings != null)
            {
                AppiSettings.MainScreenImage = application_settings.MainScreenImage;
                AppiSettings.MenuLogo = application_settings.MenuLogo;
                AppiSettings.LoggingLevel = application_settings.LogingLevel;
                AppiSettings.LogSQLSelectEntriesLimit = 1000;

                AppiSettings.ActionTypeTriggerSleepTime = 500;               
            }
        }
    }
}
using ITS.POS.Resources;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Handles the runtime translation of display texts
    /// </summary>
    public class LocalizationResolver : ILocalizationResolver
    {
        private Logger Logger { get; set; }
        private IApplicationDesignModeProvider ApplicationDesignModeProvider { get; set; }

        public LocalizationResolver(Logger logger, IApplicationDesignModeProvider applicationDesignModeProvider)
        {
            Logger = logger;
            ApplicationDesignModeProvider = applicationDesignModeProvider;
        }

        public string ResolveDisplayText(string displayText)
        {
            string result = null;
            bool triedToLocalize = false;
            if (displayText.StartsWith("@@"))
            {
                triedToLocalize = true;
                result = POSClientResources.ResourceManager.GetString(displayText.TrimStart('@'));
            }
            else if (displayText.StartsWith("^@"))
            {
                triedToLocalize = true;
                result = POSClientResources.ResourceManager.GetString(displayText.TrimStart('^').TrimStart('@'));
                if (result != null)
                {
                    result = result.ToUpperGR();
                }
            }

            bool designMode = ApplicationDesignModeProvider.ApplicationIsInDesignMode();
            if (string.IsNullOrEmpty(result) && triedToLocalize && designMode == false)
            {
                Logger.Debug(String.Format("Not Translated Message : {0}", displayText));
            }
            return String.IsNullOrWhiteSpace(result) ? displayText : result;
        }
    }
}

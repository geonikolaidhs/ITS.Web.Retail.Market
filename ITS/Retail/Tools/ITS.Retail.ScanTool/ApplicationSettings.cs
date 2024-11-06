using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ITS.Common.Attributes;
using TwainDotNet;
using TwainDotNet.WinFroms;

namespace ITS.Retail.ScanTool
{
    public static class ApplicationSettings
    {
        [Description("Default Scanner Name"), Lookup(type=typeof(ApplicationSettings),Method="GetSources") ]
        public static string DefaultScanner { get; set; }

        [Description("Remote URL")]
        public static string Url { get; set; }

        [Description("Use Document Feeder")]
        public static bool UseDocumentFeeder { get; set; }

        [Description("Show Scanner User Interface")]
        public static bool ShowTwainUI { get; set; }

        [Description("Use Duplex Mode Scanning")]
        public static bool UseDuplex { get; set; }

        [Description("Show Progress Indicator Interface")]
        public static bool ShowProgressIndicatorUI { get; set; }

        [Description("Automatic Border Detection")]
        public static bool AutomaticBorderDetection { get; set; }

        public static IEnumerable<String> GetSources()
        {

            DataSourceManager dsm = new DataSourceManager(DataSourceManager.DefaultApplicationId, new WinFormsWindowMessageHook(Program.mainWindow));
            List<DataSource> sources = DataSource.GetAllSources(dsm.ApplicationId, dsm.MessageHook);
            return sources.Select(g => g.SourceId.ProductName);            
        }

    }

   
}

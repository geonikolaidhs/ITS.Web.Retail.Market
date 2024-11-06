using System;
using System.Collections.Generic;
using System.Text;
using ITS.MobileAtStore.ObjectModel;
using System.Reflection;

namespace ITS.MobileAtStore.Common.ApplicationExportSettings
{
    public abstract class ApplicationExportSettings : IXmlSubitems
    {
        public string ServerMapPathRoot{get;set;}
        protected eApplication Application;
        public ConnectionSettings ConnectionSettings{get;set;}
        public string remoteIP { get; set; }

        public ApplicationExportSettings()
        {
            InitialiaseConnectionSettings();
        }

        public ApplicationExportSettings(string serverMapPathRoot, ConnectionSettings connectionSettings, Dictionary<string, object> applicationSpecificSettings)
        {
            this.ServerMapPathRoot = serverMapPathRoot;
            this.ConnectionSettings = connectionSettings;
            InitialiaseConnectionSettings();
            InitialiaseSpecificAttributes(applicationSpecificSettings);
        }

        private void InitialiaseConnectionSettings()
        {
            if (this.ConnectionSettings == null)
            {
                this.ConnectionSettings = new ConnectionSettings();
            }
            Initialise();            
        }

        private void InitialiaseSpecificAttributes(Dictionary<string,object> applicationSpecificSettings)
        {
            if(applicationSpecificSettings==null || applicationSpecificSettings.Count <= 0)
            {
                return;
            }

            foreach(KeyValuePair<string,object> pair in applicationSpecificSettings )
            {
                PropertyInfo property = this.GetType().GetProperty(pair.Key);
                if(property==null)
                {
                    throw new Exception("Property "+pair.Key+" not found!");
                }

                if (property.CanWrite == false)
                {
                    throw new Exception("Property " + pair.Key + " can not be set because it is read only!");
                }

                property.SetValue(this, pair.Value, null);
            }
        }


        /// <summary>
        /// Performs Document (document variable) export to the specified application.
        /// </summary>
        /// <param name="document">The document to be exported</param>
        /// <param name="remoteIP">Device remote IP</param>
        /// <param name="logMessage">An output string containing details of the function outcome. For example on failure it contains details explaining why it failed.</param>
        /// <param name="applicationSpecificSettings">A list of application specific parameters where string is the parameter name and the object is the relative value.</param>
        /// <returns>True on succes. Otherwise false.</returns>
        public abstract bool PerformExport(Header document,string remoteIP,out string logMessage);

        /// <summary>
        /// Write here specific initialisation of the class.
        /// </summary>
        public abstract void Initialise();
    }
}

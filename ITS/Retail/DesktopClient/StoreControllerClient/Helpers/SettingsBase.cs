using ITS.Retail.Common.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Helpers
{
    public abstract class SettingsBase : BasicViewModel
    {
        /// <summary>
        /// Gets or sets the full file path of the save point
        /// </summary>
        [XmlIgnore]
        public string Filename { get; private set; }

        /// <summary>
        /// Gets or set if the file is automatically saved after every change
        /// </summary>
        [XmlIgnore]
        public bool AutoSave { get; private set; }

        public SettingsBase(string fullfilename, bool autoSave)
        {
            AfterConstrunction();
            this.Filename = fullfilename;
            this.AutoSave = autoSave;
        }

        public virtual void AfterConstrunction()
        {

        }
        /// <summary>
        /// Saves the current 
        /// </summary>
        public void Save()
        {
            using (TextWriter writeFileStream = new StreamWriter(Filename, false))
            {
                XmlSerializer ser = new XmlSerializer(this.GetType());
                ser.Serialize(writeFileStream, this);
            }
        }

        public void Load()
        {
            if (File.Exists(Filename))
            {
                try
                {
                    using (TextReader readFileStream = new StreamReader(Filename))
                    {
                        XmlSerializer ser = new XmlSerializer(this.GetType());
                        BasicViewModel readed = ser.Deserialize(readFileStream) as BasicViewModel;
                        Copy(readed);
                    }
                }
                catch(Exception ex)
                {
#if !_OFFICE_LOADER
                    Program.Logger.Info(ex, "Error reading user settings");
#endif
                }
            }
        }

        protected override void Copy(BasicViewModel vm)
        {
            bool _old = this.AutoSave;
            AutoSave = false;
            base.Copy(vm);
            AutoSave = _old;
        }

        protected override void AfterPropertyChange(string propertyName)
        {
            base.AfterPropertyChange(propertyName);
            if(AutoSave)
            {
                Save();
            }
        }


    }
}

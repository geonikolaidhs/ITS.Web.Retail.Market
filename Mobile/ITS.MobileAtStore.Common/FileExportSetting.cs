using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.MobileAtStore.Common
{
    public class FileExportSetting
    {
        public String Description
        {
            get
            {
                return this.FileExportHasBeenSet ? FileExport.Descriprion : "";
            }
        }
        public String Filename { get; set; }
        public String FullFileName
        {
            get
            {
                return (this.FileExportHasBeenSet ? FileExport.FullPath.TrimStart().TrimEnd().TrimEnd('\\') + "\\" + this.Filename : this.Filename).TrimStart().TrimEnd();
            }
        }

        public String HeaderStringFormat { get; set; }
        public String LineStringFormat { get; set; }
        public OverwriteMode OverwriteMode { get; set; }
        public int Id { get; set; }
        public FileExportLocation FileExport
        {
            get
            {
                return Settings.FileExportSettings.Find(fileExport => fileExport.Id == this.Id);
            }
        }
        public bool FileExportHasBeenSet
        {
            get
            {
                return this.FileExport != null;
            }
        }
    }
}

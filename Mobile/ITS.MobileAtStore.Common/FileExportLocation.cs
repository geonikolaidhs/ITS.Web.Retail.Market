using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;

namespace ITS.MobileAtStore.Common
{
    public class FileExportLocation
    {
        public int Id { get; set; }
        public string FullPath { get; set; }
        public string Descriprion { get; set; }

        public FileExportLocation()
        {
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static List<FileExportLocation> GetFileExports()
        {
            return Settings.FileExportSettings;
        }

        [DataObjectMethod(DataObjectMethodType.Insert)]
        public static int InsertFileExport(string FullPath, string Descriprion)
        {
            int Id = Settings.FileExportSettings.Count == 0 ? 1 : Settings.FileExportSettings.Max(x => x.Id) + 1;

            Settings.FileExportSettings.Add(new FileExportLocation() { Id = Id, FullPath = FullPath, Descriprion = Descriprion });
            return 1;
        }

        [DataObjectMethod(DataObjectMethodType.Update)]
        public static int UpdateFileExport(int Id, string FullPath, string Descriprion)
        {
            FileExportLocation fe = Settings.FileExportSettings.Where(x => x.Id == Id).FirstOrDefault();
            if (fe == null)
            {
                return 0;
            }
            fe.FullPath = FullPath;
            fe.Descriprion = Descriprion;
            return 1;
        }

        [DataObjectMethod(DataObjectMethodType.Delete)]
        public static int DeleteFileExport(int Id)
        {
            FileExportLocation fe = Settings.FileExportSettings.Where(x => x.Id == Id).FirstOrDefault();
            if (fe == null)
            {
                return 0;
            }
            int timesFound = Settings.ExportSettingsDictionary.Where(exportSettings => exportSettings.Value.FileExportSettings.Count > 0 && exportSettings.Value.FileExportSettings.Where(f => f.Id == fe.Id).Count() > 0).Count();
            if (timesFound > 0)
            {
                throw new Exception("Η τοποθεσία εξαγωγής " + fe.Descriprion + " χρησιμοποιείται σε " + timesFound + " τύπο/τύπους Παραστατικού.");
            }
            Settings.FileExportSettings.Remove(fe);
            return 1;
        }
    }
}

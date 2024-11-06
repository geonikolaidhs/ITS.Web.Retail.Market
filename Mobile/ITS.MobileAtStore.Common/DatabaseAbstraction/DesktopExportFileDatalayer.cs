using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using ITS.MobileAtStore.ObjectModel;
using ITS.MobileAtStore.Common.DatabaseAbstraction.Enumerations;

namespace ITS.MobileAtStore.Common.DatabaseAbstraction
{
    public class DesktopExportFileDatalayer : AbstractDataLayer
    {
        public override DbConnection CreateConnection()
        {
            return null;
        }

        public override void Initialize()
        {

        }

        public ExportResult PerformDesktopExport(Header document)
        {
            return PerformExportFile(document);
        }

    }
}

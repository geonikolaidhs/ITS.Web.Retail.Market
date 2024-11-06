using System;

using System.Collections.Generic;
using System.Text;
using ITS.MobileAtStore.ObjectModel;
using ITS.Common.Utilities.Compact;

namespace ITS.MobileAtStore.Helpers
{
    public static class ExportHelper
    {
        public static bool ExportDocuments(DOC_TYPES documentType,bool forcedOfflineMode)
        {
            string errorMessage;
            if (Main.ExportDocument(documentType, forcedOfflineMode, out errorMessage))
            {
                if (documentType != DOC_TYPES.QUEUE_QR)
                {
                    MessageForm.Execute("Επιτυχία", "H εξαγωγή ήταν επιτυχής!\r\n" + errorMessage, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                }
                return true;
                    
            }
            return false;
        }
    }
}

using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    public class Label : LookUp2Fields, ILabel
    {
        public string DirectSQL { get; set; }
        
        public byte[] LabelFile { get; set; }

        public string LabelFileName { get; set; }

        public int PrinterEncoding { get; set; }

        public bool UseDirectSQL { get; set; }

    }
}
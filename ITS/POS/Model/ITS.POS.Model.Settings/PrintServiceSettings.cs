using DevExpress.Xpo;
using System;

namespace ITS.POS.Model.Settings
{
    [NonPersistent]
    public class PrintServiceSettings : BaseObj
    {
        public PrintServiceSettings()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PrintServiceSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        private Guid _RemotePrinterService;
        private string _PrinterNickName;

        public Guid RemotePrinterService
        {
            get
            {
                return _RemotePrinterService;
            }
            set
            {
                SetPropertyValue("RemotePrinterService", ref _RemotePrinterService, value);
            }
        }


        public string PrinterNickName
        {
            get
            {
                return _PrinterNickName;
            }
            set
            {
                SetPropertyValue("PrinterNickName", ref _PrinterNickName, value);
            }
        }
    }
}

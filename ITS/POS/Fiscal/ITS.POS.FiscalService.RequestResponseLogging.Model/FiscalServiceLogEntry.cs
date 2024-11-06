using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.FiscalService.RequestResponseLogging.Model
{
    public class FiscalServiceLogEntry : BasicObject
    {
        private int _Id;
        private DateTime _TimeStamp;
        private string _Sender;
        private string _Receiver;
        private string _Request;
        private string _Response;

        public FiscalServiceLogEntry()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public FiscalServiceLogEntry(Session session)
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

        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                SetPropertyValue("Id", ref _Id, value);
            }
        }

        public DateTime TimeStamp
        {
            get
            {
                return _TimeStamp;
            }
            set
            {
                SetPropertyValue("TimeStamp", ref _TimeStamp, value);
            }
        }

        public string Sender
        {
            get
            {
                return _Sender;
            }
            set
            {
                SetPropertyValue("Sender", ref _Sender, value);
            }
        }

        public string Receiver
        {
            get
            {
                return _Receiver;
            }
            set
            {
                SetPropertyValue("Receiver", ref _Receiver, value);
            }
        }

        public string Request
        {
            get
            {
                return _Request;
            }
            set
            {
                SetPropertyValue("Request", ref _Request, value);
            }
        }

        public string Response
        {
            get
            {
                return _Response;
            }
            set
            {
                SetPropertyValue("Response", ref _Response, value);
            }
        }
    }
}

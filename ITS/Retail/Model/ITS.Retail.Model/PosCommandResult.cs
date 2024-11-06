using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class PosCommandResult : BaseObj
    {

        public PosCommandResult()
          : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PosCommandResult(Session session)
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

        private Guid _PosOid;
        private string _PosCode;
        private string _CommandType;
        private string _Command;
        private string _CommandResult;
        private DateTime _ResultTime;


        public Guid PosOid
        {
            get
            {
                return _PosOid;
            }
            set
            {
                SetPropertyValue("PosOid", ref _PosOid, value);
            }
        }

        public string CommandType
        {
            get
            {
                return _CommandType;
            }
            set
            {
                SetPropertyValue("PosOid", ref _CommandType, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Command
        {
            get
            {
                return _Command;
            }
            set
            {
                SetPropertyValue("Command", ref _Command, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string CommandResult
        {
            get
            {
                return _CommandResult;
            }
            set
            {
                SetPropertyValue("CommandResult", ref _CommandResult, value);
            }
        }

        public string PosCode
        {
            get
            {
                return _PosCode;
            }
            set
            {
                SetPropertyValue("PosCode", ref _PosCode, value);
            }
        }

        public DateTime ResultTime
        {
            get
            {
                return _ResultTime;
            }
            set
            {
                SetPropertyValue("ResultTime", ref _ResultTime, value);
            }
        }





    }
}

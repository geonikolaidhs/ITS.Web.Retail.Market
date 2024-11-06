using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
namespace ITS.Retail.Common.ViewModel
{
    public class PosStatusViewModel: BasicViewModel
    {       
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                SetPropertyValue("ID", ref _ID, value);
            }
        }

        public string IPAddress
        {
            get
            {
                return _IPAddress;
            }
            set
            {
                SetPropertyValue("IPAddress", ref _IPAddress, value);
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }

        public bool IsAlive
        {
            get
            {
                return _IsAlive;
            }
            set
            {
                SetPropertyValue("IsAlive", ref _IsAlive, value);
            }
        }
        public bool IsCashierRegister
        {
            get
            {
                return _IsCashierRegister;
            }
            set
            {
                SetPropertyValue("IsCashierRegister", ref _IsCashierRegister, value);
            }
        }
        public DateTime MachineStatusDate
        {
            get
            {
                return _MachineStatusDate;
            }
            set
            {
                SetPropertyValue("MachineStatusDate", ref _MachineStatusDate, value);
            }
        }


        public eMachineStatus MachineStatus
        {
            get
            {
                return _MachineStatus;
            }
            set
            {
                SetPropertyValue("MachineStatus", ref _MachineStatus, value);
            }
        }


        public decimal XCount
        {
            get
            {
                return _XCount;
            }
            set
            {
                SetPropertyValue("XCount", ref _XCount, value);
            }
        }


        public decimal XValue
        {
            get
            {
                return _XValue;
            }
            set
            {
                SetPropertyValue("XValue", ref _XValue, value);
            }
        }

        public decimal ZCount
        {
            get
            {
                return _ZCount;
            }
            set
            {
                SetPropertyValue("ZCount", ref _ZCount, value);
            }
        }

        public decimal ZValue
        {
            get
            {
                return _ZValue;
            }
            set
            {
                SetPropertyValue("ZValue", ref _ZValue, value);
            }
        }

        // Fields...
        private decimal _ZCount;
        private decimal _ZValue;
        private decimal _XValue;
        private decimal _XCount;
        private ITS.Retail.Platform.Enumerations.eMachineStatus _MachineStatus;
        private DateTime _MachineStatusDate;
        private bool _IsAlive;
        private string _Name;
        private string _IPAddress;
        private int _ID;
        private bool _IsCashierRegister;
    }
}

using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ITS.Retail.Model.SupportingClasses
{
    [NonPersistent]
    public class CashRegisterItemUpdate : INotifyPropertyChanged
    {
        public CashRegisterItemUpdate()
        {
           
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChange(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        private Guid _POSOid;
        public Guid POSOid
        {
            get { return _POSOid; }
            set { if (_POSOid != value) { _POSOid = value; RaisePropertyChange("POSOid"); } }
        }
        private Guid _DeviceOid;
        public Guid DeviceOid
        {
            get { return _DeviceOid; }
            set { if (_DeviceOid != value) { _DeviceOid = value; RaisePropertyChange("DeviceOid"); } }
        }
        private bool _Selected;
        public bool Selected
        {
            get { return _Selected; }
            set {
                    if (_Selected != value)
                    {
                        _Selected = value;
                        RaisePropertyChange("Selected");
                    }
                }
        }
        private string _POSName;
        public string POSName
        {
            get { return _POSName; }
            set { if (_POSName != value) { _POSName = value; RaisePropertyChange("POSName"); } }
        }
        private string _DeviceName;
        public string DeviceName
        {
            get { return _DeviceName; }
            set { if (_DeviceName != value) { _DeviceName = value; RaisePropertyChange("DeviceName"); } }
        }
        private string _DeviceType;
        public string DeviceType
        {
            get { return _DeviceType; }
            set { if (_DeviceType != value) { _DeviceType = value; RaisePropertyChange("DeviceType"); } }
        }
        private DateTime _LastSuccefullyUpdate;
        public DateTime LastSuccefullyUpdate
        {
            get { return _LastSuccefullyUpdate; }
            set { if (_LastSuccefullyUpdate != value) { _LastSuccefullyUpdate = value; RaisePropertyChange("LastSuccefullyUpdate"); } }
        }
        private string _Progress;
        public string Progress
        {
            get { return _Progress; }
            set { if (_Progress != value) { _Progress = value; RaisePropertyChange("Progress"); } }
        }
        private decimal _ProgressPercent;
        public decimal ProgressPercent
        {
            get { return _ProgressPercent; }
            set { if (_ProgressPercent != value) { _ProgressPercent = value; RaisePropertyChange("ProgressPercent"); } }
        }
        public Guid ItemCategoryOid { get; set; }
        public Guid BarcodeTypeOid { get; set; }
        public Guid PriceCatalogOid { get; set; }
        public bool Running { get; set; }
        public int LastSendedItemIndex { get; set; }
        public int MaxItemsAdd { get; set; }
        public DailyTotal DayTotals { get; set; }
        public Object CashierUpdateItems { get; set; }
        public string CurrentAction { get; set; }
    }
}

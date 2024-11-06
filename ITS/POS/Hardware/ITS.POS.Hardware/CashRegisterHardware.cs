using ITS.POS.Hardware.Common;
using ITS.Retail.Model.SupportingClasses;
using ITS.Retail.Platform.Enumerations;
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace ITS.POS.Hardware
{
    public abstract class CashRegisterHardware : IDisposable
    {
        /*
        public CashRegisterHardware(Settings settings)
        {
            this.Settings = settings;
        }

        private Settings Settings;
        protected FiscalPrinter FiscalPrinter { get; set; }
        public abstract eDeviceCheckResult SendItemToDevice(int index, string Code, string ItemDescription, double itemPrice1, double itemMaxPrice, double itemPrice2, int Points, double stockQTY, int vatCode, out string message);
        public abstract void SendPriceToDevice();
        public abstract void GetDocumentFromDevice();
        public abstract eDeviceCheckResult CheckDevice(out string message);
        }*/
        private string _DeviceName;
        private int _ID;
        private ConnectionType _ConnectionType;
        private Settings _Settings;
        private DeviceType _DeviceType;
        public CashRegisterHardware(DeviceType deviceType, Settings settings, string device, int id, ConnectionType Type)
        {
            this._DeviceType = deviceType;
            this.Settings = settings;
            this._DeviceName = device;
            this._ID = id;
            this._ConnectionType = Type;
        }

        private Settings Settings;
        protected FiscalPrinter FiscalPrinter { get; set; }
        public abstract eDeviceCheckResult SendItemToDevice(int index, string Code, string ItemDescription, double itemPrice1, double itemMaxPrice, double itemPrice2, int Points, double stockQTY, int vatCode, out string message);
        public abstract void SendPriceToDevice();
        public abstract void GetDocumentFromDevice();
        public abstract eDeviceCheckResult CheckDevice(out string message);
        public abstract eDeviceCheckResult GetTotalSalesOfDay(out string message);
        public abstract eDeviceCheckResult GetDailySalesOfItem(out string message);
        public abstract eDeviceCheckResult GetDeviceInfo(out string message);
        public abstract eDeviceCheckResult ProgramDevice(int PosNumber, string deviceDescription, out string message);
        public abstract eDeviceCheckResult GetDeviceParameters(out string message);
        public abstract eDeviceCheckResult GetEthernetSettings(out string message);
        public abstract eDeviceCheckResult SetEthernetSettings(int portNumber, int WatchdogTime, int Delay, string IP, string RemoteIP, string Gateway, string DNS1, string DNS2, string MASK, int EnableEthernt, int EnableDHCP, int TCPUDP, int EnableWatchdogTimer, int CloseOpenReceipt, out string HexResult);
        public abstract eDeviceCheckResult IssueZReport(out string message);
        public abstract eDeviceCheckResult IssueXReport(out string message);
        public abstract eDeviceCheckResult SendComand(string command, out string message);
        public abstract eDeviceCheckResult ProgramPaymentType(int paymentCode, string paymentDescription, bool active, out string message);
        public abstract eDeviceCheckResult ReadDailyPaymentMethods(int paymentCode, out string message);
        public abstract eDeviceCheckResult IssueZReportCashierRegister(string pathToAbc, out int zReportNumber, out string pathToEJFiles, out string message);
        public abstract eDeviceCheckResult GetDepartmentInfo(int DepartmentID, out string message);
        public abstract eMachineStatus ReadStatusOfDevice(out string message);
        public abstract eDeviceCheckResult ReadPaymentType(int paymentCode, out string message);
        public abstract eDeviceCheckResult GetVatRates(CashierVatRates VatRates);
        public abstract DailyTotal CalculateDaylyTotals(string message);
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
                //
            }

            disposed = true;
        }
    }
}

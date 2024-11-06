using ITS.Hardware.RBSPOSEliot.CashRegister;
using ITS.POS.Hardware;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Model.SupportingClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms.CashRegisters
{
    public partial class TotalDailySales : XtraLocalizedForm
    {
        public TotalDailySales(CashRegisterHardware cashRegisterHardware,string message)
        {
            InitializeComponent();
            PopulateItemsInGrid(cashRegisterHardware,message);
            this.Text = this.Text = ResourcesLib.Resources.DailySales;
        }
        private void PopulateItemsInGrid(CashRegisterHardware cashRegisterHardware,string message)
        {
            DailyTotal total = (cashRegisterHardware as RBSElioCashRegister).CalculateDaylyTotals(message);
            List<DailyTotal> DailyTotalListSource = new List<DailyTotal>();
            DailyTotalListSource.Add(total);
            gridTotalSales.DataSource = DailyTotalListSource;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using System.Text.RegularExpressions;
using ITS.POS.Client.Forms;
using System.Threading;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// A Numeric keypad, used for touch screens
    /// </summary>
    public partial class ucCashNonMoneyInput : ucBaseControl, ICashCounForm
    {
        public bool IsNowVisible { get; set; }
        public event EventHandler Return;
        public BindingList<CashCountAmountObj> GetCashCountObjs()
        {
            return cashCountObjs;
        }
        public void SetCashCountObjs(BindingList<CashCountAmountObj> CashCountObjs)
        {
            cashCountObjs = CashCountObjs;
        }
        private BindingList<CashCountAmountObj> cashCountObjs { get; set; }
        public ucCashNonMoneyInput()
        {
            InitializeComponent();
            cashCountObjs = new BindingList<CashCountAmountObj>();
            grdMain.DataSource = cashCountObjs;
            (new Thread(() =>
            {
                Thread.Sleep(500);
                SetFocusFromThread();
            })).Start();

            //this.btnEnter.Image = ITS.POS.Client.Properties.Resources.availibity_ok_32;
        }
        public delegate void SafeInvokeDelegate();
        public void SetFocusFromThread()
        {
            if (edtInput.InvokeRequired)
                edtInput.Invoke(new SafeInvokeDelegate(SetFocusFromThread));
            else edtInput.Focus();
        }
        public void SetFocusFromParentInThread()
        {
            try
            {
                (new Thread(() => { Thread.Sleep(1000); SetFocusFromThread(); })).Start();
            }
            catch { }

        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (edtInput.Text != "")
            {
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                formManager.ShowMessageBox(Resources.POSClientResources.CASH_COUNT_NOT_SAVED_AMOUNTS_EXISTING);
                edtInput.Focus();
                return;
            }
            EventHandler handler = Return;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private void ucPad_KeyNotify(object sender, KeyNotifyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.D0:
                    edtInput.Focus();
                    SendKeys.SendWait("{NUM0}");
                    break;
                case Keys.D1:
                    edtInput.Focus();

                    SendKeys.SendWait("{NUM1}");
                    break;
                case Keys.D2:
                    edtInput.Focus();
                    SendKeys.SendWait("{NUM2}");
                    break;
                case Keys.D3:
                    edtInput.Focus();
                    SendKeys.SendWait("{NUM3}");
                    break;
                case Keys.D4:
                    edtInput.Focus();
                    SendKeys.SendWait("{NUM4}");
                    break;
                case Keys.D5:
                    edtInput.Focus();
                    SendKeys.SendWait("{NUM5}");
                    break;
                case Keys.D6:
                    edtInput.Focus();
                    SendKeys.SendWait("{NUM6}");
                    break;
                case Keys.D7:
                    edtInput.Focus();
                    SendKeys.SendWait("{NUM7}");
                    break;
                case Keys.D8:
                    edtInput.Focus();
                    SendKeys.SendWait("{NUM8}");
                    break;
                case Keys.D9:
                    edtInput.Focus();
                    SendKeys.SendWait("{NUM9}");
                    break;
                case Keys.Back:
                    edtInput.Focus();
                    SendKeys.SendWait("{BS}");
                    break;
                case Keys.Decimal:
                    edtInput.Focus();
                    SendKeys.SendWait(",");
                    break;
                case Keys.Multiply:
                    edtInput.Focus();
                    SendKeys.SendWait("{*}");
                    break;
                case Keys.Enter:
                    edtInput.Focus();
                    SendKeys.SendWait("{Enter}");
                    break;
                default:
                    break;
            }
            edtInput.Focus();
        }
        private void edtInput_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string text = edtInput.Text;
                if (text.Length > 0)
                {
                    Regex regex = new Regex(@"^[0-9]*[.,]?[0-9]?[0-9]?$");
                    if (!regex.IsMatch(text))
                    {
                        edtInput.Focus();
                        SendKeys.SendWait("{BS}");
                    }
                }
            }
            catch
            {
                try
                {
                    edtInput.Focus();
                    SendKeys.SendWait("{BS}");
                }
                catch { }
            }
        }
        private decimal MakeDecimal(string stringNum)
        {
            decimal x = 0;
            string[] splitted;
            if (stringNum.Contains("."))
            {
                splitted = stringNum.Split('.');
            }
            else if (stringNum.Contains(","))
            {
                splitted = stringNum.Split(',');
            }
            else
            {
                splitted = new string[] { stringNum, "0" };
            }
            if (splitted.Length == 2)
            {
                Regex regex = new Regex(@"^[0-9]*$");
                if (regex.IsMatch(splitted[0]) && regex.IsMatch(splitted[0]))
                {
                    decimal d1 = 0;
                    decimal d2 = 0;
                    decimal.TryParse(splitted[0], out d1);
                    decimal.TryParse(splitted[1], out d2);
                    for (int i = 0; i < splitted[1].Length; i++)
                    {
                        d2 /= 10;
                    }
                    x = d1 + d2;
                }
                else
                {
                    throw new Exception(string.Format(Resources.POSClientResources.CASH_WRONG_CASH_AMOUNT, stringNum));
                }
            }
            else
            {
                throw new Exception(string.Format(Resources.POSClientResources.CASH_WRONG_CASH_AMOUNT, stringNum));
            }
            return x;
        }
        private void edtInput_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {

                    decimal coinvalue = MakeDecimal(edtInput.Text);
                    if (coinvalue >0)
                    {
                        cashCountObjs.Add(new CashCountAmountObj() { Amount = coinvalue });
                        grvMain.ClearSelection();
                        for (int i = 0; i < grvMain.RowCount; i++)
                        {
                            if ((CashCountAmountObj)grvMain.GetRow(i) == cashCountObjs[cashCountObjs.Count - 1])
                            {
                                grvMain.FocusedRowHandle = i;
                                break;
                            }
                        }
                        edtInput.Focus();
                        int ctr = 100;
                        while (edtInput.Text.Length > 0 && ctr > 0)
                        {
                            SendKeys.SendWait("{BS}");
                            ctr--;
                        }
                        Recalculate();
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message, false);
                }
            }
        }
        public void Recalculate()
        {
            try
            {
                decimal sumValue = 0;
                foreach (CashCountAmountObj item in cashCountObjs)
                {
                    sumValue += item.Amount;
                }
                edtSummary.Text = string.Format(Resources.POSClientResources.CASH_COUNT_COUNT_AND_AMOUNT, cashCountObjs.Count, sumValue.ToString("0.00"));
                edtInput.Focus();
            }
            catch { }
        }
        private DialogResult ShowMessage(string Message, bool showCancel)
        {
            using (frmMessageBox frm = new frmMessageBox(Kernel))
            {
                frm.Message = Message;
                frm.btnCancel.Visible = showCancel;
                frm.btnRetry.Visible = false;
                frm.AcceptButton = frm.btnOK;
                frm.CancelButton = frm.btnCancel;
                return frm.ShowDialog();
            }
            return DialogResult.Cancel;
        }
        public void FocusInput()
        {
            edtInput.Focus();
        }
        private void rpsDelButton_Click(object sender, EventArgs e)
        {

        }
        public void MakeSmall()
        {
            btnOk.Visible = true;
            grdMain.Size = new Size(387, this.Height - 131);
            //edtInput.Location = new Point(3, this.Height - 132);
        }
        private void ucCashNonMoneyInput_VisibleChanged(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.Control)sender).Visible == true)
            {
                edtInput.Focus();
            }
        }
        private void edtInput_Leave(object sender, EventArgs e)
        {
            if (IsNowVisible)
                (new Thread(() =>
                {
                    Thread.Sleep(500);
                    SetFocusFromThread();
                })).Start();
        }

        private void rpsDelButton_Click_1(object sender, EventArgs e)
        {
            try
            {
                cashCountObjs.Remove((CashCountAmountObj)grvMain.GetRow(grvMain.GetSelectedRows()[0]));
                Recalculate();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, false);
            }
        }

        public void MoveUp()
        {
            try
            {
                if (grvMain.FocusedRowHandle > 0)
                {
                    grvMain.FocusedRowHandle -= 1;
                }
            }
            catch (Exception)
            {

            }
        }

        public void MoveDown()
        {
            try
            {
                if (grvMain.FocusedRowHandle + 1 <= grvMain.RowCount)
                {
                    grvMain.FocusedRowHandle += 1;
                }
            }
            catch (Exception)
            {

            }
        }

        public void MoveNext()
        {
            throw new NotImplementedException();
        }

        public void MovePrevious()
        {
            throw new NotImplementedException();
        }

        public void DeleteCurrentLine()
        {
            try
            {
                cashCountObjs.Remove((CashCountAmountObj)grvMain.GetRow(grvMain.GetSelectedRows()[0]));
                Recalculate();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, false);
            }
        }

        public void Multiply()
        {
            try
            {
                System.Windows.Forms.SendKeys.Send("*");
            }
            catch { }
        }
    }
}

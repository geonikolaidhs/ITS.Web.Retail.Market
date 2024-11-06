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
    public partial class ucCashCounterCash : ucBaseControl, ICashCounForm
    {
        public bool IsNowVisible { get; set; }
        public event EventHandler Return;
        public BindingList<CashCountCoinObj> GetCashCountObjs()
        {
            return cashCountObjs;
        }
        public void SetCashCountObjs(BindingList<CashCountCoinObj> CashCountObjs)
        {
            cashCountObjs = CashCountObjs;
        }
        private BindingList<CashCountCoinObj> cashCountObjs { get; set; }
        public List<decimal> acceptedAmounts;
        public bool nonCoinMode;
        public ucCashCounterCash()
        {
            InitializeComponent();
            cashCountObjs = new BindingList<CashCountCoinObj>();
            grdMain.DataSource = cashCountObjs;
            acceptedAmounts = new List<decimal>();
            acceptedAmounts.Add((decimal)1 / (decimal)100);
            acceptedAmounts.Add((decimal)2 / (decimal)100);
            acceptedAmounts.Add((decimal)5 / (decimal)100);
            acceptedAmounts.Add((decimal)1 / (decimal)10);
            acceptedAmounts.Add((decimal)2 / (decimal)10);
            acceptedAmounts.Add((decimal)5 / (decimal)10);
            acceptedAmounts.Add((decimal)1);
            acceptedAmounts.Add((decimal)2);
            acceptedAmounts.Add((decimal)5);
            acceptedAmounts.Add((decimal)10);
            acceptedAmounts.Add((decimal)20);
            acceptedAmounts.Add((decimal)50);
            acceptedAmounts.Add((decimal)100);
            acceptedAmounts.Add((decimal)200);
            acceptedAmounts.Add((decimal)500);
            nonCoinMode = false;
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
            else edtInput.Focus(); ;
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
            if (edtInput.Text!="")
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
                   string[] z = text.Split('*');
                    if (z.Length == 1)
                    {
                        Regex regex = new Regex(@"^[0-9]+[.,]?[0-9]?[0-9]?$");
                        if (!regex.IsMatch(z[0]))
                        {
                            if (text.Length >= 1)
                            {
                                edtInput.Focus();
                                SendKeys.SendWait("{BS}");
                            }
                            else
                                edtInput.Text = "";
                        }
                    }
                    else if (z.Length == 2)
                    {
                        Regex regex = new Regex(@"^[0-9]+$");
                        if (!regex.IsMatch(z[0]))
                        {
                            edtInput.Focus();
                            SendKeys.SendWait("{BS}");
                        }
                        regex = new Regex(@"^[0-9]*[.,]?[0-9]?[0-9]?$");
                        if (!regex.IsMatch(z[1]))
                        {
                            edtInput.Focus();
                            SendKeys.SendWait("{BS}");
                        }
                    }
                    else
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
                    if (!string.IsNullOrEmpty(edtInput.Text))
                    {
                        if (edtInput.Text.Contains("*"))
                        {
                            if (!nonCoinMode)
                            {
                                string[] z = edtInput.Text.Split('*');
                                decimal quantity = MakeDecimal(z[0]);
                                decimal coinvalue = MakeDecimal(z[1]);
                                bool found = false;
                                int foundcounter = -1;
                                if (acceptedAmounts.Contains(coinvalue))
                                {
                                    foreach (CashCountCoinObj item in cashCountObjs)
                                    {
                                        foundcounter++;
                                        if (item.CoinValue == coinvalue)
                                        {
                                            if (ShowMessage(string.Format(Resources.POSClientResources.CASH_COUNT_COIN_ALLREADY_ADDED, coinvalue, quantity), true) == DialogResult.OK)
                                            {
                                                item.CountedCoins += quantity;
                                                found = true;
                                                break;
                                            }
                                            else
                                            {
                                                found = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (!found)
                                    {
                                        cashCountObjs.Add(new CashCountCoinObj() { CoinValue = coinvalue, CountedCoins = quantity });
                                        grvMain.ClearSelection();
                                        for (int i = 0; i < grvMain.RowCount; i++)
                                        {
                                            if ((CashCountCoinObj)grvMain.GetRow(i) == cashCountObjs[cashCountObjs.Count - 1])
                                            {
                                                grvMain.FocusedRowHandle = i;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        grvMain.FocusedRowHandle = foundcounter;
                                    }
                                    edtInput.Focus();
                                    int ctr = 10000;
                                    while (edtInput.Text.Length > 0 && ctr > 0)
                                    {
                                        SendKeys.SendWait("{BS}");
                                        ctr--;
                                    }
                                }
                                else
                                {
                                    throw new Exception(string.Format(Resources.POSClientResources.CASH_COUNT_COIN_VALUE_DOES_NOT_EXIST, coinvalue.ToString("0.00")));
                                }
                            }
                            else
                                throw new Exception(Resources.POSClientResources.CASH_COUNT_THERE_CAN_BE_ONLY_ONE_AMOUNT_WITHOUT_COIN_VALUE);
                        }
                        else
                        {
                            if (cashCountObjs.Count == 0)
                            {
                                decimal coinvalue = MakeDecimal(edtInput.Text);
                                cashCountObjs.Add(new CashCountCoinObj() { CoinValue = coinvalue, CountedCoins = 1 });
                                int ctr = 10000;
                                while (edtInput.Text.Length > 0 && ctr > 0)
                                {
                                    SendKeys.SendWait("{BS}");
                                    ctr--;
                                }
                                nonCoinMode = true;
                            }
                            else
                            {
                                throw new Exception(Resources.POSClientResources.CASH_COUNT_THERE_CAN_BE_ONLY_ONE_AMOUNT_WITHOUT_COIN_VALUE);
                            }
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
                foreach (CashCountCoinObj item in cashCountObjs)
                {
                    sumValue += item.Amount;
                }
                edtSummary.Text = sumValue.ToString("0.00");
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
        }
        private void btnAmountClick(object sender, EventArgs e)
        {
            try
            {
                edtInput.Focus();
                if (edtInput.Text=="") SendKeys.SendWait("{1}");
                if (!edtInput.Text.Contains("*")) SendKeys.SendWait("{*}");
                foreach (char item in ((DevExpress.XtraEditors.SimpleButton)sender).Text.Replace(" ", ""))
                {
                    switch (item)
                    {
                        case '0':
                            edtInput.Focus();
                            SendKeys.SendWait("{NUM0}");
                            break;
                        case '1':
                            edtInput.Focus();

                            SendKeys.SendWait("{NUM1}");
                            break;
                        case '2':
                            edtInput.Focus();
                            SendKeys.SendWait("{NUM2}");
                            break;
                        case '3':
                            edtInput.Focus();
                            SendKeys.SendWait("{NUM3}");
                            break;
                        case '4':
                            edtInput.Focus();
                            SendKeys.SendWait("{NUM4}");
                            break;
                        case '5':
                            edtInput.Focus();
                            SendKeys.SendWait("{NUM5}");
                            break;
                        case '6':
                            edtInput.Focus();
                            SendKeys.SendWait("{NUM6}");
                            break;
                        case '7':
                            edtInput.Focus();
                            SendKeys.SendWait("{NUM7}");
                            break;
                        case '8':
                            edtInput.Focus();
                            SendKeys.SendWait("{NUM8}");
                            break;
                        case '9':
                            edtInput.Focus();
                            SendKeys.SendWait("{NUM9}");
                            break;
                        default:
                            SendKeys.SendWait(item.ToString());
                            break;
                    }
                }
                SendKeys.SendWait("{Enter}");
            }
            catch { }
        }
        public void FocusInput()
        {
            edtInput.Focus();
        }
        private void rpsDelButton_Click(object sender, EventArgs e)
        {
            try
            {
                cashCountObjs.Remove((CashCountCoinObj)grvMain.GetRow(grvMain.GetSelectedRows()[0]));
                if (cashCountObjs.Count == 0)
                    nonCoinMode = false;
                Recalculate();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, false);
            }
        }
        public void MakeSmall()
        {
            btnOk.Visible = true;
            grdMain.Size = new Size(387, this.Height - 134);
            //edtInput.Location = new Point(3, this.Height - 122);
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
                cashCountObjs.Remove((CashCountCoinObj)grvMain.GetRow(grvMain.GetSelectedRows()[0]));
                if (cashCountObjs.Count == 0)
                    nonCoinMode = false;
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

using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Hardware;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace ITS.POS.Client.Forms
{
    public partial class frmCashCount : frmInputFormBase
    {
        public BindingList<CashCountCoinObj> CashCountCoins;
        private bool smallWindow;
        public frmCashCount(IPosKernel kernel, bool smallWindow) : base(kernel)
        {

            InitializeComponent();
            this.smallWindow = smallWindow;
            if (smallWindow)
            {
                this.Size = new System.Drawing.Size(800, 600);
                this.Location = new System.Drawing.Point(0, 0);
                btnOk2.Visible = false;
                btnCancel2.Visible = false;
                nvfMain.Location = new System.Drawing.Point(0, 0);
                cinMain.MakeSmall();
                nmiMain.MakeSmall();
                nvfMain.SelectedPage = nvpMain;
            }
            this.btnOk.Text = POSClientResources.OK;
            this.btnCancel.Text = POSClientResources.CANCEL;


            System.Reflection.FieldInfo info = typeof(SendKeys).GetField("keywords", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            Array oldKeys = (Array)info.GetValue(null);
            Type elementType = oldKeys.GetType().GetElementType();
            Array newKeys = Array.CreateInstance(elementType, oldKeys.Length + 10);
            Array.Copy(oldKeys, newKeys, oldKeys.Length);
            for (int i = 0; i < 10; i++)
            {
                var newItem = Activator.CreateInstance(elementType, "NUM" + i, (int)Keys.NumPad0 + i);
                newKeys.SetValue(newItem, oldKeys.Length + i);
            }
            info.SetValue(null, newKeys);
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            XPCollection<PaymentMethod> payms = new XPCollection<PaymentMethod>(sessionManager.GetSession<PaymentMethod>(), CriteriaOperator.And(new BinaryOperator("DisplayInCashCount", true)));
            int ctr = 20;
            payms.Sorting = new SortingCollection(new SortProperty("HandelsCurrencies", DevExpress.Xpo.DB.SortingDirection.Descending));
            foreach (PaymentMethod item in payms)
            {
                ctr++;
                ITSCashInputButton btn = new ITSCashInputButton();
                if (item.HandelsCurrencies)
                {

                    btn.SetCoinAmounts(new BindingList<CashCountCoinObj>());
                }
                else
                {
                    btn.SetAmounts(new BindingList<CashCountAmountObj>());
                }
                btn.HandelsCurrencies = item.HandelsCurrencies;
                btn.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
                btn.TextWithoutNumbers = item.Description;
                btn.IncreasesDrawerAmount = item.IncreasesDrawerAmount;
                btn.Id = ctr;
                btn.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
                btn.Name = "btn" + ctr.ToString();
                btn.PaymentOid = item.Oid;
                btn.DocumentOid = Guid.Empty;
                btn.RecordType = eCashCountRecordTypes.COUNTED_PAYMENTS;
                btn.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.NonCash_ItemClick);
                grpAkri.Items.Add(btn);

                ctr++;
                ITSCashInputButton2 btn2 = new ITSCashInputButton2();
                if (item.HandelsCurrencies)
                {

                    btn2.SetCoinAmounts(btn.GetCoinAmounts());
                }
                else
                {
                    btn2.SetAmounts(btn.GetAmounts());
                }
                btn2.HandelsCurrencies = item.HandelsCurrencies;
                btn2.TextWithoutNumbers = item.Description;
                btn2.IncreasesDrawerAmount = item.IncreasesDrawerAmount;
                btn2.DocumentOid = Guid.Empty;
                btn2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
                btn2.Id = ctr;
                btn2.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
                btn2.Name = "btn" + ctr.ToString();
                btn2.PaymentOid = item.Oid;
                btn2.RecordType = eCashCountRecordTypes.COUNTED_PAYMENTS;
                btn2.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.NonCash2_ItemClick);
                grpMesh.Items.Add(btn2);
            }
            XPCollection<DocumentType> docTypes = new XPCollection<DocumentType>(sessionManager.GetSession<DocumentType>(), CriteriaOperator.And(new BinaryOperator("DisplayInCashCount", true)));
            foreach (DocumentType item in docTypes)
            {
                ctr++;
                ITSCashInputButton btn = new ITSCashInputButton();

                btn.SetAmounts(new BindingList<CashCountAmountObj>());
                btn.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
                btn.TextWithoutNumbers = item.Description;
                btn.Id = ctr;
                btn.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
                btn.Name = "btn" + ctr.ToString();
                btn.DocumentOid = item.Oid;
                btn.PaymentOid = Guid.Empty;
                btn.RecordType = eCashCountRecordTypes.COUNTED_DOCUMENTS;
                btn.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.NonCash_ItemClick);
                grpAkri.Items.Add(btn);

                ctr++;
                ITSCashInputButton2 btn2 = new ITSCashInputButton2();
                btn2.SetAmounts(btn.GetAmounts());
                btn2.TextWithoutNumbers = item.Description;
                btn2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
                btn2.Id = ctr;
                btn2.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
                btn2.Name = "btn" + ctr.ToString();
                btn2.DocumentOid = item.Oid;
                btn2.PaymentOid = Guid.Empty;
                btn2.RecordType = eCashCountRecordTypes.COUNTED_DOCUMENTS;
                btn2.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.NonCash2_ItemClick);
                grpMesh.Items.Add(btn2);
            }
            this.KeyDown -= this.frmDialogBaseKeyDownHandler;
            if (grpMesh.Items.Count > 0)
            {
                ITSCashInputButton x = (ITSCashInputButton)grpAkri.Items[0];
                NonCash_ItemClick(x, new DevExpress.XtraEditors.TileItemEventArgs());
                //nvfMain.SelectedPage = nvpNonMoneyInput;
                //nmiMain.edtInput.Focus();
                //this.cinMain.IsNowVisible = false;
                //this.nmiMain.IsNowVisible = true;
                //nmiMain.edtInput.Focus();
                if (x.HandelsCurrencies)
                    cinMain.SetFocusFromParentInThread();
                else
                    nmiMain.SetFocusFromParentInThread();


            }


        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            OkPressed();
        }
        private void OkPressed()
        {
            bool foundAmounts = false;
            BindingList<CashCountFinalObject> Amounts = new BindingList<Helpers.CashCountFinalObject>();
            foreach (ITSCashInputButton Btn in grpAkri.Items)
            {
                CashCountFinalObject x = new CashCountFinalObject() { Description = Btn.TextWithoutNumbers, Amount = 0 };
                if (Btn.HandelsCurrencies)
                {

                    foreach (CashCountCoinObj item in Btn.GetCoinAmounts())
                    {

                        x.Amount += item.Amount;
                        foundAmounts = true;
                    }
                    
                    }
                else
                {
                    foreach (CashCountAmountObj item in Btn.GetAmounts())
                    {
                        x.Amount += item.Amount;
                        foundAmounts = true;
                    }
                }
                Amounts.Add(x);
            }
            if (foundAmounts == false)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    IFormManager formManager = Kernel.GetModule<IFormManager>();
                    formManager.ShowMessageBox(POSClientResources.CASH_COUNT_YOU_DID_NOT_ENTER_ANY_VALUE);
                    
                }));
                ITSCashInputButton x = (ITSCashInputButton)grpAkri.Items[0];
                NonCash_ItemClick(x, new DevExpress.XtraEditors.TileItemEventArgs());
                if (x.HandelsCurrencies)
                    cinMain.SetFocusFromParentInThread();
                else
                    nmiMain.SetFocusFromParentInThread();
                return;
            }
            else
            {

                using (frmCashCountSummary frm = new frmCashCountSummary(this.Kernel, Amounts))
                {
                    try
                    {
                        if (frm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                        {
                            ITSCashInputButton x = (ITSCashInputButton)grpAkri.Items[0];
                            NonCash_ItemClick(x, new DevExpress.XtraEditors.TileItemEventArgs());
                            if (x.HandelsCurrencies)
                                cinMain.SetFocusFromParentInThread();
                            else
                                nmiMain.SetFocusFromParentInThread();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }

                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        public decimal FinalAmount()
        {
            decimal sumValue = 0;
            foreach (ITSCashInputButton Btn in grpAkri.Items)
            {
                if (Btn.IncreasesDrawerAmount)
                {
                    if (Btn.HandelsCurrencies)
                    {
                        foreach (CashCountCoinObj item in Btn.GetCoinAmounts())
                        {
                            sumValue += item.Amount;
                        }
                    }
                    else
                    {
                        foreach (CashCountAmountObj item in Btn.GetAmounts())
                        {
                            sumValue += item.Amount;
                        }
                    }
                }
            }
            return sumValue;
        }

        public void ApplyCountedAmmounts(UserDailyTotals usrTotals)
        {
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            foreach (ITSCashInputButton item in grpAkri.Items)
            {
                if (item.HandelsCurrencies)
                {
                    foreach (CashCountCoinObj item2 in item.GetCoinAmounts())
                    {
                        UserDailyTotalsCashCountDetail uddetail = new UserDailyTotalsCashCountDetail(sessionManager.GetSession<UserDailyTotalsCashCountDetail>());
                        uddetail.UserDailyTotals = usrTotals;
                        uddetail.CreatedByDevice = usrTotals.CreatedByDevice;
                        uddetail.CreatedBy = usrTotals.User;
                        uddetail.DetailType = item.RecordType;
                        uddetail.CountedAmount = item2.CoinValue;
                        uddetail.Amount = item2.Amount;
                        uddetail.QtyValue = item2.CountedCoins;
                        uddetail.Payment = item.PaymentOid;
                        uddetail.DocumentType = item.DocumentOid;
                        usrTotals.UserDailyTotalsCashCountDetails.Add(uddetail);
                    }
                }
                else
                {
                    if (item.GetAmounts().Count > 0)
                    {
                        UserDailyTotalsCashCountDetail uddetail = new UserDailyTotalsCashCountDetail(sessionManager.GetSession<UserDailyTotalsCashCountDetail>());
                        uddetail.UserDailyTotals = usrTotals;
                        uddetail.CreatedByDevice = usrTotals.CreatedByDevice;
                        uddetail.CreatedBy = usrTotals.User;
                        uddetail.DetailType = item.RecordType;
                        uddetail.CountedAmount = 0;
                        uddetail.Payment = item.PaymentOid;
                        uddetail.DocumentType = item.DocumentOid;
                        foreach (CashCountAmountObj item2 in item.GetAmounts())
                        {
                            uddetail.Amount += item2.Amount;
                            uddetail.QtyValue += 1;
                        }
                        usrTotals.UserDailyTotalsCashCountDetails.Add(uddetail);
                    }

                }
            }
        }
        private void uc_Return(object sender, EventArgs e)
        {
            this.nvfMain.SelectedPage = nvpMain;
        }
        private void NonCash2_ItemClick(object sender, DevExpress.XtraEditors.TileItemEventArgs e)
        {
            resetButtonColors();
            ITSCashInputButton2 x = (ITSCashInputButton2)sender;
            x.AppearanceItem.Normal.BackColor = System.Drawing.Color.RoyalBlue;
            foreach (ITSCashInputButton item in grpAkri.Items)
            {
                if (item.PaymentOid == x.PaymentOid && item.DocumentOid == x.DocumentOid)
                    item.AppearanceItem.Normal.BackColor = System.Drawing.Color.RoyalBlue;
            }
            if (x.HandelsCurrencies)
            {
                cinMain.grdMain.DataSource = x.GetCoinAmounts();
                cinMain.SetCashCountObjs(x.GetCoinAmounts());
                this.nvfMain.SelectedPage = nvpCash;
                this.cinMain.IsNowVisible = true;
                this.nmiMain.IsNowVisible = false;
                cinMain.lblDescription.Text = x.TextWithoutNumbers;
                cinMain.Recalculate();
                cinMain.edtInput.Focus();
            }
            else
            {
                nmiMain.grdMain.DataSource = x.GetAmounts();
                nmiMain.SetCashCountObjs(x.GetAmounts());
                this.nvfMain.SelectedPage = nvpNonMoneyInput;
                this.cinMain.IsNowVisible = false;
                this.nmiMain.IsNowVisible = true;
                nmiMain.lblDescription.Text = x.TextWithoutNumbers;
                nmiMain.Recalculate();
                nmiMain.edtInput.Focus();
            }

        }
        private void NonCash_ItemClick(object sender, DevExpress.XtraEditors.TileItemEventArgs e)
        {
            if (nmiMain.edtInput.Text != "" || cinMain.edtInput.Text != "")
            {
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                formManager.ShowMessageBox(POSClientResources.CASH_COUNT_NOT_SAVED_AMOUNTS_EXISTING);
                if (nvfMain.SelectedPage == nvpCash)
                    cinMain.edtInput.Focus();
                else
                    nmiMain.edtInput.Focus();
                return;
            }
            resetButtonColors();
            ITSCashInputButton x = (ITSCashInputButton)sender;
            //foreach (ITSCashInputButton2 item in grpMesh.Items)
            //{
            //    if (item.PaymentOid == x.PaymentOid && item.DocumentOid == x.DocumentOid)
            //        item.AppearanceItem.Normal.BackColor = System.Drawing.Color.RoyalBlue;
            //}
            x.AppearanceItem.Normal.BackColor = System.Drawing.Color.RoyalBlue;
            if (x.HandelsCurrencies)
            {
                cinMain.grdMain.DataSource = x.GetCoinAmounts();
                cinMain.SetCashCountObjs(x.GetCoinAmounts());
                this.nvfMain.SelectedPage = nvpCash;
                this.cinMain.IsNowVisible = true;
                this.nmiMain.IsNowVisible = false;
                cinMain.lblDescription.Text = x.TextWithoutNumbers;
                cinMain.Recalculate();
                cinMain.edtInput.Focus();
            }
            else
            {
                nmiMain.grdMain.DataSource = x.GetAmounts();
                nmiMain.SetCashCountObjs(x.GetAmounts());
                this.nvfMain.SelectedPage = nvpNonMoneyInput;
                this.cinMain.IsNowVisible = false;
                this.nmiMain.IsNowVisible = true;
                nmiMain.lblDescription.Text = x.TextWithoutNumbers;
                nmiMain.Recalculate();
                nmiMain.edtInput.Focus();
            }
        }
        private void resetButtonColors()
        {
            foreach (DevExpress.XtraBars.Navigation.TileBarItem item in grpAkri.Items)
            {
                item.AppearanceItem.Normal.BackColor = new System.Drawing.Color();
            }
            foreach (DevExpress.XtraEditors.TileItem item in grpMesh.Items)
            {
                item.AppearanceItem.Normal.BackColor = new System.Drawing.Color();
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)
            //{
            //    frmDialogBase_KeyDown(this, new KeyEventArgs(keyData));
            //    return true;
            //}
            //return base.ProcessCmdKey(ref msg, keyData);
            return false;
        }
        private void frmCashCount_KeyDownz(object sender, KeyEventArgs e)
        {
            try
            {
                ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
                ISynchronizationManager sync = Kernel.GetModule<ISynchronizationManager>();
                IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();

                try
                {
                    if (sync.UpdateKeyMappings)
                    {
                        sync.UpdateKeyMappings = false;
                        UtilsHelper.InitKeyMaps(config.CurrentTerminalOid, sessionManager);
                    }
                    Keys keyData = e.KeyData;
                    if (!keyData.ToString().Contains("Shift")) keyData = keyData | Keys.Shift;
                    if (!keyData.ToString().Contains("Control")) keyData = keyData | Keys.Control;
                    if (!keyData.ToString().Contains("Alt")) keyData = keyData | Keys.Alt;
                    if (UtilsHelper.hKeyMap.ContainsKey(keyData.ToString()))
                    {
                        POSKeyMapping x;
                        UtilsHelper.hKeyMap.TryGetValue(keyData.ToString(), out x);
                        switch (x.ActionCode)
                        {
                            case eActions.CASH_COUNT_MOVE_UP:
                                MoveUp();
                                e.Handled = true;
                                break;
                            case eActions.CASH_COUNT_MOVE_DOWN:
                                MoveDown();
                                e.Handled = true;
                                break;
                            case eActions.CASH_COUNT_NEXT:
                                MoveNext();
                                e.Handled = true;
                                break;
                            case eActions.CASH_COUNT_PREVIOUS:
                                MovePrevious();
                                e.Handled = true;
                                break;
                            case eActions.CASH_COUNT_REMOVE_LINE:
                                DeleteCurrentLine();
                                e.Handled = true;
                                break;
                            case eActions.CASH_COUNT_MULTIPLY:
                                if (e.KeyData != Keys.Multiply)
                                {
                                    Multiply();
                                    e.Handled = true;
                                }
                                break;
                            case eActions.CASH_COUNT_ENTER:
                                if (e.KeyData != Keys.Enter)
                                    SendKeys.Send("{ENTER}");
                                break;
                            case eActions.CASH_COUNT_SUBMIT:
                                OkPressed();
                                e.Handled = true;
                                break;
                            case eActions.CASH_COUNT_CANCEL:
                                CloseForm();
                                e.Handled = true;
                                break;
                            default:
                                break;
                        }
                    }
                    else if (e.KeyCode == Keys.Escape)
                    {
                        CloseForm();
                    }
                    else if (e.KeyCode == Keys.A || e.KeyCode == Keys.B || e.KeyCode == Keys.C || e.KeyCode == Keys.D || e.KeyCode == Keys.E || e.KeyCode == Keys.F || e.KeyCode == Keys.G || e.KeyCode == Keys.H || e.KeyCode == Keys.I || e.KeyCode == Keys.J || e.KeyCode == Keys.K || e.KeyCode == Keys.L || e.KeyCode == Keys.M || e.KeyCode == Keys.N || e.KeyCode == Keys.O || e.KeyCode == Keys.P || e.KeyCode == Keys.Q || e.KeyCode == Keys.R || e.KeyCode == Keys.S || e.KeyCode == Keys.T || e.KeyCode == Keys.U || e.KeyCode == Keys.V || e.KeyCode == Keys.W || e.KeyCode == Keys.X || e.KeyCode == Keys.Y || e.KeyCode == Keys.Z)
                    {
                        e.Handled = true;
                    }
                }
                catch (Exception ex)
                {
                    //Kernel.LogFile.Info(ex, "frmMainBase:KeyDown,Exception catched");
                    //actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
                }
            }
            catch (Exception)
            {

            }

        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            CloseForm();
        }
        private void CloseForm()
        {
            this.Invoke(new MethodInvoker(delegate
            {
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                if (formManager.ShowMessageBox(POSClientResources.CONTINUE_AND_LOSE_DATA, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }));
        }
        private void btnnCancel_Click(object sender, EventArgs e)
        {
            CloseForm();
        }
        private void btnnOK_Click(object sender, EventArgs e)
        {
            OkPressed();
        }
        private void MoveUp()
        {
            if (nvfMain.SelectedPage == nvpCash)
            {
                cinMain.MoveUp();
            }
            else
            {
                nmiMain.MoveUp();
            }
        }
        private void MoveDown()
        {
            if (nvfMain.SelectedPage == nvpCash)
            {
                cinMain.MoveDown();
            }
            else
            {
                nmiMain.MoveDown();
            }
        }
        private void MoveNext()
        {
            if (nmiMain.edtInput.Text != "" || cinMain.edtInput.Text != "")
            {
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                formManager.ShowMessageBox(POSClientResources.CASH_COUNT_NOT_SAVED_AMOUNTS_EXISTING);
                if (nvfMain.SelectedPage == nvpCash)
                    cinMain.edtInput.Focus();
                else
                    nmiMain.edtInput.Focus();
                return;
            }
            bool next = false;
            bool foundAny = false;
            foreach (DevExpress.XtraBars.Navigation.TileBarItem item in grpAkri.Items)
            {
                if (next == true)
                {

                    NonCash_ItemClick(item, new DevExpress.XtraEditors.TileItemEventArgs());
                    next = false;
                    break;
                }
                else
                {
                    if (item.AppearanceItem.Normal.BackColor != new System.Drawing.Color())
                    {
                        next = true;
                        foundAny = true;
                    }
                }
            }
            if (!foundAny)
            {
                if (grpAkri.Items.Count > 0)
                    NonCash_ItemClick(grpAkri.Items[0], new DevExpress.XtraEditors.TileItemEventArgs());
            }
            else if (next == true)
            {
                if (smallWindow)
                {
                    resetButtonColors();
                    this.nvfMain.SelectedPage = nvpMain;
                }
                else
                {
                    if (grpAkri.Items.Count > 0)
                        NonCash_ItemClick(grpAkri.Items[0], new DevExpress.XtraEditors.TileItemEventArgs());
                }
            }
        }
        private void MovePrevious()
        {
            if (nmiMain.edtInput.Text != "" || cinMain.edtInput.Text != "")
            {
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                formManager.ShowMessageBox(POSClientResources.CASH_COUNT_NOT_SAVED_AMOUNTS_EXISTING);
                if (nvfMain.SelectedPage == nvpCash)
                    cinMain.edtInput.Focus();
                else
                    nmiMain.edtInput.Focus();
                return;
            }
            bool next = false;
            bool foundAny = false;
            for (int i = grpAkri.Items.Count - 1; i >= 0; i--)
            {
                if (next == true)
                {
                    NonCash_ItemClick(grpAkri.Items[i], new DevExpress.XtraEditors.TileItemEventArgs());
                    next = false;
                    break;
                }
                else
                {
                    if (grpAkri.Items[i].AppearanceItem.Normal.BackColor != new System.Drawing.Color())
                    {
                        next = true;
                        foundAny = true;
                    }
                }
            }
            if (!foundAny)
            {
                if (grpAkri.Items.Count > 0)
                    NonCash_ItemClick(grpAkri.Items[grpAkri.Items.Count - 1], new DevExpress.XtraEditors.TileItemEventArgs());
            }
            else if (next == true)
            {
                if (smallWindow)
                {
                    resetButtonColors();
                    this.nvfMain.SelectedPage = nvpMain;
                }
                else
                {
                    if (grpAkri.Items.Count > 0)
                        NonCash_ItemClick(grpAkri.Items[grpAkri.Items.Count - 1], new DevExpress.XtraEditors.TileItemEventArgs());
                }
            }
        }
        private void DeleteCurrentLine()
        {
            if (nvfMain.SelectedPage == nvpCash)
            {
                cinMain.DeleteCurrentLine();
            }
            else
            {
                nmiMain.DeleteCurrentLine();
            }
        }
        private void Multiply()
        {
            if (nvfMain.SelectedPage == nvpCash)
            {
                cinMain.Multiply();
            }
            else
            {
                nmiMain.Multiply();
            }
        }
    }
    [Serializable]
    public class ITSCashInputButton : DevExpress.XtraBars.Navigation.TileBarItem
    {
        internal ITSCashInputButton() : base()
        {

        }
        public bool HandelsCurrencies { get; set; }
        public bool IncreasesDrawerAmount { get; set; }
        private BindingList<CashCountCoinObj> CoinAmounts;
        public BindingList<CashCountCoinObj> GetCoinAmounts()
        {
            return CoinAmounts;
        }
        public void SetCoinAmounts(BindingList<CashCountCoinObj> CoinAmounts)
        {
            this.CoinAmounts = CoinAmounts;
            CoinAmounts.ListChanged += Amounts_ListChanged;
        }

        private void ChangedData()
        {
            try
            {
                decimal total = 0;
                if (Amounts != null)
                    foreach (CashCountAmountObj item in Amounts)
                    {
                        total += item.Amount;
                    }
                if (CoinAmounts != null)
                    foreach (CashCountCoinObj item in CoinAmounts)
                    {
                        total += item.Amount;
                    }
                if (total > 0)
                    this.Text = TextWithoutNumbers + " (" + total.ToString("0.00") + ")";
                else
                    this.Text = TextWithoutNumbers;
            }
            catch
            {
                this.Text = TextWithoutNumbers;
            }
        }
        private void Amounts_ListChanged(object sender, ListChangedEventArgs e)
        {
            ChangedData();
        }
        private string _TextWithoutNumbers;
        public string TextWithoutNumbers
        {
            get { return _TextWithoutNumbers; }
            set
            {
                if (_TextWithoutNumbers != value)
                {
                    _TextWithoutNumbers = value;
                    ChangedData();
                }
            }
        }
        public Guid PaymentOid { get; set; }
        public Guid DocumentOid { get; set; }
        private BindingList<CashCountAmountObj> Amounts;
        public BindingList<CashCountAmountObj> GetAmounts()
        {
            return Amounts;
        }
        public void SetAmounts(BindingList<CashCountAmountObj> Amounts)
        {
            this.Amounts = Amounts;
            Amounts.ListChanged += Amounts_ListChanged;
        }
        public eCashCountRecordTypes RecordType { get; set; }
    }
    [Serializable]
    public class ITSCashInputButton2 : DevExpress.XtraEditors.TileItem
    {
        private BindingList<CashCountCoinObj> CoinAmounts;
        public BindingList<CashCountCoinObj> GetCoinAmounts()
        {
            return CoinAmounts;
        }
        public void SetCoinAmounts(BindingList<CashCountCoinObj> CoinAmounts)
        {
            this.CoinAmounts = CoinAmounts;
            CoinAmounts.ListChanged += Amounts_ListChanged;
        }
        public bool HandelsCurrencies { get; set; }
        public bool IncreasesDrawerAmount { get; set; }
        private BindingList<CashCountAmountObj> Amounts;
        public BindingList<CashCountAmountObj> GetAmounts()
        {
            return Amounts;
        }
        public void SetAmounts(BindingList<CashCountAmountObj> Amounts)
        {
            this.Amounts = Amounts;
            this.Amounts.ListChanged += Amounts_ListChanged;
        }
        internal ITSCashInputButton2() : base()
        {

        }
        private void Amounts_ListChanged(object sender, ListChangedEventArgs e)
        {
            ChangedData();
        }
        private void ChangedData()
        {
            try
            {
                decimal total = 0;
                if (Amounts != null)
                    foreach (CashCountAmountObj item in Amounts)
                    {
                        total += item.Amount;
                    }
                if (CoinAmounts != null)
                    foreach (CashCountCoinObj item in CoinAmounts)
                    {
                        total += item.Amount;
                    }
                if (total > 0)
                    this.Text = TextWithoutNumbers + " (" + total.ToString("0.00") + ")";
                else
                    this.Text = TextWithoutNumbers;
            }
            catch
            {
                this.Text = TextWithoutNumbers;
            }

        }
        private string _TextWithoutNumbers;
        public string TextWithoutNumbers
        {
            get { return _TextWithoutNumbers; }
            set
            {
                if (_TextWithoutNumbers != value)
                {
                    _TextWithoutNumbers = value;
                    ChangedData();
                }
            }
        }
        public Guid PaymentOid { get; set; }
        public Guid DocumentOid { get; set; }
        public eCashCountRecordTypes RecordType { get; set; }

    }

}

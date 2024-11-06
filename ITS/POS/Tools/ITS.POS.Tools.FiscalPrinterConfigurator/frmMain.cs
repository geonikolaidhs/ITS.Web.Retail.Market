using ITS.POS.Hardware;
using ITS.POS.Hardware.Micrelec.Fiscal;
using ITS.POS.Hardware.Wincor.Fiscal;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Tools.FiscalPrinterConfigurator
{



    public partial class frmMain : Form
    {

        public class DebugListener : TraceListener
        {
            public System.Windows.Forms.RichTextBox tbLog;

            public DebugListener(RichTextBox tb)
            {
                this.tbLog = tb;
            }

            public override void Write(string message)
            {
                tbLog.AppendText(message);
            }

            public override void WriteLine(string message)
            {
                tbLog.AppendText(message);
                tbLog.AppendText(Environment.NewLine);
            }
        }

        public frmMain()
        {
            InitializeComponent();

            Debug.Listeners.Add(new DebugListener(this.tbLog));

            tabControl1.Enabled = false;
            cmbPrinter.DataSource = new List<eFiscalDevice> { eFiscalDevice.WINCOR_FISCAL_PRINTER, eFiscalDevice.MICRELEC_FISCAL_PRINTER };
            cmbPrinter.SelectedIndex = 0;
            cmbPort.DataSource = SerialPort.GetPortNames();
            cmbPort.SelectedIndex = 0;
            cmbParity.DataSource = Enum.GetValues(typeof(Parity));
            cmbParity.SelectedIndex = 1;
            numBaudRate.Value = 115200;
        }

        private FiscalPrinter fiscalPrinter;
        private bool connected = false;

        private void SetUI()
        {
            btnConnect.Text = connected ? "Disconnect" : "Connect";
            tabControl1.Enabled = connected;
            cmbPrinter.Enabled = !connected;
            cmbPort.Enabled = !connected;
            cmbParity.Enabled = !connected;
            numBaudRate.Enabled = !connected;
            lblStatus.Text = connected ? "Connected!" : "Not Connected";
            lblStatus.ForeColor = connected ? Color.DarkGreen : Color.Red;
            if (fiscalPrinter != null && fiscalPrinter is MicrelecFiscalPrinter)
            {
                lblLine7.Visible = connected;
                lblLine8.Visible = connected;
                txtLine7.Visible = connected;
                txtLine8.Visible = connected;
                cmbPrintType7.Visible = connected;
                cmbPrintType8.Visible = connected;
                cmbPrintType1.DataSource = Enum.GetValues(typeof(ePrintType)).Cast<ePrintType>().ToList();
                cmbPrintType2.DataSource = Enum.GetValues(typeof(ePrintType)).Cast<ePrintType>().ToList();
                cmbPrintType3.DataSource = Enum.GetValues(typeof(ePrintType)).Cast<ePrintType>().ToList();
                cmbPrintType4.DataSource = Enum.GetValues(typeof(ePrintType)).Cast<ePrintType>().ToList();
                cmbPrintType5.DataSource = Enum.GetValues(typeof(ePrintType)).Cast<ePrintType>().ToList();
                cmbPrintType6.DataSource = Enum.GetValues(typeof(ePrintType)).Cast<ePrintType>().ToList();
                cmbPrintType7.DataSource = Enum.GetValues(typeof(ePrintType)).Cast<ePrintType>().ToList();
                cmbPrintType8.DataSource = Enum.GetValues(typeof(ePrintType)).Cast<ePrintType>().ToList();

            }
            else if (fiscalPrinter != null && fiscalPrinter is WincorFiscalPrinter)
            {
                lblLine7.Visible = false;
                lblLine8.Visible = false;
                txtLine7.Visible = false;
                txtLine8.Visible = false;
                cmbPrintType7.Visible = false;
                cmbPrintType8.Visible = false;
                cmbPrintType1.DataSource = new List<ePrintType> { ePrintType.NORMAL, ePrintType.DOUBLE_BOTH }; //supported only
                cmbPrintType2.DataSource = new List<ePrintType> { ePrintType.NORMAL, ePrintType.DOUBLE_BOTH }; //supported only
                cmbPrintType3.DataSource = new List<ePrintType> { ePrintType.NORMAL, ePrintType.DOUBLE_BOTH }; //supported only
                cmbPrintType4.DataSource = new List<ePrintType> { ePrintType.NORMAL, ePrintType.DOUBLE_BOTH }; //supported only
                cmbPrintType5.DataSource = new List<ePrintType> { ePrintType.NORMAL, ePrintType.DOUBLE_BOTH }; //supported only
                cmbPrintType6.DataSource = new List<ePrintType> { ePrintType.NORMAL, ePrintType.DOUBLE_BOTH }; //supported only
            }

            cmbPrintType1.SelectedIndex = 0;
            cmbPrintType2.SelectedIndex = 0;
            cmbPrintType3.SelectedIndex = 0;
            cmbPrintType4.SelectedIndex = 0;
            cmbPrintType5.SelectedIndex = 0;
            cmbPrintType6.SelectedIndex = 0;

            if (connected) //read header
            {
                try
                {
                    FiscalLine[] lines;
                    fiscalPrinter.ReadHeader(out lines);
                    int i = 1;
                    foreach (FiscalLine line in lines)
                    {
                        if (line != null)
                        {
                            TextBox textbox = this.tabControl1.Controls.Find("txtLine" + i, true).FirstOrDefault() as TextBox;
                            if (textbox != null)
                            {
                                textbox.Text = line.Value == null ? "" : line.Value.TrimEnd();
                            }
                            ComboBox typeCombobox = this.tabControl1.Controls.Find("cmbPrintType" + i, true).FirstOrDefault() as ComboBox;
                            if (typeCombobox != null)
                            {
                                typeCombobox.SelectedItem = line.Type;
                            }
                        }
                        i++;
                    }

                }
                catch (NotSupportedException ex)
                {
                    //Do nothing, printer does not support reading the header
                    string errorMessage = ex.Message;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                try
                {
                    ReloadVatRates();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (fiscalPrinter != null)
            {
                fiscalPrinter.Dispose();
                fiscalPrinter = null;
            }

            if (connected)
            {
                connected = false;
                SetUI();
            }
            else
            {

                int CharsPerLine = 0;
                int CharsCommand = 0;

                Int32.TryParse(txtCharLines.Text,out CharsPerLine);
                Int32.TryParse(txtCommandChars.Text, out CharsCommand);

                switch ((eFiscalDevice)cmbPrinter.SelectedValue)
                {
                    case eFiscalDevice.WINCOR_FISCAL_PRINTER:
                        fiscalPrinter = new WincorFiscalPrinter(ConnectionType.COM, "printer", 0, CharsPerLine, CharsCommand);
                        break;
                    case eFiscalDevice.MICRELEC_FISCAL_PRINTER:
                        fiscalPrinter = new MicrelecFiscalPrinter(ConnectionType.COM, "printer", 0, CharsPerLine, CharsCommand);
                        break;
                    case eFiscalDevice.RBS_FISCAL_PRINTER:
                        fiscalPrinter = new Hardware.RBS.Fiscal.RBSElioFiscalPrinter(ConnectionType.COM, "printer", 0, CharsPerLine, CharsCommand);
                        break;
                    default:
                        fiscalPrinter = null;
                        break;
                }

                if (fiscalPrinter != null)
                {
                    fiscalPrinter.Settings.COM.BaudRate = (int)numBaudRate.Value;
                    fiscalPrinter.Settings.COM.Parity = (Parity)cmbParity.SelectedValue;
                    fiscalPrinter.Settings.COM.PortName = (string)cmbPort.SelectedValue;
                    DeviceResult result;
                    string error;
                    try
                    {
                        result = fiscalPrinter.ReadDeviceStatus();
                        error = result.ToString();
                    }
                    catch (Exception ex)
                    {
                        result = DeviceResult.FAILURE;
                        error = ex.Message;
                    }

                    if (result != DeviceResult.SUCCESS)
                    {
                        MessageBox.Show("Connection Failed!" + "\r\n\r\n" + error);
                        return;
                    }

                    connected = true;
                    SetUI();
                }


            }
        }

        private void btnSetHeader_Click(object sender, EventArgs e)
        {
            int charlines = 0;
            Int32.TryParse(txtCharLines.Text, out charlines);
            if(charlines<=0)
            {
                MessageBox.Show("no chars per line input", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            List<FiscalLine> lines = new List<FiscalLine>();

            if (txtLine1.Visible)
            {
                lines.Add(new FiscalLine() { Value = txtLine1.Text, Type = (ePrintType)cmbPrintType1.SelectedItem });
            }
            if (txtLine2.Visible)
            {
                lines.Add(new FiscalLine() { Value = txtLine2.Text, Type = (ePrintType)cmbPrintType2.SelectedItem });
            }
            if (txtLine3.Visible)
            {
                lines.Add(new FiscalLine() { Value = txtLine3.Text, Type = (ePrintType)cmbPrintType3.SelectedItem });
            }
            if (txtLine4.Visible)
            {
                lines.Add(new FiscalLine() { Value = txtLine4.Text, Type = (ePrintType)cmbPrintType4.SelectedItem });
            }
            if (txtLine5.Visible)
            {
                lines.Add(new FiscalLine() { Value = txtLine5.Text, Type = (ePrintType)cmbPrintType5.SelectedItem });
            }
            if (txtLine6.Visible)
            {
                lines.Add(new FiscalLine() { Value = txtLine6.Text, Type = (ePrintType)cmbPrintType6.SelectedItem });
            }
            if (txtLine7.Visible)
            {
                lines.Add(new FiscalLine() { Value = txtLine7.Text, Type = (ePrintType)cmbPrintType7.SelectedItem });
            }
            if (txtLine8.Visible)
            {
                lines.Add(new FiscalLine() { Value = txtLine8.Text, Type = (ePrintType)cmbPrintType8.SelectedItem });
            }

            if (fiscalPrinter != null)
            {
                try
                {
                    fiscalPrinter.SetHeader(lines.ToArray());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnIssueZ_Click(object sender, EventArgs e)
        {
            if (PasswordCheck() == false)
            {
                MessageBox.Show("Incorrect Password");
                return;
            }

            string abcPath = "c:\\abc";
            if (Directory.Exists(abcPath) == false)
            {
                Directory.CreateDirectory(abcPath);
            }
            int zNumber = 0;
            string pathToEJ;
            tbLog.AppendText("STARTING ISSUE Z..." + Environment.NewLine);

            try
            {
                DeviceResult result = fiscalPrinter.IssueZReport(abcPath, out zNumber, out pathToEJ);
                tbLog.AppendText("RESULT \"" + result + "\"" + Environment.NewLine);
                tbLog.AppendText("Z-NUMBER " + zNumber + ", FILES EXPORTED TO: " + pathToEJ + Environment.NewLine);
            }
            catch (Exception ex)
            {
                tbLog.AppendText("ERROR \"" + ex.Message + "\"" + Environment.NewLine + ex.StackTrace);
            }
        }

        private string Password = "itsits";

        private bool PasswordCheck()
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter Password");
            return input == Password;
        }

        private void btnStartDay_Click(object sender, EventArgs e)
        {
            if (PasswordCheck() == false)
            {
                MessageBox.Show("Incorrect Password");
                return;
            }

            tbLog.AppendText("STARTING DAY..." + Environment.NewLine);

            try
            {
                DeviceResult result = fiscalPrinter.OpenFiscalDay(1);
                tbLog.AppendText("RESULT \"" + result + "\"" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                tbLog.AppendText("ERROR \"" + ex.Message + "\"" + Environment.NewLine + ex.StackTrace);
            }
        }

        private void btnShowStatus_Click(object sender, EventArgs e)
        {

            fiscalPrinter.ReadDeviceStatus();
            tbLog.AppendText("STATUS:" + Environment.NewLine);
            foreach (PropertyInfo prop in fiscalPrinter.FiscalStatus.GetType().GetProperties())
            {
                tbLog.AppendText(prop.Name + ":" + prop.GetValue(fiscalPrinter.FiscalStatus, null).ToString() + Environment.NewLine);
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            tbLog.Clear();
        }

        private void ReloadVatRates()
        {
            double vatRateA, vatRateB, vatRateC, vatRateD, vatRateE;
            fiscalPrinter.ReadVatRates(out vatRateA, out vatRateB, out vatRateC, out vatRateD, out vatRateE);

            numericVatA.Value = (decimal)vatRateA;
            numericVatB.Value = (decimal)vatRateB;
            numericVatC.Value = (decimal)vatRateC;
            numericVatD.Value = (decimal)vatRateD;
            numericVatE.Value = (decimal)vatRateE;
        }

        private void btnBtnReloadVatRates_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadVatRates();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSetVatRates_Click(object sender, EventArgs e)
        {
            if (PasswordCheck() == false)
            {
                MessageBox.Show("Incorrect Password");
                return;
            }

            fiscalPrinter.ReadDeviceStatus();
            if(fiscalPrinter.FiscalStatus.DayOpen)
            {
                MessageBox.Show("Fiscal Day Open. Cannot Set Vat Rates");
                return;
            }

            double vatRateA, vatRateB, vatRateC, vatRateD, vatRateE;
            vatRateA = (double)numericVatA.Value;
            vatRateB = (double)numericVatB.Value;
            vatRateC = (double)numericVatC.Value;
            vatRateD = (double)numericVatD.Value;
            vatRateE = (double)numericVatE.Value;

            double currentVatRateA, currentVatRateB, currentVatRateC, currentVatRateD, currentVatRateE;

            fiscalPrinter.ReadVatRates(out currentVatRateA, out currentVatRateB, out currentVatRateC, out currentVatRateD, out currentVatRateE);
            string preview = "Preview:" + Environment.NewLine;
            preview += String.Format("VAT A   {0:0.0000} --> {1:0.0000}" + Environment.NewLine, currentVatRateA, vatRateA);
            preview += String.Format("VAT B   {0:0.0000} --> {1:0.0000}" + Environment.NewLine, currentVatRateB, vatRateB);
            preview += String.Format("VAT C   {0:0.0000} --> {1:0.0000}" + Environment.NewLine, currentVatRateC, vatRateC);
            preview += String.Format("VAT D   {0:0.0000} --> {1:0.0000}" + Environment.NewLine, currentVatRateD, vatRateD);
            preview += String.Format("VAT E   {0:0.0000} --> {1:0.0000}" + Environment.NewLine, currentVatRateE, vatRateE);

            double newVatA = (double)numericVatA.Value;
            double newVatB = (double)numericVatB.Value;
            double newVatC = (double)numericVatC.Value;
            double newVatD = (double)numericVatD.Value;
            double newVatE = (double)numericVatE.Value;

            if (newVatA != currentVatRateA ||
                newVatB != currentVatRateB ||
                newVatC != currentVatRateC ||
                newVatD != currentVatRateD ||
                newVatE != currentVatRateE)
            {
                if (MessageBox.Show(preview, "Preview Changes", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK &&
                   MessageBox.Show("This change can be applied a limited amount of times. Continue ?" + Environment.NewLine + preview, "Are you sure?", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        fiscalPrinter.SetVatRates(newVatA, newVatB, newVatC, newVatD, newVatE);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vat Rates match Printer's Rates. No need to change");
            }
        }

        private void btnGetVatRates_Click(object sender, EventArgs e)
        {
            double currentVatRateA, currentVatRateB, currentVatRateC, currentVatRateD, currentVatRateE;
            fiscalPrinter.ReadVatRates(out currentVatRateA, out currentVatRateB, out currentVatRateC, out currentVatRateD, out currentVatRateE);
            tbLog.AppendText("VAT A:  " + currentVatRateA + Environment.NewLine);
            tbLog.AppendText("VAT B:  " + currentVatRateB + Environment.NewLine);
            tbLog.AppendText("VAT C:  " + currentVatRateC + Environment.NewLine);
            tbLog.AppendText("VAT D:  " + currentVatRateD + Environment.NewLine);
            tbLog.AppendText("VAT E:  " + currentVatRateD + Environment.NewLine);
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void cmbPrintType1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

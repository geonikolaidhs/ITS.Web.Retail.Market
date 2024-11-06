using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.Common.Communication;
using ITS.POS.Fiscal.Common;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Fiscal.Common.Requests;
using ITS.POS.Fiscal.Common.Responses;
using ITS.POS.Fiscal.Service.Requests;

namespace ITS.Fiscal.TestApplication
{
    public partial class TestApplication : Form
    {
        protected Header Header { get; set; }

        public TestApplication()
        {
            InitializeComponent();
            Header = new Header();

            txtReceipt.DataBindings.Add("Text", Header, "Receipt", true, DataSourceUpdateMode.OnPropertyChanged);
            txtTotal.DataBindings.Add("Text", Header, "Gross", true, DataSourceUpdateMode.OnPropertyChanged);
            txtCompanyTaxCode.DataBindings.Add("Text", Header, "OwnerTaxCode", true, DataSourceUpdateMode.OnPropertyChanged);
            txtCustomerTaxCode.DataBindings.Add("Text", Header, "CustomerTaxCode", true, DataSourceUpdateMode.OnPropertyChanged);

        }

        void TestApplication_BindingComplete(object sender, BindingCompleteEventArgs e)
        {
            //throw new NotImplementedException();
        }



        private void btnSign_Click(object sender, EventArgs e)
        {
            int executionTimes = 1;
            if (!string.IsNullOrEmpty(this.ExecuteTimes.Text))
            {
                Int32.TryParse(this.ExecuteTimes.Text, out executionTimes);
            }

            DateTime dateTime1 = DateTime.Now;
            for (int i = 0; i < executionTimes; i++)
            {

                int port = 0;
                if (Int32.TryParse(txtIPPort.Text, out port) == false)
                {
                    MessageBox.Show("Invalid port", "Error", MessageBoxButtons.OK);
                    return;
                }
                MessageClient client = new MessageClient(txtIP.Text, port, null);
                Application.DoEvents();
                Header.Closed = true;
                FiscalSignDocumentRequest request = new FiscalSignDocumentRequest(Header.Receipt, Header.OfficialString);
                FiscalSignDocumentResponse response = client.SendMessageAndWaitResponse<FiscalSignDocumentResponse>(request, 120000);
                if (response != null && String.IsNullOrWhiteSpace(response.ErrorMessage) && response.Result == eFiscalResponseType.SUCCESS)
                {
                    Header.Signature = response.Signature;
                    if (i + 1 == executionTimes)
                    {
                        DateTime dateTime2 = DateTime.Now;
                        var diffInSeconds = (dateTime1 - dateTime2).TotalSeconds;
                        MessageBox.Show(" Signatures : " + executionTimes + " In " + diffInSeconds.ToString() + " Seconds");
                        return;
                    }

                }
                else
                {
                    MessageBox.Show("Error: " + (response == null ? "Timeout" : response.ErrorCode.ToString() + Environment.NewLine + response.ErrorMessage));
                }
            }


        }

        private void AddDetail(eMinistryVatCategoryCode vat)
        {
            if (Header.Closed)
            {
                txtReceipt.DataBindings.Clear();
                txtTotal.DataBindings.Clear();
                txtCompanyTaxCode.DataBindings.Clear();
                txtCustomerTaxCode.DataBindings.Clear();
                Header = new Header();
                txtReceipt.DataBindings.Add("Text", Header, "Receipt", true, DataSourceUpdateMode.OnPropertyChanged);
                txtTotal.DataBindings.Add("Text", Header, "Gross", true, DataSourceUpdateMode.OnPropertyChanged);
                txtCompanyTaxCode.DataBindings.Add("Text", Header, "OwnerTaxCode", true, DataSourceUpdateMode.OnPropertyChanged);
                txtCustomerTaxCode.DataBindings.Add("Text", Header, "CustomerTaxCode", true, DataSourceUpdateMode.OnPropertyChanged);
            }
            double Amount;
            if (!Double.TryParse(txtAmount.Text, out Amount))
            {
                MessageBox.Show("Λάθος Ποσό");
            }
            else
            {
                Header.AddDetail(vat, Amount);

            }
        }

        private void btnAddA_Click(object sender, EventArgs e)
        {
            AddDetail(eMinistryVatCategoryCode.A);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddDetail(eMinistryVatCategoryCode.B);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddDetail(eMinistryVatCategoryCode.C);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddDetail(eMinistryVatCategoryCode.D);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AddDetail(eMinistryVatCategoryCode.E);
        }

        private void IssueZ_Click(object sender, EventArgs e)
        {

            int port = 0;
            if (Int32.TryParse(txtIPPort.Text, out port) == false)
            {
                MessageBox.Show("Invalid port", "Error", MessageBoxButtons.OK);
                return;
            }

            MessageClient Client = new MessageClient(txtIP.Text, port, null);
            if (Client != null)
            {
                FiscalIssueZResponse response = Client.SendMessageAndWaitResponse<FiscalIssueZResponse>(new FiscalIssueZRequest(), 120000);
                if (response != null && String.IsNullOrWhiteSpace(response.ErrorMessage))
                {
                    string message = "";
                    if (response.Result == eFiscalResponseType.SUCCESS)
                    {
                        message = "SUCCESS!";
                    }
                    else
                    {
                        message = "FAILURE: " + Environment.NewLine + "Error Code:" + response.ErrorCode + Environment.NewLine + "Result: " + response.ExResult;
                    }
                    MessageBox.Show(message);
                }
                else if (response != null && String.IsNullOrWhiteSpace(response.ErrorMessage) == false)
                {
                    MessageBox.Show(response.ErrorMessage);
                }
                else
                {
                    MessageBox.Show("Response Timeout.");
                }
            }
            else
            {
                MessageBox.Show("Not connected. Please connect first.");
            }
        }



    }
}

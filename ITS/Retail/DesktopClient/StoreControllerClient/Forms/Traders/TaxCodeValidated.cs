using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms.Traders
{
    public partial class TaxCodeValidated : XtraLocalizedForm
    {
        public bool result { get; set; }
        private String CompanyNameForm { get; set; }
        private String AddressForm { get; set; }
        private String CityForm { get; set; }
        private String NumberForm { get; set; }
        private String PostCodeForm { get; set; }

        public CustomerEditForm form { get; set; }

        public TaxCodeValidated()
        {
            InitializeComponent();
        }

        public TaxCodeValidated(String Name, String Address, String City, String Number, String PostCode, CustomerEditForm f)
        {
            InitializeComponent();

            this.form = f;
            this.CompanyNameForm = Name;
            this.AddressForm = Address;
            this.CityForm = City;
            this.NumberForm = Number;
            this.PostCodeForm = PostCode;

            this.CompanyName.Text = ResourcesLib.Resources.CompanyName + " : ";
            this.Address.Text = ResourcesLib.Resources.Address + " : ";
            this.PostCode.Text = ResourcesLib.Resources.PostCode + " : ";
            this.Number.Text = ResourcesLib.Resources.Number + " : ";
            this.City.Text = ResourcesLib.Resources.City + " : ";
            this.FillTheForm.Text = ResourcesLib.Resources.FillTheForm;
            this.YesBtn.Text = ResourcesLib.Resources.Yes;
            this.NoBtn.Text = ResourcesLib.Resources.No;
            this.label1.Text = ResourcesLib.Resources.TaxCodeValidated;

            this.AddressText.Text = Address;
            this.CompanyNameText.Text = Name;
            this.NumberText.Text = Number;
            this.CityText.Text = City;
            this.PostCodeText.Text = PostCode;
        }

        private void YesBtn_Click(object sender, EventArgs e)
        {
            form.SetValuesFromCheckTaxCode(this.CompanyNameForm, this.AddressForm, this.CityForm, this.NumberForm, this.PostCodeForm);
            form.SetResult(true);
            this.Close();
        }

        private void NoBtn_Click(object sender, EventArgs e)
        {
            form.SetResult(false);
            this.Close();
        }

        private void TaxCodeValidated_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void TaxCodeValidated_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void Address_Click(object sender, EventArgs e)
        {

        }
    }
}

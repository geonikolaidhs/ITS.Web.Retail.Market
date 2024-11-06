using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Microsoft.Win32;
using Microsoft.VisualBasic;

namespace ITS.Licensing.ClientLibrary
{
    public partial class ActivationForm : Form
    {
        public ActivationForm()
        {
            autoActivation = false;
            InitializeComponent();
        }

        public bool autoActivation;
        private void Activation_Load(object sender, EventArgs e)
        {

            txtApplicationName.Text = ClientLicense.itsLicense.ApplicationName;
            txtMachineID.Text = ClientLicense.itsLicense.MachineID;
            txtSerialNumber.Text = ClientLicense.itsLicense.SerialNumber;
        }

        private void btnOnlineActivation_Click(object sender, EventArgs e)
        {
            LicenseWebService.LicenceWebService webservice = new LicenseWebService.LicenceWebService();
            webservice.Url = Configuration.webServiceUrl;//"http://dvk-pc/ITS.Licensing.Web/LicenseWebService.asmx";
            try
            {
                webservice.Timeout = 10000;
                webservice.CheckOnlineStatus();

                DateTime startDate, endDate;
                String ActivationKey;
                LicenseWebService.ValidationStatus status = webservice.ActivateLicense(ClientLicense.itsLicense.ApplicationID,ClientLicense.itsLicense.SerialNumber, ClientLicense.itsLicense.MachineID, ClientLicense.itsLicense.ApplicationDateTime, out startDate, out endDate,out ActivationKey );

                switch (status)
                {
                    case LicenseWebService.ValidationStatus.LICENSE_INVALID:
                        MessageBox.Show("Η συγκεκριμένη άδεια δεν υπάρχει. Αν έχετε νόμιμο αντίγραφο, ελέγξτε αν έχετε εισάγει σωστά το κλειδί της εφαρμογής. Για βοήθεια, παρακαλώ επικοινωνήστε με την ITS Α.Ε.");
                        DialogResult = System.Windows.Forms.DialogResult.Cancel;
                        break;
                    case LicenseWebService.ValidationStatus.LICENSE_MAXIMUM_REACHED:
                        MessageBox.Show("Ο συγκεκριμένος σειριακός αριθμός έχει ενεργοποιηθεί για όλες τις συσκευές που ήταν δυνατό. Παρακαλώ επικοινωνήστε με την ITS Α.Ε.");
                        DialogResult = System.Windows.Forms.DialogResult.Cancel;
                        break;
                    case LicenseWebService.ValidationStatus.LICENSE_VERSION_INVALID:
                        MessageBox.Show("H άδεια για το συγκεκριμένο λογισμικό δεν καλύπτει την συγκεκριμένη έκδοση. Παρακαλώ επικοινωνήστε με την ITS Α.Ε.");
                        DialogResult = System.Windows.Forms.DialogResult.Cancel;
                        break;
                    case LicenseWebService.ValidationStatus.LICENSE_VALID_UPDATES_EXPIRED:
                        MessageBox.Show("Το συγκεκριμένο αντίγραφο καλύπτεται από την άδεια που έχετε αγοράσει." +
                            Environment.NewLine +  "H άδεια αναβαθμίσεων για το συγκεκριμένο λογισμικό έχει λήξει. Αν επιθυμείτε ανανέωση της συγκεκριμένης υπηρεσίας, παρακαλώ επικοινωνήστε με την ITS Α.Ε.");
                        DialogResult = System.Windows.Forms.DialogResult.OK;
                        goto case LicenseWebService.ValidationStatus.LICENSE_VALID;
                    case LicenseWebService.ValidationStatus.LICENSE_VALID:
                        String whereInRegistry = "SOFTWARE\\I.T.S. S.A.\\" + ClientLicense.itsLicense.ApplicationName;
                        RegistryKey regKey = Registry.LocalMachine.OpenSubKey(whereInRegistry,true);
                        regKey.SetValue("ActivationKey", ActivationKey);
                        regKey.Close();                    
                        MessageBox.Show("H ενεργοποίηση του προϊόντος ολοκληρώθηκε με επιτυχία.");
                        DialogResult = System.Windows.Forms.DialogResult.OK;
                        break;
                    default:
                        throw new Exception();
                }
                //DialogResult = System.Windows.Forms.DialogResult.OK;
                //this.Close();
            }
            catch (Exception )
            {
                MessageBox.Show("Αυτήν τη στιγμή η σύνδεση με το διακομιστή ενεργοποίησης δεν είναι διαθέσιμή. Παρακαλώ δοκιμάστε αργότερα, ή μέσω τηλεφώνου");
                return;
            }
        }

        private void ActivationForm_Shown(object sender, EventArgs e)
        {
            //this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            if (autoActivation)
            {
                if (MessageBox.Show("Πατήστε ΟΚ για να ενεργοποιηθεί αυτόματα το προϊόν μέσω Internet", "Ενργοποίηση λογισμικού της ITS ΑΕ", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                {
                    this.btnOnlineActivation_Click(null, null);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //TODO
        }

        private void btnOfflineActivation_Click(object sender, EventArgs e)
        {
            //TODO
        }









    }
}

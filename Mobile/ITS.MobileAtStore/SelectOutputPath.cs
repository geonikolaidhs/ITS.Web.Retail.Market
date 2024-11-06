using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ITS.Common.Utilities.Compact;
using ITS.MobileAtStore.ObjectModel;
using ITS.MobileAtStore.AuxilliaryClasses;

namespace ITS.MobileAtStore
{
    public partial class SelectOutputPath : Form
    {
        private FileExport _outputPath;

        public SelectOutputPath(DOC_TYPES docType)
        {
            InitializeComponent();
            //CommonUtilities.FillOutputPathsList(listboxOutputLocations, docType);
            if (listboxOutputLocations.Items == null || listboxOutputLocations.Items.Count <= 0)
            {
                MessageForm.Execute("Ειδοποίηση", "Δεν βρέθηκαν τοποθεσίες εξαγωγής για αυτό τον τύπο παραστατικού\r\n", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                _outputPath = null;
                this.DialogResult = DialogResult.Cancel;
            }
            CheckForSingleLocation();
        }

        public FileExport SelectedOutputPath
        {
            get
            {
                return _outputPath;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (listboxOutputLocations.SelectedIndex == -1)
                MessageForm.Execute("Πληροφορία", "Παρακαλώ επιλέξτε μία τοποθεσία εξαγωγής από την λίστα και ξαναπροσπαθήστε", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
            else
            {
                _outputPath = listboxOutputLocations.SelectedItem is FileExport?(FileExport)listboxOutputLocations.SelectedItem:null;
                if (_outputPath != null)
                {
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageForm.Execute("Πληροφορία", "Παρακαλώ επιλέξτε μία τοποθεσία εξαγωγής από την λίστα και ξαναπροσπαθήστε", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _outputPath = null;
            this.DialogResult = DialogResult.Cancel;
        }

        private void CheckForSingleLocation()
        {
            if (listboxOutputLocations.Items.Count == 1)
            {
                _outputPath = (FileExport)listboxOutputLocations.Items[0];
                this.DialogResult = DialogResult.OK;
            }
        }

        private void SelectOutputPath_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }
    }
}

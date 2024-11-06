using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Xpo;
using ITS.Retail.ResourcesLib;
using DevExpress.XtraEditors.Controls;
using System.IO;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.Common;
using ITS.Retail.Common.ViewModel;
using System.Collections;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraEditors;
namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class ImportForm : XtraLocalizedForm, INotifyPropertyChanged
    {
        protected void Notify(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void SetPropertyValue<T>(string propertyName, ref T privateField, T newValue)
        {
            privateField = newValue;
            Notify(propertyName);
        }


        // Fields...
        private int _Locale;
        private string _FileName;        
        private SupplierImportFilesSet _FileSet;

        public SupplierImportFilesSet FileSet
        {
            get
            {
                return _FileSet;
            }
            set
            {
                
                SetPropertyValue("FileSet", ref _FileSet, value);
                if (this._FileSet != null)
                {
                    this.Locale = this._FileSet.CodePage;
                }
            }
        }

        
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                SetPropertyValue("FileName", ref _FileName, value);
            }
        }


        public int Locale
        {
            get
            {
                return _Locale;
            }
            set
            {
                SetPropertyValue("Locale", ref _Locale, value);
            }
        }

        public ImportForm()
        {
            InitializeComponent();
            this.WritableUnitOfWork = XpoHelper.GetNewUnitOfWork();
            
        }

        protected void InitBindingsLookups()
        {
            //Bindings
            this.lueFile.DataBindings.Add("EditValue", this, "FileName");
            this.lueSuppliderImportFilesSet.DataBindings.Add("EditValue", this, "FileSet");
            this.lueEncoding.DataBindings.Add("EditValue", this, "Locale");

            //Import File Set Lookup properties            
            XPCollection<SupplierImportFilesSet> supplierImportFilesSets = new XPCollection<SupplierImportFilesSet>(Program.Settings.ReadOnlyUnitOfWork);
            supplierImportFilesSets.Reload();
            this.lueSuppliderImportFilesSet.Properties.DataSource = supplierImportFilesSets;
            this.lueSuppliderImportFilesSet.Properties.Columns.Add(new LookUpColumnInfo("Code", Resources.Code));
            this.lueSuppliderImportFilesSet.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));

            //Encoding
            this.lueEncoding.Properties.DataSource = Encoding.GetEncodings();
            this.lueEncoding.Properties.Columns.Add(new LookUpColumnInfo("CodePage", Resources.Code));
            this.lueEncoding.Properties.Columns.Add(new LookUpColumnInfo("DisplayName", Resources.Description));

            this.lueEncoding.Properties.ValueMember = "CodePage";
            this.lueEncoding.Properties.DisplayMember = "DisplayName";

            //info.CodePage;
            //info.DisplayName

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ImportForm_Load(object sender, EventArgs e)
        {
            this.InitBindingsLookups();
            this.PropertyChanged += ImportForm_PropertyChanged;
        }

        void ImportForm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            btnOk.Enabled = !(String.IsNullOrWhiteSpace(this.FileName) == true || this.FileSet == null);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lueFile_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Ellipsis)
            {
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        this.FileName = dialog.FileName;
                    }
                }
            }
        }

        protected UnitOfWork WritableUnitOfWork { get; set; }

        private void btnOk_Click(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);

            if (String.IsNullOrWhiteSpace(lueFile.EditValue.ToString()) || String.IsNullOrWhiteSpace(lueSuppliderImportFilesSet.EditValue.ToString()))
            {
                XtraMessageBox.Show(Resources.FillAllMissingFields, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                Dictionary<string, IList> list;
                list = LoadImportable();
                this.gridControl1.DataSource = list;
            }

            SplashScreenManager.CloseForm(false);
        }

        private Dictionary<string, IList> LoadImportable()
        {
            Dictionary<string, IList> list;
            using (StreamReader stream = new StreamReader(this.FileName, Encoding.GetEncoding(this.Locale)))
            {
                this.FileSet.Owner = Program.Settings.ReadOnlyUnitOfWork.GetObjectByKey<CompanyNew>(Program.Settings.StoreControllerSettings.Owner.Oid);

                IEnumerable<SupplierImportFileRecordHeader> frHeaders = this.FileSet.SupplierImportFileRecordHeaders.Where(x => x.MasterSupplierImportFileRecordHeader == null);

                list = frHeaders.ToDictionary(x => x.EntityName,
                   x => BridgeHelper.PerformImport(this.WritableUnitOfWork, this.FileSet, x, stream).Cast<object>().ToList() as IList);
            }
            return list;
        }

        private void btnPerformImport_Click(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);

            if(String.IsNullOrWhiteSpace(lueFile.EditValue.ToString()) || String.IsNullOrWhiteSpace(lueSuppliderImportFilesSet.EditValue.ToString())){
                XtraMessageBox.Show(Resources.FillAllMissingFields, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else{
                Dictionary<string, IList> list;
                if (this.gridControl1.DataSource == null)
                {
                    list = LoadImportable();
                }
                else
                {
                    list = this.gridControl1.DataSource as Dictionary<string, IList>;
                }
                using (var uow = XpoHelper.GetNewUnitOfWork())
                {
                
                    foreach (KeyValuePair<string, IList> pair in list)
                    {
                        pair.Value.Cast<IImportableViewModel>().ToList().ForEach(x =>
                        {
                            x.CheckWithDatabase(uow, Program.Settings.StoreControllerSettings.Owner.Oid);
                            x.CreateOrUpdatePeristant(uow, Program.Settings.StoreControllerSettings.Owner.Oid, Program.Settings.StoreControllerSettings.Store.Oid);
                        });
                    }
                    uow.CommitChanges();
                }
            }
            SplashScreenManager.CloseForm(false);
        }

        private void ImportForm_Shown(object sender, EventArgs e)
        {
            this.lueSuppliderImportFilesSet.Focus();
        }
    }
}

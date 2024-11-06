using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.DesktopClient.StoreControllerClient.ITSStoreControllerDesktopService;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using POSCommandsLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class SelectEntitiesToUpdateOnPOSForm : XtraLocalizedForm
    {
        private bool SendEntitiesManually;
        private List<Guid> POSs;
        private ePosCommand ePOSCommand;

        public SelectEntitiesToUpdateOnPOSForm(List<Guid> poss,ePosCommand eCommand,bool sendEntitiesManually)
        {
            InitializeComponent();
            SendEntitiesManually = sendEntitiesManually;
            checkedListBoxControlEntities.DataSource = LocalisedEntities;
            checkedListBoxControlEntities.ValueMember = "Key";
            checkedListBoxControlEntities.DisplayMember = "Value";
            this.POSs = poss;
            this.ePOSCommand = eCommand;
        }

        private Dictionary<string, string> LocalisedEntities
        {
            get
            {
                Dictionary<string, string> localisedEntities = new Dictionary<string, string>();

                if (SendEntitiesManually)
                {
                    XPCollection<UpdaterMode> updaterModes = new XPCollection<UpdaterMode>(Program.Settings.ReadOnlyUnitOfWork, new BinaryOperator("Mode",eUpdaterMode.MANUAL));
                    IEnumerable<String> manualEntityNames = updaterModes.Select(updaterMode => updaterMode.EntityName);

                    foreach (string entityname in manualEntityNames)
                    {
                        if (String.IsNullOrEmpty(entityname) == false)
                        {
                            Type entityType = typeof(Item).Assembly.GetType(typeof(Item).FullName.Replace(typeof(Item).Name, entityname));
                            if (entityType != null)
                            {
                                localisedEntities.Add(entityname, entityType.ToLocalizedString());
                            }
                        }
                    }

                }
                else
                {
                    IEnumerable<Type> list = typeof(Item).Assembly.GetTypes().Where(
                                  modelType => (modelType.GetCustomAttributes(typeof(UpdaterAttribute), false).FirstOrDefault() != null)
                               && ((modelType.GetCustomAttributes(typeof(UpdaterAttribute), false).FirstOrDefault() as UpdaterAttribute).Permissions.HasFlag(eUpdateDirection.STORECONTROLLER_TO_POS)));

                    foreach (Type type in list)
                    {
                        if (type != null)
                        {
                            localisedEntities.Add(type.Name, type.ToLocalizedString());
                        }
                    }
                }

                return localisedEntities;
            }
        }

        private void simpleButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = XtraMessageBox.Show(Resources.Cancel, Resources.Cancel, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if( DialogResult == DialogResult.Yes )
            {
                this.Close();
            }
        }

        private void simpleButtonContinue_Click(object sender, EventArgs e)
        {
            // Check if entities have been checked
            if(checkedListBoxControlEntities.CheckedItemsCount <= 0)
            {
                XtraMessageBox.Show(Resources.PleaseSelectARecord, Resources.PleaseSelectARecord, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //Send Changes
            try
            {
                using (ITSStoreControllerDesktopServiceClient itsService = Program.Settings.ITSStoreControllerDesktopService)
                {
                    List<POSCommandDescription> commands = new List<POSCommandDescription>();

                    foreach (Guid posGuid in POSs)
                    {
                        POSCommandDescription posCommandDescription = new POSCommandDescription();
                        posCommandDescription.POSOid = posGuid;
                        
                        SerializableTuple<ePosCommand, string> tuple = new SerializableTuple<ePosCommand, string>();
                        tuple.Item1 = this.ePOSCommand;
                        List<string> entities = new List<string>();
                        foreach (var checkedItem in checkedListBoxControlEntities.CheckedItems)
                        {
                            entities.Add(((KeyValuePair<string, string>)checkedItem).Key);
                        }
                        tuple.Item2 = String.Join(",", entities);
                        POSCommandSet posCommandSet = new POSCommandSet();
                        posCommandSet.Commands = new List<SerializableTuple<ePosCommand, string>>() { tuple };
                        posCommandSet.Expire = DateTime.Now.AddSeconds(60).Ticks;
                        posCommandDescription.POSCommandSet = posCommandSet;
                        commands.Add(posCommandDescription);
                    }

                    itsService.SendPOSCommands(commands.ToArray());
                }

                //Inform User
                XtraMessageBox.Show(Resources.ActionSuccesfullyCompleted, Resources.ActionSuccesfullyCompleted, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception exception)
            {
                XtraMessageBox.Show(exception.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Close Form
            this.Close();
        }

        private void SelectEntitiesToUpdateOnPOSForm_Shown(object sender, EventArgs e)
        {
            this.checkedListBoxControlEntities.Focus();
        }
    }
}

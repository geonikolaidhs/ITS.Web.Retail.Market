using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Resources;

namespace ITS.POS.Client.UserControls
{
    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    public partial class CommunicationStatusSmall : ucItsObserver, IObserverStatusDisplayer
    {

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PictureBox StatusImage
        {
            get
            {
                return this.imgOffline;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PictureBox DownloadImage
        {
            get
            {
                return this.imgDown;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PictureBox UploadImage
        {
            get
            {
                return this.imgUp;
            }
        }

        public CommunicationStatusSmall()
        {
            InitializeComponent();
            ActionsToObserve.Add(eActions.UPDATE_COMMUNICATION_STATUS);
            SetTooltip("Offline");
           
        }

        private void SetTooltip(String text, Control parent=null)
        {
            if (parent == null)
            {
                parent = this;
            }
            tooltip.SetToolTip(parent, text);
            foreach (Control ctl in parent.Controls)
            {
                SetTooltip(text, ctl);
            }
        }

        Type[] paramsTypes = new Type[] { typeof(StatusDisplayerParams) };

        public override Type[] GetParamsTypes()
        {
            return paramsTypes;
        }

        public void Update(StatusDisplayerParams parameters)
        {
            
            if (parameters.IsConnected)
            {
                Invoke((MethodInvoker)delegate {
                    String tip = "Online"+Environment.NewLine+"Download:";
                    imgOffline.Image = POS.Client.Properties.Resources.online_dot;
                    imgUp.Visible = true;
                    imgDown.Visible = true;
                    switch (parameters.DownloadingStatus)
                    {
                        case eDownloadingStatus.CHECKING:
                            imgDown.Image = Properties.Resources.arrow_down_green;
                            tip += POSClientResources.CHECKING + " " + parameters.DownloadingType.Name + Environment.NewLine+"Uploading:";
                            break;
                        case eDownloadingStatus.DOWNLOADING:
                            imgDown.Image = Properties.Resources.arrow_down_green;
                            tip += POSClientResources.DOWNLOADING + " " + parameters.DownloadingType.Name + Environment.NewLine + "Uploading:";
                            break;
                        case eDownloadingStatus.PAUSED:
                            tip += POSClientResources.PAUSED + Environment.NewLine + "Uploading:";
                            imgDown.Image = Properties.Resources.arrow_down_orange;
                            break;
                        case eDownloadingStatus.IDLE:
                            tip += POSClientResources.IDLE + Environment.NewLine + "Uploading:";
                            imgDown.Image = Properties.Resources.arrow_down;
                            break;
                    }
                    switch (parameters.UploadingStatus)
                    {
                        case eUploadingStatus.CHECKING:
                            tip += POSClientResources.CHECKING;
                            imgUp.Image = Properties.Resources.arrow_up_green;
                            break;                             
                        case eUploadingStatus.UPLOADING:
                            tip += POSClientResources.UPLOADING + " " + parameters.UploadingType.Name;
                            imgUp.Image = Properties.Resources.arrow_up_green;
                            break;                                     
                        case eUploadingStatus.PAUSED:
                            tip += POSClientResources.PAUSED;
                            imgUp.Image = Properties.Resources.arrow_up_orange;
                            break;
                        case eUploadingStatus.IDLE:
                            tip += POSClientResources.IDLE;
                            imgUp.Image = Properties.Resources.arrow_up;
                            break;
                    }
                    //tooltip.SetToolTip(this, tip);
                    SetTooltip(tip);
                });
            }
            else
            {
                Invoke((MethodInvoker)delegate
                {
                    imgOffline.Image = POS.Client.Properties.Resources.offline_dot;
                    imgUp.Visible = false;
                    imgDown.Visible = false;
                    SetTooltip("Offline");
                });
            }
        }
    }
}

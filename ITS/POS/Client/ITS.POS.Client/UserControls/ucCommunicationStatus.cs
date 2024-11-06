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
using ITS.POS.Client.Synchronization;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Component for displaying the communication status of the application
    /// </summary>
    [ChildsUseSameFont]
    public partial class ucCommunicationStatus : ucObserver, IObserverStatusDisplayer
    {
        public ucCommunicationStatus()
        {
            InitializeComponent();
            ActionsToObserve.Add(eActions.UPDATE_COMMUNICATION_STATUS);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolStripStatusLabel StatusIconLabel
        {
            get
            {
                return this.lblStatusColor;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolStripStatusLabel DownloadingLabel
        {
            get
            {
                return this.lblDownloading;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolStripStatusLabel DownloadingMessageLabel
        {
            get
            {
                return this.lblDownloadingMessage;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolStripStatusLabel UploadingLabel
        {
            get
            {
                return this.lblUploading;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolStripStatusLabel UploadingMessageLabel
        {
            get
            {
                return this.lblUploadingMessage;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolStripProgressBar DownloadingProgressBar
        {
            get
            {
                return this.pbDownloading;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolStripProgressBar UploadingProgressBar
        {
            get
            {
                return this.pbUploading;
            }
        }

        private delegate void SetTextDelegate(ToolStripStatusLabel label, string text);
        private delegate void SetBackgroundImageDelegate(ToolStripStatusLabel label, Bitmap image);
        private delegate void SetVisibleDelegate(ToolStripItem control, bool visible);
        private delegate void SetProgressDelegate(ToolStripProgressBar pb, int progress);

        private void SetText(ToolStripStatusLabel label, string text)
        {
            label.Text = text;
        }

        private void SetBackgroundImage(ToolStripStatusLabel label, Bitmap image)
        {
            label.BackgroundImage = image;
        }

        private void SetVisible(ToolStripItem control, bool visible)
        {
            control.Visible = visible;
        }

        private void SetProgress(ToolStripProgressBar pb, int progress)
        {
            if(progress > pb.Maximum)
            {
                progress = pb.Maximum;
            }

            pb.Value = progress;
        }

        Type[] paramsTypes = new Type[] { typeof(StatusDisplayerParams) };

        public override Type[] GetParamsTypes()
        {
            return paramsTypes;
        }

        public void Update(StatusDisplayerParams parameters)
        {
            if (ParentForm != null && ParentForm.IsHandleCreated == false)
            {
                return;
            }
            try
            {

                SetVisibleDelegate setVisibleDel = new SetVisibleDelegate(SetVisible);
                SetBackgroundImageDelegate setBackgroundImageDel = new SetBackgroundImageDelegate(SetBackgroundImage);
                SetTextDelegate setTextDel = new SetTextDelegate(SetText);
                SetProgressDelegate setProgressDelegate = new SetProgressDelegate(SetProgress);

                if (parameters.IsConnected)
                {
                    try
                    {
                      
                        if (parameters.DownloadingStatus == eDownloadingStatus.PAUSED)
                        {
                            this.Invoke(setBackgroundImageDel, new object[] { lblStatusColor, ITS.POS.Client.Properties.Resources.paused_dot });
                        }
                        else
                        {
                          
                          this.Invoke(setBackgroundImageDel, new object[] { lblStatusColor, ITS.POS.Client.Properties.Resources.online_dot });
                        }

                        switch (parameters.DownloadingStatus)
                        {
                            case eDownloadingStatus.CHECKING:
                                this.Invoke(setTextDel, new object[] { lblDownloading, POSClientResources.CHECKING+":" });
                                this.Invoke(setTextDel, new object[] { lblDownloadingMessage, parameters.DownloadingType.Name + " .." });
                                this.Invoke(setProgressDelegate, new object[] { pbDownloading, 0 });
                                this.Invoke(setVisibleDel, new object[] { pbDownloading, false });
                                break;
                            case eDownloadingStatus.DOWNLOADING:
                                this.Invoke(setTextDel, new object[] { lblDownloading, POSClientResources.DOWNLOADING+":" });
                                this.Invoke(setTextDel, new object[] { lblDownloadingMessage, parameters.DownloadingType.Name });
                                this.Invoke(setProgressDelegate, new object[] { pbDownloading, parameters.DownloadProgress });
                                this.Invoke(setVisibleDel, new object[] { pbDownloading, true });
                                break;
                            case eDownloadingStatus.IDLE:
                                this.Invoke(setTextDel, new object[] { lblDownloading, POSClientResources.DOWNLOADING + ":" });
                                this.Invoke(setTextDel, new object[] { lblDownloadingMessage, POSClientResources.IDLE });
                                this.Invoke(setProgressDelegate, new object[] { pbDownloading, 0 });
                                this.Invoke(setVisibleDel, new object[] { pbDownloading , false});
                                break;
                            case eDownloadingStatus.PAUSED:
                                this.Invoke(setTextDel, new object[] { lblDownloading, POSClientResources.DOWNLOADING + ":" });
                                this.Invoke(setTextDel, new object[] { lblDownloadingMessage, POSClientResources.PAUSED });
                                break;
                        }

                        switch (parameters.UploadingStatus)
                        {
                            case eUploadingStatus.CHECKING:
                                this.Invoke(setTextDel, new object[] { lblUploading, POSClientResources.CHECKING + ":" });
                                this.Invoke(setTextDel, new object[] { lblUploadingMessage, parameters.UploadingType.Name });
                                this.Invoke(setProgressDelegate, new object[] { pbUploading, 0 });
                                this.Invoke(setVisibleDel, new object[] { pbUploading, false });
                                break;
                            case eUploadingStatus.UPLOADING:
                                this.Invoke(setTextDel, new object[] { lblUploading, POSClientResources.UPLOADING + ":" });
                                this.Invoke(setTextDel, new object[] { lblUploadingMessage, parameters.UploadingType.Name });
                                this.Invoke(setProgressDelegate, new object[] { pbUploading, parameters.UploadProgress });
                                this.Invoke(setProgressDelegate, new object[] { pbUploading, 0 });
                                this.Invoke(setVisibleDel, new object[] { pbUploading, true });
                                break;
                            case eUploadingStatus.IDLE:
                                this.Invoke(setTextDel, new object[] { lblUploading, POSClientResources.UPLOADING + ":" });
                                this.Invoke(setTextDel, new object[] { lblUploadingMessage, POSClientResources.IDLE });
                                this.Invoke(setVisibleDel, new object[] { pbUploading, false });
                                break;
                            case eUploadingStatus.PAUSED:
                                this.Invoke(setTextDel, new object[] { lblDownloading, POSClientResources.UPLOADING + ":" });
                                this.Invoke(setTextDel, new object[] { lblDownloadingMessage, POSClientResources.PAUSED });
                                break;
                        }

                    }
                    catch(Exception e)
                    {
                        //GlobalContext.LogFile.InfoException("ucCommunicationStatus:Update,Exception catched", e);
                        string errorMessage = e.GetFullMessage();
                    }
                }
                else
                {
                    this.Invoke(setBackgroundImageDel, new object[] { lblStatusColor, ITS.POS.Client.Properties.Resources.offline_dot });
                    this.Invoke(setTextDel, new object[] { lblDownloading, POSClientResources.OFFLINE });
                    this.Invoke(setProgressDelegate, new object[] { pbDownloading, 0 });
                    this.Invoke(setProgressDelegate, new object[] { pbUploading, 0 });


                }
            }
            catch (Exception e)
            {
                //GlobalContext.LogFile.InfoException("ucCommunicationStatus:Update,Exception catched", e);
                string errorMessage = e.GetFullMessage();
            }
        }
    }
}

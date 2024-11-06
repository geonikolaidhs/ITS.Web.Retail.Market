using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using OpenNETCF.Windows.Forms;
using System.IO;
using ITS.Common.Utilities.Compact;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ITS.MobileAtStore
{
    /// <summary>
    /// Summary description for SideBar.
    /// </summary>
    /// 

    public struct SYSTEMTIME
    {
         public short year;
         public short month;
         public short dayOfWeek;
         public short day;
         public short hour;
         public short minute;
         public short second;
         public short milliseconds;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]

    public struct TimeZoneInformation
    {

        public int bias;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string standardName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string daylightName;
        SYSTEMTIME standardDate;
        SYSTEMTIME daylightDate;
        public int standardBias;
        public int daylightBias;

    }

    public class SideBar80x320 : System.Windows.Forms.Form
    {
        #region Data Members
        private delegate void ControlVoidMethod();

        private System.Windows.Forms.Label lblBattery;
        private System.Windows.Forms.PictureBox picITSLogo;
        private System.Windows.Forms.Label lblNetStatus;
        private Thread _connCheckerThread;
        private bool connectionStatus = false;
        private OpenNETCF.Windows.Forms.BatteryLife batMainBattLife;
        private Label lblVersion;
        private Label label1;
        private Label label2;
        private Panel panel1;
        private Label label4;
        private Label lblOrderCount;
        private Label lblInvoiceCount;
        private Label lblLabelCount;
        private Label lblCompetitionCount;
        private Label lblReceiptCount;
        private PictureBox pictureBox1;
        private Bitmap customLogo = null;
        private Label lblTerminalID;
        private Label lblIInvoiceSalesCount;
        private Label lblTransferCount;
        Main givenMain;

        private delegate void StatusUpdate(bool connectionStatus);
        private event StatusUpdate OnStatusUpdate;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the side bar and starts the require thread objects as to update the information provided
        /// </summary>
        public SideBar80x320()
        {
            InitializeComponent();
            lblNetStatus.Text = "Off-Line";
            lblNetStatus.Visible = true;
            SetCustomLogo();
            if (AppSettings.UseSales == false)
            {
                lblIInvoiceSalesCount.Visible = false;
            }
            OnStatusUpdate += new StatusUpdate(ThreadSafeOnStatusUpdate);
            ThreadStart ts = new ThreadStart(this.CheckConnectionStatus);
            _connCheckerThread = new Thread(ts);
            _connCheckerThread.Start();
            lblTerminalID.DataBindings.Add(new Binding("Text", AppSettings.Terminal, "ID"));
            try
            {
                lblVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch(Exception exception)
            {
                lblVersion.Text = "Σύνδεση";
            }
        }

        private void SetCustomLogo()
        {
            try
            {
                string customLogoPath = Application2.StartupPath + "\\Images\\logosmall.png";
                if (File.Exists(customLogoPath))
                {
                    customLogo = new Bitmap(customLogoPath);
                    pictureBox1.Image = customLogo;
                }
            }
            catch (Exception ex)
            {
                MessageForm.Execute("Σφάλμα", "Υπήρξε ένα σφάλμα κατά το φόρτωμα του logo\r\n" + ex.Message);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fires the thread safe event with the required information
        /// </summary>
        /// <param name="connectionStatus"></param>
        private void FireStatusUpdate(bool connectionStatus)
        {
            // UIControl is set and OnStatusUpdate has subscriber
            if (lblNetStatus != null && OnStatusUpdate != null)
            {
                if (lblNetStatus.InvokeRequired)
                {
                    lblNetStatus.Invoke(new StatusUpdate(FireStatusUpdate),
                    new object[] { connectionStatus });
                    return;
                }

                OnStatusUpdate(connectionStatus);
            }
        }

        /// <summary>
        /// This method is called whenever the event OnStatusUpdate is called and processes the information passed
        /// </summary>
        /// <param name="connectionStatus"></param>
        private void ThreadSafeOnStatusUpdate(bool connectionStatus)
        {
            if (AppSettings.ConnectedToWebService)
            {
                lblNetStatus.Text = "On-Line";
                lblNetStatus.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                lblNetStatus.Text = "Off-Line";
                lblNetStatus.ForeColor = System.Drawing.Color.Red;
            }
        }
        #endregion


        [DllImport("coredll.dll", SetLastError = true)]
        static extern bool SetSystemTime(ref SYSTEMTIME time);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool SetTimeZoneInformation(ref TimeZoneInformation lpTimeZoneInformation);

        #region Event Handlers
        /// <summary>
        /// Gives back focus to the main form if by accident it gets clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SideBar80x320_GotFocus(object sender, EventArgs e)
        {
            givenMain.Activate();
        }

        /// <summary>
        /// Checks the connection status and battery asychronously and fires a thread safe event to update the form info
        /// </summary>
        private void CheckConnectionStatus()
        {
            while (true)
            {
                try
                {
                    this.Invoke(new ControlVoidMethod(this.batMainBattLife.UpdateBatteryLife));
                    if (AppSettings.OperationMode == AppSettings.OPERATION_MODE.ONLINE)
                    {
                        ITS.MobileAtStore.WRMMobileAtStore.WRMMobileAtStore ws = null;
                        try
                        {
                            ws = MobileAtStore.GetWebService(AppSettings.Timeout);
                            DateTime remote;
                            bool remoteSetted;
                            ws.GetNow(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, out remote, out remoteSetted);
                            AppSettings.ConnectedToWebService = true;                            
                            long dif = Math.Abs(remote.Ticks - DateTime.Now.ToUniversalTime().Ticks);
                            if (dif  > TimeSpan.TicksPerMinute * 10)
                            {                                
                                SYSTEMTIME newTime = new SYSTEMTIME()
                                {
                                    year = (short)remote.Year,
                                    month = (short)remote.Month,
                                    day = (short)remote.Day,
                                    hour = (short)remote.Hour,
                                    minute = (short)remote.Minute,
                                    second = (short)remote.Second,
                                   
                                };
                                SetSystemTime(ref newTime);
                            }
                            string webserviceVersion = ws.GetWebServiceVersion(AppSettings.Terminal.ID,true,AppSettings.Terminal.IP);
                            string thisVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                            
                            if (thisVersion != webserviceVersion)
                            {
                                MainForm.BeginUpdateFromOtherThread();
                            }

                            ITS.MobileAtStore.WRMMobileAtStore.MobileAtStoreSettings mobileAtStoreSettings = ws.GetSettings();
                            AppSettings.UpdateSettings(mobileAtStoreSettings);
                        }
                        catch (Exception ex)
                        {
                            string exceptionMessage = ex.Message + "\r\n" + ex.StackTrace;
                            AppSettings.ConnectedToWebService = false;
                        }
                        if (ws != null)
                        {
                            ws.Dispose();
                        }
                        FireStatusUpdate(AppSettings.ConnectedToWebService);
                    }
                }
                catch
                {
                }
                Thread.Sleep(10000);
            }
        }
        
        #endregion

        #region Properties
        public Main MainForm
        {
            get
            {
                return givenMain;
            }
            set
            {
                givenMain = value;
            }
        }

        public bool ConnectionStatus
        {
            get
            {
                return connectionStatus;
            }
        }

        public string OrderCount
        {
            get
            {
                return lblOrderCount.Text;
            }
            set
            {
                lblOrderCount.Text = "Παραγ. : " + value;
            }
        }

        public string InvoiceCount
        {
            get
            {
                return lblInvoiceCount.Text;
            }
            set
            {
                lblInvoiceCount.Text = "Δελ. Απ. : " + value;
            }
        }

        public string TransferCount
        {
            get
            {
                return lblTransferCount.Text;
            }
            set
            {
                lblTransferCount.Text = "Ενδοδιακ. : " + value;
            }
        }

        public string InvoiceSales
        {
            get
            {
                return lblIInvoiceSalesCount.Text;
            }
            set
            {
                lblIInvoiceSalesCount.Text = "Δ.Απ.Πωλ. : " + value;
            }
        }       

        public string LabelCount
        {
            get
            {
                return lblLabelCount.Text;
            }
            set
            {
                lblLabelCount.Text = "Ετικ. : " + value;
            }
        }

        public string CompetitionCount
        {
            get
            {
                return lblCompetitionCount.Text;
            }
            set
            {
                lblCompetitionCount.Text = "Ανταγ. : " + value;
            }
        }

        public string ReceiptCount
        {
            get
            {
                return lblReceiptCount.Text;
            }
            set
            {
                lblReceiptCount.Text = "Παραλ. : " + value;
            }
        }
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SideBar80x320));
            this.lblBattery = new System.Windows.Forms.Label();
            this.picITSLogo = new System.Windows.Forms.PictureBox();
            this.batMainBattLife = new OpenNETCF.Windows.Forms.BatteryLife();
            this.lblNetStatus = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblOrderCount = new System.Windows.Forms.Label();
            this.lblInvoiceCount = new System.Windows.Forms.Label();
            this.lblLabelCount = new System.Windows.Forms.Label();
            this.lblCompetitionCount = new System.Windows.Forms.Label();
            this.lblReceiptCount = new System.Windows.Forms.Label();
            this.lblTerminalID = new System.Windows.Forms.Label();
            this.lblIInvoiceSalesCount = new System.Windows.Forms.Label();
            this.lblTransferCount = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblBattery
            // 
            this.lblBattery.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblBattery.Location = new System.Drawing.Point(1, 0);
            this.lblBattery.Name = "lblBattery";
            this.lblBattery.Size = new System.Drawing.Size(80, 14);
            this.lblBattery.Text = "Μπαταρία";
            this.lblBattery.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // picITSLogo
            // 
            this.picITSLogo.Location = new System.Drawing.Point(0, 0);
            this.picITSLogo.Name = "picITSLogo";
            this.picITSLogo.Size = new System.Drawing.Size(81, 153);
            this.picITSLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            // 
            // batMainBattLife
            // 
            this.batMainBattLife.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.batMainBattLife.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.batMainBattLife.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.batMainBattLife.Location = new System.Drawing.Point(0, 17);
            this.batMainBattLife.Name = "batMainBattLife";
            this.batMainBattLife.PercentageBarColor = System.Drawing.Color.Lime;
            this.batMainBattLife.Size = new System.Drawing.Size(80, 23);
            this.batMainBattLife.TabIndex = 0;
            // 
            // lblNetStatus
            // 
            this.lblNetStatus.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblNetStatus.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblNetStatus.ForeColor = System.Drawing.Color.Red;
            this.lblNetStatus.Location = new System.Drawing.Point(0, 14);
            this.lblNetStatus.Name = "lblNetStatus";
            this.lblNetStatus.Size = new System.Drawing.Size(77, 17);
            this.lblNetStatus.Text = "Off-Line";
            this.lblNetStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblNetStatus.Visible = false;
            // 
            // lblVersion
            // 
            this.lblVersion.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblVersion.Location = new System.Drawing.Point(-3, 0);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(80, 14);
            this.lblVersion.Text = "Έκδοση";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(2, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 14);
            this.label1.Text = "Υποστήριξη";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(2, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 14);
            this.label2.Text = "2310486304";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.lblBattery);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.batMainBattLife);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 154);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(81, 140);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 39);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(82, 70);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Underline);
            this.label4.Location = new System.Drawing.Point(3, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 16);
            this.label4.Text = "Προς Εξαγωγή";
            // 
            // lblOrderCount
            // 
            this.lblOrderCount.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.lblOrderCount.Location = new System.Drawing.Point(3, 45);
            this.lblOrderCount.Name = "lblOrderCount";
            this.lblOrderCount.Size = new System.Drawing.Size(73, 12);
            this.lblOrderCount.Text = "Παραγ. : 0";
            // 
            // lblInvoiceCount
            // 
            this.lblInvoiceCount.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.lblInvoiceCount.Location = new System.Drawing.Point(3, 56);
            this.lblInvoiceCount.Name = "lblInvoiceCount";
            this.lblInvoiceCount.Size = new System.Drawing.Size(73, 12);
            this.lblInvoiceCount.Text = "Δελ. Απ. : 0";
            // 
            // lblLabelCount
            // 
            this.lblLabelCount.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.lblLabelCount.Location = new System.Drawing.Point(3, 103);
            this.lblLabelCount.Name = "lblLabelCount";
            this.lblLabelCount.Size = new System.Drawing.Size(74, 13);
            this.lblLabelCount.Text = "Ετικ. : 0";
            // 
            // lblCompetitionCount
            // 
            this.lblCompetitionCount.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.lblCompetitionCount.Location = new System.Drawing.Point(3, 91);
            this.lblCompetitionCount.Name = "lblCompetitionCount";
            this.lblCompetitionCount.Size = new System.Drawing.Size(73, 12);
            this.lblCompetitionCount.Text = "Ανταγ. : 0";
            // 
            // lblReceiptCount
            // 
            this.lblReceiptCount.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.lblReceiptCount.Location = new System.Drawing.Point(3, 115);
            this.lblReceiptCount.Name = "lblReceiptCount";
            this.lblReceiptCount.Size = new System.Drawing.Size(74, 13);
            this.lblReceiptCount.Text = "Παραλ. : 0";
            // 
            // lblTerminalID
            // 
            this.lblTerminalID.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.lblTerminalID.Location = new System.Drawing.Point(3, 126);
            this.lblTerminalID.Name = "lblTerminalID";
            this.lblTerminalID.Size = new System.Drawing.Size(74, 25);
            this.lblTerminalID.Text = "11";
            this.lblTerminalID.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblIInvoiceSalesCount
            // 
            this.lblIInvoiceSalesCount.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.lblIInvoiceSalesCount.Location = new System.Drawing.Point(3, 68);
            this.lblIInvoiceSalesCount.Name = "lblIInvoiceSalesCount";
            this.lblIInvoiceSalesCount.Size = new System.Drawing.Size(73, 12);
            this.lblIInvoiceSalesCount.Text = "Δ.Απ.Πωλ. : 0";
            // 
            // lblTransferCount
            // 
            this.lblTransferCount.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.lblTransferCount.Location = new System.Drawing.Point(3, 80);
            this.lblTransferCount.Name = "lblTransferCount";
            this.lblTransferCount.Size = new System.Drawing.Size(73, 12);
            this.lblTransferCount.Text = "Ενδοδιακ. : 0";
            // 
            // SideBar80x320
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(83, 294);
            this.ControlBox = false;
            this.Controls.Add(this.lblTransferCount);
            this.Controls.Add(this.lblIInvoiceSalesCount);
            this.Controls.Add(this.lblTerminalID);
            this.Controls.Add(this.lblReceiptCount);
            this.Controls.Add(this.lblCompetitionCount);
            this.Controls.Add(this.lblLabelCount);
            this.Controls.Add(this.lblInvoiceCount);
            this.Controls.Add(this.lblOrderCount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblNetStatus);
            this.Controls.Add(this.picITSLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(240, 0);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SideBar80x320";
            this.GotFocus += new System.EventHandler(this.SideBar80x320_GotFocus);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Disposer
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_connCheckerThread != null)
                {
                    _connCheckerThread.Abort();
                    _connCheckerThread = null;
                }
                this.batMainBattLife.Dispose();
                if (customLogo != null)
                    customLogo.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}

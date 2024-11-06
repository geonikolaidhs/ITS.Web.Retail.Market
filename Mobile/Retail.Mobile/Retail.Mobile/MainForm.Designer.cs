namespace Retail.Mobile
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MaintabControl = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnConnect = new OpenNETCF.Windows.Forms.Button2();
            this.btnClose = new OpenNETCF.Windows.Forms.Button2();
            this.label2 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txt_pass = new System.Windows.Forms.TextBox();
            this.txt_user = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.panel5 = new System.Windows.Forms.Panel();
            this.button23 = new OpenNETCF.Windows.Forms.Button2();
            this.label14 = new System.Windows.Forms.Label();
            this.cbKind = new System.Windows.Forms.ComboBox();
            this.txtTotalAmount = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txtTotalSum = new System.Windows.Forms.TextBox();
            this.txtVAT = new System.Windows.Forms.TextBox();
            this.txtDiscount = new System.Windows.Forms.TextBox();
            this.btn_showmainmenu = new OpenNETCF.Windows.Forms.Button2();
            this.btnExport = new OpenNETCF.Windows.Forms.Button2();
            this.label11 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbseries = new System.Windows.Forms.ComboBox();
            this.cbtypes = new System.Windows.Forms.ComboBox();
            this.OrderstabPage = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button24 = new OpenNETCF.Windows.Forms.Button2();
            this.btnNew = new OpenNETCF.Windows.Forms.Button2();
            this.txtRecords = new System.Windows.Forms.TextBox();
            this.txt_price = new System.Windows.Forms.TextBox();
            this.txt_qty = new System.Windows.Forms.TextBox();
            this.txt_search_item = new System.Windows.Forms.TextBox();
            this.button22 = new OpenNETCF.Windows.Forms.Button2();
            this.btnReturn = new OpenNETCF.Windows.Forms.Button2();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblProduct = new System.Windows.Forms.Label();
            this.lbl_status = new System.Windows.Forms.Label();
            this.txt_item_details = new System.Windows.Forms.TextBox();
            this.btnForward = new OpenNETCF.Windows.Forms.Button2();
            this.btnRewind = new OpenNETCF.Windows.Forms.Button2();
            this.lblRecords = new System.Windows.Forms.Label();
            this.SettingstabPage = new System.Windows.Forms.TabPage();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblBuildVersion = new System.Windows.Forms.Label();
            this.button21 = new OpenNETCF.Windows.Forms.Button2();
            this.btn_settings = new OpenNETCF.Windows.Forms.Button2();
            this.btn_orders = new OpenNETCF.Windows.Forms.Button2();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txtcustomer1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.inputPanel1 = new Microsoft.WindowsCE.Forms.InputPanel();
            this.MaintabControl.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.OrderstabPage.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SettingstabPage.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MaintabControl
            // 
            this.MaintabControl.Controls.Add(this.tabPage2);
            this.MaintabControl.Controls.Add(this.tabPage3);
            this.MaintabControl.Controls.Add(this.OrderstabPage);
            this.MaintabControl.Controls.Add(this.SettingstabPage);
            this.MaintabControl.Controls.Add(this.tabPage1);
            this.MaintabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MaintabControl.Location = new System.Drawing.Point(0, 0);
            this.MaintabControl.Name = "MaintabControl";
            this.MaintabControl.SelectedIndex = 0;
            this.MaintabControl.Size = new System.Drawing.Size(640, 480);
            this.MaintabControl.TabIndex = 0;
            this.MaintabControl.SelectedIndexChanged += new System.EventHandler(this.MaintabControl_SelectedIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(632, 451);
            this.tabPage2.Text = "Είσοδος";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.LightYellow;
            this.panel2.Controls.Add(this.btnConnect);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.pictureBox2);
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.txt_pass);
            this.panel2.Controls.Add(this.txt_user);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(632, 451);
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnConnect.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.btnConnect.ForeColor = System.Drawing.Color.Navy;
            this.btnConnect.Image = ((System.Drawing.Image)(resources.GetObject("btnConnect.Image")));
            this.btnConnect.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleRight;
            this.btnConnect.Location = new System.Drawing.Point(517, 190);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(109, 43);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Σύνδεση";
            this.btnConnect.TextAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnConnect.Click += new System.EventHandler(this.bntlogin_Click);
            // 
            // btnClose
            // 
            this.btnClose.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnClose.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.btnClose.ForeColor = System.Drawing.Color.Navy;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(3, 190);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(108, 43);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Έξοδος";
            this.btnClose.TextAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleRight;
            this.btnClose.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label2.Location = new System.Drawing.Point(41, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(586, 20);
            this.label2.Text = "Κωδικός χρήστη";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label10.Location = new System.Drawing.Point(41, 37);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(586, 20);
            this.label10.Text = "Όνομα χρήστη";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(3, 102);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(35, 37);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(3, 37);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(35, 37);
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label9.Dock = System.Windows.Forms.DockStyle.Top;
            this.label9.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(0, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(632, 26);
            this.label9.Text = "Εισάγετε τα στοιχεία σας";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // txt_pass
            // 
            this.txt_pass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_pass.Location = new System.Drawing.Point(41, 128);
            this.txt_pass.Name = "txt_pass";
            this.txt_pass.PasswordChar = '*';
            this.txt_pass.Size = new System.Drawing.Size(585, 23);
            this.txt_pass.TabIndex = 1;
            this.txt_pass.GotFocus += new System.EventHandler(this.txt_pass_GotFocus);
            this.txt_pass.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_pass_KeyPress);
            this.txt_pass.LostFocus += new System.EventHandler(this.txt_pass_LostFocus);
            // 
            // txt_user
            // 
            this.txt_user.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_user.Location = new System.Drawing.Point(41, 64);
            this.txt_user.Name = "txt_user";
            this.txt_user.Size = new System.Drawing.Size(585, 23);
            this.txt_user.TabIndex = 0;
            this.txt_user.GotFocus += new System.EventHandler(this.txt_user_GotFocus);
            this.txt_user.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            this.txt_user.LostFocus += new System.EventHandler(this.txt_user_LostFocus);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.panel5);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(632, 451);
            this.tabPage3.Text = "Check out";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.LemonChiffon;
            this.panel5.Controls.Add(this.button23);
            this.panel5.Controls.Add(this.label14);
            this.panel5.Controls.Add(this.cbKind);
            this.panel5.Controls.Add(this.txtTotalAmount);
            this.panel5.Controls.Add(this.label7);
            this.panel5.Controls.Add(this.label8);
            this.panel5.Controls.Add(this.label12);
            this.panel5.Controls.Add(this.label13);
            this.panel5.Controls.Add(this.txtTotalSum);
            this.panel5.Controls.Add(this.txtVAT);
            this.panel5.Controls.Add(this.txtDiscount);
            this.panel5.Controls.Add(this.btn_showmainmenu);
            this.panel5.Controls.Add(this.btnExport);
            this.panel5.Controls.Add(this.label11);
            this.panel5.Controls.Add(this.label6);
            this.panel5.Controls.Add(this.cbseries);
            this.panel5.Controls.Add(this.cbtypes);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(632, 451);
            // 
            // button23
            // 
            this.button23.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.button23.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Bold);
            this.button23.ForeColor = System.Drawing.Color.Navy;
            this.button23.Image = ((System.Drawing.Image)(resources.GetObject("button23.Image")));
            this.button23.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.button23.Location = new System.Drawing.Point(5, 170);
            this.button23.Name = "button23";
            this.button23.Size = new System.Drawing.Size(72, 53);
            this.button23.TabIndex = 50;
            this.button23.Text = "Διαγραφή";
            this.button23.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.button23.Click += new System.EventHandler(this.button23_Click);
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.Color.LemonChiffon;
            this.label14.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label14.ForeColor = System.Drawing.Color.Black;
            this.label14.Location = new System.Drawing.Point(5, 2);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(116, 26);
            this.label14.Text = "Κατάσταση";
            // 
            // cbKind
            // 
            this.cbKind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbKind.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.cbKind.Location = new System.Drawing.Point(139, 0);
            this.cbKind.Name = "cbKind";
            this.cbKind.Size = new System.Drawing.Size(484, 26);
            this.cbKind.TabIndex = 43;
            // 
            // txtTotalAmount
            // 
            this.txtTotalAmount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTotalAmount.BackColor = System.Drawing.Color.LightBlue;
            this.txtTotalAmount.Enabled = false;
            this.txtTotalAmount.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.txtTotalAmount.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtTotalAmount.Location = new System.Drawing.Point(139, 28);
            this.txtTotalAmount.Multiline = true;
            this.txtTotalAmount.Name = "txtTotalAmount";
            this.txtTotalAmount.Size = new System.Drawing.Size(486, 23);
            this.txtTotalAmount.TabIndex = 35;
            this.txtTotalAmount.TabStop = false;
            this.txtTotalAmount.Text = "0,00 €";
            this.txtTotalAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.LemonChiffon;
            this.label7.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(4, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(136, 23);
            this.label7.Text = "Αξία προ έκπτωσης";
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.LemonChiffon;
            this.label8.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(4, 103);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 17);
            this.label8.Text = "Σύνολο";
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.LemonChiffon;
            this.label12.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label12.ForeColor = System.Drawing.Color.Black;
            this.label12.Location = new System.Drawing.Point(4, 79);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(90, 17);
            this.label12.Text = "ΦΠΑ";
            // 
            // label13
            // 
            this.label13.BackColor = System.Drawing.Color.LemonChiffon;
            this.label13.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label13.ForeColor = System.Drawing.Color.Black;
            this.label13.Location = new System.Drawing.Point(4, 55);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(90, 17);
            this.label13.Text = "Έκπτωση";
            // 
            // txtTotalSum
            // 
            this.txtTotalSum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTotalSum.BackColor = System.Drawing.Color.LightBlue;
            this.txtTotalSum.Enabled = false;
            this.txtTotalSum.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.txtTotalSum.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtTotalSum.Location = new System.Drawing.Point(139, 100);
            this.txtTotalSum.Multiline = true;
            this.txtTotalSum.Name = "txtTotalSum";
            this.txtTotalSum.ReadOnly = true;
            this.txtTotalSum.Size = new System.Drawing.Size(486, 23);
            this.txtTotalSum.TabIndex = 34;
            this.txtTotalSum.Text = "0.00 €";
            this.txtTotalSum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtVAT
            // 
            this.txtVAT.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtVAT.BackColor = System.Drawing.Color.LightBlue;
            this.txtVAT.Enabled = false;
            this.txtVAT.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.txtVAT.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtVAT.Location = new System.Drawing.Point(139, 76);
            this.txtVAT.Multiline = true;
            this.txtVAT.Name = "txtVAT";
            this.txtVAT.Size = new System.Drawing.Size(486, 23);
            this.txtVAT.TabIndex = 33;
            this.txtVAT.Text = "0.00 €";
            this.txtVAT.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtDiscount
            // 
            this.txtDiscount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDiscount.BackColor = System.Drawing.Color.LightBlue;
            this.txtDiscount.Enabled = false;
            this.txtDiscount.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.txtDiscount.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtDiscount.Location = new System.Drawing.Point(139, 52);
            this.txtDiscount.Multiline = true;
            this.txtDiscount.Name = "txtDiscount";
            this.txtDiscount.Size = new System.Drawing.Size(486, 23);
            this.txtDiscount.TabIndex = 32;
            this.txtDiscount.Text = "0.00 €";
            this.txtDiscount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btn_showmainmenu
            // 
            this.btn_showmainmenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_showmainmenu.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btn_showmainmenu.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Bold);
            this.btn_showmainmenu.ForeColor = System.Drawing.Color.Navy;
            this.btn_showmainmenu.Image = ((System.Drawing.Image)(resources.GetObject("btn_showmainmenu.Image")));
            this.btn_showmainmenu.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btn_showmainmenu.Location = new System.Drawing.Point(551, 170);
            this.btn_showmainmenu.Name = "btn_showmainmenu";
            this.btn_showmainmenu.Size = new System.Drawing.Size(72, 53);
            this.btn_showmainmenu.TabIndex = 25;
            this.btn_showmainmenu.Text = "Έξοδος";
            this.btn_showmainmenu.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btn_showmainmenu.Click += new System.EventHandler(this.btn_showmainmenu_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnExport.Enabled = false;
            this.btnExport.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnExport.ForeColor = System.Drawing.Color.Navy;
            this.btnExport.Image = ((System.Drawing.Image)(resources.GetObject("btnExport.Image")));
            this.btnExport.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnExport.Location = new System.Drawing.Point(83, 170);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(462, 53);
            this.btnExport.TabIndex = 24;
            this.btnExport.Text = "Αποστολή";
            this.btnExport.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.Gainsboro;
            this.label11.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.label11.ForeColor = System.Drawing.Color.Black;
            this.label11.Location = new System.Drawing.Point(2, 279);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(93, 26);
            this.label11.Text = "Σειρά";
            this.label11.Visible = false;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Gainsboro;
            this.label6.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(2, 250);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 26);
            this.label6.Text = "Τύπος";
            this.label6.Visible = false;
            // 
            // cbseries
            // 
            this.cbseries.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbseries.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.cbseries.Location = new System.Drawing.Point(139, 279);
            this.cbseries.Name = "cbseries";
            this.cbseries.Size = new System.Drawing.Size(486, 26);
            this.cbseries.TabIndex = 5;
            this.cbseries.Visible = false;
            // 
            // cbtypes
            // 
            this.cbtypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbtypes.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.cbtypes.Location = new System.Drawing.Point(139, 250);
            this.cbtypes.Name = "cbtypes";
            this.cbtypes.Size = new System.Drawing.Size(487, 26);
            this.cbtypes.TabIndex = 4;
            this.cbtypes.Visible = false;
            this.cbtypes.SelectedValueChanged += new System.EventHandler(this.cbtypes_SelectedValueChanged);
            // 
            // OrderstabPage
            // 
            this.OrderstabPage.Controls.Add(this.panel3);
            this.OrderstabPage.Location = new System.Drawing.Point(4, 25);
            this.OrderstabPage.Name = "OrderstabPage";
            this.OrderstabPage.Size = new System.Drawing.Size(632, 451);
            this.OrderstabPage.Text = "Γραμμές Παρ.";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.LemonChiffon;
            this.panel3.Controls.Add(this.button24);
            this.panel3.Controls.Add(this.btnNew);
            this.panel3.Controls.Add(this.txtRecords);
            this.panel3.Controls.Add(this.txt_price);
            this.panel3.Controls.Add(this.txt_qty);
            this.panel3.Controls.Add(this.txt_search_item);
            this.panel3.Controls.Add(this.button22);
            this.panel3.Controls.Add(this.btnReturn);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.lblProduct);
            this.panel3.Controls.Add(this.lbl_status);
            this.panel3.Controls.Add(this.txt_item_details);
            this.panel3.Controls.Add(this.btnForward);
            this.panel3.Controls.Add(this.btnRewind);
            this.panel3.Controls.Add(this.lblRecords);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(632, 451);
            // 
            // button24
            // 
            this.button24.ActiveBackColor = System.Drawing.Color.LemonChiffon;
            this.button24.ActiveBorderColor = System.Drawing.Color.LemonChiffon;
            this.button24.ActiveForeColor = System.Drawing.Color.LemonChiffon;
            this.button24.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button24.BackColor = System.Drawing.Color.LemonChiffon;
            this.button24.BorderColor = System.Drawing.Color.LemonChiffon;
            this.button24.ForeColor = System.Drawing.Color.LemonChiffon;
            this.button24.Image = ((System.Drawing.Image)(resources.GetObject("button24.Image")));
            this.button24.Location = new System.Drawing.Point(583, 48);
            this.button24.Name = "button24";
            this.button24.Size = new System.Drawing.Size(40, 46);
            this.button24.TabIndex = 31;
            this.button24.Click += new System.EventHandler(this.button24_Click);
            // 
            // btnNew
            // 
            this.btnNew.ActiveBackColor = System.Drawing.Color.Green;
            this.btnNew.ActiveBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNew.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.btnNew.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnNew.DisabledBackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnNew.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnNew.ForeColor = System.Drawing.Color.Navy;
            this.btnNew.Image = ((System.Drawing.Image)(resources.GetObject("btnNew.Image")));
            this.btnNew.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnNew.Location = new System.Drawing.Point(81, 189);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(465, 53);
            this.btnNew.TabIndex = 25;
            this.btnNew.Text = "Νέο Είδος";
            this.btnNew.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // txtRecords
            // 
            this.txtRecords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRecords.BackColor = System.Drawing.Color.LightBlue;
            this.txtRecords.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.txtRecords.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtRecords.Location = new System.Drawing.Point(126, 9);
            this.txtRecords.Multiline = true;
            this.txtRecords.Name = "txtRecords";
            this.txtRecords.Size = new System.Drawing.Size(451, 29);
            this.txtRecords.TabIndex = 16;
            this.txtRecords.TabStop = false;
            this.txtRecords.Text = "0";
            this.txtRecords.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtRecords.WordWrap = false;
            // 
            // txt_price
            // 
            this.txt_price.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_price.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.txt_price.Location = new System.Drawing.Point(83, 114);
            this.txt_price.Name = "txt_price";
            this.txt_price.ReadOnly = true;
            this.txt_price.Size = new System.Drawing.Size(473, 23);
            this.txt_price.TabIndex = 3;
            this.txt_price.GotFocus += new System.EventHandler(this.txt_price_GotFocus);
            // 
            // txt_qty
            // 
            this.txt_qty.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_qty.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.txt_qty.Location = new System.Drawing.Point(83, 90);
            this.txt_qty.Name = "txt_qty";
            this.txt_qty.Size = new System.Drawing.Size(493, 23);
            this.txt_qty.TabIndex = 2;
            this.txt_qty.TextChanged += new System.EventHandler(this.txt_qty_TextChanged);
            this.txt_qty.GotFocus += new System.EventHandler(this.txt_qty_GotFocus);
            this.txt_qty.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_qty_KeyDown);
            this.txt_qty.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_qty_KeyPress);
            // 
            // txt_search_item
            // 
            this.txt_search_item.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_search_item.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.txt_search_item.Location = new System.Drawing.Point(3, 61);
            this.txt_search_item.Name = "txt_search_item";
            this.txt_search_item.Size = new System.Drawing.Size(573, 23);
            this.txt_search_item.TabIndex = 1;
            this.txt_search_item.GotFocus += new System.EventHandler(this.txt_search_item_GotFocus);
            this.txt_search_item.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_search_item_KeyDown);
            this.txt_search_item.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_search_item_KeyPress);
            // 
            // button22
            // 
            this.button22.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button22.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.button22.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Bold);
            this.button22.ForeColor = System.Drawing.Color.Navy;
            this.button22.Image = ((System.Drawing.Image)(resources.GetObject("button22.Image")));
            this.button22.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.button22.Location = new System.Drawing.Point(550, 189);
            this.button22.Name = "button22";
            this.button22.Size = new System.Drawing.Size(72, 53);
            this.button22.TabIndex = 5;
            this.button22.Text = "Έξοδος";
            this.button22.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.button22.Click += new System.EventHandler(this.button22_Click);
            // 
            // btnReturn
            // 
            this.btnReturn.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnReturn.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnReturn.ForeColor = System.Drawing.Color.Navy;
            this.btnReturn.Image = ((System.Drawing.Image)(resources.GetObject("btnReturn.Image")));
            this.btnReturn.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnReturn.Location = new System.Drawing.Point(3, 189);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(72, 53);
            this.btnReturn.TabIndex = 6;
            this.btnReturn.Text = "Λεπτομερ.";
            this.btnReturn.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnReturn.Click += new System.EventHandler(this.btnLines_Click);
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.LemonChiffon;
            this.label4.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(3, 117);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 17);
            this.label4.Text = "Τιμή";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.LemonChiffon;
            this.label3.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(3, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 17);
            this.label3.Text = "Ποσότητα";
            // 
            // lblProduct
            // 
            this.lblProduct.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblProduct.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblProduct.ForeColor = System.Drawing.Color.Black;
            this.lblProduct.Location = new System.Drawing.Point(4, 41);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(90, 17);
            this.lblProduct.Text = "Κωδ. Είδους";
            // 
            // lbl_status
            // 
            this.lbl_status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_status.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lbl_status.Location = new System.Drawing.Point(562, 119);
            this.lbl_status.Name = "lbl_status";
            this.lbl_status.Size = new System.Drawing.Size(60, 18);
            this.lbl_status.Text = "OnOff";
            // 
            // txt_item_details
            // 
            this.txt_item_details.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_item_details.BackColor = System.Drawing.Color.LightCyan;
            this.txt_item_details.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.txt_item_details.Location = new System.Drawing.Point(3, 140);
            this.txt_item_details.Multiline = true;
            this.txt_item_details.Name = "txt_item_details";
            this.txt_item_details.ReadOnly = true;
            this.txt_item_details.Size = new System.Drawing.Size(621, 43);
            this.txt_item_details.TabIndex = 4;
            // 
            // btnForward
            // 
            this.btnForward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnForward.BackColor = System.Drawing.Color.LemonChiffon;
            this.btnForward.BorderColor = System.Drawing.Color.LemonChiffon;
            this.btnForward.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnForward.Image = ((System.Drawing.Image)(resources.GetObject("btnForward.Image")));
            this.btnForward.Location = new System.Drawing.Point(583, 4);
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(40, 40);
            this.btnForward.TabIndex = 24;
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            // 
            // btnRewind
            // 
            this.btnRewind.BackColor = System.Drawing.Color.LemonChiffon;
            this.btnRewind.BorderColor = System.Drawing.Color.LemonChiffon;
            this.btnRewind.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnRewind.Image = ((System.Drawing.Image)(resources.GetObject("btnRewind.Image")));
            this.btnRewind.Location = new System.Drawing.Point(84, 3);
            this.btnRewind.Name = "btnRewind";
            this.btnRewind.Size = new System.Drawing.Size(40, 41);
            this.btnRewind.TabIndex = 23;
            this.btnRewind.Click += new System.EventHandler(this.btnRewind_Click);
            // 
            // lblRecords
            // 
            this.lblRecords.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblRecords.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblRecords.ForeColor = System.Drawing.Color.Black;
            this.lblRecords.Location = new System.Drawing.Point(4, 14);
            this.lblRecords.Name = "lblRecords";
            this.lblRecords.Size = new System.Drawing.Size(90, 17);
            this.lblRecords.Text = "Αναζήτηση";
            // 
            // SettingstabPage
            // 
            this.SettingstabPage.Controls.Add(this.panel4);
            this.SettingstabPage.Location = new System.Drawing.Point(4, 25);
            this.SettingstabPage.Name = "SettingstabPage";
            this.SettingstabPage.Size = new System.Drawing.Size(632, 451);
            this.SettingstabPage.Text = "Επιλογές";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.LemonChiffon;
            this.panel4.Controls.Add(this.lblVersion);
            this.panel4.Controls.Add(this.lblBuildVersion);
            this.panel4.Controls.Add(this.button21);
            this.panel4.Controls.Add(this.btn_settings);
            this.panel4.Controls.Add(this.btn_orders);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(632, 451);
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblVersion.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.lblVersion.Location = new System.Drawing.Point(3, 438);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(60, 13);
            this.lblVersion.Text = "v.1.1.1.1";
            // 
            // lblBuildVersion
            // 
            this.lblBuildVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBuildVersion.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.lblBuildVersion.Location = new System.Drawing.Point(558, 438);
            this.lblBuildVersion.Name = "lblBuildVersion";
            this.lblBuildVersion.Size = new System.Drawing.Size(71, 13);
            this.lblBuildVersion.Text = "2013.05.28.001";
            // 
            // button21
            // 
            this.button21.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button21.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.button21.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold);
            this.button21.ForeColor = System.Drawing.Color.Navy;
            this.button21.Image = ((System.Drawing.Image)(resources.GetObject("button21.Image")));
            this.button21.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.button21.Location = new System.Drawing.Point(3, 188);
            this.button21.Name = "button21";
            this.button21.Size = new System.Drawing.Size(626, 48);
            this.button21.TabIndex = 19;
            this.button21.Text = "[0] Έξοδος";
            this.button21.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // btn_settings
            // 
            this.btn_settings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_settings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btn_settings.ForeColor = System.Drawing.Color.Navy;
            this.btn_settings.Image = ((System.Drawing.Image)(resources.GetObject("btn_settings.Image")));
            this.btn_settings.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btn_settings.Location = new System.Drawing.Point(517, 18);
            this.btn_settings.Name = "btn_settings";
            this.btn_settings.Size = new System.Drawing.Size(112, 76);
            this.btn_settings.TabIndex = 2;
            this.btn_settings.Text = "[2] Ρυθμίσεις ";
            this.btn_settings.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btn_settings.Click += new System.EventHandler(this.btn_settings_Click);
            // 
            // btn_orders
            // 
            this.btn_orders.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btn_orders.ForeColor = System.Drawing.Color.Navy;
            this.btn_orders.Image = ((System.Drawing.Image)(resources.GetObject("btn_orders.Image")));
            this.btn_orders.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btn_orders.Location = new System.Drawing.Point(3, 18);
            this.btn_orders.Name = "btn_orders";
            this.btn_orders.Size = new System.Drawing.Size(112, 76);
            this.btn_orders.TabIndex = 1;
            this.btn_orders.Text = "[1] Παραγγελίες";
            this.btn_orders.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btn_orders.Click += new System.EventHandler(this.btn_orders_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(282, 280);
            this.tabPage1.Text = "tabPage1";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.LemonChiffon;
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.txtcustomer1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(282, 280);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(36, 178);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(142, 37);
            this.button2.TabIndex = 5;
            this.button2.Text = "button2";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(36, 85);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(142, 58);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtcustomer1
            // 
            this.txtcustomer1.Location = new System.Drawing.Point(3, 27);
            this.txtcustomer1.Name = "txtcustomer1";
            this.txtcustomer1.Size = new System.Drawing.Size(213, 23);
            this.txtcustomer1.TabIndex = 1;
            this.txtcustomer1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtcustomer_KeyPress);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.Text = "Πελάτης";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(640, 480);
            this.ControlBox = false;
            this.Controls.Add(this.MaintabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "Retail";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
            this.MaintabControl.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.OrderstabPage.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.SettingstabPage.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl MaintabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtcustomer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txt_pass;
        private System.Windows.Forms.TextBox txt_user;
        private System.Windows.Forms.TabPage OrderstabPage;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox txt_qty;
        private System.Windows.Forms.TextBox txt_item_details;
        private System.Windows.Forms.TextBox txt_search_item;
        private System.Windows.Forms.TabPage SettingstabPage;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.ComboBox cbseries;
        private System.Windows.Forms.ComboBox cbtypes;
        private System.Windows.Forms.Label lbl_status;
        private System.Windows.Forms.TextBox txt_price;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtRecords;
        private System.Windows.Forms.Label lblRecords;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label6;
        private OpenNETCF.Windows.Forms.Button2 btn_orders;
        private OpenNETCF.Windows.Forms.Button2 btnConnect;
        private OpenNETCF.Windows.Forms.Button2 btnClose;
        private OpenNETCF.Windows.Forms.Button2 btnReturn;
        private OpenNETCF.Windows.Forms.Button2 btnExport;
        private OpenNETCF.Windows.Forms.Button2 btn_showmainmenu;
        private OpenNETCF.Windows.Forms.Button2 button21;
        private OpenNETCF.Windows.Forms.Button2 btn_settings;
        private OpenNETCF.Windows.Forms.Button2 button22;
        private System.Windows.Forms.TextBox txtTotalAmount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtTotalSum;
        private System.Windows.Forms.TextBox txtVAT;
        private System.Windows.Forms.TextBox txtDiscount;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cbKind;
        private OpenNETCF.Windows.Forms.Button2 btnForward;
        private OpenNETCF.Windows.Forms.Button2 btnRewind;
        private OpenNETCF.Windows.Forms.Button2 btnNew;
        private OpenNETCF.Windows.Forms.Button2 button23;
        private OpenNETCF.Windows.Forms.Button2 button24;
        private Microsoft.WindowsCE.Forms.InputPanel inputPanel1;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblBuildVersion;
    }
}


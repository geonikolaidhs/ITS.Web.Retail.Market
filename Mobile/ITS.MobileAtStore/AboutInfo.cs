using System;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using OpenNETCF.Win32;

namespace ITS.MobileAtStore
{
    /// <summary>
    /// Summary description for AboutInfo.
    /// </summary>
    public class AboutInfo : System.Windows.Forms.Form
    {
        #region Data Members
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblApplicationName;
        private System.Windows.Forms.Label lblEdition;
        private System.Windows.Forms.Label lblAddon2;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private Label lblBuild;
        private Label label1;
        #endregion


        public AboutInfo()
        {
            InitializeComponent();
            this.lblEdition.Text = GetVersion();
            this.lblBuild.Text = GetBuildLabel();
            this.Paint += new PaintEventHandler(Main.Form_Paint);
        }



        /// <summary>
        /// Returns the assembly description which includes the build label of this installation.
        /// </summary>
        /// <returns></returns>
        public string GetBuildLabel()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(false);
            string buildLabel = string.Empty;
            if (attributes != null)
                foreach (object o in attributes)
                {
                    if (o.GetType() == typeof(AssemblyDescriptionAttribute))
                    {
                        AssemblyDescriptionAttribute ada = o as AssemblyDescriptionAttribute;
                        buildLabel = ada.Description;
                    }
                }

            return buildLabel;
        }

        /// <summary>
        /// Returns the version of the executing assembly
        /// </summary>
        /// <returns></returns>
        public string GetVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            return string.Format("v{0}.{1}.{2}.{3}", new object[] { version.Major, version.Minor, version.Build, version.Revision });
        }



        private void btnOK_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void AboutInfo_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.btnOK_Click(this, new EventArgs());
        }



        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lblApplicationName = new System.Windows.Forms.Label();
            this.lblEdition = new System.Windows.Forms.Label();
            this.lblAddon2 = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblBuild = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.btnOK.Location = new System.Drawing.Point(81, 228);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 32);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.label2.ForeColor = System.Drawing.Color.Navy;
            this.label2.Location = new System.Drawing.Point(8, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(224, 20);
            this.label2.Text = "Υπηρεσίες Πληροφορικής && Τηλεπικοινωνιών";
            // 
            // lblApplicationName
            // 
            this.lblApplicationName.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblApplicationName.Location = new System.Drawing.Point(8, 72);
            this.lblApplicationName.Name = "lblApplicationName";
            this.lblApplicationName.Size = new System.Drawing.Size(224, 20);
            this.lblApplicationName.Text = "WRM Mobile@Store";
            this.lblApplicationName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblEdition
            // 
            this.lblEdition.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular);
            this.lblEdition.Location = new System.Drawing.Point(8, 92);
            this.lblEdition.Name = "lblEdition";
            this.lblEdition.Size = new System.Drawing.Size(224, 20);
            this.lblEdition.Text = "Version-Label-Holder";
            this.lblEdition.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblAddon2
            // 
            this.lblAddon2.Location = new System.Drawing.Point(8, 132);
            this.lblAddon2.Name = "lblAddon2";
            this.lblAddon2.Size = new System.Drawing.Size(224, 20);
            this.lblAddon2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblEmail
            // 
            this.lblEmail.Location = new System.Drawing.Point(8, 192);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(224, 20);
            this.lblEmail.Text = "E-mail: support@itservices.gr";
            this.lblEmail.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(8, 152);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(224, 20);
            this.label4.Text = "Τεχνική Υποστήριξη";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(7, 172);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(224, 20);
            this.label3.Text = "Τηλ. Κέντρο 2310 486304";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 20F, System.Drawing.FontStyle.Regular);
            this.label1.Location = new System.Drawing.Point(66, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 33);
            this.label1.Text = "I.T.S.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblBuild
            // 
            this.lblBuild.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular);
            this.lblBuild.Location = new System.Drawing.Point(8, 112);
            this.lblBuild.Name = "lblBuild";
            this.lblBuild.Size = new System.Drawing.Size(224, 20);
            this.lblBuild.Text = "Build-Label-Holder";
            this.lblBuild.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // AboutInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(239, 269);
            this.ControlBox = false;
            this.Controls.Add(this.lblBuild);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.lblAddon2);
            this.Controls.Add(this.lblEdition);
            this.Controls.Add(this.lblApplicationName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutInfo";
            this.Text = "Βοήθεια";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.AboutInfo_Paint);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AboutInfo_KeyPress);
            this.ResumeLayout(false);

        }
        #endregion


        private void AboutInfo_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }
    }
}

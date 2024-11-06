namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    partial class SelectEntitiesToUpdateOnPOSForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectEntitiesToUpdateOnPOSForm));
            this.simpleButtonContinue = new DevExpress.XtraEditors.SimpleButton();
            this.checkedListBoxControlEntities = new DevExpress.XtraEditors.CheckedListBoxControl();
            this.simpleButtonCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.checkedListBoxControlEntities)).BeginInit();
            this.SuspendLayout();
            // 
            // simpleButtonContinue
            // 
            this.simpleButtonContinue.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Forward_32;
            this.simpleButtonContinue.Location = new System.Drawing.Point(94, 429);
            this.simpleButtonContinue.Name = "simpleButtonContinue";
            this.simpleButtonContinue.Size = new System.Drawing.Size(121, 48);
            this.simpleButtonContinue.TabIndex = 0;
            this.simpleButtonContinue.Text = "@@Continue";
            this.simpleButtonContinue.Click += new System.EventHandler(this.simpleButtonContinue_Click);
            // 
            // checkedListBoxControlEntities
            // 
            this.checkedListBoxControlEntities.Location = new System.Drawing.Point(94, 12);
            this.checkedListBoxControlEntities.Name = "checkedListBoxControlEntities";
            this.checkedListBoxControlEntities.Size = new System.Drawing.Size(425, 397);
            this.checkedListBoxControlEntities.TabIndex = 2;
            // 
            // simpleButtonCancel
            // 
            this.simpleButtonCancel.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Cancel_32;
            this.simpleButtonCancel.Location = new System.Drawing.Point(410, 429);
            this.simpleButtonCancel.Name = "simpleButtonCancel";
            this.simpleButtonCancel.Size = new System.Drawing.Size(109, 48);
            this.simpleButtonCancel.TabIndex = 1;
            this.simpleButtonCancel.Text = "@@Cancel";
            this.simpleButtonCancel.Click += new System.EventHandler(this.simpleButtonCancel_Click);
            // 
            // SelectEntitiesToUpdateOnPOSForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(647, 505);
            this.Controls.Add(this.checkedListBoxControlEntities);
            this.Controls.Add(this.simpleButtonCancel);
            this.Controls.Add(this.simpleButtonContinue);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "SelectEntitiesToUpdateOnPOSForm";
            this.Text = "@@Select";
            this.Shown += new System.EventHandler(this.SelectEntitiesToUpdateOnPOSForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.checkedListBoxControlEntities)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton simpleButtonContinue;
        private DevExpress.XtraEditors.SimpleButton simpleButtonCancel;
        private DevExpress.XtraEditors.CheckedListBoxControl checkedListBoxControlEntities;
    }
}
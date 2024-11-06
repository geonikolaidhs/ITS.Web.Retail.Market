namespace ITS.Licensing.ClientLibrary
{
    partial class SerialNumberForm
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtSerial1 = new System.Windows.Forms.TextBox();
            this.txtSerial2 = new System.Windows.Forms.TextBox();
            this.txtSerial3 = new System.Windows.Forms.TextBox();
            this.txtSerial4 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(309, 104);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(175, 30);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "ΟΚ";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(21, 106);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(167, 28);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Ακύρωση";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // txtSerial1
            // 
            this.txtSerial1.Location = new System.Drawing.Point(21, 38);
            this.txtSerial1.Name = "txtSerial1";
            this.txtSerial1.Size = new System.Drawing.Size(112, 20);
            this.txtSerial1.TabIndex = 2;
            this.txtSerial1.TextChanged += new System.EventHandler(this.txtSerial1_TextChanged);
            // 
            // txtSerial2
            // 
            this.txtSerial2.Location = new System.Drawing.Point(139, 38);
            this.txtSerial2.Name = "txtSerial2";
            this.txtSerial2.Size = new System.Drawing.Size(112, 20);
            this.txtSerial2.TabIndex = 3;
            this.txtSerial2.TextChanged += new System.EventHandler(this.txtSerial2_TextChanged);
            // 
            // txtSerial3
            // 
            this.txtSerial3.Location = new System.Drawing.Point(257, 38);
            this.txtSerial3.Name = "txtSerial3";
            this.txtSerial3.Size = new System.Drawing.Size(112, 20);
            this.txtSerial3.TabIndex = 4;
            this.txtSerial3.TextChanged += new System.EventHandler(this.txtSerial3_TextChanged);
            // 
            // txtSerial4
            // 
            this.txtSerial4.Location = new System.Drawing.Point(375, 38);
            this.txtSerial4.Name = "txtSerial4";
            this.txtSerial4.Size = new System.Drawing.Size(112, 20);
            this.txtSerial4.TabIndex = 5;
            this.txtSerial4.TextChanged += new System.EventHandler(this.txtSerial4_TextChanged);
            // 
            // SerialNumberForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 145);
            this.Controls.Add(this.txtSerial4);
            this.Controls.Add(this.txtSerial3);
            this.Controls.Add(this.txtSerial2);
            this.Controls.Add(this.txtSerial1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "SerialNumberForm";
            this.Text = "Παρακαλώ Εισάγετε το Σειριακό αριθμό της εφαρμογής σας";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtSerial1;
        private System.Windows.Forms.TextBox txtSerial2;
        private System.Windows.Forms.TextBox txtSerial3;
        private System.Windows.Forms.TextBox txtSerial4;
    }
}
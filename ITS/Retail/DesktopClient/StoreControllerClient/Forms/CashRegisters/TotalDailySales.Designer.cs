namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms.CashRegisters
{
    partial class TotalDailySales
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
            this.gridTotalSales = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridTotalSales)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridTotalSales
            // 
            this.gridTotalSales.Location = new System.Drawing.Point(21, 12);
            this.gridTotalSales.MainView = this.gridView1;
            this.gridTotalSales.Name = "gridTotalSales";
            this.gridTotalSales.Size = new System.Drawing.Size(957, 277);
            this.gridTotalSales.TabIndex = 0;
            this.gridTotalSales.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridTotalSales;
            this.gridView1.Name = "gridView1";
            // 
            // TotalDailySales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1070, 508);
            this.Controls.Add(this.gridTotalSales);
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "TotalDailySales";
            this.Text = "TotalDailySales";
            ((System.ComponentModel.ISupportInitialize)(this.gridTotalSales)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridTotalSales;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
    }
}
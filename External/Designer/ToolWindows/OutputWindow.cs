using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Designer.ToolWindows
{
	/// <summary>
	/// OutputWindow.
	/// </summary>
	public class OutputWindow : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.RichTextBox RichTextBox;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		public OutputWindow()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.RichTextBox = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();

			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(327, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Output";

			// 
			// RichTextBox
			// 
			this.RichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.RichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.RichTextBox.Location = new System.Drawing.Point(0, 16);
			this.RichTextBox.Name = "RichTextBox";
			this.RichTextBox.Size = new System.Drawing.Size(327, 140);
			this.RichTextBox.TabIndex = 2;
			this.RichTextBox.Text = "";

			// 
			// OutputWindow
			// 
			this.Controls.Add(this.RichTextBox);
			this.Controls.Add(this.label1);
			this.Name = "OutputWindow";
			this.Size = new System.Drawing.Size(327, 156);
			this.ResumeLayout(false);
		}
		#endregion
	}
}

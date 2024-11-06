using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace DataLogger
{
	/// <summary>
	/// Summary description for NumKeypad.
	/// </summary>
	public class NumKeypad : System.Windows.Forms.Form
	{
		#region Enumerators
		public enum OPERATOR: byte
		{
			NO_OPERATOR = 0,
			ADDITION = 1,
			SUBSTRACTION = 2
		}
		#endregion

		#region Data Members
		private const double EVENT_TIME_OUT = 50;

		private OpenNETCF.Windows.Forms.Button2 btnKey4;
		private OpenNETCF.Windows.Forms.Button2 btnKey1;
		private OpenNETCF.Windows.Forms.Button2 btnKey2;
		private OpenNETCF.Windows.Forms.Button2 btnKey3;
		private OpenNETCF.Windows.Forms.Button2 btnKey8;
		private OpenNETCF.Windows.Forms.Button2 btnKey9;
		private OpenNETCF.Windows.Forms.Button2 btnKey7;
		private OpenNETCF.Windows.Forms.Button2 btnKey6;
		private OpenNETCF.Windows.Forms.Button2 btnKey5;
		private OpenNETCF.Windows.Forms.Button2 btnKey0;
		private OpenNETCF.Windows.Forms.Button2 btnEnter;
		private OpenNETCF.Windows.Forms.Button2 btnBackSpace;
		private OpenNETCF.Windows.Forms.Button2 btnPeriod;
		private OpenNETCF.Windows.Forms.TextBox2 txtValue;
		
		private float initial_value;
		private NumKeypad.OPERATOR initial_operator;
		private float inputValue;
		private float minValue;
		private float maxValue;
		private OpenNETCF.Windows.Forms.Button2 btnMinus;
		private OpenNETCF.Windows.Forms.Button2 btnPlus;
		private bool checkMaximum;
		private System.DateTime startLoad;
		#endregion
	
		#region Constructors
		public NumKeypad() : this(0f, 0f, NumKeypad.OPERATOR.NO_OPERATOR, 0f, 0f, false){}
		public NumKeypad(float input_value) : this(input_value, input_value, NumKeypad.OPERATOR.NO_OPERATOR, 0f, 0f, false){}
		public NumKeypad(float input_value, float min_value, float max_value) : this(input_value, input_value, NumKeypad.OPERATOR.NO_OPERATOR, min_value, max_value, true){}
		public NumKeypad(float input_value, float min_value, float max_value, bool inside_limits) : this(input_value, input_value, NumKeypad.OPERATOR.NO_OPERATOR, min_value, max_value, inside_limits){}

		public NumKeypad(float input_value, float startup_value, OPERATOR startup_operator, float min_value, float max_value, bool inside_limits)
		{
			InitializeComponent();

			this.btnBackSpace.Text = ((char)'\xEF').ToString();
			this.btnEnter.Text = ((char)'\xC3').ToString();

			this.inputValue = input_value;
			this.minValue = min_value;
			this.maxValue = max_value;
			this.checkMaximum = inside_limits;
			//this.initial_value = startup_value; replaced with the below line
            this.initial_value = 0;
			this.initial_operator = startup_operator;

			decimal temp1 = Convert.ToDecimal(input_value);
			decimal temp2 = Decimal.Truncate(temp1);
			string formatStr = (temp1 - temp2) == 0m ? "N00" : "N03";
			this.inputValue = input_value;
			this.txtValue.Text = input_value.ToString(formatStr, DB4Connector.Instance.CI_EL);

			switch(this.initial_operator)
			{
				case OPERATOR.ADDITION:
					this.txtValue.Text = string.Format("+{0}", this.initial_value);
					break;
				case OPERATOR.SUBSTRACTION:
					this.txtValue.Text = string.Format("-{0}", this.initial_value);                                     
					break;
				case OPERATOR.NO_OPERATOR:
					this.txtValue.Text = string.Format("{0}", this.initial_value);
					break;
			}
            this.Paint += new PaintEventHandler(Main.Form_Paint);
			this.btnEnter.Focus();
			Cursor.Current = Cursors.Default;
		}
		#endregion

		#region Methods
		private bool checkDateTime()
		{
			System.TimeSpan x = DateTime.Now.Subtract(this.startLoad);
			return x.TotalMilliseconds >= NumKeypad.EVENT_TIME_OUT;
		}
		#endregion

		#region Disposer
		protected override void Dispose( bool disposing )
		{
			if(disposing)
			{
			}
			base.Dispose(disposing);
		}
		#endregion

		#region Event Handlers
		private void btnKey1_Click(object sender, System.EventArgs e)
		{
			if(this.checkDateTime())
			{
				if(this.txtValue.Text == "0")
				{
					this.txtValue.Text = "";
				}
				this.txtValue.Text += "1";
			}			
		}

		private void btnKey2_Click(object sender, System.EventArgs e)
		{
			if(this.checkDateTime())
			{
				if(this.txtValue.Text == "0")
				{
					this.txtValue.Text = "";
				}
				this.txtValue.Text += "2";
			}
		}

		private void btnKey3_Click(object sender, System.EventArgs e)
		{
			if(this.checkDateTime())
			{
				if(this.txtValue.Text == "0")
				{
					this.txtValue.Text = "";
				}
				this.txtValue.Text += "3";
			}
		}

		private void btnKey4_Click(object sender, System.EventArgs e)
		{
			if(this.checkDateTime())
			{
				if(this.txtValue.Text == "0")
				{
					this.txtValue.Text = "";
				}
				this.txtValue.Text += "4";
			}
		}

		private void btnKey5_Click(object sender, System.EventArgs e)
		{
			if(this.checkDateTime())
			{
				if(this.txtValue.Text == "0")
				{
					this.txtValue.Text = "";
				}
				this.txtValue.Text += "5";
			}
		}

		private void btnKey6_Click(object sender, System.EventArgs e)
		{
			if(this.checkDateTime())
			{
				if(this.txtValue.Text == "0")
				{
					this.txtValue.Text = "";
				}
				this.txtValue.Text += "6";
			}
		}

		private void btnKey7_Click(object sender, System.EventArgs e)
		{
			if(this.checkDateTime())
			{
				if(this.txtValue.Text == "0")
				{
					this.txtValue.Text = "";
				}
				this.txtValue.Text += "7";
			}
		}

		private void btnKey8_Click(object sender, System.EventArgs e)
		{
			if(this.checkDateTime())
			{
				if(this.txtValue.Text == "0")
				{
					this.txtValue.Text = "";
				}
				this.txtValue.Text += "8";
			}
		}

		private void btnKey9_Click(object sender, System.EventArgs e)
		{
			if(this.checkDateTime())
			{
				if(this.txtValue.Text == "0")
				{
					this.txtValue.Text = "";
				}
				this.txtValue.Text += "9";
			}
		}

		private void btnKey0_Click(object sender, System.EventArgs e)
		{
			if(this.checkDateTime())
			{
				if(this.txtValue.Text == "0")
				{
					this.txtValue.Text = "";
				}
				this.txtValue.Text += "0";
			}
		}

		private void btnEnter_Click(object sender, System.EventArgs e)
		{
			if(this.checkDateTime())
			{
				if(this.txtValue.Text.StartsWith("+") || this.txtValue.Text.StartsWith("-"))
				{
					this.inputValue = this.inputValue + float.Parse(this.txtValue.Text, DB4Connector.Instance.CI_EL);
				}
				else
				{
					this.inputValue = float.Parse(this.txtValue.Text, DB4Connector.Instance.CI_EL);
				}
				this.Close();
			}
		}

		private void btnBackSpace_Click(object sender, System.EventArgs e)
		{
			if(this.checkDateTime())
			{
				if(this.txtValue.Text.Length > 0)
				{
					this.txtValue.Text = this.txtValue.Text.Substring(0, this.txtValue.Text.Length - 1);
				}
				if(this.txtValue.Text == string.Empty)
				{
					this.txtValue.Text = "0";
				}
			}
		}

		private void btnPeriod_Click(object sender, System.EventArgs e)
		{
			if(this.checkDateTime())
			{
				this.txtValue.Text += ",";
			}
		}

		private void btnPlus_Click(object sender, System.EventArgs e)
		{
			if(this.checkDateTime())
			{
				if(this.txtValue.Text.StartsWith("-") || (this.txtValue.Text == "0")) 
				{
					this.txtValue.Text = "+";
				}
				else if(this.txtValue.Text.StartsWith("+") == false)
				{
					this.txtValue.Text = "+" + this.txtValue.Text;
				}		
			}
		}

		private void btnMinus_Click(object sender, System.EventArgs e)
		{
			if(this.checkDateTime())
			{
				if(this.txtValue.Text.StartsWith("+") || (this.txtValue.Text == "0")) 
				{
					this.txtValue.Text = "-";
				}
				else if(this.txtValue.Text.StartsWith("-") == false)
				{
					this.txtValue.Text = "-" + this.txtValue.Text;
				}
			}
		}

		private void NumKeypad_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if(this.checkDateTime())
			{
				switch(e.KeyChar)
				{
					case (char)Keys.D0:
						this.btnKey0_Click(sender, e);
						break;
					case (char)Keys.D1:
						this.btnKey1_Click(sender, e);
						break;
					case (char)Keys.D2:
						this.btnKey2_Click(sender, e);
						break;
					case (char)Keys.D3:
						this.btnKey3_Click(sender, e);
						break;
					case (char)Keys.D4:
						this.btnKey4_Click(sender, e);
						break;
					case (char)Keys.D5:
						this.btnKey5_Click(sender, e);
						break;
					case (char)Keys.D6:
						this.btnKey6_Click(sender, e);
						break;
					case (char)Keys.D7:
						this.btnKey7_Click(sender, e);
						break;
					case (char)Keys.D8:
						this.btnKey8_Click(sender, e);
						break;
					case (char)Keys.D9:
						this.btnKey9_Click(sender, e);
						break;
					case (char)Keys.Enter:
						this.btnEnter_Click(sender, e);
						break;
					case (char)Keys.Back:
						this.btnBackSpace_Click(sender, e);
						break;
					case (char)Keys.Decimal:
						this.btnPeriod_Click(sender, e);
						break;
					case (char)Keys.Add:
						this.btnPlus_Click(sender, e);
						break;
					case (char)Keys.Subtract:
						this.btnMinus_Click(sender, e);
						break;
				}
			}
		}

		private void NumKeypad_Load(object sender, System.EventArgs e)
		{
			this.startLoad = DateTime.Now;
			this.Focus();
		}

		private void NumKeypad_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(this.checkMaximum)
			{
				if(this.inputValue > this.maxValue)
				{
					MessageBox.Show("Η τιμή αυτή υπερβαίνει την μέγιστη επιτρέπτη.", "Προειδοποίηση", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    e.Cancel = true;
				}				
				else if(this.inputValue < this.minValue)
				{
                    MessageBox.Show("Η τιμή αυτή είναι μικρότερη από την ελάχιστη επιτρέπτη.", "Προειδοποίηση", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    e.Cancel = true;
				}
				else
				{
					e.Cancel = false;
				}
			}
		}
		#endregion

		#region Properties
		public float Result
		{
			get
			{
				return this.inputValue;
			}
		}
		#endregion

		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			this.btnKey4 = new OpenNETCF.Windows.Forms.Button2();
			this.btnKey1 = new OpenNETCF.Windows.Forms.Button2();
			this.btnKey2 = new OpenNETCF.Windows.Forms.Button2();
			this.btnKey3 = new OpenNETCF.Windows.Forms.Button2();
			this.btnKey8 = new OpenNETCF.Windows.Forms.Button2();
			this.btnKey9 = new OpenNETCF.Windows.Forms.Button2();
			this.btnKey7 = new OpenNETCF.Windows.Forms.Button2();
			this.btnKey6 = new OpenNETCF.Windows.Forms.Button2();
			this.btnKey5 = new OpenNETCF.Windows.Forms.Button2();
			this.btnKey0 = new OpenNETCF.Windows.Forms.Button2();
			this.btnBackSpace = new OpenNETCF.Windows.Forms.Button2();
			this.btnEnter = new OpenNETCF.Windows.Forms.Button2();
			this.btnPeriod = new OpenNETCF.Windows.Forms.Button2();
			this.txtValue = new OpenNETCF.Windows.Forms.TextBox2();
			this.btnMinus = new OpenNETCF.Windows.Forms.Button2();
			this.btnPlus = new OpenNETCF.Windows.Forms.Button2();
			this.SuspendLayout();
			// 
			// btnKey4
			// 
			this.btnKey4.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnKey4.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnKey4.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold);
			this.btnKey4.ForeColor = System.Drawing.Color.Black;
			this.btnKey4.ImageIndex = -1;
			this.btnKey4.ImageList = null;
			this.btnKey4.Location = new System.Drawing.Point(22, 100);
			this.btnKey4.Name = "btnKey4";
			this.btnKey4.Size = new System.Drawing.Size(48, 48);
			this.btnKey4.TabIndex = 5;
			this.btnKey4.Text = "4";
			this.btnKey4.Click += new System.EventHandler(this.btnKey4_Click);
			// 
			// btnKey1
			// 
			this.btnKey1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnKey1.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnKey1.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold);
			this.btnKey1.ForeColor = System.Drawing.Color.Black;
			this.btnKey1.ImageIndex = -1;
			this.btnKey1.ImageList = null;
			this.btnKey1.Location = new System.Drawing.Point(22, 52);
			this.btnKey1.Name = "btnKey1";
			this.btnKey1.Size = new System.Drawing.Size(48, 48);
			this.btnKey1.TabIndex = 2;
			this.btnKey1.Text = "1";
			this.btnKey1.Click += new System.EventHandler(this.btnKey1_Click);
			// 
			// btnKey2
			// 
			this.btnKey2.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnKey2.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnKey2.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold);
			this.btnKey2.ForeColor = System.Drawing.Color.Black;
			this.btnKey2.ImageIndex = -1;
			this.btnKey2.ImageList = null;
			this.btnKey2.Location = new System.Drawing.Point(70, 52);
			this.btnKey2.Name = "btnKey2";
			this.btnKey2.Size = new System.Drawing.Size(48, 48);
			this.btnKey2.TabIndex = 3;
			this.btnKey2.Text = "2";
			this.btnKey2.Click += new System.EventHandler(this.btnKey2_Click);
			// 
			// btnKey3
			// 
			this.btnKey3.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnKey3.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnKey3.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold);
			this.btnKey3.ForeColor = System.Drawing.Color.Black;
			this.btnKey3.ImageIndex = -1;
			this.btnKey3.ImageList = null;
			this.btnKey3.Location = new System.Drawing.Point(118, 52);
			this.btnKey3.Name = "btnKey3";
			this.btnKey3.Size = new System.Drawing.Size(48, 48);
			this.btnKey3.TabIndex = 4;
			this.btnKey3.Text = "3";
			this.btnKey3.Click += new System.EventHandler(this.btnKey3_Click);
			// 
			// btnKey8
			// 
			this.btnKey8.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnKey8.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnKey8.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold);
			this.btnKey8.ForeColor = System.Drawing.Color.Black;
			this.btnKey8.ImageIndex = -1;
			this.btnKey8.ImageList = null;
			this.btnKey8.Location = new System.Drawing.Point(70, 148);
			this.btnKey8.Name = "btnKey8";
			this.btnKey8.Size = new System.Drawing.Size(48, 48);
			this.btnKey8.TabIndex = 9;
			this.btnKey8.Text = "8";
			this.btnKey8.Click += new System.EventHandler(this.btnKey8_Click);
			// 
			// btnKey9
			// 
			this.btnKey9.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnKey9.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnKey9.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold);
			this.btnKey9.ForeColor = System.Drawing.Color.Black;
			this.btnKey9.ImageIndex = -1;
			this.btnKey9.ImageList = null;
			this.btnKey9.Location = new System.Drawing.Point(118, 148);
			this.btnKey9.Name = "btnKey9";
			this.btnKey9.Size = new System.Drawing.Size(48, 48);
			this.btnKey9.TabIndex = 10;
			this.btnKey9.Text = "9";
			this.btnKey9.Click += new System.EventHandler(this.btnKey9_Click);
			// 
			// btnKey7
			// 
			this.btnKey7.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnKey7.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnKey7.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold);
			this.btnKey7.ForeColor = System.Drawing.Color.Black;
			this.btnKey7.ImageIndex = -1;
			this.btnKey7.ImageList = null;
			this.btnKey7.Location = new System.Drawing.Point(22, 148);
			this.btnKey7.Name = "btnKey7";
			this.btnKey7.Size = new System.Drawing.Size(48, 48);
			this.btnKey7.TabIndex = 8;
			this.btnKey7.Text = "7";
			this.btnKey7.Click += new System.EventHandler(this.btnKey7_Click);
			// 
			// btnKey6
			// 
			this.btnKey6.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnKey6.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnKey6.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold);
			this.btnKey6.ForeColor = System.Drawing.Color.Black;
			this.btnKey6.ImageIndex = -1;
			this.btnKey6.ImageList = null;
			this.btnKey6.Location = new System.Drawing.Point(118, 100);
			this.btnKey6.Name = "btnKey6";
			this.btnKey6.Size = new System.Drawing.Size(48, 48);
			this.btnKey6.TabIndex = 7;
			this.btnKey6.Text = "6";
			this.btnKey6.Click += new System.EventHandler(this.btnKey6_Click);
			// 
			// btnKey5
			// 
			this.btnKey5.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnKey5.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnKey5.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold);
			this.btnKey5.ForeColor = System.Drawing.Color.Black;
			this.btnKey5.ImageIndex = -1;
			this.btnKey5.ImageList = null;
			this.btnKey5.Location = new System.Drawing.Point(70, 100);
			this.btnKey5.Name = "btnKey5";
			this.btnKey5.Size = new System.Drawing.Size(48, 48);
			this.btnKey5.TabIndex = 6;
			this.btnKey5.Text = "5";
			this.btnKey5.Click += new System.EventHandler(this.btnKey5_Click);
			// 
			// btnKey0
			// 
			this.btnKey0.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnKey0.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnKey0.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold);
			this.btnKey0.ForeColor = System.Drawing.Color.Black;
			this.btnKey0.ImageIndex = -1;
			this.btnKey0.ImageList = null;
			this.btnKey0.Location = new System.Drawing.Point(70, 196);
			this.btnKey0.Name = "btnKey0";
			this.btnKey0.Size = new System.Drawing.Size(48, 48);
			this.btnKey0.TabIndex = 12;
			this.btnKey0.Text = "0";
			this.btnKey0.Click += new System.EventHandler(this.btnKey0_Click);
			// 
			// btnBackSpace
			// 
			this.btnBackSpace.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnBackSpace.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnBackSpace.Font = new System.Drawing.Font("Wingdings", 28F, System.Drawing.FontStyle.Bold);
			this.btnBackSpace.ForeColor = System.Drawing.Color.Blue;
			this.btnBackSpace.ImageIndex = -1;
			this.btnBackSpace.ImageList = null;
			this.btnBackSpace.Location = new System.Drawing.Point(22, 196);
			this.btnBackSpace.Name = "btnBackSpace";
			this.btnBackSpace.Size = new System.Drawing.Size(48, 48);
			this.btnBackSpace.TabIndex = 11;
			this.btnBackSpace.Text = "i";
			this.btnBackSpace.Click += new System.EventHandler(this.btnBackSpace_Click);
			// 
			// btnEnter
			// 
			this.btnEnter.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnEnter.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnEnter.Font = new System.Drawing.Font("Wingdings", 28F, System.Drawing.FontStyle.Bold);
			this.btnEnter.ForeColor = System.Drawing.Color.Blue;
			this.btnEnter.ImageIndex = -1;
			this.btnEnter.ImageList = null;
			this.btnEnter.Location = new System.Drawing.Point(166, 148);
			this.btnEnter.Name = "btnEnter";
			this.btnEnter.Size = new System.Drawing.Size(48, 96);
			this.btnEnter.TabIndex = 0;
			this.btnEnter.Text = "A";
			this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
			// 
			// btnPeriod
			// 
			this.btnPeriod.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnPeriod.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnPeriod.Font = new System.Drawing.Font("Tahoma", 26.25F, System.Drawing.FontStyle.Bold);
			this.btnPeriod.ForeColor = System.Drawing.Color.Blue;
			this.btnPeriod.ImageIndex = -1;
			this.btnPeriod.ImageList = null;
			this.btnPeriod.Location = new System.Drawing.Point(118, 196);
			this.btnPeriod.Name = "btnPeriod";
			this.btnPeriod.Size = new System.Drawing.Size(48, 48);
			this.btnPeriod.TabIndex = 13;
			this.btnPeriod.Text = ",";
			this.btnPeriod.Click += new System.EventHandler(this.btnPeriod_Click);
			// 
			// txtValue
			// 
			this.txtValue.BackColor = System.Drawing.Color.White;
			this.txtValue.CharacterCasing = OpenNETCF.Windows.Forms.CharacterCasing.Normal;
			this.txtValue.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold);
			this.txtValue.Location = new System.Drawing.Point(0, 0);
			this.txtValue.MaxLength = 17;
			this.txtValue.Multiline = true;
			this.txtValue.Name = "txtValue";
			this.txtValue.ReadOnly = true;
			this.txtValue.Size = new System.Drawing.Size(240, 32);
			this.txtValue.TabIndex = 1;
			this.txtValue.Text = "0";
			this.txtValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.txtValue.WordWrap = false;
			this.txtValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumKeypad_KeyPress);
			// 
			// btnMinus
			// 
			this.btnMinus.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnMinus.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnMinus.Font = new System.Drawing.Font("Tahoma", 27.75F, System.Drawing.FontStyle.Bold);
			this.btnMinus.ForeColor = System.Drawing.Color.Red;
			this.btnMinus.ImageIndex = -1;
			this.btnMinus.ImageList = null;
			this.btnMinus.Location = new System.Drawing.Point(166, 100);
			this.btnMinus.Name = "btnMinus";
			this.btnMinus.Size = new System.Drawing.Size(48, 48);
			this.btnMinus.TabIndex = 15;
			this.btnMinus.Text = "-";
			this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
			// 
			// btnPlus
			// 
			this.btnPlus.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnPlus.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnPlus.Font = new System.Drawing.Font("Tahoma", 27.75F, System.Drawing.FontStyle.Bold);
			this.btnPlus.ForeColor = System.Drawing.Color.Green;
			this.btnPlus.ImageIndex = -1;
			this.btnPlus.ImageList = null;
			this.btnPlus.Location = new System.Drawing.Point(166, 52);
			this.btnPlus.Name = "btnPlus";
			this.btnPlus.Size = new System.Drawing.Size(48, 48);
			this.btnPlus.TabIndex = 14;
			this.btnPlus.Text = "+";
			this.btnPlus.Click += new System.EventHandler(this.btnPlus_Click);
			// 
			// NumKeypad
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.ClientSize = new System.Drawing.Size(237, 267);
			this.Controls.Add(this.btnMinus);
			this.Controls.Add(this.btnPlus);
			this.Controls.Add(this.txtValue);
			this.Controls.Add(this.btnPeriod);
			this.Controls.Add(this.btnKey0);
			this.Controls.Add(this.btnKey5);
			this.Controls.Add(this.btnKey6);
			this.Controls.Add(this.btnKey7);
			this.Controls.Add(this.btnKey9);
			this.Controls.Add(this.btnKey8);
			this.Controls.Add(this.btnKey3);
			this.Controls.Add(this.btnKey2);
			this.Controls.Add(this.btnKey1);
			this.Controls.Add(this.btnKey4);
			this.Controls.Add(this.btnEnter);
			this.Controls.Add(this.btnBackSpace);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NumKeypad";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.NumKeypad_Closing);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumKeypad_KeyPress);
			this.Load += new System.EventHandler(this.NumKeypad_Load);
			this.ResumeLayout(false);
		}
		#endregion
	}
}

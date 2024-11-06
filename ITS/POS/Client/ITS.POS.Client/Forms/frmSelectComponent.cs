using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client;
using ITS.POS.Client.Extensions;
using ITS.POS.Client.ObserverPattern;

namespace ITS.POS.Client.Forms
{
    public partial class frmSelectComponent : Form
    {
        Control SenderParentForm { get; set; }
        Control Sender { get; set; }
        public frmSelectComponent(List<string> selectedObservers,Form parentForm,Control sender)
        {
            //MessageBox.Show("Form Init start");
            InitializeComponent();
            //MessageBox.Show("Component Init End");

            this.SenderParentForm = parentForm;
            this.Sender = sender;

            //MessageBox.Show("Controls enumeration start");

            foreach (Control control in GetAllChildControls(SenderParentForm))
            {
                //MessageBox.Show("Control :" + control);
                if (control is IObserver && control != sender)
                {
                    this.checkedListBox1.Items.Add(control);
                }

            }

            //MessageBox.Show("Controls enumeration end");


            //MessageBox.Show("Observers selection Start");

            if (selectedObservers != null)
            {
                foreach (string obs in selectedObservers)
                {
                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        if ((checkedListBox1.Items[i] as Control).GetName() == obs)
                        {
                            checkedListBox1.SetItemChecked(i, true);
                        }
                    }

                }
            }

            //MessageBox.Show("Observers selection end");
        }


        public List<IObserver> GetCheckedObservers()
        {
            //MessageBox.Show("Get Checked Observers start");
            List<IObserver> observers = new List<IObserver>();
            foreach(object item in checkedListBox1.CheckedItems)
            {
                //MessageBox.Show("Item :" + item);
                observers.Add(item as IObserver);
            }
            //MessageBox.Show("List count: "+ observers.Count);
            //MessageBox.Show("Get Checked Observers end");
            return observers;

        }


        public List<string> GetCheckedObserversNames()
        {
            List<string> checkedObserversNames = new List<string>();
            List<IObserver> checkedObservers = GetCheckedObservers();
            foreach(IObserver obs in checkedObservers)
            {
                checkedObserversNames.Add((obs as Control).GetName());
            }
            return checkedObserversNames;

        }

        public IEnumerable<Control> GetAllChildControls(Control control, Type type = null)
        {
            var controls = control.Controls.Cast<Control>();


            if (type == null)
            {
                return controls.SelectMany(ctrl => GetAllChildControls(ctrl, type))
                      .Concat(controls);
            }
            else
            {
                return controls.SelectMany(ctrl => GetAllChildControls(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}

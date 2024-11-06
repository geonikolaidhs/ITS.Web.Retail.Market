using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using ITS.Common.Attributes;
using System.Collections;

namespace ITS.Common.Utilities.Forms
{
    internal partial class ITSInputForm : Form
    {
        public object ReturnValue { get; protected set; }

        Type returnType;

        protected int GetTop(int i)
        {
            return 30 * i+20;
        }

        public ITSInputForm(Type type, object obj)
        {
            InitializeComponent();
            returnType = type;

            DescriptionAttribute[] classDescriptions = type.GetCustomAttributes(typeof(DescriptionAttribute),true) as DescriptionAttribute[];
            this.Text = (classDescriptions.Length == 0)?type.Name:classDescriptions.First().Description;
            int i = 0;
            this.Width = 500;
            this.Height = type.GetProperties().Length * 40 + 40;
            foreach (PropertyInfo propInfo in type.GetProperties())
            {

                
                DescriptionAttribute[] propsDescriptions = propInfo.GetCustomAttributes(typeof(DescriptionAttribute), true) as DescriptionAttribute[];
                BrowsableAttribute[] propsBrowsable = propInfo.GetCustomAttributes(typeof(BrowsableAttribute), true) as BrowsableAttribute[];
                LookupAttribute[] propsLookup = propInfo.GetCustomAttributes(typeof(LookupAttribute), true) as LookupAttribute[];
                if(propsBrowsable.Length == 1 && propsBrowsable[0].Browsable ==false)
                {
                    continue;
                }

                object value=null;
                try
                {
                    value = propInfo.GetValue(obj, null);
                }
                catch
                {
                }
                
                
                String labelText = propsDescriptions.Length == 0 ? propInfo.Name : propsDescriptions.First().Description;
                if (propsLookup.Length == 1)
                {
                    Label lbl = new Label() { Text = labelText, Top = GetTop(i), Left = 10, AutoSize = false, Width = 100, Height = 25 };
                    ComboBox cmb = new ComboBox() { Name = "property_" + propInfo.Name, Top = lbl.Top, Left = 120, AutoSize = false, Width = 200, Height = 25 };
                    LookupAttribute lup = propsLookup[0];

                    MethodInfo mInfo = lup.type.GetMethod(lup.Method);
                    object objc = null;
                    if (!mInfo.IsStatic)
                    {
                        objc = Activator.CreateInstance(lup.type);
                    }
                    var result =   mInfo.Invoke(objc, null);
                    if (result is IEnumerable)
                    {
                        cmb.Items.AddRange((result as IEnumerable).Cast<object>().ToArray());
                    }
                    this.Controls.Add(lbl);
                    this.Controls.Add(cmb);
                    cmb.SelectedItem = value;
                }
                else if (propInfo.PropertyType == typeof(String) || propInfo.PropertyType == typeof(int) || propInfo.PropertyType == typeof(long))
                {
                    Label lbl = new Label() { Text = labelText, Top = GetTop(i), Left = 10, AutoSize = false, Width = 100, Height = 25 };

                    TextBox txt = new TextBox() { Name = "property_" + propInfo.Name, Top = lbl.Top, Left = 120, AutoSize = false, Width = 200, Height = 25 };

                    this.Controls.Add(lbl);
                    this.Controls.Add(txt);
                    if(value!=null)
                        txt.Text = value.ToString();
                }
                else if (propInfo.PropertyType == typeof(bool))
                {
                    CheckBox chkbox = new CheckBox() { Name = "property_" + propInfo.Name, Text = labelText, Top = GetTop(i), Left = 10, AutoSize = false, Width = 310, Height = 25 };
                    this.Controls.Add(chkbox);
                    if (value != null)
                        chkbox.Checked = (bool)value;
                }
                else if (propInfo.PropertyType == typeof(DateTime))
                {
                    Label lbl = new Label() { Text = labelText, Top = GetTop(i), Left = 10, AutoSize = false, Width = 100, Height = 25 };
                    DateTimePicker txt = new DateTimePicker() { Name = "property_" + propInfo.Name, Top = lbl.Top, Left = 120, AutoSize = false, Width = 200, Height = 25 };

                    this.Controls.Add(lbl);
                    this.Controls.Add(txt);
                    if (value != null)
                        txt.Value = (DateTime)value;
                }
                else if (propInfo.PropertyType.IsEnum)
                {
                    Label lbl = new Label() { Text = labelText, Top = GetTop(i), Left = 10, AutoSize = false, Width = 100, Height = 25 };
                    ComboBox combo = new ComboBox() { Name = "property_" + propInfo.Name, Top = lbl.Top, Left = 120, AutoSize = false, Width = 200, Height = 25 };
                    combo.Items.AddRange(Enum.GetNames(propInfo.PropertyType));

                    this.Controls.Add(lbl);
                    this.Controls.Add(combo);
                    if(value !=null)
                        combo.SelectedItem = value.ToString();
                }
                i++;
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!(returnType.IsAbstract && returnType.IsSealed))
            {
                ReturnValue = Activator.CreateInstance(returnType);
            }
            else
            {
                ReturnValue = null;
            }

            foreach (PropertyInfo propInfo in returnType.GetProperties())
            {
                BrowsableAttribute[] propsBrowsable = propInfo.GetCustomAttributes(typeof(BrowsableAttribute), true) as BrowsableAttribute[];
                if (propsBrowsable.Length == 1 && propsBrowsable[0].Browsable == false)
                {
                    continue;
                }
                IEnumerable<Control> controlsCastWhere = this.Controls.Cast<Control>().Where(g => g.Name == "property_" + propInfo.Name);
                if (controlsCastWhere.Count() == 0)
                {
                    continue;
                }
                Control ctrl = controlsCastWhere.First();
                if (propInfo.PropertyType == typeof(int) || propInfo.PropertyType == typeof(long))
                {
                    IEnumerable<MemberInfo> inf = propInfo.PropertyType.GetMember("Parse").Where(g => (g as MethodInfo).GetParameters().Count() == 1 &&
                        (g as MethodInfo).GetParameters()[0].ParameterType == typeof(String));
                    if (inf.Count() == 1)
                    {
                        MethodInfo mi = inf.First() as MethodInfo;
                        var v = mi.Invoke(null, new object[] { ctrl.Text });
                        propInfo.SetValue(ReturnValue,v, null);
                    }
                    
                }
                else if (propInfo.PropertyType == typeof(string))
                {
                    propInfo.SetValue(ReturnValue, ctrl.Text, null);
                }
                else if (propInfo.PropertyType == typeof(bool))
                {
                    propInfo.SetValue(ReturnValue, (ctrl as CheckBox).Checked, null);

                }
                else if (propInfo.PropertyType == typeof(DateTime))
                {
                    propInfo.SetValue(ReturnValue, (ctrl as DateTimePicker).Value, null);
                }
                else if (propInfo.PropertyType.IsEnum)
                {
                    if ((ctrl as ComboBox).SelectedItem != null)
                    {
                        propInfo.SetValue(ReturnValue, Enum.Parse(propInfo.PropertyType, (ctrl as ComboBox).SelectedItem.ToString()), null);
                    }
                }

            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
using System.IO;
using System.Globalization;
using System.Xml;
using Retail.Mobile.Helpers;

namespace Retail.Mobile
{
    public partial class SettingsForm : Form
    {
        public MainForm ParentForm { get; set; }

        public SettingsForm(Form parentForm)
        {
            try
            {
                ParentForm = parentForm as MainForm;
            }
            catch
            {
            }
            InitializeComponent();
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.Resources.ConfigConfirmMessage, AppSettings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                try
                {
                    XmlDocument xml = new XmlDocument();
                    if (!File.Exists(AppSettings.executingPath + "\\config.xml"))
                    {
                        xml.LoadXml("<?xml version=\"1.0\"?><request  errorcode=\"\" errordescr=\"\"><settings><item id=\"ip\">0.0.0.0</item><item id=\"PDA_ID\">1</item><item id=\"LocalDBPath\"></item><item id=\"Username\"></item><item id=\"Password\"></item><item id=\"Language\"></item></settings></request>");                       
                    }
                    else
                        xml.Load(AppSettings.executingPath + "\\config.xml");
                    AppSettings.IP = txt_ip.Text;
                    AppSettings.Pda_ID = txt_pdaID.Text;
                    AppSettings.databasePath = txtLocalDBPath.Text;
                    XmlNode settingsNode = xml["request"]["settings"];
                    foreach (XmlNode node in settingsNode.ChildNodes)
                    {
                        if (node.Attributes["id"].Value == "ip")
                            node.InnerText = AppSettings.IP;
                        else if (node.Attributes["id"].Value == "PDA_ID")
                            node.InnerText = AppSettings.Pda_ID;
                        else if (node.Attributes["id"].Value == "LocalDBPath")
                            node.InnerText = AppSettings.databasePath;
                        else if (node.Attributes["id"].Value == "Language")
                        {
                            string lang = "en-US";
                            switch (cmbLanguage.SelectedItem.ToString())
                            {
                                case "Ελληνικά":
                                    lang = "el-gr";
                                    break;
                                case "Norsk":
                                    lang = "nb-NO";
                                    break;
                                case "English":
                                default:
                                    lang = "en-US";
                                    break;
                            }
                            //AppSettings.databasePath = node.InnerText;
                            node.InnerText = lang;
                            CultureHelper.SetCulture(new CultureInfo(lang));//Common.CultureInfo = new CultureInfo(lang);// InputXml.GetProperty("settings", "Language") );
                        }
                    }
                      
                    xml.Save(AppSettings.executingPath + "\\config.xml");
                    this.Close();
                    /*
                    NewXmlCreator settingsxml = new NewXmlCreator("", "");
                    List<string> data = new List<string>();

                    data.Add("text=" + txt_ip.Text + "|id=ip");
                    AppSettings.IP = txt_ip.Text;

                    data.Add("text=" + txt_pdaID.Text + "|id=PDA_ID");
                    
                    data.Add("text=" + txtLocalDBPath.Text + "|id=LocalDBPath");
                    AppSettings.databasePath = txtLocalDBPath.Text;
                    data.Add("text=|id=Username");
                    data.Add("text=|id=Password");

                    string lang = "en";
                    switch(cmbLanguage.SelectedItem.ToString()){                        
                        case  "Ελληνικά" :
                            lang = "el-gr";
                            break;
                        case "Norsk" :
                            lang = "no";
                            break;
                        case "English" :
                        default:
                            lang = "en";
                            break;
                    }
                    Common.CultureInfo = new CultureInfo(lang);
                    Resources.Resources.Culture = Common.CultureInfo;
                    if (ParentForm != null)
                    {
                        ParentForm.TranslateMainForm();
                    }

                    data.Add("text="+lang+"|id=Language");


                    settingsxml.CreateNodes("settings", "item", data.ToArray());
                    settingsxml.Xmlclose();

                    using (StreamWriter sw = new StreamWriter(AppSettings.executingPath + "\\config.xml", false, Encoding.UTF8))
                    {
                        sw.Write(settingsxml.FormatedMyXml);
                        sw.Close();
                    }
                    this.Close();
                     * */
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            btn_Save.Text = Resources.Resources.Save;
            button1.Text = Resources.Resources.Cancel;

            txt_ip.Text = AppSettings.IP;
            txt_pdaID.Text = AppSettings.Pda_ID;
            txtLocalDBPath.Text = AppSettings.databasePath;

            try
            {

                switch (Resources.Resources.Culture.ToString())
                {
                    case "el-GR":
                    case "el-gr":
                        cmbLanguage.SelectedIndex = 1;
                        break;
                    case "nb-NO":
                        cmbLanguage.SelectedIndex = 2;
                        break;
                    case "en-US":
                    default:
                        cmbLanguage.SelectedIndex = 0;
                        break;
                }
            }
            catch
            {
                cmbLanguage.SelectedIndex = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txt_pdaID_TextChanged(object sender, EventArgs e)
        {

        }
        
    }
}
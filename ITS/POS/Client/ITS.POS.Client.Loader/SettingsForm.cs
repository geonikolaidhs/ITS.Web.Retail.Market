using POSLoader.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace POSLoader
{
    public partial class SettingsForm : Form
    {
        private string global_config_file;
        private LogHelper logger;
        private string web_service_file;// = "/POSUpdateService.asmx";//Προσοχή στο πρώτο '/'!!!

        public SettingsForm(string global_conf_filename, string web_service_file, LogHelper logger)
        {
            InitializeComponent();
            global_config_file = global_conf_filename;
            this.logger = logger;
            this.web_service_file = web_service_file;
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!File.Exists(global_config_file))
            {
                e.Cancel = true;
            }
        }

        private void btnInitialise_Click(object sender, EventArgs e)
        {
            #region Validate Inputs

            if (txtbxServerUrl.Text.Length == 0)
            {
                logger.Error("Please specify Server URL");
                return;
            }
            if (txtbxDeviceID.Text.Length == 0)
            {
                logger.Error("Please specify Device ID");
                return;
            }
            #endregion

            #region Try to connect to Server(Store Controller)
            logger.Message("Attempting to connect to Server");
            int deviceID;
            if (int.TryParse(txtbxDeviceID.Text, out deviceID))
            {
                string globals_xml = "";
                try
                {
                    string web_service_url = txtbxServerUrl.Text.TrimEnd('/') + web_service_file;
                    string web_method = "GetGlobalsXml";
                    Dictionary<string, object> args = new Dictionary<string, object>();
                    args.Add("posid", deviceID);
                    globals_xml = (string)WebServiceHelper.ExecuteCallWebServiceCommand(web_service_url, web_method, args);
                    XmlDocument xml_doc = new XmlDocument();
                    xml_doc.LoadXml(globals_xml);
                    if (globals_xml.Contains("<Error>"))
                    {
                        XmlNode error_node = xml_doc.SelectSingleNode("Error");
                        throw new Exception(error_node.InnerText);
                    }

                    //Save configuration file ( global_conf_filename)
                    xml_doc.Save(global_config_file);
                    this.Close();
                }
                catch (Exception exc)
                {
                    string error_message = exc.Message;
                    if (exc.InnerException != null && exc.InnerException.Message != "")
                    {
                        error_message += Environment.NewLine + exc.InnerException.Message;
                    }
                    logger.Error(error_message + Environment.NewLine + globals_xml);
                }

            }
            #endregion
        }
    }
}

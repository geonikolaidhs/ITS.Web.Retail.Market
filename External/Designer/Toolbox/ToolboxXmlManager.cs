using System;
using System.Xml;
using System.Windows.Forms;
using ITS.POS.Client.UserControls;
using DevExpress.XtraTab;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraRichEdit;
using System.Linq;
using ITS.POS.Client.Forms;

namespace Designer.ToolboxLibrary
{
    /// <summary>
    /// ToolboxXmlManager - Reads an XML file and populates the toolbox.
    /// </summary>
    public class ToolboxXmlManager
    {
        Toolbox m_toolbox = null;
        //*
        private Type[] windowsFormsToolTypes = new Type[] {
            typeof(Label),typeof(GroupBox), typeof(PictureBox),
            typeof(Panel), typeof(System.Windows.Forms.HScrollBar), typeof(System.Windows.Forms.VScrollBar),
             typeof(Splitter), typeof(WebBrowser), typeof(TabControl), typeof(TableLayoutPanel)

        }.OrderBy(x => x.Name).ToArray();

        private Type[] dataToolTypes = new Type[] {
            typeof(System.Data.OleDb.OleDbCommandBuilder), typeof(System.Data.OleDb.OleDbConnection), typeof(System.Data.SqlClient.SqlCommandBuilder), typeof(System.Data.SqlClient.SqlConnection),
        };
        private Type[] userControlsToolTypes = new Type[] {
            typeof(ucErrorMessage),typeof(ucCheckPriceButton),typeof(ucKeyPad),
            typeof(ucLinePriceView),typeof(ucLineQuantity),typeof(ucTotalPriceView),typeof(ucLogo),typeof(ucAddItemButton),
            typeof(ucActionButton),typeof(ucCommunicationStatus),typeof(ucIssueX),typeof(ucIssueZ),typeof(ucDeleteLineButton),//typeof(AddPaymentButton), deprecated
            typeof(ucAddTotalPaymentButton),typeof(ucDoubleLineView),typeof(ucCashierMessage), typeof(ucPosStatus), typeof(ucCommunicationStatusSmall),
            typeof(ucCustomKeyPressButton),typeof(ucDocumentChange), typeof(ucPaymentDetailsGrid), typeof(ucDetailDataGrid),typeof(ucTotalQuantity),
            typeof(ucDocumentRemainingAmount), typeof(ucDiscountsDataGrid), typeof(ucDocumentDiscount), typeof(ucSetFiscalError), typeof(ucAddWeightedItemButton),
            typeof(ITS.POS.Client.UserControls.ucImageSlider),
            typeof(ucUseDocumentType), typeof(ucGroupButton), typeof(ucItemCategoryGroupButton), typeof(ucBlinkingErrorMessage),typeof(ucReportGroupButton),typeof(ucUseReportTypeButton)
        }.OrderBy(x => x.Name).ToArray();

        public ToolboxXmlManager(Toolbox toolbox)
        {
            m_toolbox = toolbox;
        }

        public ToolboxTabCollection PopulateToolboxInfo()
        {
            try
            {
                if (Toolbox.FilePath == null || Toolbox.FilePath == "" || Toolbox.FilePath == String.Empty)
                    return PopulateToolboxTabs();

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(Toolbox.FilePath);
                return PopulateToolboxTabs(xmlDocument);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured in reading Toolbox.xml file.\n" + ex.ToString());
                return null;
            }
        }

        private Toolbox Toolbox
        {
            get
            {
                return m_toolbox;
            }
        }

        private ToolboxTabCollection PopulateToolboxTabs()
        {
            ToolboxTabCollection toolboxTabs = new ToolboxTabCollection();
            string[] tabNames = { Strings.WindowsForms, /*Strings.Components, Strings.Data,*/ Strings.UserControls };

            for (int i = 0; i < tabNames.Length; i++)
            {
                ToolboxTab toolboxTab = new ToolboxTab();

                toolboxTab.Name = tabNames[i];
                PopulateToolboxItems(toolboxTab);
                toolboxTabs.Add(toolboxTab);
            }

            return toolboxTabs;
        }
        private void PopulateToolboxItems(ToolboxTab toolboxTab)
        {
            if (toolboxTab == null)
                return;

            Type[] typeArray = null;

            switch (toolboxTab.Name)
            {
                case Strings.WindowsForms:
                    typeArray = windowsFormsToolTypes;
                    break;
                //case Strings.Components:
                //	typeArray = componentsToolTypes;
                //	break;
                case Strings.Data:
                    typeArray = dataToolTypes;
                    break;
                case Strings.UserControls:
                    typeArray = userControlsToolTypes;
                    break;
                default:
                    break;
            }

            ToolboxItemCollection toolboxItems = new ToolboxItemCollection();

            for (int i = 0; i < typeArray.Length; i++)
            {
                ToolboxItem toolboxItem = new ToolboxItem();

                toolboxItem.Type = typeArray[i];
                toolboxItem.Name = typeArray[i].Name;
                toolboxItems.Add(toolboxItem);
            }

            toolboxTab.ToolboxItems = toolboxItems;
        }

        private ToolboxTabCollection PopulateToolboxTabs(XmlDocument xmlDocument)
        {
            if (xmlDocument == null)
                return null;

            XmlNode toolboxNode = xmlDocument.FirstChild;
            if (toolboxNode == null)
                return null;

            XmlNode tabCollectionNode = toolboxNode.FirstChild;
            if (tabCollectionNode == null)
                return null;

            XmlNodeList tabsNodeList = tabCollectionNode.ChildNodes;
            if (tabsNodeList == null)
                return null;

            ToolboxTabCollection toolboxTabs = new ToolboxTabCollection();

            foreach (XmlNode tabNode in tabsNodeList)
            {
                if (tabNode == null)
                    continue;

                XmlNode propertiesNode = tabNode.FirstChild;
                if (propertiesNode == null)
                    continue;

                XmlNode nameNode = propertiesNode[Strings.Name];
                if (nameNode == null)
                    continue;

                ToolboxTab toolboxTab = new ToolboxTab();
                toolboxTab.Name = nameNode.InnerXml.ToString();
                PopulateToolboxItems(tabNode, toolboxTab);
                toolboxTabs.Add(toolboxTab);
            }
            if (toolboxTabs.Count == 0)
                return null;

            return toolboxTabs;
        }

        private void PopulateToolboxItems(XmlNode tabNode, ToolboxTab toolboxTab)
        {
            if (tabNode == null)
                return;

            XmlNode toolboxItemCollectionNode = tabNode[Strings.ToolboxItemCollection];
            if (toolboxItemCollectionNode == null)
                return;

            XmlNodeList toolboxItemNodeList = toolboxItemCollectionNode.ChildNodes;
            if (toolboxItemNodeList == null)
                return;

            ToolboxItemCollection toolboxItems = new ToolboxItemCollection();

            foreach (XmlNode toolboxItemNode in toolboxItemNodeList)
            {
                if (toolboxItemNode == null)
                    continue;

                XmlNode typeNode = toolboxItemNode[Strings.Type];
                if (typeNode == null)
                    continue;

                bool found = false;
                System.Reflection.Assembly[] loadedAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < loadedAssemblies.Length && !found; i++)
                {
                    System.Reflection.Assembly assembly = loadedAssemblies[i];
                    System.Type[] types = assembly.GetTypes();
                    for (int j = 0; j < types.Length && !found; j++)
                    {
                        System.Type type = types[j];
                        if (type.FullName == typeNode.InnerXml.ToString())
                        {
                            ToolboxItem toolboxItem = new ToolboxItem();
                            toolboxItem.Type = type;
                            toolboxItems.Add(toolboxItem);
                            found = true;
                        }
                    }
                }
            }
            toolboxTab.ToolboxItems = toolboxItems;
            return;
        }

        private class Strings
        {
            public const string Toolbox = "Toolbox";
            public const string TabCollection = "TabCollection";
            public const string Tab = "Tab";
            public const string Properties = "Properties";
            public const string Name = "Name";
            public const string ToolboxItemCollection = "ToolboxItemCollection";
            public const string ToolboxItem = "ToolboxItem";
            public const string Type = "Type";
            public const string WindowsForms = "Windows Forms";
            public const string Components = "Components";
            public const string Data = "Data";
            public const string UserControls = "User Controls";
        }

    }// class
}// namespace

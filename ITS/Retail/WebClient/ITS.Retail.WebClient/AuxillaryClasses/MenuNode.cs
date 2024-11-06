using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.WebClient.AuxillaryClasses
{

    public class MenuNode
    {
        protected List<String> forbidden;
        private UrlHelper _Url ;
        public UrlHelper Url
        {
            get
            {
                return this._Url;
            }
            set
            {
                this._Url = value;
                Children.ForEach(x => x.Url = value);
            }
        }

        public MenuNode(List<String> forbidden, UrlHelper url) { Children = new List<MenuNode>(); this.forbidden = forbidden; Url = url; ShowOrder = -1; }

        public List<MenuNode> Children { get; set; }
        public MenuNode Parent { get; set; }
        public String Caption { get; set; }
        public String DirectLink { get; set; }
        public String Javascript { get; set; }
        public bool DataOid {get; set;}
        public String DataDocType { get; set;}
        public eDivision DataMode { get; set; }
        private String _HtmlID;
        public String HtmlID
        {
            get
            {
                return _HtmlID;
            }
            set
            {
                _HtmlID = value;
                IDSuffixChanged = true;
            }
        }
        public String HtmlClass { get; set; }
        string _Controller;
        public String Controller
        {
            get
            {
                return _Controller;
            }
            set
            {
                _Controller = value;
                IDSuffixChanged = true;
            }
        }
        String _Action;
        public String Action
        {
            get
            {
                return _Action;
            }
            set
            {
                _Action = value;
                IDSuffixChanged = true;
            }
        }
        object _LinkParameters;
        public object LinkParameters
        {
            get
            {
                return _LinkParameters;
            }
            set
            {
                _LinkParameters = value;
                IDSuffixChanged = true;
            }
        }
        public int ShowOrder { get; set; }


        private Guid _UniqueIdentifier = Guid.Empty;
        bool IDSuffixChanged;
        public Guid UniqueIdentifier
        {
            get
            {
                if (_UniqueIdentifier == Guid.Empty || IDSuffixChanged)
                {
                    using (MD5 md5 = MD5.Create())
                    {
                        byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(IDSuffix));
                        _UniqueIdentifier = new Guid(hash);
                        IDSuffixChanged = false;
                    }
                }
                return _UniqueIdentifier;
            }
        }

        private String IDSuffix
        {
            get
            {
                return this.Controller + '_' + this.Action + '_' + this.LinkParameters + '_' + this.HtmlID;
            }
        }

        public String CheckBoxID
        {
            get
            {
                return "chk_" + this.UniqueIdentifier.ToString();
                //this.IDSuffix;
            }
        }

        public String DockPanelID
        {
            get
            {
                return "dockPanel_" + this.UniqueIdentifier.ToString();
                //+ this.IDSuffix;
            }
        }

        public String HtmlPropertiesString
        {
            get
            {
                String toReturn = "";
                if (String.IsNullOrWhiteSpace(HtmlClass) == false)
                {
                    toReturn += " class=\"" + HtmlClass + "\"";
                }
                return toReturn;
            }
        }

        public String DockPanelHtmlPropertiesString
        {
            get
            {
                String toReturn = "";
                if (String.IsNullOrWhiteSpace(HtmlClass) == false)
                {
                    toReturn += HtmlClass;
                }

                return toReturn;
            }
        }

        public String JavascriptString
        {
            get
            {
                String toReturn = "";
                if (String.IsNullOrWhiteSpace(Javascript) == false)
                {
                    toReturn += " onClick=\"" + Javascript + "\"";
                }
                return toReturn;
            }
        }

        public String DataOidString
        {
            get
            {
                String toReturn = "";
                if (DataOid)
                {
                    toReturn += " data-oid=\"" + Guid.Empty + "\"";
                }

                return toReturn;
            }
        }

        public String DataDocTypeString
        {
            get
            {
                String toReturn = "";
                if (String.IsNullOrWhiteSpace(DataDocType) == false)
                {
                    toReturn += " data-doctype=\"" + DataDocType + "\"";
                }

                return toReturn;
            }
        }

        public String DataModeString
        {
            get
            {
                String toReturn = "";
                if (Enum.IsDefined(typeof(eDivision), DataMode))
                {
                    toReturn += " data-mode=\"" + DataMode + "\"";
                }

                return toReturn;
            }
        }

        public String HtmlLinkString
        {
            get
            {
                return "<a href=\"" + Link + "\"" + JavascriptString + DataOidString + DataDocTypeString + DataModeString + " id=" + HtmlID + ">" + Caption + "</a>";
            }
        }
        public String Link
        {
            get
            {
                if (String.IsNullOrWhiteSpace(Action) == false)
                {
                    return Url.Action(Action, Controller, LinkParameters);
                }
                return DirectLink;
            }
        }

        public void AddChildren(MenuNode node)
        {
            if (String.IsNullOrWhiteSpace(node.Controller) == false && forbidden.Contains(node.Controller))
            {
                return;
            }
            if (node.ShowOrder == -1)
            {
                node.ShowOrder = Children.Count == 0 ? 0 : Children.Max(g => g.ShowOrder) + 1;
            }
            node.Parent = this;
            Children.Add(node);
        }

        public void SetHtmlClass(string newClass)
        {
            this.HtmlClass = newClass;
            this.Children.ForEach(x => x.SetHtmlClass(newClass));
        }

        public void RemoveHtmlClass(string classToRemove)
        {
            if (string.IsNullOrWhiteSpace(this.HtmlClass) == false)
            {
                this.HtmlClass = this.HtmlClass.Replace(classToRemove, "");
            }
            this.Children.ForEach(x => x.RemoveHtmlClass(classToRemove));
        }

        public MenuNode FindActiveNode(System.Uri url)
        {
            string link = string.IsNullOrEmpty(this.Link) ? string.Empty : this.Link.ToLower();
            string pathAndQuery = string.IsNullOrEmpty(url.PathAndQuery) ? string.Empty : url.PathAndQuery.ToLower();
            if (link == pathAndQuery)
            {
                return this;
            }
            if (Children.Count > 0)
            {
                return Children.Select(node => node.FindActiveNode(url)).Where(node => node != null).FirstOrDefault();
            }
            return null;
        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ITS.Retail.WebClient.Helpers
{
    public static class XmlHelper
    {
        public static string GetNodeValue(XmlDocument xmlDocument,string nodeName)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode(nodeName);
            if(xmlNode==null)
            {
                return "";
            }
            return xmlNode.InnerText;
        }

        public static string GetNodeValue(XmlNode xmlNode, string nodeName)
        {
            XmlNode innerXmlNode = xmlNode.SelectSingleNode(nodeName);
            if (innerXmlNode == null)
            {
                return "";
            }
            return innerXmlNode.InnerText;
        }
    }
}

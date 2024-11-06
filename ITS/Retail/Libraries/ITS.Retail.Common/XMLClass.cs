using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ITS.Retail.Common
{
    public class NewXmlParser
    {
        private XmlDocument xmldoc;
        private string cook; //to string to opoio ginetai oti property thelo
        private Boolean valid; //elegxei ston constructor an to xml einai valid
        public List<AttributesStruct> ListOfAttributes = new List<AttributesStruct>();
        public string MyXml;

        private AttributesStruct AttributesStructS;

        public struct AttributesStruct //structure sto opoio sozo ta attributes tou xml
        {
            public string id;
            public string title;
            public string type;
            public string rel;
            public string qty;
            public string eventid;
            public string user;
            public string errorcode;
            public string list_value;
            public string descr;
            public string code;
            public string errordescr;
            public string userid;
            public string findoc;
            public string action;

            public string cust_id;
            public string br_id;
            public string item_id;

            public string name;
            public string addr;
            public string doy;
            public string city;
            public string zip;
            public string phone;
            public string afm;
            public string shipment;
            public string payment;

            public string price;
            public string group;
            public string category;
            public string mtrunit;
            public string gridrow;
            public string discount1;
            public string discount2;
            public string discount3;
            public string linevalue;

            public string fromdate;
            public string todate;
            public string docdate;
            public string branch;

            public string cfncusdoc;
            public string docnumber;
            public string expiredate;
            public string bank;
            public string accountmoney;
            public string fpa;
            public string percent;

            public string aa;
            public string debit;
            public string credit;
            public string remain;
            public string date;

            public string ip;
            public string port;
            public string district;
            public string series;
            public string isedit;

            public string turnover;
            public string qtyremain;
            public string id1;
            public string id2;

            public string warning;
            public string stigma;
            public string header;
            public string footer;
            public string show;
            public string length;
            public string dec;
            public string align;
            public string sscc;
            public string table;
            public string field;
            public string phone01;
            public string address;
            public string code1;
            public string mtrunit1;
            public string reserved;
            public string ordered;
            public string mtrunit2;
            public string qty1;
            public string qty2;
            public string disc1prc;
            public string disc2prc;
            public string disc3prc;
            public string netlineval;
            public string purqty;
            public string salqty;
            public string iteqty;
            public string mtrl;
            public string pricer;
            public string pricew;
            public string defaultdoc;
            public string whouse;
            public string printdoc;
            public string secqty;
            public string priceuse;
            public string paylock;
            public string sumamntlock;
            public string branchlock;
            public string kind;
            public string fincode;
            public string email;
            public string phone02;
            public string fax;
            public string webpage;
            public string irsdata;
            public string jobtypetrd;
            public string phone1;
            public string phone2;
            public string vat;
            public string code2;
            public string whousesec;
            public string gascustype;
            public string oldeh;
            public string gastype;
            public string sntype;
            public string islabel;
            public string Xreosi;
            public string Pistosi;
            public string QtyIn;
            public string QtyOut;
            public string ValueIn;
            public string ValueOut;
            public string seriesnum;
            public string handmd;
            public string cseries;

            public string cdim1;
            public string cdim2;
            public string cdim3;
            public string cdimnuse1;
            public string cdimnuse2;
            public string cdimnuse3;
            public string cdimn01;
            public string cdimn02;
            public string cdimn03;

            public string mtrunit3;
            public string mtrunit4;
            public string lots;
        }

        public NewXmlParser(string xml) //Constructor
        {
            xmldoc = new XmlDocument();
            valid = true;
            try
            {
                xml = xml.Replace((char)5, (char)13);
                MyXml = xml;
                xmldoc.LoadXml(xml);
                xmldoc.InnerXml = xmldoc.InnerXml.Replace("_^^@@", " ");
                xmldoc.InnerXml = xmldoc.InnerXml.Replace("^^@@_", " ");
            }
            catch//(Exception e)
            {
                xmldoc = null;
                valid = false;
            }

        }

        ~NewXmlParser()
        {
            xmldoc = null;
            ListOfAttributes = null;
        }

        public Boolean Fvalid //ena property gia oles tis periptoseis
        {
            get
            {
                return valid;
            }
        }


        private void Add2ListOfAttributes(int index, string node_name, string node_value)
        {
            AttributesStructS = ListOfAttributes[index];

            switch (node_name)
            {
                case "id":
                    AttributesStructS.id = node_value;
                    break;
                case "qty":
                    AttributesStructS.qty = node_value;
                    break;
                case "rel":
                    AttributesStructS.rel = node_value;
                    break;
                case "title":
                    AttributesStructS.title = node_value;
                    break;
                case "type":
                    AttributesStructS.type = node_value;
                    break;
                case "event":
                    AttributesStructS.eventid = node_value;
                    break;
                case "user":
                    AttributesStructS.user = node_value;
                    break;
                case "errorcode":
                    AttributesStructS.errorcode = node_value;
                    break;
                case "list_value":
                    AttributesStructS.list_value = node_value;
                    break;
                case "descr":
                    AttributesStructS.descr = node_value;
                    break;
                case "code":
                    AttributesStructS.code = node_value;
                    break;
                case "errordescr":
                    AttributesStructS.errordescr = node_value;
                    break;
                case "userid":
                    AttributesStructS.userid = node_value;
                    break;
                case "findoc":
                    AttributesStructS.findoc = node_value;
                    break;
                case "action":
                    AttributesStructS.action = node_value;
                    break;


                case "cust_id":
                    AttributesStructS.cust_id = node_value;
                    break;
                case "br_id":
                    AttributesStructS.br_id = node_value;
                    break;
                case "item_id":
                    AttributesStructS.item_id = node_value;
                    break;

                case "name":
                    AttributesStructS.name = node_value;
                    break;
                case "addr":
                    AttributesStructS.addr = node_value;
                    break;
                case "doy":
                    AttributesStructS.doy = node_value;
                    break;
                case "city":
                    AttributesStructS.city = node_value;
                    break;
                case "zip":
                    AttributesStructS.zip = node_value;
                    break;
                case "phone":
                    AttributesStructS.phone = node_value;
                    break;
                case "afm":
                    AttributesStructS.afm = node_value;
                    break;
                case "shipment":
                    AttributesStructS.shipment = node_value;
                    break;
                case "payment":
                    AttributesStructS.payment = node_value;
                    break;

                case "price":
                    AttributesStructS.price = node_value;
                    break;
                case "group":
                    AttributesStructS.group = node_value;
                    break;
                case "category":
                    AttributesStructS.category = node_value;
                    break;
                case "mtrunit":
                    AttributesStructS.mtrunit = node_value;
                    break;
                case "gridrow":
                    AttributesStructS.gridrow = node_value;
                    break;
                case "discount1":
                    AttributesStructS.discount1 = node_value;
                    break;
                case "discount2":
                    AttributesStructS.discount2 = node_value;
                    break;
                case "discount3":
                    AttributesStructS.discount3 = node_value;
                    break;
                case "linevalue":
                    AttributesStructS.linevalue = node_value;
                    break;
                case "fromdate":
                    AttributesStructS.fromdate = node_value;
                    break;
                case "todate":
                    AttributesStructS.todate = node_value;
                    break;
                case "docdate":
                    AttributesStructS.docdate = node_value;
                    break;
                case "branch":
                    AttributesStructS.branch = node_value;
                    break;
                case "cfncusdoc":
                    AttributesStructS.cfncusdoc = node_value;
                    break;
                case "docnumber":
                    AttributesStructS.docnumber = node_value;
                    break;
                case "expiredate":
                    AttributesStructS.expiredate = node_value;
                    break;
                case "bank":
                    AttributesStructS.bank = node_value;
                    break;
                case "accountmoney":
                    AttributesStructS.accountmoney = node_value;
                    break;
                case "fpa":
                    AttributesStructS.fpa = node_value;
                    break;
                case "percent":
                    AttributesStructS.percent = node_value;
                    break;
                case "aa":
                    AttributesStructS.aa = node_value;
                    break;
                case "debit":
                    AttributesStructS.debit = node_value;
                    break;
                case "credit":
                    AttributesStructS.credit = node_value;
                    break;
                case "remain":
                    AttributesStructS.remain = node_value;
                    break;
                case "date":
                    AttributesStructS.date = node_value;
                    break;
                case "ip":
                    AttributesStructS.ip = node_value;
                    break;
                case "port":
                    AttributesStructS.port = node_value;
                    break;
                case "district":
                    AttributesStructS.district = node_value;
                    break;
                case "series":
                    AttributesStructS.series = node_value;
                    break;
                case "isedit":
                    AttributesStructS.isedit = node_value;
                    break;

                case "turnover":
                    AttributesStructS.turnover = node_value;
                    break;
                case "qtyremain":
                    AttributesStructS.qtyremain = node_value;
                    break;
                case "id1":
                    AttributesStructS.id1 = node_value;
                    break;
                case "id2":
                    AttributesStructS.id2 = node_value;
                    break;
                case "warning":
                    AttributesStructS.warning = node_value;
                    break;
                case "stigma":
                    AttributesStructS.stigma = node_value;
                    break;
                case "header":
                    AttributesStructS.header = node_value;
                    break;
                case "footer":
                    AttributesStructS.footer = node_value;
                    break;
                case "show":
                    AttributesStructS.show = node_value;
                    break;
                case "length":
                    AttributesStructS.length = node_value;
                    break;
                case "dec":
                    AttributesStructS.dec = node_value;
                    break;
                case "align":
                    AttributesStructS.align = node_value;
                    break;
                case "sscc":
                    AttributesStructS.sscc = node_value;
                    break;
                case "table":
                    AttributesStructS.table = node_value;
                    break;
                case "field":
                    AttributesStructS.field = node_value;
                    break;
                case "phone01":
                    AttributesStructS.phone01 = node_value;
                    break;
                case "address":
                    AttributesStructS.address = node_value;
                    break;
                case "code1":
                    AttributesStructS.code1 = node_value;
                    break;
                case "mtrunit1":
                    AttributesStructS.mtrunit1 = node_value;
                    break;
                case "reserved":
                    AttributesStructS.reserved = node_value;
                    break;
                case "ordered":
                    AttributesStructS.ordered = node_value;
                    break;
                case "mtrunit2":
                    AttributesStructS.mtrunit2 = node_value;
                    break;
                case "qty1":
                    AttributesStructS.qty1 = node_value;
                    break;
                case "qty2":
                    AttributesStructS.qty2 = node_value;
                    break;
                case "disc1prc":
                    AttributesStructS.disc1prc = node_value;
                    break;
                case "disc2prc":
                    AttributesStructS.disc2prc = node_value;
                    break;
                case "disc3prc":
                    AttributesStructS.disc3prc = node_value;
                    break;
                case "netlineval":
                    AttributesStructS.netlineval = node_value;
                    break;
                case "purqty":
                    AttributesStructS.purqty = node_value;
                    break;
                case "salqty":
                    AttributesStructS.salqty = node_value;
                    break;
                case "iteqty":
                    AttributesStructS.iteqty = node_value;
                    break;
                case "mtrl":
                    AttributesStructS.mtrl = node_value;
                    break;
                case "pricer":
                    AttributesStructS.pricer = node_value;
                    break;
                case "pricew":
                    AttributesStructS.pricew = node_value;
                    break;
                case "defaultdoc":
                    AttributesStructS.defaultdoc = node_value;
                    break;
                case "whouse":
                    AttributesStructS.whouse = node_value;
                    break;
                case "printdoc":
                    AttributesStructS.printdoc = node_value;
                    break;
                case "secqty":
                    AttributesStructS.secqty = node_value;
                    break;
                case "priceuse":
                    AttributesStructS.priceuse = node_value;
                    break;
                case "paylock":
                    AttributesStructS.paylock = node_value;
                    break;
                case "sumamntlock":
                    AttributesStructS.sumamntlock = node_value;
                    break;
                case "branchlock":
                    AttributesStructS.branchlock = node_value;
                    break;
                case "kind":
                    AttributesStructS.kind = node_value;
                    break;
                case "fincode":
                    AttributesStructS.fincode = node_value;
                    break;
                case "email":
                    AttributesStructS.email = node_value;
                    break;
                case "phone02":
                    AttributesStructS.phone02 = node_value;
                    break;
                case "fax":
                    AttributesStructS.fax = node_value;
                    break;
                case "webpage":
                    AttributesStructS.webpage = node_value;
                    break;
                case "irsdata":
                    AttributesStructS.irsdata = node_value;
                    break;
                case "jobtypetrd":
                    AttributesStructS.jobtypetrd = node_value;
                    break;
                case "phone1":
                    AttributesStructS.phone1 = node_value;
                    break;
                case "phone2":
                    AttributesStructS.phone2 = node_value;
                    break;
                case "vat":
                    AttributesStructS.vat = node_value;
                    break;
                case "code2":
                    AttributesStructS.code2 = node_value;
                    break;
                case "whousesec":
                    AttributesStructS.whousesec = node_value;
                    break;
                case "gascustype":
                    AttributesStructS.gascustype = node_value;
                    break;
                case "sntype":
                    AttributesStructS.sntype = node_value;
                    break;
                case "islabel":
                    AttributesStructS.islabel = node_value;
                    break;
                case "oldeh":
                    AttributesStructS.oldeh = node_value;
                    break;
                case "gastype":
                    AttributesStructS.gastype = node_value;
                    break;
                case "cdim1":
                    AttributesStructS.cdim1 = node_value;
                    break;
                case "cdim2":
                    AttributesStructS.cdim2 = node_value;
                    break;
                case "cdim3":
                    AttributesStructS.cdim3 = node_value;
                    break;
                case "cdimnuse1":
                    AttributesStructS.cdimnuse1 = node_value;
                    break;
                case "cdimnuse2":
                    AttributesStructS.cdimnuse2 = node_value;
                    break;
                case "cdimnuse3":
                    AttributesStructS.cdimnuse3 = node_value;
                    break;
                case "cdimn01":
                    AttributesStructS.cdimn01 = node_value;
                    break;
                case "cdimn02":
                    AttributesStructS.cdimn02 = node_value;
                    break;
                case "cdimn03":
                    AttributesStructS.cdimn03 = node_value;
                    break;
                case "Xreosi":
                    AttributesStructS.Xreosi = node_value;
                    break;
                case "Pistosi":
                    AttributesStructS.Pistosi = node_value;
                    break;
                case "QtyIn":
                    AttributesStructS.QtyIn = node_value;
                    break;
                case "QtyOut":
                    AttributesStructS.QtyOut = node_value;
                    break;
                case "ValueIn":
                    AttributesStructS.ValueIn = node_value;
                    break;
                case "ValueOut":
                    AttributesStructS.ValueOut = node_value;
                    break;
                case "seriesnum":
                    AttributesStructS.seriesnum = node_value;
                    break;
                case "handmd":
                    AttributesStructS.handmd = node_value;
                    break;
                case "cseries":
                    AttributesStructS.cseries = node_value;
                    break;
                case "mtrunit3":
                    AttributesStructS.mtrunit3 = node_value;
                    break;
                case "mtrunit4":
                    AttributesStructS.mtrunit4 = node_value;
                    break;
                case "lots":
                    AttributesStructS.lots = node_value;
                    break;
            }

            ListOfAttributes[index] = AttributesStructS;

        }

        public Boolean NodeExists(string node)
        {
            XmlNodeList xmlnode = xmldoc.GetElementsByTagName("request");

            for (int i = 0; i < xmlnode.Count; i++)
                for (int k = 0; k < xmlnode[i].ChildNodes.Count; k++)
                    if (xmlnode[i].ChildNodes[k].Name == node)//to onoma tou node einai idio
                        return true;
            return false;
        }

        public string GetProperty(string node, string value)//dinei timi sto cook kai epistrefei tin timi tou
        {
            cook = "";
            GetInnerText(node, value);
            return cook;
        }

        public string GetProperty2(string node, string value)//dinei timi sto cook kai epistrefei tin timi tou
        {
            cook = "";
            XmlNodeList xmlnode = xmldoc.GetElementsByTagName(node);
            if (xmlnode.Count > 0)
            {
                GetInnerText(xmlnode[0], value);
            }
            return cook;
        }

        public string GetProperty(XmlNode node, string value)//dinei timi sto cook kai epistrefei tin timi tou
        {
            cook = "";
            GetInnerText(node, value);
            return cook;
        }

        private void GetInnerText(XmlNode xmlnode, string value) //diavazei tin timi tou innertext
        {
            //ListOfAttributes.Clear();

            for (int i = 0; i < xmlnode.ChildNodes.Count; i++)
                if (xmlnode.ChildNodes[i].Name == value)
                {
                    cook = xmlnode.ChildNodes[i].InnerText;//dino timi sto property

                    return;
                }
        }

        private void GetInnerText(string node, string value) //diavazei tin timi tou innertext
        {
            ListOfAttributes.Clear();
            //InializeArray(calclength(node));//arxikopoio ton pinaka
            XmlNodeList xmlnode = xmldoc.GetElementsByTagName(node);

            for (int i = 0; i < xmlnode.Count; i++)
                for (int k = 0; k < xmlnode[i].ChildNodes.Count; k++)
                    for (int p = 0; p < xmlnode[i].ChildNodes[k].Attributes.Count; p++)
                        if (xmlnode[i].ChildNodes[k].Attributes[p].Value == value)
                        {
                            cook = xmlnode[i].ChildNodes[k].InnerText;//dino timi sto property
                            //for (int m = 0; m < xmlnode[i].ChildNodes[k].Attributes.Count; m++)
                            //{//gemizo ton pinaka
                            //    //FillArray(0, xmlnode[i].ChildNodes[k].Attributes[m].Name, xmlnode[i].ChildNodes[k].Attributes[m].Value);
                            //}
                            return;
                        }
        }

        public void GetAttributesOfRequest()//gemizei ton pinaka me ta attributes mias mono grammis enos node e.g. request
        {
            ListOfAttributes.Clear();
            AttributesStructS = new AttributesStruct();
            ListOfAttributes.Add(AttributesStructS);
            XmlNodeList xmlnode = xmldoc.GetElementsByTagName("request");

            for (int p = 0; p < xmlnode[0].Attributes.Count; p++)
                Add2ListOfAttributes(0, xmlnode[0].Attributes[p].Name, xmlnode[0].Attributes[p].Value);
            return;
        }

        public List<AttributesStruct> GetList(string node)//gemizei ton Structarray me ta stoixeia mias listas
        {
            XmlNodeList xmlnode = xmldoc.GetElementsByTagName("request");//sinithos "request"
            ListOfAttributes.Clear();

            for (int i = 0; i < xmlnode.Count; i++)
                for (int k = 0; k < xmlnode[i].ChildNodes.Count; k++)
                {
                    if (xmlnode[i].ChildNodes[k].Name == node)//to onoma tou node einai idio
                    {
                        //AttributesStructS = new AttributesStruct();
                        for (int p = 0; p < xmlnode[i].ChildNodes[k].ChildNodes.Count; p++)//gia kathe "item" tis listas
                        {
                            AttributesStructS = new AttributesStruct();
                            ListOfAttributes.Add(AttributesStructS);
                            for (int r = 0; r < xmlnode[i].ChildNodes[k].ChildNodes[p].Attributes.Count; r++)//gia kathe attributes tou "item" tis listas
                            {
                                Add2ListOfAttributes(p, xmlnode[i].ChildNodes[k].ChildNodes[p].Attributes[r].Name, xmlnode[i].ChildNodes[k].ChildNodes[p].Attributes[r].Value);
                            }
                            Add2ListOfAttributes(p, "list_value", xmlnode[i].ChildNodes[k].ChildNodes[p].InnerText);//edo kanei overwrite r fores tin timi tou innertext
                        }
                    }
                }
            return ListOfAttributes;
        }

        public List<AttributesStruct> GetList(string node, string id)//gemizei ton Structarray me ta stoixeia mias listas
        {
            XmlNodeList xmlnode = xmldoc.GetElementsByTagName("request");//sinithos "request"
            ListOfAttributes.Clear();

            for (int i = 0; i < xmlnode.Count; i++)
                for (int k = 0; k < xmlnode[i].ChildNodes.Count; k++)
                {
                    if (xmlnode[i].ChildNodes[k].Name == node)//to onoma tou node einai idio
                        if (xmlnode[i].ChildNodes[k].Attributes.Count > 0)//to node exei toulaxiston 1 attribute
                            if (xmlnode[i].ChildNodes[k].Attributes[0].Value == id)//exei tin idia timi attribute
                            {
                                //AttributesStructS = new AttributesStruct();
                                for (int p = 0; p < xmlnode[i].ChildNodes[k].ChildNodes.Count; p++)//gia kathe "item" tis listas
                                {
                                    AttributesStructS = new AttributesStruct();
                                    ListOfAttributes.Add(AttributesStructS);
                                    for (int r = 0; r < xmlnode[i].ChildNodes[k].ChildNodes[p].Attributes.Count; r++)//gia kathe attributes tou "item" tis listas
                                    {
                                        Add2ListOfAttributes(p, xmlnode[i].ChildNodes[k].ChildNodes[p].Attributes[r].Name, xmlnode[i].ChildNodes[k].ChildNodes[p].Attributes[r].Value);
                                    }
                                    Add2ListOfAttributes(p, "list_value", xmlnode[i].ChildNodes[k].ChildNodes[p].InnerText);//edo kanei overwrite r fores tin timi tou innertext
                                }
                            }
                }
            return ListOfAttributes;
        }

        public List<AttributesStruct> GetList(string node, string[] id)//gemizei ton Structarray me ta stoixeia mias listas
        {
            XmlNodeList xmlnode = xmldoc.GetElementsByTagName("request");//sinithos "request"
            ListOfAttributes.Clear();

            for (int i = 0; i < xmlnode.Count; i++)
                for (int k = 0; k < xmlnode[i].ChildNodes.Count; k++)
                {
                    if (xmlnode[i].ChildNodes[k].Name == node)//to onoma tou node einai idio
                    {
                        Boolean validation = true;
                        if (xmlnode[i].ChildNodes[k].Attributes.Count == id.Length)//to node exei idio arithmo attributes
                            for (int p = 0; p < id.Length; p++)
                                if (xmlnode[i].ChildNodes[k].Attributes[p].Value != id[p])//oles oi times ton attributes simfonoun
                                    validation = false;
                        if (validation)//ola kala
                        {
                            //AttributesStructS = new AttributesStruct();

                            for (int p = 0; p < xmlnode[i].ChildNodes[k].ChildNodes.Count; p++)//gia kathe "item" tis listas
                            {
                                AttributesStructS = new AttributesStruct();
                                ListOfAttributes.Add(AttributesStructS);
                                for (int r = 0; r < xmlnode[i].ChildNodes[k].ChildNodes[p].Attributes.Count; r++)//gia kathe attributes tou "item" tis listas
                                {
                                    Add2ListOfAttributes(p, xmlnode[i].ChildNodes[k].ChildNodes[p].Attributes[r].Name, xmlnode[i].ChildNodes[k].ChildNodes[p].Attributes[r].Value);
                                }
                                Add2ListOfAttributes(p, "list_value", xmlnode[i].ChildNodes[k].ChildNodes[p].InnerText);//edo kanei overwrite r fores tin timi tou innertext
                            }
                        }

                    }
                }
            return ListOfAttributes;
        }


        public XmlNodeList GetNodeList(string node)
        {
            XmlNodeList xmlnode = xmldoc.GetElementsByTagName(node);
            return xmlnode;
        }

    }

    public class NewXmlCreator
    {

        private StringBuilder MyXmlBuilder = new StringBuilder();
        public string MyXml = "";
        public string FormatedMyXml = "";

        public NewXmlCreator(string id, string eventid, string user, string userid, string errorcode, string errordescr)
        {
            MyXmlBuilder.Append("<?xml version=\"1.0\"?>");
            MyXmlBuilder.Append("<request id=\"");
            MyXmlBuilder.Append(id);
            MyXmlBuilder.Append("\" event=\"");
            MyXmlBuilder.Append(eventid);
            MyXmlBuilder.Append("\" user=\"");
            MyXmlBuilder.Append(user);
            MyXmlBuilder.Append("\" userid=\"");
            MyXmlBuilder.Append(userid);
            MyXmlBuilder.Append("\" errorcode=\"");
            MyXmlBuilder.Append(errorcode);
            MyXmlBuilder.Append("\" errordescr=\"");
            MyXmlBuilder.Append(errordescr);
            MyXmlBuilder.Append("\">");
        }

        public NewXmlCreator(string errorcode, string errordescr)
        {
            MyXmlBuilder.Append("<?xml version=\"1.0\"?>");
            MyXmlBuilder.Append("<request ");
            MyXmlBuilder.Append(" errorcode=\"");
            MyXmlBuilder.Append(errorcode);
            MyXmlBuilder.Append("\" errordescr=\"");
            MyXmlBuilder.Append(errordescr);
            MyXmlBuilder.Append("\">");
        }

        public NewXmlCreator()
        {
            MyXmlBuilder.Append("<?xml version=\"1.0\"?><DocumentElement>");
        }

        public void XmlOfflineClose()
        {
            MyXmlBuilder.Append("</DocumentElement>");
            MyXml = MyXmlBuilder.ToString();
        }

        public void CreateNodes(string section, string[] data)
        {
            MyXmlBuilder.Append("<");
            MyXmlBuilder.Append(section);
            MyXmlBuilder.Append(">");

            for (int i = 0; i < data.Length; i++)//gia kathe grammi tou pinaka
            {
                string[] tmpline = data[i].Split('|');
                StringBuilder linebuilder = new StringBuilder();
                linebuilder.Append("<");
                linebuilder.Append(tmpline[0]);
                linebuilder.Append(">");
                linebuilder.Append(ReplaceSpecialChars(tmpline[1]));
                linebuilder.Append("</");
                linebuilder.Append(tmpline[0]);
                linebuilder.Append(">");

                MyXmlBuilder.Append(linebuilder.ToString());
            }

            MyXmlBuilder.Append("</");
            MyXmlBuilder.Append(section);
            MyXmlBuilder.Append(">");
        }



        public void Xmlclose()
        {
            MyXmlBuilder.Append("</request>");
            MyXml = MyXmlBuilder.ToString();
            FormatedMyXml = MyXmlBuilder.ToString().Replace("><", ">\r\n<");
        }


        public void CreateNodes(string section, string element, string[] data)//new
        {
            MyXmlBuilder.Append("<");
            MyXmlBuilder.Append(section);
            MyXmlBuilder.Append(">");
            for (int i = 0; i < data.Length; i++)//gia kathe grammi tou pinaka
            {
                StringBuilder linebuilder = new StringBuilder();
                linebuilder.Append("<");
                linebuilder.Append(element);
                linebuilder.Append(" ");

                string[] dataarray = data[i].Split('|');

                for (int k = 1; k < dataarray.Length; k++)//spase to string ana "|"
                {
                    string[] tmp = dataarray[k].Split('=');
                    linebuilder.Append(tmp[0]);
                    linebuilder.Append("=");
                    linebuilder.Append(QuotedStr(ReplaceSpecialChars(tmp[1])));
                    if (k != dataarray.Length - 1)
                        linebuilder.Append(" ");
                }
                linebuilder.Append(">");
                linebuilder.Append(ReplaceSpecialChars(dataarray[0].Split('=')[1]));
                linebuilder.Append("</");
                linebuilder.Append(element);
                linebuilder.Append(">");
                MyXmlBuilder.Append(linebuilder.ToString());
            }
            MyXmlBuilder.Append("</");
            MyXmlBuilder.Append(section);
            MyXmlBuilder.Append(">");
        }

        public void CreateNodes(string section, string element, string attr_name, string atrr_value, string[] data)//new
        {
            MyXmlBuilder.Append("<");
            MyXmlBuilder.Append(section);
            MyXmlBuilder.Append(" ");
            MyXmlBuilder.Append(attr_name);
            MyXmlBuilder.Append("=");
            MyXmlBuilder.Append(QuotedStr(atrr_value));
            MyXmlBuilder.Append(">");

            for (int i = 0; i < data.Length; i++)//gia kathe grammi tou pinaka
            {
                StringBuilder linebuilder = new StringBuilder();
                linebuilder.Append("<");
                linebuilder.Append(element);
                linebuilder.Append(" ");

                string[] dataarray = data[i].Split('|');

                for (int k = 1; k < dataarray.Length; k++)//spase to string ana "|"
                {
                    string[] tmp = dataarray[k].Split('=');
                    linebuilder.Append(tmp[0]);
                    linebuilder.Append("=");
                    linebuilder.Append(QuotedStr(ReplaceSpecialChars(tmp[1])));
                    if (k != dataarray.Length - 1)
                        linebuilder.Append(" ");
                }
                linebuilder.Append(">");
                linebuilder.Append(ReplaceSpecialChars(dataarray[0].Split('=')[1]));
                linebuilder.Append("</");
                linebuilder.Append(element);
                linebuilder.Append(">");
                MyXmlBuilder.Append(linebuilder.ToString());
            }
            MyXmlBuilder.Append("</");
            MyXmlBuilder.Append(section);
            MyXmlBuilder.Append(">");
        }

        public void CreateNodes(string section, string element, string[] attr_name, string[] atrr_value, string[] data)//new
        {
            StringBuilder attributes = new StringBuilder();

            for (int i = 0; i < attr_name.Length; i++)
            {
                attributes.Append(attr_name[i]);
                attributes.Append("=");
                attributes.Append(QuotedStr(atrr_value[i]));
                if (i != attr_name.Length - 1)
                    attributes.Append(" ");
            }

            MyXmlBuilder.Append("<");
            MyXmlBuilder.Append(section);
            MyXmlBuilder.Append(" ");
            MyXmlBuilder.Append(attributes.ToString());
            MyXmlBuilder.Append(">");

            for (int i = 0; i < data.Length; i++)//gia kathe grammi tou pinaka
            {
                StringBuilder linebuilder = new StringBuilder();
                linebuilder.Append("<");
                linebuilder.Append(element);
                linebuilder.Append(" ");

                string[] dataarray = data[i].Split('|');

                for (int k = 1; k < dataarray.Length; k++)//spase to string ana "|"
                {
                    string[] tmp = dataarray[k].Split('=');
                    linebuilder.Append(tmp[0]);
                    linebuilder.Append("=");
                    linebuilder.Append(QuotedStr(ReplaceSpecialChars(tmp[1])));
                    if (k != dataarray.Length - 1)
                        linebuilder.Append(" ");
                }
                linebuilder.Append(">");
                linebuilder.Append(ReplaceSpecialChars(dataarray[0].Split('=')[1]));
                linebuilder.Append("</");
                linebuilder.Append(element);
                linebuilder.Append(">");
                MyXmlBuilder.Append(linebuilder.ToString());
            }
            MyXmlBuilder.Append("</");
            MyXmlBuilder.Append(section);
            MyXmlBuilder.Append(">");
        }



        private string QuotedStr(string Value)
        {
            StringBuilder tmp = new StringBuilder("\"");
            tmp.Append(Value);
            tmp.Append("\"");
            return tmp.ToString();
            //return "\"" + Value + "\"";
        }

        private string ReplaceSpecialChars(string Value)
        {
            Value = Value.Replace("&", "&amp;");
            Value = Value.Replace((char)13, (char)5);
            Value = Value.Replace("<", "_^^@@");
            Value = Value.Replace(">", "^^@@_");
            Value = Value.Replace("\"", "&quot;");
            return Value;
        }


    }

    public class XmlCreator
    {
        private StringBuilder MyXmlBuilder = new StringBuilder();
        public string MyXml = "";
        public string FormatedMyXml = "";
        private string _mainelement;

        public XmlCreator(string mainelement)
        {
            _mainelement = mainelement;
            MyXmlBuilder.Append("<?xml version=\"1.0\"?><" + mainelement + ">");
        }

        public void Xmlclose()
        {
            MyXmlBuilder.Append("</" + _mainelement + ">");
            MyXml = MyXmlBuilder.ToString();
            FormatedMyXml = MyXmlBuilder.ToString().Replace("><", ">\r\n<");
        }

        private string QuotedStr(string Value)
        {
            StringBuilder tmp = new StringBuilder("\"");
            tmp.Append(Value);
            tmp.Append("\"");
            return tmp.ToString();
            //return "\"" + Value + "\"";
        }

        private string ReplaceSpecialChars(string Value)
        {
            Value = Value.Replace("&", "&amp;");
            Value = Value.Replace((char)13, (char)5);
            Value = Value.Replace("<", "_^^@@");
            Value = Value.Replace(">", "^^@@_");
            Value = Value.Replace("\"", "&quot;");
            return Value;
        }

        public void CreateNodes(string section, string[] data)
        {
            MyXmlBuilder.Append("<");
            MyXmlBuilder.Append(section);
            MyXmlBuilder.Append(">");

            for (int i = 0; i < data.Length; i++)//gia kathe grammi tou pinaka
            {
                string[] tmpline = data[i].Split('|');
                StringBuilder linebuilder = new StringBuilder();
                linebuilder.Append("<");
                linebuilder.Append(tmpline[0]);
                linebuilder.Append(">");
                linebuilder.Append(ReplaceSpecialChars(tmpline[1]));
                linebuilder.Append("</");
                linebuilder.Append(tmpline[0]);
                linebuilder.Append(">");

                MyXmlBuilder.Append(linebuilder.ToString());
            }

            MyXmlBuilder.Append("</");
            MyXmlBuilder.Append(section);
            MyXmlBuilder.Append(">");
        }
    }
}

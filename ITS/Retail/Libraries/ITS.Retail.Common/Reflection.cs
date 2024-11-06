using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;

namespace ITS.Retail.Common
{
    public class ReflectionClass
    {

        private string _AsseblyName;
        private Type[] _types;

        public ReflectionClass(string AsseblyName)
        {
            _AsseblyName = AsseblyName;
            Assembly assem = Assembly.Load(_AsseblyName);
            _types = assem.GetTypes();
        }

        public static FieldInfo[] GetAllFields(Type t, List<FieldInfo> mylist = null, Boolean first = true)
        {
            try
            {
                if (t == null)
                    return null;

                BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
                if (first)
                    mylist = t.GetFields(flags).ToList();

                if (t.BaseType.Name != "XPBaseObject" && t.BaseType.Name != "XPObject")
                {
                    FieldInfo[] tmp = t.BaseType.GetFields(flags);

                    for (int i = 0; i < tmp.Length; i++)
                    {
                        mylist.Add(tmp[i]);
                    }

                    GetAllFields(t.BaseType, mylist, false);
                }

                return mylist.ToArray(); ;
            }
            catch 
            {
                return null;
            }
        }

        public string GetXml()
        {
            try
            {
                List<string> data = new List<string>();
                NewXmlCreator xmldata = new NewXmlCreator("","");
                for (int i = 0; i < _types.Length; i++)
                {
                    data.Add("text=" + _types[i].Name);// + "|id=Name");
                }
                xmldata.CreateNodes("Tables", "li", data.ToArray());

                for (int i = 0; i < _types.Length; i++)
                {
                    Type t = Type.GetType(_types[i].FullName + ", " + _AsseblyName);

                    data = new List<string>();
                    FieldInfo[] ArrayOfFields = GetAllFields(t);
                    for (int k = 0; k < ArrayOfFields.Length; k++)
                    {
                        data.Add("text=" + ArrayOfFields[k].Name);
                    }
                    xmldata.CreateNodes("Fields", "li", "TableName", t.Name, data.ToArray());
                }

                xmldata.Xmlclose();
                return xmldata.MyXml;
            }
            catch 
            {
                return "";   
            }
        }

        public DataSet GetDataSet()
        {
            try
            {
                DataSet ds = new DataSet();
                string xml = GetXml();
                NewXmlParser xmldata = new NewXmlParser(xml);

                //List<string> Tables = new List<string>();
                xmldata.GetList("Tables");

                DataTable MasterTable = new DataTable("Tables");
                MasterTable.Columns.Add("TableId", typeof(int));
                MasterTable.Columns.Add("TableName", typeof(string));

                for (int i = 0; i < xmldata.ListOfAttributes.Count; i++)
                {
                    //Tables.Add(xmldata.ListOfAttributes[i].list_value);
                    MasterTable.Rows.Add(i,xmldata.ListOfAttributes[i].list_value);
                }

                DataTable FieldsTable = new DataTable("Fields");
                FieldsTable.Columns.Add("TableId", typeof(int));
                FieldsTable.Columns.Add("FieldName", typeof(string));

                for (int i = 0; i < MasterTable.Rows.Count; i++)
                {
                    //xmldata.GetList("Fields", MasterTable.Rows[i][0].ToString());
                    xmldata.GetList("Fields", MasterTable.Rows[i][1].ToString());

                    for (int k = 0; k < xmldata.ListOfAttributes.Count; k++)
                    {
                        FieldsTable.Rows.Add(MasterTable.Rows[i][0], xmldata.ListOfAttributes[k].list_value);
                    }
                }

                ds.Tables.Add(MasterTable);
                ds.Tables.Add(FieldsTable);

                DataColumn colParent = ds.Tables[0].Columns["TableId"];
                DataColumn colChild = ds.Tables[1].Columns["TableId"];
                DataRelation Drel = new DataRelation("FieldTable", colParent, colChild);
                ds.Relations.Add(Drel);
               
                return ds;
            }
            catch //(Exception ex)
            {
                return null;
            }
        }

        public DataTable GetTables()
        {
            try
            {
                
                string xml = GetXml();
                NewXmlParser xmldata = new NewXmlParser(xml);

                //List<string> Tables = new List<string>();
                xmldata.GetList("Tables");

                DataTable MasterTable = new DataTable("Tables");
                MasterTable.Columns.Add("TableId", typeof(int));
                MasterTable.Columns.Add("TableName", typeof(string));

                for (int i = 0; i < xmldata.ListOfAttributes.Count; i++)
                {
                    //Tables.Add(xmldata.ListOfAttributes[i].list_value);
                    MasterTable.Rows.Add(i, xmldata.ListOfAttributes[i].list_value);
                }

                return MasterTable;
            }
            catch //(Exception ex)
            {
                return null;
            }
        }

        public DataTable GetFields(string tablename)
        {
            try
            {
                string xml = GetXml();
                NewXmlParser xmldata = new NewXmlParser(xml);

                //List<string> Tables = new List<string>();
                xmldata.GetList("Tables");

                using (DataTable MasterTable = new DataTable("Tables"))
                {
                    MasterTable.Columns.Add("TableId", typeof(int));
                    MasterTable.Columns.Add("TableName", typeof(string));
                    for (int i = 0; i < xmldata.ListOfAttributes.Count; i++)
                    {
                        MasterTable.Rows.Add(i, xmldata.ListOfAttributes[i].list_value);
                    }

                    DataTable FieldsTable = new DataTable("Fields");
                    FieldsTable.Columns.Add("TableName", typeof(string));
                    FieldsTable.Columns.Add("FieldName", typeof(string));
                    for (int i = 0; i < MasterTable.Rows.Count; i++)
                    {
                        if (MasterTable.Rows[i][1].ToString() == tablename)
                        {
                            xmldata.GetList("Fields", MasterTable.Rows[i][1].ToString());
                            for (int k = 0; k < xmldata.ListOfAttributes.Count; k++)
                            {
                                FieldsTable.Rows.Add(MasterTable.Rows[i][0], xmldata.ListOfAttributes[k].list_value);
                            }
                        }
                    }
                    return FieldsTable;
                }
            }
            catch //(Exception ex)
            {
                return null;
            }
        }
    }
}

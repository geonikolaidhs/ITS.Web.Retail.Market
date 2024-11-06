using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Services;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ITS.Retail.WebClient
{
    
    public class JsonImageConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(Bitmap));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var m = new MemoryStream(Convert.FromBase64String((string)reader.Value));
            return (Bitmap)Bitmap.FromStream(m);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Bitmap bmp = (Bitmap)value;
            MemoryStream m = new MemoryStream();
            bmp.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);

            writer.WriteValue(Convert.ToBase64String(m.ToArray()));
        }
    }

    /// <summary>
    /// Summary description for ScannedDocumentHeaderService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ScannedDocumentHeaderService : System.Web.Services.WebService
    {
        public object DecompressLZMA(string value)
        {
            object retval = null;
            try
            {
                byte[] byt = Convert.FromBase64String(value);

                byte[] outByt = SevenZip.Compression.LZMA.SevenZipHelper.Decompress(byt);


                MemoryStream outMs = new MemoryStream(outByt);
                outMs.Seek(0, 0);
                BinaryFormatter bf = new BinaryFormatter();
                retval = (object)bf.Deserialize(outMs, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retval;
        }

        [WebMethod]
        public bool GetData(String data, out String messages)
        {
            messages = "";
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    int lines = 0;
                    JObject jsonObj;
                    //List<string> listData = JsonConvert.DeserializeObject<List<string>>(data);
                    //foreach (string obj in listData)
                    string obj = data;

                    lines++;
                    jsonObj = JObject.Parse(obj);
                    IEnumerable<JProperty> objValues = jsonObj.Properties();
                    string key = jsonObj.Property("Oid").Value.ToString();
                    ScannedDocumentHeader head = uow.GetObjectByKey<ScannedDocumentHeader>(Guid.Parse(key));
                    if (head == null)
                    {
                        head = new ScannedDocumentHeader(uow);
                    }
                    foreach (JProperty objValue in objValues)
                    {
                        try
                        {
                            Type objPropertyType = head.GetType().GetProperty(objValue.Name).PropertyType;
                            object fValue;
                            if (objPropertyType.IsSubclassOf(typeof(BaseObj)))
                            {

                                Guid id = Guid.Empty;
                                try
                                {
                                    id = new Guid(objValue.Value.ToString());
                                }
                                catch
                                {
                                    id = new Guid(objValue.Value.ToString().Substring(objValue.Value.ToString().IndexOf('(')).Trim('(').Trim(')'));
                                }

                                fValue = uow.GetObjectByKey(objPropertyType, id);

                            }
                            else
                            {
                                if (objPropertyType == typeof(Guid))
                                {
                                    fValue = new Guid(objValue.Value.ToString());
                                }
                                else if (objPropertyType == typeof(Image))
                                {

                                    fValue = JsonConvert.DeserializeObject<Image>(objValue.Value.ToString(), new JsonImageConverter());

                                }
                                else if (objPropertyType.IsEnum)
                                    fValue = Enum.Parse(objPropertyType, objValue.Value.ToString());
                                else
                                    fValue = Convert.ChangeType(objValue.Value.ToString(), objPropertyType);
                            }
                            head.GetType().GetProperty(objValue.Name).SetValue(head, fValue, null);
                        }
                        catch (Exception e)
                        {
                            messages += e.Message;
                            continue;
                        }

                    }
                    head.Save();
                    XpoHelper.CommitChanges(uow);                    
                }


                return true;
            }
            catch (Exception ex)
            {
                messages += Environment.NewLine + ex.Message;
                if (ex.InnerException != null)
                {
                    messages += Environment.NewLine + ex.InnerException.Message;
                }
                return false;
            }
        }
    }
}

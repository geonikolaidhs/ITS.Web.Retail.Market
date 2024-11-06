using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.Xpo;
using Newtonsoft.Json;

namespace ITS.Retail.ScanTool.Model
{
    public class ImageConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Bitmap);
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

    public enum UploadResult
    {
        NOT_SENT,
        FAIL,
        SUCCESS
    }

    public class ScannedDocumentHeader : XPBaseObject
    {
        public ScannedDocumentHeader()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ScannedDocumentHeader(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            this.Oid = Guid.NewGuid();
        }

        // Fields...
        private UploadResult _Uploaded;
        private Guid _Oid;
        private string _SupplierTaxCode;
        private String _DocumentNumber;
        private long _ScannedOn;
        private DateTime _DocumentIssueDate;
        private double _DocumentAmount;




        public UploadResult Uploaded
        {
            get
            {
                return _Uploaded;
            }
            set
            {
                SetPropertyValue("Uploaded", ref _Uploaded, value);
            }
        }

        [ValueConverter(typeof(DevExpress.Xpo.Metadata.ImageValueConverter)), Delayed]
        public Image ScannedImage
        {
            get
            { 
                return GetDelayedPropertyValue<Image>("ScannedImage");  
            }
            set
            {
                SetDelayedPropertyValue<Image>("ScannedImage", value);
                ScannedOn = DateTime.Now.Ticks;
            }
            
        }

        public DateTime DocumentIssueDate
        {
            get
            {
                return _DocumentIssueDate;
            }
            set
            {
                SetPropertyValue("DocumentIssueDate", ref _DocumentIssueDate, value);
            }
        }

        public double DocumentAmount
        {
            get
            {
                return _DocumentAmount;
            }
            set
            {
                SetPropertyValue("DocumentAmount", ref _DocumentAmount, value);
            }
        }


        public String DocumentNumber
        {
            get
            {
                return _DocumentNumber;
            }
            set
            {
                SetPropertyValue("DocumentNumber", ref _DocumentNumber, value);
            }
        }


        public string SupplierTaxCode
        {
            get
            {
                return _SupplierTaxCode;
            }
            set
            {
                SetPropertyValue("SupplierTaxCode", ref _SupplierTaxCode, value);
            }
        }

        public long ScannedOn
        {
            get
            {
                return _ScannedOn;
            }
            set
            {
                SetPropertyValue("ScannedOn", ref _ScannedOn, value);
            }
        }

        public DateTime ScannedOnDateTime
        {
            get
            {
                return new DateTime(ScannedOn);
            }
        }

        public bool IsValid
        {
            get 
            {
                return ScannedImage != null && ScannedImage.Height > 0 && !String.IsNullOrWhiteSpace(this.DocumentNumber) && DocumentIssueDate.Ticks > 1000 && !String.IsNullOrWhiteSpace(this.SupplierTaxCode);
            }
        }

        [Key]
        public Guid Oid
        {
            get
            {
                return _Oid;
            }
            set
            {
                SetPropertyValue("Oid", ref _Oid, value);
            }
        }

        public virtual string ToJson(JsonSerializerSettings settings, bool includeType = false)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            IList<PropertyInfo> props = new List<PropertyInfo>(this.GetType().GetProperties());
            foreach (PropertyInfo prop in props)
            {
                string propName = prop.Name;
                //Τα Non persistent ειναι υπολογιζόμενα συνήθως. Σε κάθε περίπτωση δεν χρειάζεται να μεταφερθούν
                if (prop.GetCustomAttributes(typeof(NonPersistentAttribute), false).Count() > 0)
                {
                    if (!prop.CanWrite)
                        continue;
                }

                //Attributes που χρειάζονται στο desing time
                if (prop.GetCustomAttributes(typeof(MemberDesignTimeVisibilityAttribute), false).Count() > 0)
                    continue;
                //Συγκεκριμένα Attribute που προκαλούν διάφορα προβλήματα
                if (propName == "Session" || propName == "ClassInfo" || propName == "IsLoading")
                    continue;
                //Computed Attributes. Αυτά δεν μπορούν να γίνουν set. Τζάμπα θα μεταφερθούν!
                if (!prop.CanWrite)
                    continue;

                //Για όλα τα υπόλοιπα....
                object propValue = prop.GetValue(this, null);
                if (!(propValue is XPBaseCollection))
                {
                    if (propValue is Image && propValue != null)
                    {
                        string json = JsonConvert.SerializeObject(propValue, new ImageConverter());
                        dict.Add(propName, json);
                    }
                    else if (propValue != null && propValue.GetType().IsSubclassOf(typeof(Enum)))
                        dict.Add(propName, ((int)propValue).ToString());
                    else
                        dict.Add(propName, propValue != null ? propValue.ToString() : "");
                }
            }
            //dict.Add("GCRecord", this.GetPropertyValue("GCRecord") == null ? "null" : this.GetPropertyValue("GCRecord").ToString());
            if (includeType)
            {
                dict.Add("Type", this.GetType().Name);
            }
            return JsonConvert.SerializeObject(dict, Formatting.Indented, settings);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Linq;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using ITS.Retail.Model;
using DevExpress.Data.Linq.Helpers;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using SevenZip.Compression.LZMA;
using DevExpress.XtraReports.Native;
using System.Text.RegularExpressions;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using System.Collections;

namespace ITS.Retail.Common
{
    public abstract class XtraReportBaseExtension : XtraReport, System.ComponentModel.INotifyPropertyChanged
    {
        public XtraReportBaseExtension()
            : base()
        {

            this.DataSourceDemanded += XtraReportBaseExtension_DataSourceDemanded;
            SupportingQuery1 = new LinqServerModeSource();
            SupportingQuery2 = new LinqServerModeSource();
            SupportingQuery3 = new LinqServerModeSource();
            SupportingQuery4 = new LinqServerModeSource();
            SupportingQuery5 = new LinqServerModeSource();
            UserStores = new LinqServerModeSource();
            CurrentOwner = new LinqServerModeSource();
        }

        /// <summary>
        /// Gets or sets the current duplicate number
        /// </summary>
        public int CurrentDuplicate { get; set; }

        /// <summary>
        /// Gets or sets the total number of duplicates of this report
        /// </summary>
        public int Duplicates
        {
            get
            {
                return _Duplicates;
            }
            set
            {
                _Duplicates = value;
                Notify("Duplicates");
            }
        }

        protected void Notify(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        private int _Duplicates;

        private string OLAPConnectionString;

        public void SetOLAPConnectionString(string olapConnectionString)
        {
            this.OLAPConnectionString = olapConnectionString;
        }

        public string GetOLAPConnectionString()
        {
            return this.OLAPConnectionString;
        }

        protected List<ParameterInfo> ConvertParametersToParameterInfo(IEnumerable<Parameter> parameters)
        {
            List<ParameterInfo> converted = new List<ParameterInfo>();
            foreach (Parameter parameter in this.Parameters)
            {
                converted.Add(new ParameterInfo(parameter, editor: new System.Windows.Forms.Control()));
            }
            return converted;
        }

        private NonSerializableComponentsHelperBase GetNonSerializableComponentsHelper()
        {
            if (this.DesignMode)
                return this.Site.GetService(typeof(NonSerializableComponentsHelperBase)) as NonSerializableComponentsHelperBase;
            return null;
        }

        protected IQueryable OriginalQueryableSource { get; set; }

        protected virtual void XtraReportBaseExtension_DataSourceDemanded(object sender, EventArgs e)
        {
            CriteriaToExpressionConverter converter = new CriteriaToExpressionConverter();
            string filterStringWithValues = ReplaceParametersWithValues(this.FilterString);
            CriteriaOperator getFilterCriteria = this.GetFilterCriteria(filterStringWithValues);
            foreach (CalculatedField calcField in this.CalculatedFields)
            {
                getFilterCriteria = CriteriaHelper.RemoveCriteriaByFieldName(calcField.Name, getFilterCriteria);
            }
            OriginalQueryableSource = this._ModelQuery.MainQuerySet();
            this.ModelProperty.QueryableSource = OriginalQueryableSource.AppendWhere(converter, getFilterCriteria);
            this.DataSource = this.ModelProperty;

        }


        protected override void OnParametersRequestBeforeShow(ParametersRequestEventArgs e)
        {
            base.OnParametersRequestBeforeShow(e);
            foreach (ParameterInfo info in e.ParametersInformation)
            {
                if ((info.Parameter is ReportParameterExtension) && info.Parameter.LookUpSettings != null)
                {
                    if ((info.Parameter as ReportParameterExtension).MultiSelect)
                    {
                        CheckedComboBoxEdit lookUpEdit = new CheckedComboBoxEdit();

                        if ((info.Parameter.LookUpSettings is DynamicListLookUpSettings))
                        {
                            DynamicListLookUpSettings settings = info.Parameter.LookUpSettings as DynamicListLookUpSettings;
                            lookUpEdit.Properties.DataSource = settings.DataSource;
                            lookUpEdit.Properties.IncrementalSearch = true;
                            lookUpEdit.Properties.DisplayMember = settings.DisplayMember;
                            lookUpEdit.Properties.ValueMember = settings.ValueMember;
                        }
                        else if ((info.Parameter.LookUpSettings is StaticListLookUpSettings))
                        {
                            StaticListLookUpSettings settings = info.Parameter.LookUpSettings as StaticListLookUpSettings;
                            lookUpEdit.Properties.DataSource = settings.LookUpValues;
                            lookUpEdit.Properties.DisplayMember = "Description";
                            lookUpEdit.Properties.ValueMember = "Value";
                        }

                        lookUpEdit.Properties.NullText = "<Επιλέξτε " + info.Parameter.Description + ">";
                        info.Editor = lookUpEdit;
                        info.Parameter.Value = DBNull.Value;

                        if (lookUpEdit.Properties.DataSource is XPCollection && String.IsNullOrWhiteSpace(((ReportParameterExtension)info.Parameter).SortingProperty) == false)
                        {
                            ((XPCollection)lookUpEdit.Properties.DataSource).Sorting.Add(
                                new SortProperty(
                                    ((ReportParameterExtension)info.Parameter).SortingProperty, DevExpress.Xpo.DB.SortingDirection.Ascending
                                    )
                                );
                        }
                    }

                    (info.Parameter as ReportParameterExtension).LookUpEditor = info.Editor;
                }
            }
        }

        public override string FilterString
        {
            get
            {
                return base.FilterString;
            }
            set
            {
                base.FilterString = value;
            }
        }

        protected string ConvertValueToString(string parameterName, Type parameterType, object value)
        {
            /* Numeric comparison values. Numeric literals of different types can be specified in a string form using suffixes:
             * Int32 - 1
             * Int16 (short) - 1s
             * Byte (byte) - 1b
             * Double (double) - 1.0
             * Single (float) - 1.0f
             * Decimal (decimal) - 1.0m
             */
            string convertedValue = null;
            if (parameterType == typeof(string))
            {
                convertedValue = "'" + value.ToString() + "'";
            }
            else if (parameterType == typeof(short))
            {
                convertedValue = value.ToString() + "s";
            }
            else if (parameterType == typeof(byte))
            {
                convertedValue = value.ToString() + "b";
            }
            else if (parameterType == typeof(double))
            {
                convertedValue = String.Format("{0:0.0000}", value);
            }
            else if (parameterType == typeof(float))
            {
                convertedValue = String.Format("{0:0.0000}f", value);
            }
            else if (parameterType == typeof(decimal))
            {
                convertedValue = String.Format("{0:0.0000}m", value);
            }
            else if (parameterType == typeof(DateTime))
            {
                string parced = new BinaryOperator(parameterName, value).LegacyToString();
                convertedValue = "#" + parced.Split('#')[1] + "#";
            }
            else if(parameterType == typeof(Guid))
            {
                convertedValue = "{" + value.ToString() + "}";
            }
            else
            {
                convertedValue = value == null ? "" : value.ToString();
            }

            return convertedValue;
        }

        protected string ReplaceParametersWithValues(string filterString)
        {
            string result = filterString;

            foreach (Parameter parameter in this.Parameters)
            {
                string val = null;
                if (parameter.MultiValue)
                {
                    foreach (object listValue in (parameter.Value as IEnumerable))
                    {
                        val += ConvertValueToString(parameter.Name, parameter.Type, listValue) + ",";
                    }
                    if (val != null)
                    {
                        val = val.TrimEnd(',');
                    }
                }
                else
                {
                    val = ConvertValueToString(parameter.Name, parameter.Type, parameter.Value);
                }

                result = result.Replace("?" + parameter.Name, val);
            }
            return result;

        }

        protected static byte[] CompressLZMA(string inFile)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, inFile);
                byte[] inbyt = ms.ToArray();
                byte[] b = SevenZipHelper.Compress(inbyt);
                return b;
            }
            catch (Exception ex)
            {
                throw new Exception("General compress error:" + ex.Message, ex);
            }
        }

        protected static string DecompressLZMAFromBytes(byte[] value)
        {
            string retval = null;
            try
            {

                byte[] outByt = SevenZipHelper.Decompress(value);
                MemoryStream outMs = new MemoryStream(outByt);
                outMs.Seek(0, 0);
                BinaryFormatter bf = new BinaryFormatter();
                retval = (string)bf.Deserialize(outMs, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retval;
        }

        public void SaveEncrypted(string fileName)
        {

            MemoryStream mm = new MemoryStream();
            this.SaveLayout(mm);
            mm.Position = 0;
            string data;
            using (StreamReader reader = new StreamReader(mm))
            {
                data = reader.ReadToEnd();
            }
            string encryptedString = Encryption.EncryptString(data, "1t$ervices2013");
            byte[] compressedBytes = CompressLZMA(encryptedString);
            FileStream newStream = new FileStream(fileName, FileMode.Create);
            using (BinaryWriter sr = new BinaryWriter(newStream))
            {
                sr.Write(compressedBytes);
                sr.Close();
            }
            newStream.Close();
            newStream.Dispose();
            mm.Close();
            mm.Dispose();
        }

        private static string GetReportCode(byte[] bytes)
        {
            string uncompressed = DecompressLZMAFromBytes(bytes);
            string unencrypted = Encryption.DecryptString(uncompressed, "1t$ervices2013");
            //Backwards Compatibility 
            if (unencrypted.Contains("ITS.RetailModel"))
            {
                unencrypted = unencrypted.Replace("Retail.Common", "ITS.Retail.Common").Replace("ITS.RetailModel", "ITS.Retail.Model").Replace("ITS.POSClient.Enumerations", "ITS.Retail.Platform.Enumerations").Replace("ITS.ITS.", "ITS.").Replace("<TypeName>Common.XtraReportExtension</TypeName>", "<TypeName>ITS.Retail.Common.XtraReportExtension</TypeName>");
            }
            //End Backwards Compatibility ;
            return unencrypted;
        }

        public static Type GetReportTypeFromFile(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open);
            byte[] compressed;
            using (BinaryReader sr = new BinaryReader(stream))
            {
                compressed = sr.ReadBytes((int)stream.Length);
                sr.Close();
            }
            stream.Close();
            stream.Dispose();
            return GetReportTypeFromFile(compressed);
        }

        public static Type GetReportTypeFromFile(byte[] bytes)
        {
            string unencrypted = GetReportCode(bytes);
            Regex findType = new Regex("<TypeName>ITS\\.Retail\\.Common\\.(?<type>[^<]*)</TypeName>");
            Match match = findType.Match(unencrypted);
            string typeName = match.Groups["type"].Value;
            string fullTypeName = typeof(XtraReportBaseExtension).FullName.Replace("XtraReportBaseExtension", typeName);

            return typeof(XtraReportBaseExtension).Assembly.GetType(fullTypeName);
        }

        public static Type GetSingleObjectTypeFromFile(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open);
            byte[] compressed;
            using (BinaryReader sr = new BinaryReader(stream))
            {
                compressed = sr.ReadBytes((int)stream.Length);
                sr.Close();
            }
            stream.Close();
            stream.Dispose();
            return GetSingleObjectTypeFromFile(compressed);
        }

        public static Type GetSingleObjectTypeFromFile(byte[] bytes)
        {
            try
            {
                SingleObjectXtraReport xtraReport = new SingleObjectXtraReport();
                xtraReport.LoadEncrypted(bytes);
                return xtraReport.ObjectType;
            }
            catch
            {
                return null;
            }
        }

        public void LoadEncrypted(string fileName)
        {
            FileStream newStream = new FileStream(fileName, FileMode.Open);
            this.LoadEncrypted(newStream);
        }

        public void LoadEncrypted(Stream stream)
        {
            byte[] compressed;
            using (BinaryReader sr = new BinaryReader(stream))
            {
                compressed = sr.ReadBytes((int)stream.Length);
                sr.Close();
            }
            stream.Close();
            stream.Dispose();

            LoadEncrypted(compressed);
        }

        public void LoadEncrypted(byte[] bytes)
        {
            string unencrypted = GetReportCode(bytes);
            MemoryStream unencryptedStream = new MemoryStream();
            using (StreamWriter sr = new StreamWriter(unencryptedStream))
            {

                //int start = unencrypted.IndexOf("<Resources>");
                //int end = unencrypted.IndexOf("// label4");
                //string toDelete = unencrypted.Substring(start,end-start);
                //unencrypted = unencrypted.Substring(0, start) + unencrypted.Substring(end, unencrypted.Length - (end-start)-1);
                //unencrypted = unencrypted.Replace(toDelete, "");
                //File.WriteAllLines("c:\\kitsos.txt", new string[] { unencrypted });

                sr.Write(unencrypted);
                sr.Flush();
                unencryptedStream.Position = 0;
                this.LoadLayout(unencryptedStream);
                try
                {
                    if (this.ExtraReportScripts != null)
                    {
                        ExtraReportScripts.OnAfterLoad(this);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error in Extra Scripts:" + ex.Message + Environment.NewLine + ex.StackTrace);
                }

            }
            unencryptedStream.Close();
            unencryptedStream.Dispose();
        }

        protected AbstractLinqQuery _ModelQuery;
        private TopMarginBand topMarginBand1;

        protected LinqServerModeSource Model;

        public LinqServerModeSource ModelProperty
        {
            get
            {
                return Model;
            }
            set
            {
                Model = value;
            }
        }

        private Guid currentOwnerOid;
        public Guid CurrentOwnerOid
        {
            get
            {
                return currentOwnerOid;
            }
            set
            {
                currentOwnerOid = value;
                if (ModelQuery != null)
                {
                    ModelQuery.CurrentOwnerOid = value;
                    UserStores.QueryableSource = this.StoresOfUser;
                    CurrentOwner.QueryableSource = this.CurrentCompany == null ? new List<CompanyNew>().AsQueryable() : new List<CompanyNew>() { this.CurrentCompany }.AsQueryable();
                }
            }
        }

        private Guid currentUserOid;
        public Guid CurrentUserOid
        {
            get
            {
                return currentUserOid;
            }
            set
            {
                currentUserOid = value;
                if (ModelQuery != null)
                {
                    ModelQuery.CurrentUserOid = value;
                    UserStores.QueryableSource = this.StoresOfUser;
                    CurrentOwner.QueryableSource = this.CurrentCompany == null ? new List<CompanyNew>().AsQueryable() : new List<CompanyNew>() { this.CurrentCompany }.AsQueryable();
                }
            }
        }

        public IQueryable StoresOfUser
        {
            get
            {
                if (ModelQuery != null)
                {
                    var currectUser = ModelQuery.Session.GetObjectByKey<User>(CurrentUserOid);
                    if (currectUser == null)
                    {
                        return new List<Store>().AsQueryable();
                    }
                    if (currectUser.Role.Type == Platform.Enumerations.eRoleType.SystemAdministrator ||
                        currectUser.Role.Type == Platform.Enumerations.eRoleType.CompanyAdministrator)
                    {
                        return new XPQuery<Store>(ModelQuery.Session).Where(x => x.Owner.Oid == CurrentOwnerOid);
                    }

                    IQueryable<UserTypeAccess> uta = new XPQuery<UserTypeAccess>(ModelQuery.Session);
                    List<Guid> effectiveGuids = uta.Where(ut => ut.User.Oid == this.CurrentUserOid &&
                        ut.EntityType == typeof(Store).ToString()).Select(x => x.EntityOid).ToList();
                    var v = new XPQuery<Store>(ModelQuery.Session)
                        .Where(x => effectiveGuids.Contains(x.Oid) && x.Owner.Oid == CurrentOwnerOid).ToList();
                    return v.AsQueryable();
                }
                return new List<Store>().AsQueryable();
            }
        }

        public CompanyNew CurrentCompany
        {
            get
            {
                if (ModelQuery != null)
                {
                    return ModelQuery.Session.GetObjectByKey<CompanyNew>(this.CurrentOwnerOid);
                }
                return null;
            }
        }

        protected override void SerializeProperties(DevExpress.XtraReports.Serialization.XRSerializer serializer)
        {
            base.SerializeProperties(serializer);
            try //backwards compartibility
            {
                serializer.SerializeValue("CurrentOwnerOid", CurrentOwnerOid, typeof(Guid));
                serializer.SerializeValue("CurrentUserOid", CurrentUserOid, typeof(Guid));
                serializer.SerializeValue("ExtraScriptsCode", ExtraScriptsCode, typeof(string));
            }
            catch
            {
            }
            serializer.SerializeValue("LinqCode", LinqCode, typeof(string));
        }

        protected override void DeserializeProperties(DevExpress.XtraReports.Serialization.XRSerializer serializer)
        {
            base.DeserializeProperties(serializer);
            DataSource = null;
            string lcode = serializer.DeserializeValue("LinqCode", typeof(string), null) as string;
            try//backwards compartibility
            {
                if (this.CurrentOwnerOid == Guid.Empty)
                {
                    this.CurrentOwnerOid = (Guid)serializer.DeserializeValue("CurrentOwnerOid", typeof(Guid), Guid.Empty);
                }
                if (this.CurrentUserOid == Guid.Empty)
                {
                    this.CurrentUserOid = (Guid)serializer.DeserializeValue("CurrentUserOid", typeof(Guid), Guid.Empty);
                }
                this.ExtraScriptsCode = serializer.DeserializeValue("ExtraScriptsCode", typeof(string), null) as string;
            }
            catch
            {
            }


            if (lcode.Contains("ITS.RetailModel"))
            {
                lcode = lcode.Replace("(Common.XpoHelper", "(ITS.Retail.Common.XpoHelper").Replace("ITS.RetailModel", "ITS.Retail.Model");
            }
            LinqCode = lcode;
        }

        public List<CompilerMessage> Warning;
        public List<CompilerMessage> Error;
        public string compiledAssemblyLocation;

        public bool TryCompile(string code, out List<CompilerMessage> Warnings, out List<CompilerMessage> Errors, out string compiledAssembly)
        {
            return ApplicationDomainUtility.CompileInSeparateApplicationDomain(code, out Warnings, out Errors, out compiledAssembly);
        }

        private bool TryCompile()
        {
            return TryCompile(_LinqCode, out  Warning, out Error, out compiledAssemblyLocation);
        }


        public AbstractLinqQuery ModelQuery
        {
            get
            {
                return _ModelQuery;
            }
            protected set
            {
                _ModelQuery = value;
                UpdateModel();
            }
        }

        public void UpdateModel()
        {
            if (ModelProperty == null)
            {
                ModelProperty = new LinqServerModeSource();
                ModelProperty.KeyExpression = _ModelQuery.KeyExpression;
            }
            if (_ModelQuery != null)
            {
                ModelProperty.QueryableSource = _ModelQuery.MainQuerySet();
                FiveIQueryables fiq = _ModelQuery.SupportingQuerySets();
                SupportingQuery1.QueryableSource = fiq.SupportingQuery1;
                SupportingQuery2.QueryableSource = fiq.SupportingQuery2;
                SupportingQuery3.QueryableSource = fiq.SupportingQuery3;
                SupportingQuery4.QueryableSource = fiq.SupportingQuery4;
                SupportingQuery5.QueryableSource = fiq.SupportingQuery5;
                UserStores.QueryableSource = this.StoresOfUser;
                CurrentOwner.QueryableSource = this.CurrentCompany == null ? new List<CompanyNew>().AsQueryable() : new List<CompanyNew>() { this.CurrentCompany }.AsQueryable();
                this.DataSource = ModelProperty;
            }
        }

        public LinqServerModeSource SupportingQuery1;
        public LinqServerModeSource SupportingQuery2;
        public LinqServerModeSource SupportingQuery3;
        public LinqServerModeSource SupportingQuery4;
        public LinqServerModeSource SupportingQuery5;
        public LinqServerModeSource UserStores;
        public LinqServerModeSource CurrentOwner;
        public ExtraReportScripts ExtraReportScripts { get; set; }

        protected String _ExtraScriptsCode;
        public String ExtraScriptsCode
        {
            get
            {
                return _ExtraScriptsCode;
            }
            set
            {
                _ExtraScriptsCode = value;
                this.ExtraReportScripts = null;
                if (String.IsNullOrWhiteSpace(_ExtraScriptsCode) == false && 
                    _ExtraScriptsCode != DefaultExtraScriptsCode.Replace("{0}", "ITS").Replace("{1}", "ReportName") &&
                    TryCompile(_ExtraScriptsCode, out  Warning, out Error, out compiledAssemblyLocation))
                {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(compiledAssemblyLocation);
                    var types = assembly.GetTypes().Where(g => g.IsSubclassOf(typeof(ExtraReportScripts)));
                    if (types.Count() == 1)
                    {
                        this.ExtraReportScripts = Activator.CreateInstance(types.First()) as ExtraReportScripts;
                    }
                }
                else
                {
                    compiledAssemblyLocation = null;
                }
            }
        }

        protected string _LinqCode;
        public virtual string LinqCode
        {
            get
            {
                return _LinqCode;
            }
            set
            {
                _LinqCode = value;

                if (TryCompile())
                {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(compiledAssemblyLocation);
                    var types = assembly.GetTypes().Where(g => g.IsSubclassOf(typeof(AbstractLinqQuery)));
                    if (types.Count() == 1)
                    {
                        AbstractLinqQuery query = Activator.CreateInstance(types.First()) as AbstractLinqQuery;
                        query.CurrentOwnerOid = this.CurrentOwnerOid;
                        query.CurrentUserOid = this.CurrentUserOid;
                        query.Parameters = this.Parameters;
                        ModelQuery = query;
                    }
                }
                else
                {
                    compiledAssemblyLocation = null;
                }
            }
        }

        private void InitializeComponent()
        {
            DevExpress.Data.Helpers.IsDesignModeHelper.BypassDesignModeAlterationDetection = true;
            this.topMarginBand1 = new TopMarginBand();
            this.detailBand1 = new DetailBand();
            this.bottomMarginBand1 = new BottomMarginBand();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // topMarginBand1
            // 
            this.topMarginBand1.HeightF = 109.375F;
            this.topMarginBand1.Name = "topMarginBand1";
            // 
            // detailBand1
            // 
            this.detailBand1.Name = "detailBand1";
            // 
            // bottomMarginBand1
            // 
            this.bottomMarginBand1.Name = "bottomMarginBand1";
            // 
            // XtraReportBaseExtension
            // 
            this.Bands.AddRange(new Band[] {
            this.topMarginBand1,
            this.detailBand1,
            this.bottomMarginBand1});
            this.Margins = new System.Drawing.Printing.Margins(100, 100, 109, 100);
            this.Version = "13.2";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        public virtual string DefaultExtraScriptsCode
        {
            get
            {
                return @"
using DevExpress.Xpo;
using ITS.Retail.Model;
using System.Linq;
using System;
using System.Collections.Generic;
using DevExpress.Data;
using DevExpress.Data.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Metadata;
using System.Collections;
using System.Linq.Expressions;
using ITS.Retail.Common;

namespace {0}
{
    public class ExtraReportScriptsExtension: ExtraReportScripts
    {

        public override void OnAfterLoad(XtraReportBaseExtension report)
        {
            /*  Code Samples:
                ================
                
                Getting a date's absolute start and end:
                
                    DateTime anyDateTime = DateTime.Now;
                    DateTime absoluteStart = anyDateTime.Date;
                    DateTime absoluteEnd = anyDateTime.AddDays(1).AddSeconds(-1);
            */

            
        }
    }
}";
            }
        }

        public virtual string DefaultQueryCode
        {
            get
            {
                return @"
using DevExpress.Xpo;
using ITS.Retail.Model;
using System.Linq;
using System;
using System.Collections.Generic;
using DevExpress.Data;
using DevExpress.Data.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Metadata;
using System.Collections;
using System.Linq.Expressions;

namespace {0}
{
    public class {1}: AbstractLinqQuery
    {
        public {1}(): base(ITS.Retail.Common.XpoHelper.GetNewUnitOfWork()) { }
       
        public override string KeyExpression
        {
            get
            {
                return """ + "Oid" + @"""" + @";
            }   
        }        

        public override IQueryable MainQuerySet()
        {
            //Examples
            //----------------------
            /* var result = from  docHead in DocumentHeadersQuery
                         group docHead by docHead.Customer into grp
                         select new { Oid = grp.Key.Oid, Customer = grp.Key.Code, Total = grp.Sum(gr =>gr.GrossTotal) };

            //group sales by item and store -----
			var result = from  detail in DocumentDetailsQuery
                         group detail by new { Store = detail.DocumentHeader.Store,Item = detail.Item }into grp
                         select new { Oid = grp.Key.Item.Oid ,Item = grp.Key.Item, Store = grp.Key.Store ,GrossTotal = grp.Sum(gr=>gr.GrossTotal) };

            var result = from  customer in CustomersQuery                                         
            			 orderby customer.DocumentHeaders.Count descending
                         select new { Oid = customer.Trader.Oid, TaxCode = customer.Trader.TaxCode, NumberOfDocuments = customer.DocumentHeaders.Count ,Total = customer.DocumentHeaders.Sum(gr =>gr.GrossTotal) };
            return result;*/
            //----------------------

            return {2};
        }        
        public override FiveIQueryables SupportingQuerySets()
        {
            return new FiveIQueryables(){SupportingQuery1=null, SupportingQuery2=null, SupportingQuery3=null,  SupportingQuery4=null, SupportingQuery5=null};
        } 

    }
}";
            }
        }


        private DetailBand detailBand1;
        private BottomMarginBand bottomMarginBand1;

        private void GroupHeader1_SortingSummaryGetResult(object sender, GroupSortingSummaryGetResultEventArgs e)
        {
            object res = e.Result;
            object calculatedValues = e.CalculatedValues.Aggregate((a, b) => a.ToString() + b.ToString());
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    }
}

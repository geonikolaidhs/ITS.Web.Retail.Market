using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;
using ITS.POS.Client.Forms;
using ITS.POS.Client.UserControls;
using ITS.Retail.Platform.Common.AuxilliaryClasses;
using ITS.Retail.Platform.Common.OposReportSchema;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Kernel
{
    public class OposReportService : IOposReportService
    {

        private ISessionManager SessionManager { get; set; }
        private IConfigurationManager Configuration { get; set; }
        private IPosKernel Kernel { get; set; }


        public OposReportService(ISessionManager sessionManager, IConfigurationManager configuration, IPosKernel kernel)
        {
            this.Configuration = configuration;
            this.SessionManager = sessionManager;
            this.Kernel = kernel;

        }
        public object GetReportInstance(String assemblyName, String ClassName)
        {
            object instance = null;
            try
            {
                string assemblyPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Modules\\" + assemblyName;
                Assembly assembly = Assembly.LoadFrom(assemblyPath);
                Type T = assembly.GetType("ITS.POS.Client." + ClassName);
                object[] constructorParams = new object[] { Kernel };
                instance = Activator.CreateInstance(T, constructorParams);
                assembly = null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return instance;
        }




        public List<UserParameter> GetReportParameters(object instance)
        {

            List<UserParameter> parameters = new List<UserParameter>();
            Type T = instance.GetType();
            List<PropertyInfo> properties = T.GetProperties().ToList();
            foreach (PropertyInfo prop in properties)
            {
                if (prop.PropertyType.Name == "UserParameter")
                {
                    string labeltext = GetPropertyValue(instance, prop.Name + ".LabelText").ToString();
                    eUserParameterType paramType = (eUserParameterType)GetPropertyValue(instance, prop.Name + ".Type");
                    object defaultValue = GetPropertyValue(instance, prop.Name + ".DefaultValue");

                    UserParameter p = new UserParameter();
                    p.LabelText = labeltext;
                    p.Type = paramType;
                    p.DefaultValue = defaultValue;
                    p.ParameterName = prop.Name;
                    if (paramType == eUserParameterType.LookUpEdit)
                    {

                        p.Datasource = (Dictionary<object, object>)GetPropertyValue(instance, prop.Name + ".Datasource");
                    }

                    parameters.Add(p);
                }
            }
            return parameters;
        }


        public void SetParameterValue(object target, string compoundProperty, object value)
        {
            try
            {
                string[] bits = compoundProperty.Split('.');
                for (int i = 0; i < bits.Length - 1; i++)
                {
                    PropertyInfo propertyToGet = target.GetType().GetProperty(bits[i]);
                    target = propertyToGet.GetValue(target, null);
                }
                PropertyInfo propertyToSet = target.GetType().GetProperty(bits.Last());
                propertyToSet.SetValue(target, value, null);
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Error(DateTime.Now.ToString() + "SetParameterValue : " + ex.Message);
            }
        }

        public object GetPropertyValue(object src, string propName)
        {
            if (src == null) throw new ArgumentException("Value cannot be null.", "src");
            if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

            if (propName.Contains("."))//complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                return GetPropertyValue(GetPropertyValue(src, temp[0]), temp[1]);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
                return prop != null ? prop.GetValue(src, null) : null;
            }
        }

        public List<ReportLine> ExecuteDynamicFunction(String FunctionName, object instance)
        {
            List<ReportLine> result = new List<ReportLine>();

            try
            {
                Type T = instance.GetType();
                BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
                MethodInfo callMethod = T.GetMethod(FunctionName, flags);
                if (callMethod != null)
                {
                    Type returnType = callMethod.ReturnType;
                    if (returnType.Name == "void")
                    {
                        callMethod.Invoke(instance, null);
                    }
                    else if (returnType.Name == "bool")
                    {
                        callMethod.Invoke(instance, null);
                    }
                    else
                    {
                        result = (List<ReportLine>)callMethod.Invoke(instance, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Error(DateTime.Now.ToString() + "ExecuteDynamicFunction : " + FunctionName + " " + ex.Message);
            }
            return result;
        }


        public void CreateReportParameters(frmReportParameters frm, object reportClassInstace, List<UserParameter> reportParams)
        {
            try
            {
                foreach (Control control in frm.Controls)
                {
                    if (control.GetType() == typeof(TableLayoutPanel))
                    {
                        TableLayoutPanel layoutControl = (TableLayoutPanel)control;
                        foreach (Control con in layoutControl.Controls)
                        {
                            if (con.GetType().BaseType == typeof(ucBaseFilterControl))
                            {
                                ucBaseFilterControl filterControl = (ucBaseFilterControl)con;
                                foreach (UserParameter rep in reportParams)
                                {
                                    if (filterControl.ParameterName == rep.ParameterName)
                                    {
                                        object val = filterControl.GetControlValue();
                                        SetParameterValue(reportClassInstace, filterControl.ParameterName + ".SelectedValue", val);
                                    }
                                }
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Error(DateTime.Now.ToString() + "CreateReportParameters " + ex.GetFullMessage());
            }


        }



        public List<ReportLine> GetReportLines(object reportClassInstace)
        {
            List<ReportLine> reportLines = new List<ReportLine>() { };
            try
            {

                ExecuteDynamicFunction("AlwaysExecuted", reportClassInstace);
                reportLines = ExecuteDynamicFunction("GetPrintedLines", reportClassInstace);
                return reportLines;
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Error(DateTime.Now.ToString() + "CreateReportParameters " + ex.GetFullMessage());
            }
            return reportLines;
        }


        public string GetPreviewTextResult(frmReportParameters frm, object reportClassInstace, List<UserParameter> reportParams, IAppContext appContext, Form ShowOnForm)
        {
            List<ReportLine> reportLines = new List<ReportLine>() { };
            frmWaitForm waitform = new frmWaitForm();
            waitform.ShowOnTopMode = ShowFormOnTopMode.AboveAll;
            waitform.StartPosition = FormStartPosition.CenterParent;
            SplashScreenManager.ShowForm(ShowOnForm, typeof(frmWaitForm), true, true, false);
            try
            {
                CreateReportParameters(frm, reportClassInstace, reportParams);
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Error(DateTime.Now.ToString() + "CreateReportParameters " + ex.GetFullMessage());
            }

            reportLines = GetReportLines(reportClassInstace);
            List<string> printerlines = new List<string>();
            string result = "";
            foreach (ReportLine reportLine in reportLines)
            {
                string currentLine = reportLine.BuildReportLine();
                result = result + currentLine + "    " + Environment.NewLine;
            }
            SplashScreenManager.CloseForm();

            return result;
        }


        public List<string> GetStringLinesResult(frmReportParameters frm, object reportClassInstace, List<UserParameter> reportParams, IAppContext appContext, Form ShowOnForm)
        {
            List<ReportLine> reportLines = new List<ReportLine>() { };
            frmWaitForm waitform = new frmWaitForm();
            waitform.ShowOnTopMode = ShowFormOnTopMode.AboveAll;
            waitform.StartPosition = FormStartPosition.CenterParent;
            SplashScreenManager.ShowForm(ShowOnForm, typeof(frmWaitForm), true, true, false);
            try
            {
                CreateReportParameters(frm, reportClassInstace, reportParams);
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Error(DateTime.Now.ToString() + "CreateReportParameters " + ex.GetFullMessage());
            }

            reportLines = GetReportLines(reportClassInstace);
            List<string> printerlines = new List<string>();
            foreach (ReportLine reportLine in reportLines)
            {
                string currentLine = new string(reportLine.BuildReportLine().Take(42).ToArray());
                printerlines.Add(currentLine);
            }
            SplashScreenManager.CloseForm();

            return printerlines;
        }
    }
}

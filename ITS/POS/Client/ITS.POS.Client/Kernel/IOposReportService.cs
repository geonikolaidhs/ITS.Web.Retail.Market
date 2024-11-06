using ITS.POS.Client.Forms;
using ITS.Retail.Platform.Common.AuxilliaryClasses;
using ITS.Retail.Platform.Common.OposReportSchema;
using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Kernel
{
    public interface IOposReportService : IKernelModule
    {

        List<string> GetStringLinesResult(frmReportParameters frm, object reportClassInstace, List<UserParameter> reportParams, IAppContext appContext, Form ShowOnForm);

        void CreateReportParameters(frmReportParameters frm, object reportClassInstace, List<UserParameter> reportParams);

        List<ReportLine> ExecuteDynamicFunction(String FunctionName, object instance);

        object GetPropertyValue(object src, string propName);

        void SetParameterValue(object target, string compoundProperty, object value);

        List<UserParameter> GetReportParameters(object instance);

        object GetReportInstance(String assemblyName, String ClassName);

        string GetPreviewTextResult(frmReportParameters frm, object reportClassInstace, List<UserParameter> reportParams, IAppContext appContext, Form ShowOnForm);
    }
}

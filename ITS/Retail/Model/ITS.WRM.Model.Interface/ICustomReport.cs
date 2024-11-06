using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface ICustomReport: ILookUpFields
    {
        string Code { get; set; }
        string Title { get; set; }
        byte[] ReportFil { get; set; }
        string FileName { get; set; }
        eCultureInfo CultureInfo { get; set; }
        ICompanyNew Owner { get; set; }
        //IReportCategory ReportCategory { get; set; }
        string ObjectType { get; set; }
        string ReportType { get; set; }
    }
}

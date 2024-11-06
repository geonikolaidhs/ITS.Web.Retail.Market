using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ITS.Retail.Common;
using DevExpress.Xpo;
using ITS.Retail.Model;
using DevExpress.XtraReports.Parameters;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using System.IO;
using ITS.Retail.ResourcesLib;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;

namespace ITS.Retail.WebClient.Helpers
{
    public static class ReportsHelper
    {
        /// <summary>
        /// Gets the XtraReportExtension object from a CustomReport persistent object
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static XtraReportBaseExtension GetXtraReport(Guid id, CompanyNew currentOwner, User currentUser, string olapConnectionString, out string title, out string description)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Guid oid = id;
                    CustomReport customReport = uow.GetObjectByKey<CustomReport>(oid);

                    if (customReport != null)
                    {
                        title = customReport.Title;
                        description = customReport.Description;
                        Type reportType = XtraReportBaseExtension.GetReportTypeFromFile(customReport.ReportFile);
                        XtraReportBaseExtension report = Activator.CreateInstance(reportType) as XtraReportBaseExtension;
                        if (currentOwner != null)
                        {
                            report.CurrentOwnerOid = currentOwner.Oid;
                        }
                        if (currentUser != null)
                        {
                            report.CurrentUserOid = currentUser.Oid;
                        }
                        report.LoadEncrypted(customReport.ReportFile);
                        report.CurrentDuplicate = 1;
                        report.SetOLAPConnectionString(olapConnectionString);
                        return report;
                    }
                    else
                    {
                        title = "";
                        description = "";
                        return null;
                    }
                }
            }
            catch
            {
                title = "";
                description = "";
                return null;
            }
        }
        public static XtraReportBaseExtension DuplicateReport(XtraReportBaseExtension report, int duplicates, Guid id, CompanyNew effectiveOwner, User currentUser)
        {
            report.Duplicates = duplicates > 1 ? duplicates : 1;
            if (duplicates > 1)
            {
                report.CreateDocument();
                string title, description;
                for (int i = 1; i < duplicates; i++)
                {
                    XtraReportBaseExtension copyReport = ReportsHelper.GetXtraReport(id, effectiveOwner, currentUser,
                        report.GetOLAPConnectionString(), out title, out description);
                    copyReport.CurrentDuplicate = i + 1;
                    copyReport.Duplicates = report.Duplicates;
                    if (copyReport is SingleObjectXtraReport)
                    {
                        (copyReport as SingleObjectXtraReport).ObjectOid = (report as SingleObjectXtraReport).ObjectOid;
                    }
                    copyReport.CreateDocument();
                    report.Pages.AddRange(copyReport.Pages);
                }
            }
            return report;
        }
        public static XtraReportBaseExtension DuplicateReport(XtraReportBaseExtension report, int duplicates)
        {
            report.Duplicates = duplicates > 1 ? duplicates : 1;
            if (duplicates > 1)
            {
                report.CreateDocument();
                using (MemoryStream stream = new System.IO.MemoryStream())
                {
                    report.SaveLayout(stream);
                    for (int i = 1; i < duplicates; i++)
                    {
                        XtraReportBaseExtension copyReport = Activator.CreateInstance(report.GetType()) as XtraReportBaseExtension;
                        copyReport.SetOLAPConnectionString(report.GetOLAPConnectionString());
                        stream.Position = 0;
                        copyReport.LoadLayout(stream);
                        if (copyReport is SingleObjectXtraReport)
                        {
                            (copyReport as SingleObjectXtraReport).ObjectOid = (report as ITS.Retail.Common.SingleObjectXtraReport).ObjectOid;
                        }
                        copyReport.CurrentDuplicate = i + 1;
                        copyReport.CreateDocument();
                        report.Pages.AddRange(copyReport.Pages);
                    }
                }
            }
            return report;
        }
        /// <summary>
        /// Parses the parameters found in the request
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="request"></param>
        public static void ProccessReportParameters(ParameterCollection parameters, HttpRequestBase request)
        {
            if (parameters.Count == 0)
            {
                return;
            }

            foreach (Parameter parameter in parameters)
            {
                object value = null;
                try
                {
                    string requestValue = request[parameter.Name + "_reportParam"];
                    if (parameter.Type == typeof(bool))
                    {
                        value = requestValue.Equals("C") || requestValue.Equals("true");//bool.Parse(requestValue);
                    }
                    else if (parameter.Type == typeof(int) && parameter.LookUpSettings == null)
                    {
                        value = int.Parse(requestValue) / 10000;
                    }
                    else if (parameter.Type == typeof(double) && parameter.LookUpSettings == null)
                    {
                        value = Double.Parse(requestValue) / 10000;
                    }
                    else if (parameter.Type == typeof(DateTime))
                    {
                        value = DateTime.Parse(requestValue);
                    }
                    else if (parameter.Type == typeof(Guid))
                    {
                        value = Guid.Parse(requestValue);
                    }
                    else
                    {
                        value = requestValue;
                    }
                }
                catch
                {
                    value = GetDefault(parameter.Type);
                }
                parameter.Value = value;
                if (parameter.Type == typeof(Guid))
                {
                    parameter.ValueInfo = value.ToString();
                }
            }
        }
        public static List<ReportCategory> GetVisibleReportCategories(User user, int? count = null)
        {
            if (user == null)
            {
                return new List<ReportCategory>();
            }

            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            List<ReportCategory> reportCategories;
            IEnumerable<ReportCategory> allReportCategories = new XPCollection<ReportCategory>(uow, null, new SortProperty("Description", DevExpress.Xpo.DB.SortingDirection.Ascending)).Where(cat => cat.CustomReports.Count > 0);
            if (count != null)
            {
                reportCategories = allReportCategories.Take((int)count).ToList();
            }
            else
            {
                reportCategories = allReportCategories.ToList();
            }
            //List<ReportCategory> reportCategories = new XPCollection<ReportCategory>(uow).Where(cat => cat.CustomReports.Count > 0).ToList();

            XPCollection<CustomReport> uncategorisedReports = new XPCollection<CustomReport>(uow,
                CriteriaOperator.And(new NullOperator("ReportCategory"), new BinaryOperator("ReportType", "General Report")));
            if (uncategorisedReports.Count > 0)
            {
                ReportCategory uncategorisedReportCategory = new ReportCategory(uow);
                uncategorisedReportCategory.Description = Resources.Uncategorised;
                foreach (CustomReport custReport in uncategorisedReports)
                {
                    custReport.ReportCategory = uncategorisedReportCategory;
                }
                reportCategories.Add(uncategorisedReportCategory);
            }
            foreach (ReportCategory r in reportCategories)
            {
                if (UserHelper.IsSystemAdmin(user) == false)
                {
                    r.CustomReports.Filter = CriteriaOperator.Or(
                        new ContainsOperator("ReportRoles",
                        new BinaryOperator("Role.Oid", user.Role.Oid)),
                        new NotOperator(new AggregateOperand("ReportRoles", Aggregate.Exists))
                        );

                }
            }

            return reportCategories;
        }
        public static ReportCategory GetVisibleReportCategory(User user, Guid CategoryOID)
        {
            if (user == null || CategoryOID == null)
            {
                return new ReportCategory();
            }

            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            List<ReportCategory> reportCategories;
            IEnumerable<ReportCategory> allReportCategories = new XPCollection<ReportCategory>(uow, null, new SortProperty("Description", DevExpress.Xpo.DB.SortingDirection.Ascending)).Where(cat => cat.Oid == CategoryOID);
            reportCategories = allReportCategories.ToList();
            ////List<ReportCategory> reportCategories = new XPCollection<ReportCategory>(uow).Where(cat => cat.CustomReports.Count > 0).ToList();

            //XPCollection<CustomReport> uncategorisedReports = new XPCollection<CustomReport>(uow,
            //    CriteriaOperator.And(new NullOperator("ReportCategory"), new BinaryOperator("ReportType", "General Report"), new BinaryOperator("OID", CategoryOID)));
            //if (uncategorisedReports.Count > 0)
            //{
            //    ReportCategory uncategorisedReportCategory = new ReportCategory(uow);
            //    uncategorisedReportCategory.Description = Resources.Uncategorised;
            //    foreach (CustomReport custReport in uncategorisedReports)
            //    {
            //        custReport.ReportCategory = uncategorisedReportCategory;
            //    }
            //    reportCategories.Add(uncategorisedReportCategory);
            //}
            //foreach (ReportCategory r in reportCategories)
            //{
            //    if (UserHelper.IsSystemAdmin(user) == false)
            //    {
            //        r.CustomReports.Filter = CriteriaOperator.Or(
            //            new ContainsOperator("ReportRoles",
            //            new BinaryOperator("Role.Oid", user.Role.Oid)),
            //            new NotOperator(new AggregateOperand("ReportRoles", Aggregate.Exists))
            //            );

            //    }
            //}
            if (reportCategories.Count > 0)
                return reportCategories[0];
            else
                return new ReportCategory();
        }
        public static Dictionary<Guid, string> GetVisibleReportCategoriesDict(User user, int? count = null)
        {
            List<ReportCategory> reportCategories = GetVisibleReportCategories(user, count);
            foreach (ReportCategory r in reportCategories)
            {
                if (UserHelper.IsSystemAdmin(user) == false)
                {
                    r.CustomReports.Filter = CriteriaOperator.Or(
                        new ContainsOperator("ReportRoles",
                        new BinaryOperator("Role.Oid", user.Role.Oid)),
                        new NotOperator(new AggregateOperand("ReportRoles", Aggregate.Exists))
                        );
                }
            }
            Dictionary<Guid, string> dict = new Dictionary<Guid, string>();
            foreach (ReportCategory r in reportCategories)
            {
                dict.Add(r.Oid, r.Description);
            }
            return dict;
        }
        public static XPCollection<CustomReport> GetVisibleCustomReports(User user)
        {
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            XPCollection<CustomReport> visibleReports;
            if (UserHelper.IsSystemAdmin(user))
            {

                visibleReports = new XPCollection<CustomReport>(uow);
            }
            else
            {
                visibleReports = new XPCollection<CustomReport>(uow,
                                                                  CriteriaOperator.Or(
                                                                  new ContainsOperator("ReportRoles", new BinaryOperator("Role.Oid", user.Role.Oid)),
                                                                  new NotOperator(new AggregateOperand("ReportRoles", Aggregate.Exists))));
            }

            return visibleReports;
        }
        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
        public static IEnumerable<DocumentTypeCustomReport> GetValidDocumentTypeCustomReports(User user, DocumentType documentType, UnitOfWork uow)
        {
            UserType userType = UserHelper.GetUserType(user);

            if (user.Role.Type == eRoleType.CompanyAdministrator || user.Role.Type == eRoleType.SystemAdministrator)
            {
                return new XPCollection<DocumentTypeCustomReport>(uow, new BinaryOperator("DocumentType.Oid", documentType.Oid));
            }
            else
            {
                XPCollection<DocumentTypeCustomReport> valid = new XPCollection<DocumentTypeCustomReport>(uow,
                                                            CriteriaOperator.And(
                                                            new BinaryOperator("DocumentType.Oid", documentType.Oid),
                                                            new ContainsOperator("Report.ReportRoles", new BinaryOperator("Role.Oid", user.Role.Oid)),
                                                            CriteriaOperator.Or(new BinaryOperator("UserType", userType), new BinaryOperator("UserType", UserType.ALL))));

                return valid;
            }
        }
    }
}

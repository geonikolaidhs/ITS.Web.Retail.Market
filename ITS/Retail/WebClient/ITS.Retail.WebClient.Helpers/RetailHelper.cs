using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using System.Text.RegularExpressions;
using System.IO;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Common;
using DevExpress.Xpo;
using ITS.Retail.ResourcesLib;
using ITS.Retail.Platform;
using Newtonsoft.Json.Linq;
using ITS.Retail.WebClient.Helpers.Exceptions;
using ITS.Licensing.Client.Core;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.WebClient.Licensing;

namespace ITS.Retail.WebClient.Helpers
{
    public static class RetailHelper
    {
        public static Image ResizeImage(Image image, Size size, bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = (float)size.Width / (float)originalWidth;
                float percentHeight = (float)size.Height / (float)originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        public static eDXCallbackArgument GetGridCallbackType(string dxCallbackArgument)
        {
            if (String.IsNullOrWhiteSpace(dxCallbackArgument))
            {
                return eDXCallbackArgument.UNKNOWN;
            }
            if (dxCallbackArgument.Contains("STARTEDIT"))
            {
                return eDXCallbackArgument.STARTEDIT;
            }
            else if (dxCallbackArgument.Contains("ADDNEWROW"))
            {
                return eDXCallbackArgument.ADDNEWROW;
            }
            else if (dxCallbackArgument.Contains("CANCELEDIT"))
            {
                return eDXCallbackArgument.CANCELEDIT;
            }
            else if (dxCallbackArgument.Contains("APPLYCOLUMNFILTER"))
            {
                return eDXCallbackArgument.APPLYCOLUMNFILTER;
            }
            else if (dxCallbackArgument.Contains("DELETESELECTED"))
            {
                return eDXCallbackArgument.DELETESELECTED;
            }
            else if (dxCallbackArgument.Contains("SELECTROWS"))
            {
                return eDXCallbackArgument.SELECTROWS;
            }
            else if (dxCallbackArgument.Contains("PAGERONCLICK"))
            {
                return eDXCallbackArgument.PAGERONCLICK;
            }
            else if (dxCallbackArgument.Contains("COLUMNMOVE"))
            {
                return eDXCallbackArgument.COLUMNMOVE;
            }
            else if (dxCallbackArgument.Contains("SORT"))
            {
                return eDXCallbackArgument.SORT;
            }
            else if (dxCallbackArgument.Contains("APPLYFILTER"))
            {
                return eDXCallbackArgument.APPLYFILTER;
            }
            else
            {
                return eDXCallbackArgument.UNKNOWN;
            }
        }

        public static List<Guid> GetOidsToDeleteFromDxCallbackArgument(string dxcallbackArgument)
        {
            List<Guid> oids = new List<Guid>();
            string allOids = dxcallbackArgument.Split(new string[] { "DELETESELECTED|" }, new StringSplitOptions())[1].Trim(';');
            string[] unparsed = allOids.Split(',');
            foreach (string unparsedOid in unparsed)
            {
                oids.Add(Guid.Parse(unparsedOid));
            }

            return oids;
        }

        public static Guid GetOidToEditFromDxCallbackArgument(string dxcallbackArgument)
        {
            Guid guid = Guid.Empty;
            if (dxcallbackArgument.Contains("STARTEDIT") || dxcallbackArgument.Contains("UPDATEEDIT"))
            {
                string[] array = dxcallbackArgument.Split('|', ';');
                int i = array.Length;
                bool found = false;
                while (i > 0 && found == false)
                {
                    i--;
                    if (Guid.TryParse(array[i], out guid))
                    {
                        found = true;
                    }

                }
            }

            return guid;
        }

        public static CriteriaOperator ApplyOwnerCriteria(CriteriaOperator inputCriteria, Type type, CompanyNew owner)
        {
            if (owner == null && (typeof(IRequiredOwner).IsAssignableFrom(type) || type == typeof(User)))
            {
                throw new ArgumentNullException("owner", "IRequiredOwner cannot have null owner.");
            }

            if (type == typeof(User))
            {
                CriteriaOperator joinCriteria = (new OperandProperty("^.EntityOid") == new OperandProperty("Oid")) & (new OperandProperty("Owner") == owner.Oid);

                JoinOperand joinOperand = new JoinOperand("Customer", joinCriteria);

                CriteriaOperator crop = CriteriaOperator.Or(new ContainsOperator("UserTypeAccesses",
                                                                                CriteriaOperator.And(new BinaryOperator("EntityType", typeof(Customer).FullName), joinOperand)),
                                                            new ContainsOperator("UserTypeAccesses",
                                                                                CriteriaOperator.And(new BinaryOperator("EntityType", typeof(CompanyNew).FullName),
                                                                                                     new BinaryOperator("EntityOid", owner.Oid))));
                return CriteriaOperator.And(inputCriteria, crop);
            }
            
            if (typeof(IOwner).IsAssignableFrom(type) && owner != null)
            {
                return CriteriaOperator.And(inputCriteria,
                                            CriteriaOperator.Or(new BinaryOperator("Owner.Oid", owner.Oid),
                                                                new NullOperator("Owner")));
            }
            return inputCriteria;
        }


        public static List<string> GoBack()
        {
            List<string> originalURL = HttpContext.Current.Request.UrlReferrer.LocalPath.Split('/').ToList();
            originalURL.Remove(originalURL.First());
            if (originalURL.Count == 1)
            {
                originalURL.Add("Index");
            }
            return originalURL;
        }


        public static string Capitalise(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            if (s.Length > 1)
            {
                return char.ToUpper(s[0]) + s.Substring(1).ToLower();
            }
            return string.Empty;
        }

        /// <summary>
        /// Count words with Regex.
        /// </summary>
        public static int CountWords(string s)
        {
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }

        /// <summary>
        /// Gets the exception message plus the innner exception message is any.
        /// </summary>
        /// <param name="exception">The exception that has beenthrown</param>
        /// <param name="getStackTrace">Get stack trace if True. Defaults to true.</param>
        /// <returns></returns>
        public static string GetErrorMessage(Exception exception, bool getStackTrace = true)
        {
            string errorMessage = exception.Message;

            if (exception.InnerException != null && String.IsNullOrEmpty(exception.InnerException.Message) == false)
            {
                errorMessage += Environment.NewLine + exception.InnerException.Message;
            }

            if (getStackTrace && String.IsNullOrEmpty(exception.StackTrace) == false)
            {
                errorMessage += String.Format("{0}StackTrace:{0}{1}", Environment.NewLine, exception.StackTrace);
            }

            return errorMessage;
        }

        public static List<string> GetAvailableFonts(string fontsFolder)
        {
            string fileType = ".css";
            string fontsSearchPattern = "*" + fileType;
            List<string> files = Directory.GetFiles(fontsFolder, fontsSearchPattern).ToList();
            List<string> fileNames = new List<string>();
            files.ForEach(filename => fileNames.Add(filename.Replace(fontsFolder, "").Replace(fileType, "").Replace("\\", "")));
            return fileNames;
        }

        /// <summary>
        /// Combines the date and time in one variable
        /// </summary>
        /// <param name="date">Date value</param>
        /// <param name="time">Time value</param>
        /// <returns>The combined value</returns>
        public static DateTime? GetDateTimeValue(DateTime? date, DateTime? time)
        {
            return Common.ViewModel.DateTimeHelper.GetDateTimeValue(date, time);
        }

        /// <summary>
        /// Returns the provide date with time changed to 12am
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Returns the provide date with time changed to 12am</returns>
        public static DateTime GetTimeFromDayStart(DateTime date)
        {
            return date.AddDays(-1).AddMilliseconds(1);
        }

        /// <summary>
        /// Returns the provide date with time changed to 12pm
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Returns the provide date with time changed to 12pm</returns>
        public static DateTime GetTimeToDayEnd(DateTime date)
        {
            return date.AddDays(1).AddMilliseconds(-1);
        }

        public static void CheckRemoteDocumentSequence(DocumentHeader documentHeader, string sequenceJson)
        {
            JObject jsonItem = JObject.Parse(sequenceJson);
            string error;
            DocumentSequence databaseDocumentSequence = documentHeader.Session.GetObjectByKey<DocumentSequence>( Guid.Parse(jsonItem.Property("Oid").Value.ToString()));

            int scSequence = 0;
            if (databaseDocumentSequence != null)
            {
                scSequence = databaseDocumentSequence.DocumentNumber;
            }

            if (databaseDocumentSequence == null && documentHeader.DocumentSeries.DocumentSequence == null)
            {
                //NOP
                //document sequence is saved for the very first time
            }
            else if (databaseDocumentSequence == null && documentHeader.DocumentSeries.DocumentSequence != null)
            {
                throw new NotSupportedException();
            }
            else if (databaseDocumentSequence != null && documentHeader.DocumentSeries.DocumentSequence == null)
            {
                if (databaseDocumentSequence.DocumentSeries != null
                    && databaseDocumentSequence.DocumentSeries.Oid == documentHeader.DocumentSeries.Oid
                    )
                {
                    documentHeader.DocumentSeries.DocumentSequence = databaseDocumentSequence;
                    documentHeader.DocumentSeries.Save();
                }
                else
                {
                    throw new DocumentSequenceSyncException(string.Format(Resources.DocumentSeriesIsInconsistentWithHQsData, documentHeader.DocumentSeries.Description));
                }
            }
            else if (databaseDocumentSequence != null && documentHeader.DocumentSeries.DocumentSequence != null)
            {
                if (databaseDocumentSequence.Oid != documentHeader.DocumentSeries.DocumentSequence.Oid
                  || databaseDocumentSequence.DocumentSeries.Oid != documentHeader.DocumentSeries.Oid
                   )
                {
                    //TODO
                    throw new DocumentSequenceSyncException(string.Format(Resources.DocumentSeriesIsInconsistentWithHQsData, documentHeader.DocumentSeries.Description));
                }
            }

            int masterSequence = int.Parse(jsonItem.Property("DocumentNumber").Value.ToString());
            if (masterSequence > scSequence || databaseDocumentSequence == null)
            {
                if (databaseDocumentSequence == null)
                {
                    databaseDocumentSequence = new DocumentSequence(documentHeader.Session);
                }
                databaseDocumentSequence.FromJson(sequenceJson, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out error);
                databaseDocumentSequence.CreatedByDevice = StoreControllerAppiSettings.StoreControllerOid.ToString();
                databaseDocumentSequence.UpdatedOnTicks = DateTime.Now.Ticks;
                databaseDocumentSequence.Save();
            }
        }

        public static void Log(Exception exception)
        {
            using (UnitOfWork unitOfWork = XpoHelper.GetNewUnitOfWork())
            {
                ApplicationLog applicationLog = new ApplicationLog(unitOfWork);
                string error = exception.GetFullMessage() + Environment.NewLine + exception.StackTrace;
                applicationLog.Error = error;
                applicationLog.Result = error;
                applicationLog.Save();
                XpoHelper.CommitTransaction(unitOfWork);
            }
        }

        public static void SetLicenseUserDistribution(object sender, EventArgs eArgs)
        {
            LicenseManager licenseManager = sender as LicenseManager;
            LicenseInfo licenseInfo = licenseManager.ReadStoredLicenseInfo();
            using ( UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                XPCollection<LicenseUserDistribution> licenceUsersDitributions = new XPCollection<LicenseUserDistribution>(uow);

                int maxConnectedUsers = licenceUsersDitributions.Sum(distribution => distribution.ServerLicenseInfo.MaxConnectedUsers);
                int maxPeripheralUsers = licenceUsersDitributions.Sum(distribution => distribution.ServerLicenseInfo.MaxPeripheralsUsers);
                int maxTabletSmartPhoneUsers = licenceUsersDitributions.Sum(distribution => distribution.ServerLicenseInfo.MaxTabletSmartPhoneUsers);

                if ( maxConnectedUsers > licenseInfo.MaxUsers )
                {

                }

                if ( maxPeripheralUsers > licenseInfo.MaxPeripheralsUsers)
                {

                }

                if ( maxTabletSmartPhoneUsers > licenseInfo.MaxTabletSmartPhoneUsers )
                {

                }

                foreach ( LicenseUserDistribution distribution in licenceUsersDitributions )
                {
                    ServerLicenseInfo serverLicenseInfo = distribution.ServerLicenseInfo;                    
                    serverLicenseInfo.StartDate = licenseInfo.StartDate;
                    serverLicenseInfo.EndDate = licenseInfo.EndDate;
                    serverLicenseInfo.DaysToAlertBeforeExpiration = licenseInfo.DaysToAlertBeforeExpiration;
                    serverLicenseInfo.GreyZoneDays = licenseInfo.GreyZoneDays;

                    distribution.SetInfo(serverLicenseInfo);
                    distribution.Save();
                }

                XpoHelper.CommitTransaction(uow);
            }
        }
    }
}
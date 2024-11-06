using ITS.Retail.WebClient.AuxillaryClasses;
using ITS.Retail.WebClient.AuxilliaryClasses;
using ITS.Retail.WebClient.Extensions;
using ITS.Retail.WebClient.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    [RoleAuthorize]
    public class POSRecoveryController : BaseController
    {
        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            string error = string.Empty;
            List<POSTransactionFile> posTransactionFiles = TransactionHelper.GetTransactionFiles(out error);
            if ( !string.IsNullOrEmpty(error) )
            {
                Session["Error"] = error;
            }
            return View(posTransactionFiles);
        }


        public JsonResult GetDocumentHeaders(string selectedFiles)
        {
            List<string> selectedFileNames = new List<string>();
            if (!string.IsNullOrEmpty(selectedFiles))
            {
                selectedFileNames = selectedFiles.Split(',').ToList();
            }
            if ( selectedFileNames.Count == 0 )
            {
                return Json(new { error = ResourcesLib.Resources.PleaseSelectARecord });
            }
            List<POSTransactionFile> posTransactionFiles = TransactionHelper.GetSelectedTransactionFiles(selectedFileNames);

            return Json(JsonConvert.SerializeObject(posTransactionFiles, Platform.PlatformConstants.JSON_SERIALIZER_SETTINGS));
        }

        public ActionResult POSDatabases()
        {
            string error = string.Empty;
            List<POSTransactionFile> posTransactionFiles = TransactionHelper.GetTransactionFiles(out error);
            if (!string.IsNullOrEmpty(error))
            {
                Session["Error"] = error;
            }
            return PartialView(posTransactionFiles);
        }

        public JsonResult GetPOSTransactionFiles(string selectedFiles)
        {
            if (string.IsNullOrEmpty(selectedFiles))
            {
                return Json(new { error = ResourcesLib.Resources.PleaseSelectARecord });
            }

            List<POSTransactionDatabasePair> POSTransactionDatabasePairs = JsonConvert.DeserializeObject<List<POSTransactionDatabasePair>>(selectedFiles, Platform.PlatformConstants.JSON_SERIALIZER_SETTINGS);

            List<ReprocessPOSTransaction> reprocessPOSTransactions = new List<ReprocessPOSTransaction>();

            IEnumerable<string> posDatabaseFiles = POSTransactionDatabasePairs.Select(pair => pair.file).Distinct();

            foreach( string posDatabaseFile in posDatabaseFiles)
            {
                string oidList = string.Join(",", POSTransactionDatabasePairs.Where(pair => pair.file == posDatabaseFile).Select(pair => pair.oid));

                reprocessPOSTransactions.Add(
                                                new ReprocessPOSTransaction()
                                                {
                                                    file = posDatabaseFile,
                                                    oids = oidList
                                                }
                                            );
            }

            if (reprocessPOSTransactions.Count == 0)
            {
                return Json(new { error = ResourcesLib.Resources.PleaseSelectARecord });
            }
            string proccesResult = string.Empty;
            reprocessPOSTransactions.Where(posTrans => posTrans.IsValid == false).Select(posTrans => posTrans.ParseErrors).ToList().ForEach(message => proccesResult += message);
            bool processContainsErrors = false;

            string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");//As loang as POStransaction.ProcessFile catches exceptions 
                                                                           //for each DocumentHeader seperately and procceds with all it can process
                                                                           // There is no need to add a try catch finally here to safely restore CultureInfo

            reprocessPOSTransactions.ForEach(POStransaction =>
            {
                string transactionInfo;
                bool processSucceeded = POStransaction.ProcessFile(out transactionInfo);
                if( !processContainsErrors )
                {
                    processContainsErrors = !processSucceeded;
                }
                proccesResult += transactionInfo;
            });

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);

            if ( processContainsErrors )
            {
                return Json(new { error = proccesResult });
            }
            return Json(new { result = proccesResult });
        }

    }
}

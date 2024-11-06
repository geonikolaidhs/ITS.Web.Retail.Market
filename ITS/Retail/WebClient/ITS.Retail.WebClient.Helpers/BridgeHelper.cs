using ITS.Retail.Common.ViewModel;
using ITS.Retail.Common.ViewModel.Importable;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.Xpo;
namespace ITS.Retail.WebClient.Helpers
{
    public static class BridgeHelper
    {
        public static string SMTPHost;
        public static string Port;
        public static string EmailFrom;
        public static string EmailUsername;
        public static string EmailPassword;
        public static string Domain;
        public static bool EnableSSL;
		//public static string OwnerTaxCode = "094063140"; //default Masoutis: taxcode 094063140
        //public static bool MultipleItemCategoryTrees = false; //default Masoutis : false
        //public static string RootItemCategoryCode = "00"; //default Masoutis: "00"
		//public static string BasicPriceCatalogCode = "1"; //default Masoutis: 1
        
        public static string SpecialCharacterReplacement(string input)
        {
            string[][] map = new string[][] { new string[] { "\\t", "\t" }, new string[] { "\\r", "\r" } };
            string output = input;
            foreach (string[] el in map)
            {
                output = output.Replace(el[0], el[1]);
            }
            return output;
        }


        private static void FillChildSupplierImportFileRecordHeader(List<SupplierImportFileRecordHeader> list, SupplierImportFileRecordHeader currentLevel)
        {
            list.Add(currentLevel);
            if(currentLevel.DetailSupplierImportFileRecordHeaders.Count >0)
            {                
                currentLevel.DetailSupplierImportFileRecordHeaders.ToList().ForEach(x => FillChildSupplierImportFileRecordHeader(list, x));
            }
        }

        public static List<IImportableViewModel> PerformImport(UnitOfWork uow, SupplierImportFilesSet supplierImportFilesSet, 
            SupplierImportFileRecordHeader supplierImportFileRecordHeader, StreamReader stream)
        {
            //Get Importable Types
            IEnumerable<Type> importableTypes = typeof(IImportableViewModel).Assembly.GetTypes()
                .Where(x => x.IsAbstract == false && typeof(IImportableViewModel).IsAssignableFrom(x));

            
            Dictionary<Type, List<IImportableViewModel>> dictionary = importableTypes.ToDictionary(x => x, x => new List<IImportableViewModel>());

            List<SupplierImportFileRecordHeader> allHeaders = new List<SupplierImportFileRecordHeader>();
            FillChildSupplierImportFileRecordHeader(allHeaders, supplierImportFileRecordHeader);

            List<IImportableViewModel> importableViewModels = importableTypes.Select(x => Activator.CreateInstance(x) as IImportableViewModel).ToList();
            Type mainType = importableViewModels.FirstOrDefault(x => x.EntityName == supplierImportFileRecordHeader.EntityName).GetType();

            stream.BaseStream.Seek(0, SeekOrigin.Begin); 

            while(stream.EndOfStream == false)
            {
                string line = stream.ReadLine();
                importableViewModels = importableTypes.Select(x => Activator.CreateInstance(x) as IImportableViewModel).ToList();
                foreach (IImportableViewModel vm in importableViewModels)
                {
                    foreach (SupplierImportFileRecordHeader head in allHeaders)
                    {
                        ImportResult result = vm.Import(line, head);
                        if (result.Applicable )
                        {
                            if (dictionary[vm.GetType()].Any(x => x.ImportObjectUniqueKey == vm.ImportObjectUniqueKey) == false)
                            {
                                dictionary[vm.GetType()].Add(vm);
                            }
                        }
                    }
                }
            }


            foreach(KeyValuePair<Type, List<IImportableViewModel>> pair in dictionary)
            {
                foreach (IImportableViewModel parent in pair.Value)
                {
                    foreach (KeyValuePair<Type, List<IImportableViewModel>> childpair in dictionary)
                    {
                        parent.UpdateChildren(childpair.Value, childpair.Key);
                    }
                }
            }

            List<IImportableViewModel> mainObjectList = dictionary[mainType];

            mainObjectList.ForEach(x => x.CheckWithDatabase(uow, supplierImportFilesSet.Owner.Oid));

            return mainObjectList;
        }


    }
}
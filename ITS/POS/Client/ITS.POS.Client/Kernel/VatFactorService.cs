using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Client.Exceptions;
using ITS.POS.Hardware;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Contains helper functions for VAT factor lookup and checking
    /// </summary>
    public class VatFactorService : IVatFactorService
    {
        protected ISessionManager SessionManager { get; set; }
        protected IConfigurationManager ConfigurationManager { get; set; }

        public VatFactorService(ISessionManager sessionManager,IConfigurationManager configurationManager)
        {
            this.SessionManager = sessionManager;
            this.ConfigurationManager = configurationManager;
        }

        public VatFactor GetApplicationVatFactor(eMinistryVatCategoryCode ministryCode, Guid vatLevel)
        {
            XPCollection<VatCategory> allCategoriesWithMinistryCode = new XPCollection<VatCategory>(SessionManager.GetSession<VatCategory>(), new BinaryOperator("MinistryVatCategoryCode", ministryCode));
            if (allCategoriesWithMinistryCode.Count > 1)
            {
                throw new POSException(String.Format(POSClientResources.VAT_CATEGORY_MULTIPLE_MINISTRY_CODES, ministryCode));
            }
            else if (allCategoriesWithMinistryCode.Count == 1)
            {
                VatCategory vatCategory = allCategoriesWithMinistryCode.First();
                VatFactor vatFactor = SessionManager.FindObject<VatFactor>(CriteriaOperator.And(
                                                                            new BinaryOperator("VatCategory", vatCategory),
                                                                            new BinaryOperator("VatLevel", vatLevel)));
                if (vatFactor != null)
                {
                    return vatFactor;
                }
                else
                {
                    throw new POSException(ministryCode + " " + POSClientResources.VAT_FACTOR_NOT_FOUND);
                }
            }
            else
            {
                /////Vat category with ministry code does not exist, cannot check
                return null;
            }
        }

        public void CheckIfVatFactorsAreValid(FiscalPrinter fiscalPrinter,out bool? isVatAValid, out bool? isVatBValid, out bool? isVatCValid, out bool? isVatDValid, out bool? isVatEValid)
        {
            double fiscalVatA, fiscalVatB, fiscalVatC, fiscalVatD, fiscalVatE;
            fiscalPrinter.ReadVatRates(out fiscalVatA, out fiscalVatB, out fiscalVatC, out fiscalVatD, out fiscalVatE);
            Store currentStore = SessionManager.GetObjectByKey<Store>(ConfigurationManager.CurrentStoreOid);
            Guid vatLevel = Guid.Empty;
            if (currentStore != null)
            {
                Address address = SessionManager.GetObjectByKey<Address>(currentStore.Address);
                if (address != null)
                {
                    vatLevel = address.VatLevel;
                }
                else
                {
                    throw new POSException(POSClientResources.STORE_HAS_NO_ADDRESS);
                }
            }
            else
            {
                throw new POSException(POSClientResources.CURRENT_STORE_NOT_FOUND);
            }

            if (vatLevel == Guid.Empty)
            {
                throw new POSException(POSClientResources.STORE_HAS_NO_VAT_LEVEL);
            }

            isVatAValid = CheckVatFactor(eMinistryVatCategoryCode.A, (decimal)fiscalVatA, vatLevel);
            isVatBValid = CheckVatFactor(eMinistryVatCategoryCode.B, (decimal)fiscalVatB, vatLevel);
            isVatCValid = CheckVatFactor(eMinistryVatCategoryCode.C, (decimal)fiscalVatC, vatLevel);
            isVatDValid = CheckVatFactor(eMinistryVatCategoryCode.D, (decimal)fiscalVatD, vatLevel);
            isVatEValid = CheckVatFactor(eMinistryVatCategoryCode.E, (decimal)fiscalVatE, vatLevel);
        }

        private bool? CheckVatFactor(eMinistryVatCategoryCode ministryCode, decimal fiscalVatFactor, Guid vatLevel)
        {
            XPCollection<VatCategory> allCategoriesWithMinistryCode = new XPCollection<VatCategory>(SessionManager.GetSession<VatCategory>(), new BinaryOperator("MinistryVatCategoryCode", ministryCode));
            if (allCategoriesWithMinistryCode.Count > 1)
            {
                throw new POSException(String.Format(POSClientResources.VAT_CATEGORY_MULTIPLE_MINISTRY_CODES, ministryCode));
            }
            else if (allCategoriesWithMinistryCode.Count == 1)
            {
                VatCategory vatCategory = allCategoriesWithMinistryCode.First();
                VatFactor vatFactor = SessionManager.FindObject<VatFactor>(CriteriaOperator.And(
                                                                            new BinaryOperator("VatCategory", vatCategory),
                                                                            new BinaryOperator("VatLevel", vatLevel)));
                if (vatFactor != null)
                {
                    return vatFactor.Factor == fiscalVatFactor;
                }
                else
                {
                    throw new POSException(ministryCode + " " + POSClientResources.VAT_FACTOR_NOT_FOUND);
                }
            }
            else
            {
                /////Vat category with ministry code does not exist, cannot check
                return null;
            }
        }
    }
}

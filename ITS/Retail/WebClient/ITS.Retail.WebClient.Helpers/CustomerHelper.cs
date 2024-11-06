using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public static class CustomerHelper
    {
        /// <summary>
        /// Validates a string based on C# build in MailAddress Class
        /// </summary>
        /// <param name="email"></param>
        /// <returns>True if the string represents a valid email address as a representation.NOT as an existing email address of a mail provider.Otherwise false.</returns>
        public static bool IsValidEmail(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch (FormatException formatException)
            {
                string error = formatException.GetFullMessage();
                return false;
            }
        }

        /// <summary>
        /// Validates a Customer ( CustomerUser that is actually  Customer,User) concerning B2C area.
        /// </summary>
        /// <param name="customerUser">The Customer under examination</param>
        /// <param name="message">A translated message containing a success message or the reason the Customer is invalid for B2C </param>
        /// <returns>True if Custoemr is valid for B2C area.Otherwise false.</returns>
        public static bool IsValidB2CCustomer(CustomerUser customerUser, out string message)
        {

            if (string.IsNullOrEmpty(customerUser.Email))
            {
                message = Resources.PleaseFillInEmail;
                return false;
            }


            //CriteriaOperator excludeCurrentCustomer = new BinaryOperator("Oid", customerUser.Customer.Oid, BinaryOperatorType.NotEqual);
            //CriteriaOperator excludeCurrentUser = new BinaryOperator("Oid", customerUser.User.Oid, BinaryOperatorType.NotEqual);
            //CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("Code", customerUser.Customer.Code), excludeCurrentCustomer);
            //if (customerUser.Customer.Session.FindObject<Customer>(crop) != null)
            //{
            //    message = Resources.CustomerFoundWithSameCode;
            //    return false;
            //}

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("Email", customerUser.Email));//, excludeCurrentUser);
                if (uow.FindObject<User>(crop) != null)
                {
                    message = Resources.EmailAlreadyUsed;
                    return false;
                }

                if (!IsValidEmail(customerUser.Email))
                {
                    message = Resources.InvalidEmail;
                    return false;
                }

                crop = CriteriaOperator.And(new BinaryOperator("UserName", customerUser.Email));//, excludeCurrentUser);
                if (uow.FindObject<User>(crop) != null)
                {
                    message = Resources.UserNameAlreadyExists;
                    return false;
                }
            }
            message = Resources.SuccessMessage;
            return true;
        }

        public static string CheckedCustomerStorePriceLists(Customer customer, CustomerStorePriceList customerStorePriceList, out string modelKey)
        {
            string error = "";
            modelKey = "";
            if (customer != null)
            {
                if (customer.CustomerStorePriceLists.Where(cspl => cspl.StorePriceList.PriceList.Oid == customerStorePriceList.StorePriceList.PriceList.Oid && cspl.Oid != customerStorePriceList.Oid).Count() > 0)
                {
                    error = Resources.PriceCatalogAlreadyExists;
                    modelKey = "PriceCatalogKey";
                }
                else if (customerStorePriceList.IsDefault && customer.CustomerStorePriceLists.Where(cspl => cspl.IsDefault && cspl.Oid != customerStorePriceList.Oid).Count() > 0)
                {
                    error = Resources.DefaultAllreadyExists;
                    modelKey = "IsDefault";
                }
                //else if (customerStorePriceList.Sort != -1 && customer.CustomerStorePriceLists.Where(cspl => cspl.Sort == customerStorePriceList.Sort && cspl.Oid != customerStorePriceList.Oid).Count() > 0)
                //{
                //    error = Resources.SortIndexAlreadyExists;
                //    modelKey = "Sort";
                //}
            }
            return error;
        }

        public static void CheckTaxCodeOnViesApi(String TaxCode, String CountryCode, out String Name, out String Address, out bool IsValid)
        {
            Name = string.Empty;
            Address = string.Empty;
            IsValid = false;

            if (!string.IsNullOrEmpty(TaxCode))
            {

                var wc = new System.Net.WebClient();
                wc.Encoding = Encoding.UTF8;
                var request = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:ec.europa.eu:taxud:vies:services:checkVat:types"">
                        <soapenv:Header/>
                          <soapenv:Body>
                            <urn:checkVat>
                               <urn:countryCode>COUNTRY</urn:countryCode>
                               <urn:vatNumber>VATNUMBER</urn:vatNumber>
                            </urn:checkVat>
                          </soapenv:Body>
                          </soapenv:Envelope>";

                request = request.Replace("COUNTRY", CountryCode);
                request = request.Replace("VATNUMBER", TaxCode);

                String response = string.Empty;
                try
                {
                    response = wc.UploadString("http://ec.europa.eu/taxation_customs/vies/services/checkVatService", request);
                }
                catch
                {
                    // service throws WebException e.g. when non-EU VAT is supplied
                }


                if (!string.IsNullOrEmpty(response))
                {
                    IsValid = response.Contains("<valid>true</valid>");
                    if (IsValid)
                    {
                        Name = GetStringBetween(response, "<name>", "</name>");
                        Address = GetStringBetween(response, "<address>", "</address>");

                    }
                }
            }

        }

        public static String GetDefaultVatLevel()
        {
            string Oid = string.Empty;
            VatLevel vl;
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                CriteriaOperator crit = CriteriaOperator.And(new BinaryOperator("IsDefault", true));
                vl = uow.FindObject<VatLevel>(crit);
            }

            if (vl != null)
            {
                Oid = vl.Oid.ToString();
            }
            return Oid;
        }

        private static string GetStringBetween(string STR, string FirstString, string LastString)
        {
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2 = STR.IndexOf(LastString);
            FinalString = STR.Substring(Pos1, Pos2 - Pos1);
            return FinalString;
        }
    }
}

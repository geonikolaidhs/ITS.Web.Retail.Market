using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public static class B2CHelper
    {
        /// <summary>
        /// Returns the Company Owner for B2C based on the relative B2C url in the provided Session.
        /// </summary>
        /// <param name="url">The B2C url.At the moment it is the domain only</param>
        /// <param name="uow">The Session in wich the Company is returned</param>
        /// <returns>The Company that owns that B2C.If no company is found then null is returned.</returns>
        public static CompanyNew GetB2CCompany(string url,Session uow)
        {
            CompanyNew b2cCompany = null;
            
            if(string.IsNullOrEmpty(url) == false && uow != null)
            {
                CriteriaOperator crop = new BinaryOperator("B2CURL", url);
                b2cCompany = uow.FindObject<CompanyNew>(crop);
            }

            return b2cCompany;
        }
    }
}

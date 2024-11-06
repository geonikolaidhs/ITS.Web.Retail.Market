using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public static class TransferPurposeHelper
    {
        /// <summary>
        /// Returns a TransferPurpose Object on the Session session that 
        /// has a Description Equal to description
        /// </summary>
        /// <param name="session"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public static TransferPurpose GetByDescription(UnitOfWork uow,String description)
        {
            return uow.FindObject<TransferPurpose>(new BinaryOperator("Description",description,BinaryOperatorType.Equal));
        }
    }
}

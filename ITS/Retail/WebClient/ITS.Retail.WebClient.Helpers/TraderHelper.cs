using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.Retail.Model;
using System.Web.Mvc;
using ITS.Retail.Common;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;

namespace ITS.Retail.WebClient.Helpers
{
    public class TraderHelper
    {

        public static void LoadViewPopup<T>(String stringID, ViewDataDictionary viewdata) where T : BasicObj
        {
            Guid guid = (stringID == null || stringID == "null" || stringID == "-1") ? Guid.Empty : Guid.Parse(stringID);
            if (guid != Guid.Empty)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    dynamic t = uow.FindObject<T>(new BinaryOperator("Oid", guid));
                    if (t == null || t.Trader == null)
                    {
                        viewdata["FirstName"] = "";
                        viewdata["LastName"] = "";
                        viewdata["CompanyName"] = "";
                        viewdata["TraderID"] = null;
                    }
                    else
                    {
                        viewdata["FirstName"] = t.Trader.FirstName;
                        viewdata["LastName"] = t.Trader.LastName;
                        viewdata["CompanyName"] = t.CompanyName;
                        viewdata["TraderID"] = t.Trader.Oid.ToString();
                    }
                }
            }
            else
            {
                viewdata["FirstName"] = "";
                viewdata["LastName"] = "";
                viewdata["CompanyName"] = "";
            }
        }

        public static void GetTraderDescription<T>(string stringID, ViewDataDictionary viewdata, UnitOfWork uow)
        {
            Guid guid = stringID == null || stringID == "null" || stringID == "" || stringID == "-1" ? Guid.Empty : Guid.Parse(stringID);
            if (guid != Guid.Empty)
            {
                dynamic ct = uow.FindObject<T>(new BinaryOperator("Oid", guid));

                viewdata["TraderID"] = ct.Trader.Oid;
                if (typeof(T) == typeof(CompanyNew))
                {
                    viewdata["CompanyID"] = ct.Oid;
                }
                else if (typeof(T) == typeof(SupplierNew))
                {
                    viewdata["SupplierID"] = ct.Oid;
                }
                else if (typeof(T) == typeof(Customer))
                {
                    viewdata["CustomerID"] = ct.Oid;
                }
            }
            else
            {
                Random rnd = new Random();
                int ir = rnd.Next();
                viewdata["TraderID"] = "" + ir.ToString();
            }
        }

        public static void UpdateAddressComboBox(string stringID, ViewDataDictionary viewdata, dynamic viewbag, Trader trader, Session session)
        {
            viewdata["TraderID"] = stringID;
            if (trader == null )
            {
                Guid traderGuid;
                if (!Guid.TryParse(stringID, out traderGuid)) traderGuid = Guid.Empty;
                viewbag.AddressComboBox = new XPCollection<Address>(session, new BinaryOperator("Trader", traderGuid)).OrderBy(address => address.Street);
            }
            else
            {
                viewbag.AddressComboBox = trader.Addresses.OrderBy(address => address.Street);
            }
        }
    }
}

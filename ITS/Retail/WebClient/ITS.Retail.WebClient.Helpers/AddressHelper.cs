using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public static class AddressHelper
    {

        /// <summary>
        /// Checks if an Address is 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool IsValidB2CAddress(Address address)
        {
            if( string.IsNullOrEmpty(address.City) || string.IsNullOrEmpty(address.Street)
                || string.IsNullOrEmpty(address.PostCode) || string.IsNullOrEmpty(address.Region)
                || address.Phones.Count < 1 )
            {
                return false;
            }

            return true;
        }
    }
}

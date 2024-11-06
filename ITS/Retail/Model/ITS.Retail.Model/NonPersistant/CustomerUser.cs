using DevExpress.Xpo;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{
    //[NonPersistent]
    public class CustomerUser //: BaseObj
    {
        public CustomerUser()
        {
           
        }

        //public CustomerUser(Session session)
        //    : base(session)
        //{
        //    //Do not place any code in here!!!
        //}

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City{ get; set; }
        public string Street { get; set; }
        public string PostCode { get; set; }
        public string POBox { get; set; }
        public string Region { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        


        public User SaveB2CUser(UnitOfWork session, string encodedPassword , CompanyNew owner)
        {
            Trader trader = new Trader(session);
            Customer customer = new Customer(session);
            customer.Code = this.Email;
            customer.Trader = trader;
            customer.Trader.FirstName = this.FirstName;
            customer.Trader.LastName = this.LastName;
            customer.CompanyName = this.LastName + " " + this.FirstName;

            User user = new User(session);
            user.Password = encodedPassword;
            user.UserName = this.Email;
            user.FullName = this.LastName+ " "+this.FirstName;
            user.Email = this.Email;
            user.IsB2CUser = true;

            Address address = new Address(session);
            address.Trader = trader;
            address.City = this.City;
            address.Street = this.Street;
            address.PostCode = this.PostCode;
            address.POBox = this.POBox;
            address.Region = this.Region;
            address.IsDefault = false;

            if (string.IsNullOrEmpty(this.Phone) == false)
            {
                address.Phones.Add(new Phone(session) { Number = this.Phone });
            }
            if (string.IsNullOrEmpty(this.Fax) == false)
            {
                address.Phones.Add(new Phone(session) { Number = this.Fax });
            }

            customer.DefaultAddress = address;
            address.Save();
            trader.Save();
            customer.Owner = owner;
            customer.Save();

            user.Save();
            UserTypeAccess UserTypeAccess = new UserTypeAccess(session);
            UserTypeAccess.User = user;
            UserTypeAccess.EntityOid = customer.Oid;
            UserTypeAccess.EntityType = customer.GetType().FullName;
            UserTypeAccess.Save();
            return user;
        }


        public bool IsValid(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
            {
                errorMessage += Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS;
            }

            if (string.IsNullOrEmpty(this.Password))
            {
                errorMessage += Resources.PasswordDoesNotMeetRequirements;
                return false;
            }

            if (string.IsNullOrEmpty(this.ConfirmPassword))
            {
                errorMessage += Resources.PasswordDoesNotMeetRequirements;
                return false;
            }

            if (this.Password != this.ConfirmPassword)
            {
                errorMessage += Resources.PasswordsDoNotMatch;
                return false;
            }

            if( string.IsNullOrEmpty(Phone) || string.IsNullOrEmpty(Region))
            {
                errorMessage += Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS;
            }

            return string.IsNullOrEmpty(errorMessage);
        }
    }
}

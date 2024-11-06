using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{
    public class LoginBindingModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        private bool? rememberMe;

        public string returnUrl { get; set; }

        public bool? RememberMe
        {
            get { return rememberMe ?? false; }
            set { rememberMe = value; }
        }
    }
}
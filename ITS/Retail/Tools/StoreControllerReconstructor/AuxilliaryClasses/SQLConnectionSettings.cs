using System;

namespace StoreControllerReconstructor.AuxilliaryClasses
{
    public class SQLConnectionSettings
    {
        public DatabaseType DatabaseType { get; set; }

        public string Server { get; set; }

        public String Database { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public Func<string> PasswordHandler { get; set; }

        public bool IsValid
        {
            get
            {
                return !String.IsNullOrWhiteSpace(this.Server) 
                    && !String.IsNullOrWhiteSpace(this.Database)
                    && !String.IsNullOrWhiteSpace(this.Username)
                    && !String.IsNullOrWhiteSpace(this.Password);
            }
        }

    }
}

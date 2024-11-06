using System;

namespace StoreControllerReconstructor.AuxilliaryClasses
{
    public class StoreControllerRecontructorSettings
    {
        public SQLConnectionSettings SourceConnectionSettings { get; set; }

        public SQLConnectionSettings DestinationConnectionSettings { get; set; }

        public string StoreCode { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public StoreControllerRecontructorSettings()
        {
            this.SourceConnectionSettings = new SQLConnectionSettings();
            this.DestinationConnectionSettings = new SQLConnectionSettings();
#if DEBUG
            this.SourceConnectionSettings.DatabaseType = DatabaseType.Postgres;
            this.SourceConnectionSettings.Server = "localhost";
            this.SourceConnectionSettings.Database = "feres_hq";
            this.SourceConnectionSettings.Username = "postgres";

            this.DestinationConnectionSettings.DatabaseType = DatabaseType.Postgres;
            this.DestinationConnectionSettings.Server = "localhost";
            this.DestinationConnectionSettings.Database = "feres_sc";
            this.DestinationConnectionSettings.Username = "postgres";

            this.StoreCode = "1";
#endif
        }

        public bool IsValid
        {
            get
            {
                return !String.IsNullOrWhiteSpace(this.StoreCode) && this.SourceConnectionSettings.IsValid && this.DestinationConnectionSettings.IsValid;
            }
        }

        public int IsDateValid
        {
            get
            {
                return DateTime.Compare(this.EndDate, this.StartDate);        
            }
        }
    }
}

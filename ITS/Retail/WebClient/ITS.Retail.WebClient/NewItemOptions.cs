
namespace ITS.Retail.WebClient
{
    public static class NewItemOptions
    {
        public enum TimeOptions
        {
            Today,
            LastWeek,
            LastMonth,
            TimePeriod
        }

        public static TimeOptions CustomTryParse(string parse)
        {
            switch(parse){
                case "Today":
                            return TimeOptions.Today;
                case "LastWeek":
                            return TimeOptions.LastWeek;
                case "LastMonth":
                            return TimeOptions.LastMonth;
                case "TimePeriod":
                            return TimeOptions.TimePeriod;
                default:
                            throw new System.NotImplementedException();
            }
        }
    }
}
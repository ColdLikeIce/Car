namespace HeyTripCarWeb.Share.Dtos
{
    public class LogInfo
    {
        public string tableName { get; set; }
        public int theadId { get; set; }
        public LogEnum logType { get; set; }
        public ApiEnum ApiType { get; set; }
        public DateTime Date { get; set; }
        public string Level { get; set; }
        public string rqInfo { get; set; }
        public string rsInfo { get; set; }
        public string exception { get; set; }
    }

    public enum LogEnum
    {
        ABG = 1,
        ACE = 2,
        Bargin = 3,
        Sixt = 4,
    }

    public enum ApiEnum
    {
        None = -1,
        List = 0,
        Rule = 1,
        Create = 2,
        Cancel = 3,
        Detail = 4,
        Location = 5,
        SixtCreateConfig = 6,
        SixtUpdateOfferConfig = 7,
        SixtCountry = 8
    }
}
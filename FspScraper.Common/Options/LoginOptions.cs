namespace FspScraper.Common.Options
{
    public class LoginOptions
    {
        public string FspUser { get; set; }
        public string FspPass { get; set; }
        public string LoginUrl = "https://app.flightschedulepro.com/Account/Login";
        public string AircraftUrl = "https://app.flightschedulepro.com/App/Aircraft/";
    }
}
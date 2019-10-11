using System.IO;

namespace FspScraper.Common
{
    public class Constants
    {
        public const string FSP_LOGIN_URL = "https://app.flightschedulepro.com/Account/Login";
        public const string FSP_AIRCRAFT_URL = "https://app.flightschedulepro.com/App/Aircraft/";
        // TODO: implement streamreading of credentials or some other method
        public const string CREDENTIALS_DIR = "~/Documents/Dotnet/cred.txt";
        public const string FSP_LOGIN_USER = "dustincotten@gmail.com";
        public const string FSP_LOGIN_PASS = "pedobear";
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xpinn.SportsGo.Util.Portable
{
    public static class URL
    {
        //public const string Host = @"app.1980tic.com";
        //public const string Host = @"201.184.78.68";
        //public const string Host = @"74.208.251.233"; 
        public const string Host = @"app.sportsgo.pro";

        public const string UrlHost = @"http://" + Host + "/";

        //public const string UrlHostSite = UrlHost + "WebApiService/";
        public const string UrlHostSite = UrlHost + "WebApiServicePruebas/";

        public const string UrlBase = UrlHostSite + "api/";
        //public const string UrlBase = "http://localhost:2184/api/";

        //public const string UrlWeb = UrlHost + "SportsGoWeb/";
        public const string UrlWeb = UrlHost + "sportsgowebpruebas/";
    }
}
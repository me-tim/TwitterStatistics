using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TwitterStatistics
{
    /// <summary>
    /// Go from a text that may contain a url to a Uri class
    /// </summary>
    public static class UrlParser
    {
        // v2 test whether :\\ is valid, whether it occurs, and whether we want to add to our definition
        private const string urlPattern = "://";

        private const string protocolFTP = "ftp";
        private const string protocol = "http";
        private const string protocolSSL = "https";

        private static bool anyInvalidUris;

        public static List<Uri> GetUris(string rawText)
        {
            List<Uri> list = null;

            // v3 if know where this string ends, could feed rest of string back through parser
            // however since a possible 2nd URI would only be used by Top and the chance a second
            // URL would be a top URL may be unlikely, not needed at this time
            var uri = UrlParser.GetUri(rawText);
            if (uri != null)
                list = new List<Uri>() { uri };

            return list;
        }

        public static Uri GetUri(string url)
        {
            try
            {
                var validUrl = GetValidUrl(url);
                if (string.IsNullOrWhiteSpace(validUrl))
                    return null;
                return new Uri(validUrl);
            }
            catch (UriFormatException)
            {
                anyInvalidUris = true;
                return null;
            }
        }


        /// <summary>
        /// Quick check if resembles a link, excluding '...'
        /// </summary>
        private static string StripProtocol(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            // must include ':' in search or constantly finding users with http in name @http_mynameisbob etc
            var index = url.IndexOf(urlPattern);

            // url requires a protocol
            if (index < protocolFTP.Length)
                return null;

            // protocol probably at beginning of string, easy case
            else if (index <= protocolSSL.Length)
                return url;
            else
                return StripBeforeProtocol(url);
        }

        private static string StripBeforeProtocol(string url)
        {
            // strip off preceding characters
            // lower or otherwise searches could often fail
            // but urls are case sensitive so do not make string lower
            var urlLower = url.ToLowerInvariant();

            var index = urlLower.IndexOf(protocolSSL);
            if (index > 0)
                return url.Substring(index);
            else if (index < 0)
                index = urlLower.IndexOf(protocol);

            if (index < 0)
                index = urlLower.IndexOf(protocolFTP);

            if (index >= 0)
                return url.Substring(index);
            else
                return null;
        }

        // v2: support multiple urls
        public static string GetValidUrl(string url)
        {
            // make sure it has a protocol
            string justUrl = StripProtocol(url);
            if (string.IsNullOrWhiteSpace(justUrl))
                return null;

            justUrl = Parser.StripAfterEnd(justUrl);

            // if ellipsis in link it is invalid, e.g. "t.c..."
            if (Parser.ContainsEllipsis(justUrl))
                return null;

            // link is impossibly small
            if (justUrl.Length <= protocolFTP.Length)
                return null;

            return justUrl;
        }

        /// <summary>
        /// This method is unit tested but the beta Twitter v2 api does not allow this option in the API yet
        /// even though fully documented how it will work
        /// </summary>
        public static List<Uri> GetEntityURLs(string Entities)
        {
            // always null as of Jan 2021
            if (Entities == null)
                return null;

            var list = new List<Uri>();

            dynamic entitiesJson = JsonConvert.DeserializeObject<dynamic>(Entities.ToString());

            foreach (dynamic urlObjectJson in entitiesJson.urls)
                list.Add(new Uri(urlObjectJson.display_url.Value));

            return list;
        }
    }
}

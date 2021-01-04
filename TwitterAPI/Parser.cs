using System;

namespace TwitterStatistics
{
    public static class Parser
    {
        private const string ellipsis = "…";

        public static string StripAfterEnd(string justUrl)
        {
            // strip off next space and anything after
            // strip off next quote and anything after
            justUrl = StripAfter(justUrl, " ");
            justUrl = StripAfter(justUrl, "\"");
            justUrl = StripAfter(justUrl, ",");
            justUrl = StripAfter(justUrl, "\n");    // unix
            justUrl = StripAfter(justUrl, "\r");    // windows

            return justUrl;
        }

        public static string StripAfterSpaces(string justUrl)
        {
            // strip off next space and anything after
            var anySpaces = justUrl.IndexOf(" ");
            if (anySpaces > 0)
                justUrl = justUrl.Substring(0, anySpaces);
            // should never happen as protocol stripper prevents
            else if (anySpaces == 0)
                throw new Exception("Url parser encountered impossible scenario");

            return justUrl;
        }

        public static string StripAfter(string justUrl, string target)
        {
            if (string.IsNullOrWhiteSpace(justUrl))
                return null;

            // strip off next space and anything after
            var any = justUrl.IndexOf(target);
            if (any > 0)
                justUrl = justUrl.Substring(0, any);

            return justUrl;
        }

        public static bool ContainsEllipsis(string justUrl)
        {
            return justUrl.IndexOf(ellipsis) >= 0;
        }
    }
}

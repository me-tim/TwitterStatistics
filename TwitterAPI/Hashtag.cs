using System.Collections.Generic;

namespace TwitterStatistics
{
    public class Hashtag
    {
        public const string Symbol = "#";

        public static string Parse(string text)
        {
            var start = text.IndexOf(Symbol);
            if (start < 0)
                return null;

            var tag = Parser.StripAfterEnd(text.Substring(start));
            if (tag?.Length <= Symbol.Length)
                return null;

            return tag;
        }

        public static List<string> Get(string text)
        {
            var hashtag = Parse(text);

            if (!string.IsNullOrWhiteSpace(hashtag))
                return new List<string>() { hashtag };

            return null;
        }
    }
}

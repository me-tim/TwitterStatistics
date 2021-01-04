using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace TwitterStatistics
{
    public class ViewModel : IViewModel
    {
        private IStatistics stats;

        // v2 more DRY?
        public int Total { get { return stats.Total; } }
        public int Emojis { get { return stats.Emojis; } }
        public int Urls { get { return stats.Urls; } }
        public int Hashtags { get { return stats.Hashtags; } }
        public DateTime? Begin { get { return stats.Begin; } }
        public DateTime? End { get { return stats.End; } }
        public int GIFs { get { return stats.GIFs; } }
        public int Images { get { return stats.Images; } }
        public int Media { get { return stats.Media; } }
        public int Videos { get { return stats.Videos; } }


        public string EmojiPct { get { return FormatPercent(stats.Emojis, stats.Total); } }
        public string HashtagPct { get { return FormatPercent(stats.Hashtags, stats.Total); } }
        public string UrlPct { get { return FormatPercent(stats.Urls, stats.Total); } }


        // require to track photo urls is vague, difficult, and inconsistent with all the other statistics of a general nature
        // there are significant advantages to widing the definition from 'photo urls' to 'media' which also includes video, gifs, and 
        // can better leverage twitter's meta data
        // v2: consider refactoring out Media to just Photos + Videos + GIFs?
        public string MediaPct { get { return FormatPercent(stats.Media, stats.Total); } }
        public string ImagesPct { get { return FormatPercent(stats.Images, stats.Total); } }
        public string VideosPct { get { return FormatPercent(stats.Videos, stats.Total); } }
        public string GIFsPct { get { return FormatPercent(stats.GIFs, stats.Total); } }


        // moving design towards a Backend for Frontend pattern
        public string RateHour { get { return FormatRate(stats.End - stats.Begin, stats.Total, 60 * 60); } }
        public string RateMinute { get { return FormatRate(stats.End - stats.Begin, stats.Total, 60); } }
        public string RateSecond { get { return FormatRate(stats.End - stats.Begin, stats.Total, 1); } }

        public string Uptime { get { return FormatTimespan(stats.End - stats.Begin); } }

        public IEnumerable<string> TopDomains { get { return stats.TopDomains.ToStrings(); } }
        public IEnumerable<string> TopHashtags { get { return stats.TopHashtags.ToStrings(); } }
        public IEnumerable<string> TopEmojis { get { return stats.TopEmojis.ToStrings(); } }

        private static bool anyExceptions;      // v2 wrap in an IFDEBUG


        public ViewModel(IStatistics _stats)
        {
            stats = _stats;
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this, Statistics.microsoftDateFormatSettings);
        }

        private static string FormatPercent(int numerator, int denominator)
        {
            if (denominator == 0)
                return string.Empty;

            // cast one as float / decimal or integer division was always yielding 0
            var pct = numerator / (Single)denominator;

            try
            {
                return String.Format("{0:P1}", pct);
            }
            catch (Exception)
            {
                anyExceptions = true;
                return "n/a";
            }
        }

        private static string FormatRate(TimeSpan? diff, double numerator, int perSeconds)
        {
            if (!diff.HasValue)
                return string.Empty;

            double denominator;
            if (perSeconds == 60 * 60)
                denominator = diff.Value.TotalHours;
            else if (perSeconds == 60)
                denominator = diff.Value.TotalMinutes;
            else if (perSeconds == 1)
                denominator = diff.Value.TotalSeconds;
            else
                throw new Exception("Invalid parameter perSeconds");

            // dont divide by zero
            if (denominator == 0)
                return string.Empty;

            // cast one as float / decimal or integer division was always yielding 0
            var rate = numerator / denominator;

            try
            {
                return String.Format("{0:0}", rate);
            }
            catch (Exception)
            {
                anyExceptions = true;
                return "n/a";
            }
        }

        private static string FormatTimespan(TimeSpan? upTime)
        {
            // negative timespans throw an exception
            if (!upTime.HasValue || upTime.Value.TotalMilliseconds < 0)
                return string.Empty;

            try
            {
                // "c" -> 1.12:24:02
                return upTime.Value.ToString("c");
            }
            catch (Exception)
            {
                anyExceptions = true;
                return "n/a";
            }
        }

    }
}

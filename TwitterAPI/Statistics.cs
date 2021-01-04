using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TwitterStatistics
{
    public class Statistics : IStatistics
    {
        // v2: explore refactoring into a couple smaller classes

        [JsonIgnore]
        public int ValuesTracked { get; private set; }
        [JsonIgnore]
        public int RankingsTracked { get; private set; }

        private const int defaultValuesTracked = 50000;   // seeing 50k in 15 mins
        private const int defaultRankingsTracked = 5;     // top 0.01%

        // long is not thread-safe on 32 bit machines, so using int
        public int Total { get; protected set; }
        public DateTime? Begin { get; protected set; }
        public DateTime? End { get; protected set; }
        public int Emojis { get; protected set; }
        public int Hashtags { get; protected set; }   // included for consistency
        public int Urls { get; protected set; }
        public int Domains { get; protected set; }      // v2 remove?


        // require to track photo urls is vague, difficult, and inconsistent with all the other statistics of a general nature
        // there are significant advantages to widing the definition from 'photo urls' to 'media' which also includes video, gifs, and 
        // can better leverage twitter's meta data
        // v2: consider refactoring out Media to just Photos + Videos + GIFs?
        public int Media { get; protected set; }
        public int Images { get; protected set; }
        public int Videos { get; protected set; }
        public int GIFs { get; protected set; }
        public int Unknowns { get; protected set; }


        public Top TopDomains { get; protected set; }    // v2 see field entities.urls.url
        public Top TopHashtags { get; protected set; }   // v2 see field entities.hashtags
        public Top TopEmojis { get; protected set; }

        // v2 duplicate rather than share
        public static JsonSerializerSettings microsoftDateFormatSettings { get; private set; }


        static Statistics()
        {
            microsoftDateFormatSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };
        }

        public Statistics(int _valuesTracked = defaultValuesTracked, int _rankingsTracked = defaultRankingsTracked)
        {
            ValuesTracked = _valuesTracked;
            RankingsTracked = _rankingsTracked;

            TopHashtags = new Top(ValuesTracked, RankingsTracked);
            TopEmojis = new Top(ValuesTracked, RankingsTracked);
            TopDomains = new Top(ValuesTracked, RankingsTracked);
        }

        public void Track(ITweet tweet)
        {
            try
            {
                Total++;
            }
            catch(StackOverflowException)
            {
                // just reset stats for now
                // v2 use longs ensure thread safety as 64 bit field are nonatomic nonthreadsafe ++ operation
                Start();
                Total = 1;
            }

            End = DateTime.Now;

            // replace null check with Data.Any()
            if (tweet.AnyEmojis)
                TrackEmoji(tweet.Emojis);

            if (tweet.AnyHashtags)
                TrackHashtag(tweet.Hashtags);

            if (tweet.AnyUris)
            {
                TrackUri();
                TrackDomains(tweet.Uris);
            }

            TrackMedia(tweet);
        }

        public void Reset()
        {
            // v1 i think missing something
            Total = 0;
            Begin = null;
            End = null;

            Urls = 0;
            Domains = 0;
            Emojis = 0;
            Hashtags = 0;

            Media = 0;
            Images = 0;
            Videos = 0;
            GIFs = 0;
            Unknowns = 0;

            TopDomains.Clear();
            TopHashtags.Clear();
            TopEmojis.Clear();
        }

        private void TrackEmoji(IList<string> emoji)
        {
            Emojis++;   // counts if any emojis in tweet or not, NOT how many in tweet if multiple
            TopEmojis.Track(emoji);
        }

        private void TrackHashtag(IList<string> hashtags)
        {
            Hashtags++;
            TopHashtags.Track(hashtags);
        }

        private void TrackDomains(IList<Uri> domains)
        {
            Domains++;
            TopDomains.Track(domains.Select(x => x.Host));
        }

        private void TrackUri()
        {
            Urls++;
        }

        private void TrackMedia(ITweet tweet)
        {
            if (!tweet.AnyMedia)
                return;

            Media++;

            if (tweet.AnyImages)
                Images++;
            else if (tweet.AnyGIFs)
                GIFs++;
            else if (tweet.AnyVideos)
                Videos++;
            else if (tweet.AnyUnknownMedia)
                Unknowns++;
            else
                throw new Exception("Unexpected media type");
        }

        public void Start()
        {
            Reset();
            Begin = DateTime.Now;
        }

        public void StampTime()
        {
            End = DateTime.Now;
        }

        public virtual string ToJSON()
        {
            return JsonConvert.SerializeObject((Statistics)this, microsoftDateFormatSettings);
        }
    }
}

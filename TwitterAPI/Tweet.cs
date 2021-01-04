using System;
using System.Collections.Generic;


namespace TwitterStatistics
{
    public class Tweet : ITweet
    {
        public string Text { get; private set; }
        public string Id { get; private set; }

        public IList<string> Emojis { get; private set; }
        public IList<string> Hashtags { get; private set; }
        public IList<Uri> Uris { get; private set; }
        public IList<TweetMedia> Media { get; private set; }

        public bool AnyUris { get { return Uris?.Count > 0; } }
        public bool AnyEmojis { get { return Emojis?.Count > 0; } }
        public bool AnyMedia { get { return Media?.Count > 0; } }
        public bool AnyHashtags { get { return Hashtags?.Count > 0; } }

        // Any Media
        public bool AnyImages { get; private set; }
        public bool AnyVideos { get; private set; }
        public bool AnyGIFs { get; private set; }
        public bool AnyUnknownMedia { get; private set; }            // v2 remove?


        public Tweet(TwitterTweetDTO dto) : this(dto.Id, dto.Text, TweetMedia.From(dto.Includes?.Media))
        {
        }

        public Tweet(string id, string text, IList<TweetMedia> media)   
        {
            // v2 IList<TweetMedia> to <ITweetMedia>
            Id = id;
            Text = text;


            Emojis = Emoji.Matches(Text);

            // v2 support multiple uris
            Uris = UrlParser.GetUris(Text);

            // v2 support multiple hashtags
            Hashtags = Hashtag.Get(Text);

            // where do we get media stuff?
            Media = media;
            GetAnys();
        }

        private void GetAnys()
        {
            if (Media?.Count > 0)
            {
                // v2 make linq one liner
                foreach (var item in Media)
                    GetAny(item);
            }
        }

        private void GetAny(TweetMedia item)
        {
            if (item.IsGIF)
                AnyGIFs = true;
            else if (item.IsImage)
                AnyImages = true;
            else if (item.IsVideo)
                AnyVideos = true;
            else if (item.IsUnknown)
                AnyUnknownMedia = true;
            else
                throw new Exception("unexpected missing media type");
        }

        public static Tweet From(TwitterTweetDTO dto)
        {
            return new Tweet(dto.Text, dto.Id, TweetMedia.From(dto.Includes.Media));
        }
    }
}

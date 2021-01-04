using System;
using System.Collections.Generic;

namespace TwitterStatistics
{
    public interface ITweet
    {
        string Text { get; }
        string Id { get; }


        // process
        IList<string> Emojis { get; }
        IList<string> Hashtags { get; }
        //IList<string> Domains { get; }
        IList<TweetMedia> Media { get; }
        IList<Uri> Uris { get; }


        bool AnyUris { get; }
        bool AnyEmojis { get; }
        bool AnyMedia { get; }
        bool AnyHashtags { get; }
        bool AnyImages { get; }
        bool AnyGIFs { get; }
        bool AnyVideos { get; }
        bool AnyUnknownMedia { get; }   // v2 remove?
    }
}
using System;


namespace TwitterStatistics
{
    public interface IStatistics
    {
        // params
        int ValuesTracked { get; }
        int RankingsTracked { get; }


        // data
        int Total { get; }
        DateTime? Begin { get; }
        DateTime? End { get; }
        int Hashtags { get; }
        int Media { get; }
        int Emojis { get; }
        int Urls { get; }
        int Videos { get; }
        int Images { get; }
        int GIFs { get; }

        // top
        Top TopEmojis { get; }
        Top TopHashtags { get; }
        Top TopDomains { get; }



        // methods
        void Start();
        void StampTime();
        void Track(ITweet tweet);
        string ToJSON();
        void Reset();
}
}

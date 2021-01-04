using System;
using System.Collections.Generic;

namespace TwitterStatistics
{
    public interface IViewModel
    {
        // data
        int Total { get; }
        DateTime? Begin { get; }
        DateTime? End { get; }
        int Emojis { get; }
        int Hashtags { get; }
        int Media { get; }
        int Urls { get; }
        int Videos { get; }
        int Images { get; }
        int GIFs { get; }

        // top
        IEnumerable<string> TopEmojis { get; }
        IEnumerable<string> TopHashtags { get; }
        IEnumerable<string> TopDomains { get; }
    }
}

using System.Collections.Generic;


namespace TwitterStatistics
{
    public class TweetMedia
    {
        public enum MediaType { Unknown, Image, GIF, Video };
        public string Key { get; private set; }
        public MediaType Kind { get; private set; }

        public bool IsImage { get { return Kind == MediaType.Image; } }
        public bool IsGIF { get { return Kind == MediaType.GIF; } }
        public bool IsVideo { get { return Kind == MediaType.Video; } }
        public bool IsUnknown { get { return Kind == MediaType.Unknown; } }


        public TweetMedia(TwitterMediaDTO dto) : this(dto.MediaKey, TweetMedia.Parse(dto))
        {
        }

        public TweetMedia(string key, MediaType kind)
        {
            Key = key;
            Kind = kind;
        }

        public static List<TweetMedia> From(IList<TwitterMediaDTO> dtos)
        {
            if (dtos == null)
                return null;
            
            var list = new List<TweetMedia>();

            foreach (var dto in dtos)
                list.Add(new TweetMedia(dto));

            return list;
        }

        private static MediaType Parse(TwitterMediaDTO dto)
        {
            // v2 photo vs image?
            if (dto.IsPhoto())
                return MediaType.Image;

            else if (dto.IsGIF())
                return MediaType.GIF;

            else if (dto.IsVideo())
                return MediaType.Video;

            else
                return MediaType.Unknown;
        }
    }
}

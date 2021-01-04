using Newtonsoft.Json;


namespace TwitterStatistics
{
    public class TwitterMediaDTO
    {
        // v2: explore refactoring these two fields into a TwitterAPIMediaDTO
        [JsonProperty("Media_key")]
        public string MediaKey { get; private set; }
        public string Type { get; private set; }




        public TwitterMediaDTO(string key, string type)
        {
            MediaKey = key;
            Type = type;
        }


        public static TwitterMediaDTO Deserialize(string json)
        {
            // want to parse at least id and text out
            return JsonConvert.DeserializeObject<TwitterMediaDTO>(json);
        }

        // Is there a difference between image and photo?  I think so, but twitter docs seem to use terms interchangeably
        public bool IsPhoto()
        {
            return Type == "photo";
        }

        public bool IsGIF()
        {
            return Type == "animated_gif";
        }
        public bool IsVideo()
        {
            return Type == "video";
        }
    }
}

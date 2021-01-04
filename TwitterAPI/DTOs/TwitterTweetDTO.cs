using Newtonsoft.Json;


namespace TwitterStatistics
{
    public class TwitterTweetDTO
    {
        public TwitterIncludesDTO Includes { get; private set; }
        private TwitterDataDTO Data;
        

        public TwitterTweetDTO(TwitterDataDTO data, TwitterIncludesDTO includes)
        {
            Data = data;
            Includes = includes;
        }

        // Facade design pattern to hide complexity of Twitter's tweet object
        public string Id { get { return Data?.Id; } }
        public string Text { get { return Data?.Text; } }


        public static TwitterTweetDTO Deserialize(string json)
        {
            // want to parse at least id and text out
            return JsonConvert.DeserializeObject<TwitterTweetDTO>(json);
        }
    }
}

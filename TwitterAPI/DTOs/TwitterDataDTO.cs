using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace TwitterStatistics
{
    public class TwitterDataDTO
    {
        public const int MaxTextSize = 280;             // characters

        public string Id { get; private set; }       // Twitter defines this as a string even though field appears numeric only
        public string Text { get; private set; }
        public JObject Entities { get; private set; }



        public TwitterDataDTO(string id, string text, JObject entities = null)
        {
            // v2 prevent null strings
            Id = id;
            Text = text;
            Entities = entities;
        }

        public static TwitterDataDTO Deserialize(string json)
        {
            // want to parse at least id and text out
            return JsonConvert.DeserializeObject<TwitterDataDTO>(json);
        }
    }
}

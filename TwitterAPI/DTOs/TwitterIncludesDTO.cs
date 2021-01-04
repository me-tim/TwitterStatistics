using Newtonsoft.Json;
using System.Collections.Generic;


namespace TwitterStatistics
{
    public class TwitterIncludesDTO
    {
        public List<TwitterMediaDTO> Media { get; private set; }       // Twitter defines this as a string even though field appears numeric only
        public bool AnyMedia { get { return Media?.Count > 0; } }

        public TwitterIncludesDTO(List<TwitterMediaDTO> media)
        {
            Media = media;
        }

        public static TwitterIncludesDTO Deserialize(string json)
        {
            // want to parse at least id and text out
            return JsonConvert.DeserializeObject<TwitterIncludesDTO>(json);
        }
    }
}

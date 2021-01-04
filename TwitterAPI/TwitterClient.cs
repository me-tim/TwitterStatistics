using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TwitterStatistics
{
    public class TwitterClient : HttpClient, ITwitterClient
    {
        // SET YOUR TWITTER TOKEN HERE
        private const string bearerToken = @"YOURTOKENHERE";
        private const string apiBase = @"https://api.twitter.com/2/tweets/";
        private const string pathStream = @"sample/stream?expansions=attachments.media_keys&media.fields=url";
        // expansions=entities.urls not publiclyl available yet despite being in API, this would simply getting URLs

        private static ITwitterClient client;


        // singleton pattern
        public static ITwitterClient GetSingleton()
        {
            if (client != null)
                return client;

            client = new TwitterClient();
            return client;
        }

        public ITwitterClient GetClient()
        {
            return this;
        }

        private TwitterClient()
        {
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        public async Task<Stream> GetStreamAsync()
        {
            return await base.GetStreamAsync(GetUrl(pathStream));
        }

        private string GetUrl(string path)
        {
            var apiUrl = Path.Combine(apiBase, path);

            if (!apiUrl.StartsWith(apiBase))
                throw new Exception("Invalid twitter api url " + apiUrl);

            return apiUrl;
        }
    }
}

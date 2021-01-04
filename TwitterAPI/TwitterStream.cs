using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TwitterStatistics
{
    public class TwitterStream : ITwitterStream
    {
        public bool IsStop { get; private set; }
        public int Count { get { return stats.Total; } }
        public bool Connected { get; set; }
        private readonly Func<ITwitterClient> clientFactory;     // for dependency injection

        private ViewModel vm;
        private IStatistics stats;

        // internal diagnostics
        private bool anyNullTweets;
        private bool anyExceptions;


        public TwitterStream(IStatistics _stats = null, Func<ITwitterClient> _clientFactory = null)
        {
            stats = (_stats != null) ? _stats : new Statistics();
            vm = new ViewModel(stats);
            clientFactory = (_clientFactory != null) ? _clientFactory : TwitterClient.GetSingleton;   // dependency injection
        }

        private async IAsyncEnumerable<string> GetSampleStream()
        {
            using (var client = clientFactory())
            {
                using (var apiStream = await client.GetStreamAsync())
                using (var reader = new StreamReader(apiStream))
                {
                    Connected = true;
                    while (!reader.EndOfStream && !IsStop)
                        yield return await reader.ReadLineAsync();
                }
            }

            Connected = false;
        }

        public async Task Start()
        {
            // if we restarted, reset the stats, as uptime and since will be out of sync with counts
            StartClock();

            try
            {
                await foreach (var line in GetSampleStream())
                {
                    // v2 continue instead
                    // twitter appears to send empty lines occassionally
                    if (line == string.Empty)
                        continue;

                    // want to parse at least id and text out
                    var tweetDTO = TwitterTweetDTO.Deserialize(line);
                    if (tweetDTO == null) 
                    {
                        anyNullTweets = true;
                        continue;
                    }

                    var tweet = new Tweet(tweetDTO);
                    // v2 is it better if TweetStream knows how to track stats or if TweetStatistics does?
                    stats.Track(tweet);
                }
            }
            catch(Exception ex)
            {
                anyExceptions = true;
            }
        }

        public void Stop()
        {
            IsStop = true;
        }

        public void StartClock()
        {
            IsStop = false;

            // if we restarted, reset the stats, as uptime and since will be out of sync with counts
            stats.Start();
        }

        public string ToJson()
        {
            return vm.ToJSON();
        }
    }
}

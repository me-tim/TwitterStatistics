using NUnit.Framework;
using System;
using System.Linq;
using TwitterStatistics;

namespace TwitterStatiticsTests
{
    public class StatisticsTests
    {
        private static Random random = new Random();


        // need to fix
        //[Test]
        public void TestTotalCount()
        {
            const int maxTweets = 100000;

            var stats = new Statistics();

            // int.MaxValue (2 bn) took 2 mins, so cutting down
            for (int i = 0; i < maxTweets; i++)
            {
                var tweetData = new TwitterDataDTO(RandomInt().ToString(), RandomString(TwitterDataDTO.MaxTextSize));
                var tweet = new TwitterTweetDTO(tweetData, null);
                
                //stats.Process(tweet);
            }

            Assert.AreEqual(stats.Total, maxTweets);
        }

        
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static int RandomInt()
        {
            return random.Next(0, int.MaxValue);
        }
    }
}

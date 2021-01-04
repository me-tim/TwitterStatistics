using NUnit.Framework;
using TwitterStatistics;

namespace TwitterStatiticsTests
{
    public class UrlParserTests
    {
        private const string id = "asjkl080sdfss";
        private const string urlAtEnd = "We believe the best future version of our API will come from building it with YOU.Here’s to another great year with everyone who builds on the Twitter platform.We can’t wait to continue working with you in the new year.https://t.co/yvxdK6aOo2";
        private const string truncatedUrl = @"rt @nicogonza_988: te gusta cómo la cómo? 😈💦\nsuscríbete y mira el vídeo completo en mi only.\ncontenido exclusivo y personalizado ⚡️\n\nhttps…";
        private const string spaceUrl = @"atualização de acessos no http ://revolutions.pro/ad/15681/1068678 meu perfil""}";
        private const string usernameUrl = @"sdf httpsja_:asd dfd";
        private const string doubleProtocol = @"sdf https:https://t.co/swrdqblpoq dfd";
        private const string tag = @"@httpsponton https://t.co/O8csc7CGLC";


        private const string entityTweet = @"
            {
                ""id"": ""1212092628029698048"",
                ""text"": ""We believe the best future version of our API will come from building it with YOU. Here’s to another great year with everyone who builds on the Twitter platform. We can’t wait to continue working with you in the new year. https://t.co/yvxdK6aOo2"",                
                ""possibly_sensitive"": false,
                ""referenced_tweets"": [
                    {
                        ""type"": ""replied_to"",
                        ""id"": ""1212092627178287104""
                    }
                ],
                ""entities"": {
                    ""urls"": [
                        {
                            ""start"": 222,
                            ""end"": 245,
                            ""url"": ""https://t.co/yvxdK6aOo2"",
                            ""expanded_url"": ""https://twitter.com/LovesNandos/status/1211797914437259264/photo/1"",
                            ""display_url"": ""pic.twitter.com/yvxdK6aOo2""
                        },
                        {
                            ""start"": 3345435,
                            ""end"": 345345345345,
                            ""url"": ""https://t.co/adsf"",
                            ""expanded_url"": ""https://twitter.com/asdf/asdf/sdfs/sdf"",
                            ""display_url"": ""pic.twitter.com/sdfsdf""
                        }

                    ]
                }
            }
            ";


        // Not avaiable in twitter v2 api yet
        [Test]
        public void ParseEntityUrls()
        {
            var tweet = new Tweet(id, entityTweet, null);

            Assert.AreEqual(1, tweet.Uris.Count);
            Assert.AreEqual("https://t.co/yvxdK6aOo2", tweet.Uris[0].ToString());
        }

        /// <summary>
        /// Not avaiable in twitter v2 api yet
        /// </summary>
        [TestCase(urlAtEnd)]
        public void TestValidUrls(string textWithUrl)
        {
            var tweet = new Tweet(id, textWithUrl, null);

            Assert.AreEqual(1, tweet.Uris.Count);
            Assert.AreEqual("https://t.co/yvxdK6aOo2", tweet.Uris[0].ToString());
        }


        [TestCase(truncatedUrl)]
        [TestCase(usernameUrl)]
        [TestCase(spaceUrl)]
        [TestCase(doubleProtocol)]
        [TestCase(tag)]
        public void TestInvalidUrls(string textWithUrl)
        {
            var tweet = new Tweet(id, textWithUrl, null);

            Assert.False(tweet.AnyUris);
            var none = tweet.Uris == null || tweet.Uris.Count == 0;
            Assert.True(none);
        }
    }
}

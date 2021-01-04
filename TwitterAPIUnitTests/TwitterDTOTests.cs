using NUnit.Framework;
using TwitterStatistics;

namespace TwitterStatiticsTests
{
    public class TwitterDTOTests
    {
        private const string simpleTweet = @"{""id"":""20"",""text"":""just setting up my twttr""}";
        private const string complexTweet = @"{""id"":""asdadfsdsdf f3456@$%$^"",""text"":""rt @nicogonza_988: te gusta cómo la cómo? 😈💦\nsuscríbete y mira el vídeo https://t.co/gzSoZqPmDM completo en mi only.\ncontenido exclusivo y personalizado ⚡️\n\nhttps…""}";
        private const string mediaTweet = @"{
          ""data"": {
            ""id"": ""440322224407314432"",
            ""text"": ""If only Bradley's arm was longer. Best photo ever. #oscars http://t.co/C9U5NOtGap"",
            ""attachments"": {
              ""media_keys"": [
                ""3_440322224092745728""
              ]
            }
        },
          ""includes"": {
            ""media"": [
              {
                ""media_key"": ""3_440322224092745728"",
                ""type"": ""photo""
              }
            ]
          }
        }";

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


        [Test]
        public void DeserializeSimple()
        {
            var tweetAct = TwitterDataDTO.Deserialize(simpleTweet);

            Assert.AreEqual("20", tweetAct.Id);
            Assert.AreEqual("just setting up my twttr", tweetAct.Text);
            Assert.AreEqual(null, tweetAct.Entities);
        }

        [Test]
        public void DeserializeComplex()
        {
            var tweetAct = TwitterDataDTO.Deserialize(complexTweet);

            // v2 more DRY
            Assert.AreEqual("asdadfsdsdf f3456@$%$^", tweetAct.Id);
            Assert.AreEqual("rt @nicogonza_988: te gusta cómo la cómo? 😈💦\nsuscríbete y mira el vídeo https://t.co/gzSoZqPmDM completo en mi only.\ncontenido exclusivo y personalizado ⚡️\n\nhttps…", tweetAct.Text);
        }


        [Test]
        public void Deserialize()
        {
            var tweetAct = TwitterTweetDTO.Deserialize(mediaTweet);

            Assert.True(tweetAct.Includes.AnyMedia);
            Assert.AreEqual(1, tweetAct.Includes.Media.Count);
            Assert.AreEqual("440322224407314432", tweetAct.Id);
            Assert.AreEqual("If only Bradley's arm was longer. Best photo ever. #oscars http://t.co/C9U5NOtGap", tweetAct.Text);
        }

        /// <summary>
        /// Not avaiable in twitter v2 api yet
        /// </summary>
        //[Test]
        public void ParseEntityUrls()
        {
            var tweetAct = TwitterDataDTO.Deserialize(entityTweet);

            // v2 resurrect this test
            //Assert.AreEqual(2, tweetAct.Entities?.Urls?.Count);
        }
    }
}

using Moq;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using TwitterStatistics;

namespace TwitterStatiticsTests
{
    public class TwitterStreamTests
    {
        private Mock<IStatistics> stats = new Mock<IStatistics>();
        private Mock<ITwitterClient> client = new Mock<ITwitterClient>();

        private const string json = @"{ ""data"": {""id"": ""440322224407314432"",""text"": ""If only Bradley's arm was longer. Best photo ever. #oscars http://t.co/C9U5NOtGap""}}";
        private Stream stream = new MemoryStream(BrowserStream.GetBuffer(json).ToArray());


        [Test]
        public async Task VerifySampleStream()
        {
            client.Setup(c => c.GetClient()).Returns(client.Object);
            client.Setup(c => c.GetStreamAsync()).Returns(Task.FromResult(stream));

            var app = new TwitterStream(stats.Object, client.Object.GetClient);

            await app.Start();

            stats.Verify(x => x.Track(It.IsAny<ITweet>()), Times.Once());
        }
    }
}

using Moq;
using NUnit.Framework;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using TwitterStatistics;

namespace TwitterStatisticsTests
{
    public class BrowserStreamTest
    {
        private const string testMessage = "some message";

        private Mock<ITwitterStream> twitter = new Mock<ITwitterStream>();
        private Mock<WebSocket> socket = new Mock<WebSocket>();             // v2 ->IWebSocket
        private const WebSocketMessageType messageType = WebSocketMessageType.Text;


        [Test]
        public async Task VerifyMessageSent()
        {
            var userNotCanceled = new WebSocketReceiveResult(0, messageType, true);
            var task = Task.FromResult(userNotCanceled);

            socket.Setup(m => m.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), CancellationToken.None))
                .Returns(task);

            twitter.Setup(t => t.ToJson()).Returns(testMessage);

            // benefit of dependency injection here as otherwise would actually hit twitter api and could cause rate limiting problems
            var app = new BrowserStream(twitter.Object);

            app.Process(socket.Object);

            socket.Verify(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(), messageType, true, CancellationToken.None), 
                Times.Once());
        }
    }
}

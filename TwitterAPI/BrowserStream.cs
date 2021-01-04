using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TwitterStatistics
{
    public class BrowserStream : IBrowserStream
    {
        private const int refreshMS = 1000;

        private ITwitterStream twitter;
        private WebSocket webSocket;

        public BrowserStream(ITwitterStream _twitter)
        {
            twitter = _twitter;
        }

        public async Task Process(WebSocket socket)
        {
            webSocket = socket;
            Task sendTask = Task.CompletedTask;

            var buffer = new byte[1024 * 4];
            Task<WebSocketReceiveResult> receiveTask = webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var processTask = twitter.Start();

            var lastTweetCount = -1;
            while (!HasUserCanceled(receiveTask))
            {
                // want both running in parallel
                // do not want to stack up sendsrepeat until previous send is complete
                if (sendTask.IsCompleted)
                {
                    // v2 use Observerable pattern to push updates, rather than pulling total
                    var curTweetCount = twitter.Count;
                    if (curTweetCount != lastTweetCount)
                    {
                        var data = twitter.ToJson();

                        sendTask = webSocket.SendAsync(GetBuffer(data), WebSocketMessageType.Text, true, CancellationToken.None);
                        lastTweetCount = curTweetCount;
                    }
                }

                // free thread to do other things
                await Task.Delay(refreshMS);
            }

            // stop processing gracefully
            twitter.Stop();

            var received = await receiveTask;   // should be done            
            await processTask;
            await sendTask;

            await webSocket.CloseAsync(received.CloseStatus.Value, received.CloseStatusDescription, CancellationToken.None);
        }

        public static ArraySegment<Byte> GetBuffer(string data)
        {
            var encoded = Encoding.UTF8.GetBytes(data);
            return new ArraySegment<Byte>(encoded, 0, encoded.Length);
        }

        private bool HasUserCanceled(Task<WebSocketReceiveResult> receiveTask)
        {
            if (!receiveTask.IsCompleted)
                return false;

            return receiveTask.Result.CloseStatus.HasValue;
        }
    }
}

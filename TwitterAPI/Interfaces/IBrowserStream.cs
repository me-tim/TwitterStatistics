using System.Net.WebSockets;
using System.Threading.Tasks;

namespace TwitterStatistics
{
    public interface IBrowserStream
    {
        Task Process(WebSocket socket);
    }
}

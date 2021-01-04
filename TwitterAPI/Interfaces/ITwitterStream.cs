using System.Threading.Tasks;

namespace TwitterStatistics
{
    public interface ITwitterStream
    {
        Task Start();
        string ToJson();
        void Stop();

        int Count { get; }
        bool IsStop { get; }
    }
}

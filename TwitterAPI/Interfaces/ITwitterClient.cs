using System.Threading.Tasks;
using System.IO;
using System;

namespace TwitterStatistics
{
    public interface ITwitterClient : IDisposable
    {
        ITwitterClient GetClient();
        Task<Stream> GetStreamAsync();
    }
}

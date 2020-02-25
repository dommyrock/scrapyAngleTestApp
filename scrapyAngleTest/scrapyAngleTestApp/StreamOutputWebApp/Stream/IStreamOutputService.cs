using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StreamOutputWebApp.Stream
{
    public interface IStreamOutputService
    {
        List<string> ListStreams();

        Task ExecuteStreamAsync(string name, IAsyncEnumerable<string> stream);

        IAsyncEnumerable<string> Subscribe(string name, CancellationToken token);

        //void DisconnectStream(string name);
    }
}
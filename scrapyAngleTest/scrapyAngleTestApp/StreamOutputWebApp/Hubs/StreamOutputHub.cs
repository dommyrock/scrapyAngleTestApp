using Microsoft.AspNetCore.SignalR;
using StreamOutputWebApp.Stream;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StreamOutputWebApp.Hubs
{
    public class StreamOutputHub : Hub
    {
        readonly IStreamOutputService _streamOutputService;

        public StreamOutputHub(IStreamOutputService streamOutputService)
        {
            _streamOutputService = streamOutputService;
        }

        //TODO: call this hub methods on the client side ... and feed data from pipe into this hub
        //use SignalR30SensorWebApplication --> sensorClient --> program.. as example
        public List<string> ListStreams() => _streamOutputService.ListStreams();

        public async Task PublishStreamData(string name, IAsyncEnumerable<string> stream)
        {
            try
            {
                var executeStreamTask =
                    _streamOutputService.ExecuteStreamAsync(name, stream);

                await Clients.Others.SendAsync("StreamCreated", name);
                await executeStreamTask;
            }
            finally
            {
                await Clients.All.SendAsync("StreamRemoved", name);
            }
        }

        public IAsyncEnumerable<string> WatchStream(
            string name,
            CancellationToken token) =>
            _streamOutputService.Subscribe(name, token);
    }
}

//3. if i want message pack protocol add
//import { MessagePackHubProtocol } from '@aspnet/signalr-protocol-msgpack'; + .withHubProtocol(new MessagePackHubProtocol()) to client side hub config
///for hooks hub setup  <see cref="https://fulgenc.io/react-typescript-with-signalr-chat-app-pt-1/"/>
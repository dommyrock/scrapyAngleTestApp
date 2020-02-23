using Microsoft.AspNetCore.SignalR;
using StreamOutputWebApp.Stream;

namespace StreamOutputWebApp.Hubs
{
    public class StreamOutputHub : Hub
    {
        private readonly StreamOutput _streamOutput;

        public StreamOutputHub(StreamOutput streamOutput)
        {
            _streamOutput = streamOutput;
        }

        //TODO: declare methods that will be called on client side JS(through hubConnection)
        // 2. implement client side hub -->in home component instead...[example-->IEvangelist.SignalRStreaming -home.component]
        //3. if i want message pack protocol add
        //import { MessagePackHubProtocol } from '@aspnet/signalr-protocol-msgpack'; + .withHubProtocol(new MessagePackHubProtocol()) to client side hub config
        ///for hooks hub setup  <see cref="https://fulgenc.io/react-typescript-with-signalr-chat-app-pt-1/"/>
    }
}
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
    }
}
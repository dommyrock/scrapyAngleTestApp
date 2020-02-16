using Microsoft.AspNetCore.SignalR;
using StreamOutputWebApp.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamOutputWebApp.Stream
{
    //like "SensorCollection" in signalr30sensordemo (only diff is i have interface that will contain its method signatures)
    public class StreamOutput : IStreamOutput
    {
        //add concurrent dict or other thread safe collection
        private readonly IHubContext<StreamOutputHub> _streamOutputHubContext;

        public StreamOutput(IHubContext<StreamOutputHub> streamOutputHubContext)
        {
            _streamOutputHubContext = streamOutputHubContext;
        }
    }
}
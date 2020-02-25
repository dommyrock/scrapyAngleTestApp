using Microsoft.AspNetCore.SignalR;
using StreamOutputWebApp.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace StreamOutputWebApp.Stream
{
    //like "SensorCollection" in signalr30sensordemo (only diff is i have interface that will contain its method signatures)
    public class StreamOutputService : IStreamOutputService
    {
        private readonly ConcurrentDictionary<string, StreamReference> _streams = new ConcurrentDictionary<string, StreamReference>();
        long _globalClientId;

        //Example from .net core graph demo---- im using dependenci injection in "StreamOutputHub" so i dont need it here !
        //add concurrent dict or other thread safe collection
        //private readonly IHubContext<StreamOutputHub> _streamOutputHubContext;

        //public StreamOutputService(IHubContext<StreamOutputHub> streamOutputHubContext)
        //{
        //    _streamOutputHubContext = streamOutputHubContext;
        //}

        public List<string> ListStreams() => _streams.Keys.ToList();// same as return

        public async Task ExecuteStreamAsync(string name, IAsyncEnumerable<string> stream)
        {
            var streamReference = new StreamReference(stream);
            _streams.TryAdd(name, streamReference);

            await Task.Yield();

            try
            {
                await foreach (var item in stream)
                {
                    foreach (var client in streamReference.Clients)
                    {
                        try
                        {
                            await client.Value.Writer.WriteAsync(item);
                        }
                        catch { }
                    }
                }
            }
            finally
            {
                DisconnectStream(name);
            }

            //Example from graph demo (used ievangelist example instead

            //var subscriberQueue = _streams.GetOrAdd(name, _ =>
            //{
            //    // This could be called multiple times for the same sensor, but the client will dedupe.
            //    _streamOutputHubContext.Clients.All.SendAsync("SensorAdded", name);

            //    return new ConcurrentQueue<Channel<string>>();
            //});

            //foreach (var subscriber in subscriberQueue)
            //{
            //    Trace.Assert(subscriber.Writer.TryWrite(stream));
            //}
        }

        public void DisconnectStream(string name)
        {
            if (_streams.TryRemove(name, out var streamReference))
            {
                foreach (var subscriber in streamReference.Clients)
                {
                    subscriber.Value.Writer.Complete();
                }
            }
        }

        public IAsyncEnumerable<string> Subscribe(string name, CancellationToken token = default)
        {
            if (!_streams.TryGetValue(name, out var source))
            {
                throw new HubException($"The '{name}' stream doesn't exist.");
            }

            var id = Interlocked.Increment(ref _globalClientId);
            var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(2)
            {
                FullMode = BoundedChannelFullMode.DropOldest
            });

            source.Clients.TryAdd(id, channel);

            // Register for client closing stream, this token
            // will always fire (handled by SignalR).
            token.Register(() => source.Clients.TryRemove(id, out _));

            return channel.Reader.ReadAllAsync();
        }
    }
}
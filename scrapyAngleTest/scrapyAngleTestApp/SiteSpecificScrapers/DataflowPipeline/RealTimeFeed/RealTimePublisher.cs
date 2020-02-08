using SiteSpecificScrapers.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiteSpecificScrapers.DataflowPipeline.RealTimeFeed
{
    public class RealTimePublisher : IRealTimePublisher
    {
        public async Task PublishAsync(ProcessedMessage message)
        {
            // send over a network socket
            Console.WriteLine($"Publish in real-time message {message.SourceHtml} on thread {Thread.CurrentThread.ManagedThreadId}");
            await Task.Yield();
        }
    }
}
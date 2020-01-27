using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiteSpecificScrapers.DataflowPipeline.RealTimeFeed
{
    public class RealTimePublisher:IRealTimePublisher
    {
        /// <summary>
        /// Publish output to console in real time .
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task PublishAsync(string message)
        {
            // send over a network socket
            if (ShowMessages.PrintRealTimeFeed)
                Console.WriteLine($"            Publish in real-time message Sensor: { message.Label} { message.Value} { message.Unit} on thread {Thread.CurrentThread.ManagedThreadId}");
            await Task.Yield();
        }
    }
}

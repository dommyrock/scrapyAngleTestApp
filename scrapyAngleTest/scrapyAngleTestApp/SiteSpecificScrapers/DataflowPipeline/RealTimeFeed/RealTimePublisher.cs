using SiteSpecificScrapers.Messages;
using System;
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

//Decode function in StreamProcessing ->
//yield return Decode(reading, sensorConfig, decodeCounter);//next time the itteration is started , we continue from last element we returned(and dont return previous elements again !!)
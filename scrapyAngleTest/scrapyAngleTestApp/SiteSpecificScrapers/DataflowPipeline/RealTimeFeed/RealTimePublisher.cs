using SiteSpecificScrapers.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SiteSpecificScrapers.DataflowPipeline.RealTimeFeed
{
    public class RealTimePublisher : IRealTimePublisher
    {
        // TODO: publish data from this method -->  to signalr publish method as input
        public void PublishAsync(Message message)
        {
            // send over a network socket
            Console.WriteLine($"Publish in real-time message {message.SourceHtml} on thread {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}

//1. Dont need async method (SINCE I ONLY NEED TO PUBLISH DATA TO SIGNALR METHOD FORM HERE)
//2. async method -->  await Task.Yield(); do not rely on await Task.Yield(); to keep a UI responsive
//return Task.CompletedTask; also not needed it as stated in ( 1.)

//Decode function in StreamProcessing ->
//yield return Decode(reading, sensorConfig, decodeCounter);//next time the itteration is started , we continue from last element we returned(and dont return previous elements again !!)
using SiteSpecificScrapers.Interfaces;
using SiteSpecificScrapers.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SiteSpecificScrapers.DataflowPipeline
{
    public class DataConsumer : IDataConsumer
    {
        public DataConsumer()
        {
        }

        public Task StartConsuming(ITargetBlock<Message> target, CancellationToken token)
        {
            return Task.Factory.StartNew(() => ConsumeWithDiscard(target, token));
        }

        //TODO :rest of logic
        private void ConsumeWithDiscard(ITargetBlock<Message> target, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Console.WriteLine($"Read message on thread {Thread.CurrentThread.ManagedThreadId}");
            }
        }
    }
}
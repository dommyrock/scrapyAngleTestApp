using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CoreConsole.DataflowExamples
{
    public static class LinkToOptionsMaxMessages
    {
        public async static Task Run()
        {
            var bufferBlock = new BufferBlock<int>(new DataflowBlockOptions() { BoundedCapacity = 1 });

            var ab1 = new ActionBlock<int>(a =>
            {
                Console.WriteLine($"Message {a} was processed by Consumer 1");
            });
            var ab2 = new ActionBlock<int>(a =>
            {
                Console.WriteLine($"Message {a} was processed by Consumer 2");
            });

            //Connect producer to consumers (ISourceBlock,ITargetBlock)
            bufferBlock.LinkTo(ab1);

            //Prepend 2nd consumer 1st in the list of consumers
            //Max messages accepted by 1st consumer in the list (int his case its consumer 2)
            bufferBlock.LinkTo(ab2, new DataflowLinkOptions() { Append = false, MaxMessages = 5 });

            for (int i = 0; i < 10; i++)
            {
                await bufferBlock.SendAsync(i);
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
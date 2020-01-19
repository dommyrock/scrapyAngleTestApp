using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CoreConsole.DataflowExamples
{
    public static class LinkToOptionsAppend
    {
        public async static Task Run()
        {
            var bufferBlock = new BufferBlock<int>();

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
            bufferBlock.LinkTo(ab2, new DataflowLinkOptions() { Append = false }); //prepend 2nd consumer 1st in the list of consumers

            for (int i = 0; i < 10; i++)
            {
                await bufferBlock.SendAsync(i);
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
using System;
using System.Threading.Tasks.Dataflow;
using System.Threading.Tasks;

namespace CoreConsole.DataflowExamples
{
    public static class LinkToOptionsMessageFiltering
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

            //Consumer 1 should only consume even integers
            //Done by providing a delegate returning a boolean value if msg should be propesed !

            // **** BE CAREFULL WHEN FILTERING ...IF NONE OF TARGET BLOCK DOESNT MEET THE CONDITION TPL DF WILL AWAIT INDEFINETLY FOR SOMEONE TO TAKE THE MESSAGE !!!!
            bufferBlock.LinkTo(ab1, a => a % 2 == 0);

            //Prepend 2nd consumer 1st in the list of consumers
            //Max messages accepted by 1st consumer in the list (int his case its consumer 2)
            bufferBlock.LinkTo(ab2, new DataflowLinkOptions() { Append = false, MaxMessages = 5 });

            //Solution1 !
            //bufferBlock.LinkTo(DataflowBlock.NullTarget<int>()); //It accepts and discards everything that it receives

            //Solution2 !
            //(****BETTER WAY IS TO ADD DEFAULT FALLBACK ACTION BLOCK***) -->error tracing
            bufferBlock.LinkTo(new ActionBlock<int>(a => Console.WriteLine($"Message {a} was DISCARDED!!!"))); //It accepts and discards everything that it receives

            for (int i = 0; i < 10; i++)
            {
                await bufferBlock.SendAsync(i);
            }

            Console.ReadKey();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace CoreConsole.DataflowExamples
{
    public static class WriteOnceBlock
    {
        public static void RunWorkflow()
        {
            //since writeonce discarted all the msgs except the 1st one(reason only  1 msg in WF)
            //TPL dataflow is PUSH NOT PULL Arhitecture (consumer doesnt ask for message's they are being offered to!)
            var block = new WriteOnceBlock<int>(a => a);
            var print = new ActionBlock<int>(a => Console.WriteLine($"Message{a} was received."));

            block.LinkTo(print);
            for (int i = 0; i < 10; i++)
            {
                if (block.Post(i))
                {
                    Console.WriteLine($"Message {i} was accepted!");
                }
                else
                {
                    Console.WriteLine($"Message {i} was rejected!");
                }
            }

            //Try receive more msgs than was sent to the block
            for (int i = 0; i < 15; i++)
            {
                if (block.TryReceive(out var ret))
                {
                    Console.WriteLine($"Message {ret} was receiverd. Iteration{i}");
                }
                else
                {
                    Console.WriteLine("No more messages!");
                }
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }

        public static void Run()
        {
            //requires type and cloning function
            //only clones 1st message and returns it (THATS WHY "Message 0" is repeated in output console)
            var block = new WriteOnceBlock<int>(a => a);

            for (int i = 0; i < 10; i++)
            {
                if (block.Post(i))
                {
                    Console.WriteLine($"Message {i} was accepted!");
                }
                else
                {
                    Console.WriteLine($"Message {i} was rejected!");
                }
            }

            //Try receive more msgs than was sent to the block
            for (int i = 0; i < 15; i++)
            {
                if (block.TryReceive(out var ret))
                {
                    Console.WriteLine($"Message {ret} was receiverd. Iteration{i}");
                }
                else
                {
                    Console.WriteLine("No more messages!");
                }
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
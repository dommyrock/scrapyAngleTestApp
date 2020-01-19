using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CoreConsole.DataflowExamples
{
    public static class ErrorHandling
    {
        //** Errors pop up unly on block completion ....so if we never complete the block we will NEVER SEE EXCEPTION !!!

        //*****When exception is thrown the block removes all messages from its queue and goes to state == FAULTED STATE***************************************
        public async static Task Run()
        {
            var block = new TransformBlock<int, string>(n =>
             {
                 if (n == 5)
                     throw new Exception("Something went wrong");

                 Console.WriteLine($"Message {n} processed");
                 return n.ToString();
             });

            //Print msgs frmom TransfomBlock<>
            var printBlock = new ActionBlock<string>(a => Console.WriteLine($"Message {a} was processed!"));
            block.LinkTo(printBlock, new DataflowLinkOptions() { PropagateCompletion = true }); //** PROPAGATING COMPLETIONS ALSO MEANS PROPAGATING ERRORS !!!!

            for (int i = 0; i < 10; i++)
            {
                if (block.Post(i))
                    Console.WriteLine($"Message {i} was accepted!");
                else
                    Console.WriteLine($"Message {i} was rejected!");
            }

            block.Complete();
            try
            {
                await printBlock.Completion;
            }
            // EXCEPTIONS IN TPL ARE WRAPPED IN AGGREGATE EXCEPTION !!
            catch (AggregateException ae)
            {
                //flatten structure in single aggregate exception (because it added extra layer when msg was passed from block)
                //throw ae.Flatten().InnerExceptions;..read only
            }
            //Check if msgs are in input queue
            Console.WriteLine($"Input queue size: {block.InputCount}");

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CoreConsole.DataflowExamples
{
    public static class BatchedJoinBlock
    {
        //If im optimizing from Msg COUNT over msg order
        //If we recieve data from mulitple sourcers and need to group them into batches WITHOUT need to mantain MSG order

        //Trigger for releasing msgs is TOTAL NUMBER OF MSG'S IN INPUT QUEUE'S

            //NUMBER OF MSGS VARIES ....DEPENDS ON MSGS IN EACH BRANCH , SOMETIMES IT MIXES THEM  SO WE SHOULDNT RELY ON THIS BLOCK TYPE FOR CONSISTENCY !! 

        public static void RunParallel()
        {
            // TPL Broadcast block --- by DEFAULT keeps the msg ORDER (despite branches being slower)!

            var broadcastBlock = new BroadcastBlock<int>(a => a);

            var tb1 = new TransformBlock<int, int>(a =>
            {
                //process even numbers slower
                Console.WriteLine($"Message {a} was processed by Consumer 1");
                if (a % 2 == 0)
                {
                    Task.Delay(300).Wait();
                }
                else
                {
                    Task.Delay(50).Wait();
                }
                return -1 * a;
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 3 });//set max cap at 1 msg(to load balance consumers)
            var tb2 = new TransformBlock<int, int>(a =>
            {
                Console.WriteLine($"Message {a} was processed by Consumer 2");
                //Process odd numbers slower
                if (a % 2 != 0)
                {
                    Task.Delay(300).Wait();
                }
                else
                {
                    Task.Delay(50).Wait();
                }
                return a;
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 3 });

            //Connect producer to consumers (ISourceBlock,ITargetBlock)
            broadcastBlock.LinkTo(tb1);
            broadcastBlock.LinkTo(tb2);

            var batchedjoinBlock = new BatchedJoinBlock<int, int>(3); //Pass batch size

            //Join block exposes special props for each branch to join(reason for special types for 2,3 branches to join)
            tb1.LinkTo(batchedjoinBlock.Target1);
            tb2.LinkTo(batchedjoinBlock.Target2);

            var printBlock = new ActionBlock<Tuple<IList<int>, IList<int>>>(a => Console.WriteLine($"Message {string.Join(",", a.Item1)}],[{string.Join(",", a.Item2)}] "));
            //Joinblick returns tuple so we have to as well!
            batchedjoinBlock.LinkTo(printBlock);

            for (int i = 0; i < 10; i++)
            {
                broadcastBlock.SendAsync(i)
                    .ContinueWith(a =>
                    {
                        //If msg was accepted
                        if (a.Result)
                        {
                            Console.WriteLine($"Message {i} was accepted");
                        }
                        else
                        {
                            Console.WriteLine($"Message {i} was Rejected!!");
                        }
                    });
            }
            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
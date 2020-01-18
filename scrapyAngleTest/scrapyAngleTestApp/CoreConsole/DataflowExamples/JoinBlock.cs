using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CoreConsole.DataflowExamples
{
    public static class JoinBlock
    {
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

            var joinBlock = new JoinBlock<int, int>();
            //Join block exposes special props for each branch to join(reason for special types for 2,3 branches to join)
            tb1.LinkTo(joinBlock.Target1);
            tb2.LinkTo(joinBlock.Target2);

            //Since 1st branch is negating the sum , sum should ==0
            var printBlock = new ActionBlock<Tuple<int, int>>(a => Console.WriteLine($"Message {a} was processed. [SUM ] = {a.Item2 + a.Item1}"));
            //Joinblick returns tuple so we have to as well!
            joinBlock.LinkTo(printBlock);

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

        public static void Run()
        {
            var broadcastBlock = new BroadcastBlock<int>(a => a);

            var tb1 = new TransformBlock<int, int>(a =>
             {
                 Console.WriteLine($"Message {a} was processed by Consumer 1");
                 Task.Delay(300);
                 return a;
             });//set max cap at 1 msg(to load balance consumers)
            var tb2 = new TransformBlock<int, int>(a =>
             {
                 Console.WriteLine($"Message {a} was processed by Consumer 2");
                 Task.Delay(150);
                 return a;
             });

            //Connect producer to consumers (ISourceBlock,ITargetBlock)
            broadcastBlock.LinkTo(tb1);
            broadcastBlock.LinkTo(tb2);

            var joinBlock = new JoinBlock<int, int>();
            //Join block exposes special props for each branch to join(reason for special types for 2,3 branches to join)
            tb1.LinkTo(joinBlock.Target1);
            tb2.LinkTo(joinBlock.Target2);

            //AB to print msgs from join block
            var printBlock = new ActionBlock<Tuple<int, int>>(a => Console.WriteLine($"Message {a} was processed."));
            //Joinblick returns tuple so we have to as well!
            joinBlock.LinkTo(printBlock);

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
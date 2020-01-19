using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CoreConsole.DataflowExamples
{
    public static class Completion
    {
        //Propagate completion is OFF by default so we need to turn in ON to signal completion
        //We'are awaiting for MESSAGE POSTING  but we ARENT AWAITING ON FUTER PROCESSING (TPL DF IS MESSAGE BASED !)
        // WE NEED TO WAIT FOR LAST BLOCK TO COMPLETE !!!

        public static async Task RunParallel()
        {
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
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 2 });//set max cap at 1 msg(to load balance consumers)
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
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 2 });

            //Connect producer to consumers (ISourceBlock,ITargetBlock)
            broadcastBlock.LinkTo(tb1, new DataflowLinkOptions() { PropagateCompletion = true });
            broadcastBlock.LinkTo(tb2, new DataflowLinkOptions() { PropagateCompletion = true });

            //Join block exposes special props for each branch to join(reason for special types for 2,3 branches to join)
            var joinBlock = new JoinBlock<int, int>();
            tb1.LinkTo(joinBlock.Target1, new DataflowLinkOptions() { PropagateCompletion = true });
            tb2.LinkTo(joinBlock.Target2, new DataflowLinkOptions() { PropagateCompletion = true });

            //Since 1st branch is negating the sum , sum should ==0
            var finalBlock = new ActionBlock<Tuple<int, int>>(a => Console.WriteLine($"Message {a.Item1},{a.Item2} was processed by all consumers"));

            //JoinBlock returns tuple so we have to as well!
            joinBlock.LinkTo(finalBlock, new DataflowLinkOptions() { PropagateCompletion = true });

            for (int i = 0; i < 10; i++)
            {
                await broadcastBlock.SendAsync(i);
            }
            //signal completion
            broadcastBlock.Complete();
            await finalBlock.Completion; //Block completes when it recieves Complete call or when EXCETPION was trown
            //Completed Workflows /Blocks CANT RECEIVE ANY NEW MESSAGES!!
            Console.WriteLine("Finished!");
            Console.ReadKey();
        }

        public static async Task RunParallelExtension()
        {
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
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 2 });//set max cap at 1 msg(to load balance consumers)
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
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 2 });

            //Connect producer to consumers (ISourceBlock,ITargetBlock)
            broadcastBlock.LinkToWithPropagation(tb1);
            broadcastBlock.LinkToWithPropagation(tb2);

            //Join block exposes special props for each branch to join(reason for special types for 2,3 branches to join)
            var joinBlock = new JoinBlock<int, int>();
            tb1.LinkToWithPropagation(joinBlock.Target1);
            tb2.LinkToWithPropagation(joinBlock.Target2);

            //Since 1st branch is negating the sum , sum should ==0
            var finalBlock = new ActionBlock<Tuple<int, int>>(a => Console.WriteLine($"Message {a.Item1},{a.Item2} was processed by all consumers"));

            //JoinBlock returns tuple so we have to as well!
            joinBlock.LinkToWithPropagation(finalBlock);

            for (int i = 0; i < 10; i++)
            {
                await broadcastBlock.SendAsync(i);
            }
            //signal completion
            broadcastBlock.Complete();
            await finalBlock.Completion; //Block completes when it recieves Complete call or when EXCETPION was trown
            //Completed Workflows /Blocks CANT RECEIVE ANY NEW MESSAGES!!
            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
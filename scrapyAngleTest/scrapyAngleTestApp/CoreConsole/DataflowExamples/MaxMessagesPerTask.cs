using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CoreConsole.DataflowExamples
{
    public static class MaxMessagesPerTask
    {
        public static Stopwatch _stopwatch { get; set; }
        public static ConcurrentDictionary<long, string> _timestampedList { get; set; }

        //TPL Dataflow is optimizing fror throughput while stil mentaining for relative latency (need to choose to optimize for 1 or the other)
        public static async Task Run()
        {
            var inputBlock = new BroadcastBlock<int>(a => a);
            var consumerBlocks = new List<ActionBlock<int>>();

            const int consumerCount = 10;

            for (int i = 0; i < consumerCount; i++)
            {
                //Create 10x ActionBlocks
                var actionBlock = CreateConsumingBlock(i);
                inputBlock.LinkToWithPropagation(actionBlock);
                consumerBlocks.Add(actionBlock);
            }

            //var sw = new Stopwatch();
            _stopwatch.Start();
            for (int i = 0; i < 100; i++)
            {
                inputBlock.Post(i);
            }
            inputBlock.Complete();
            Task.WaitAll(consumerBlocks.Select(a => a.Completion).ToArray());
            _stopwatch.Stop();

            //Code for displayign results
            Console.BufferWidth = 1500;
            Console.BufferWidth = 130;
            foreach (var thread in _timestampedList)
            {
                //PrintThreadWork(thread);
                Console.WriteLine();
            }
            Console.WriteLine($"Elapsed ticks: {_stopwatch.ElapsedTicks}");
        }

        private static void PrintThreadWork(int thread)
        {
            throw new NotImplementedException();
        }

        private static ActionBlock<int> CreateConsumingBlock(int id)
        {
            var actionBlock = new ActionBlock<int>(a =>

            {
                //Log relative execution time using stopwatch, processing thread and  Action id
                var blockLog = Tuple.Create(_stopwatch.ElapsedTicks, id.ToString());
                //Store & Display action execution over threads
                //var bag = _timestampedList.GetOrAdd(Thread.CurrentThread.ManagedThreadId, new ConcurrentBag<Tuple<long, string>>());
                //bag.Add(blockLog);
            }, new ExecutionDataflowBlockOptions() { MaxMessagesPerTask = 5 }); //Slower than normal ...Becazse each switch has to switch task context
            return actionBlock;
        }
    }
}
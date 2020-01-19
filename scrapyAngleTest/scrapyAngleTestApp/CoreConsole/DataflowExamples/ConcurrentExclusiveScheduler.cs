using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CoreConsole.DataflowExamples
{
    public static class ConcurrentExclusiveScheduler
    {
        public static async Task Run()
        {
            //WHENEVER YOU ENCOUNTER BLOCKS ACCESING SHARED RESOURCE -->REMEMBER "CONCURRENTEXCLUSEVESCHEDULERPAIR" TASK SCHEDULLER
            //IT ALLOWS TO MANTAIN TPL DATAFLOW SEPARATION OF BUSINESS LOGIC AND EXECUTION LOGIC

            //DONT USE LOCKS ...ITS EXE LOGIC , NOT PART OF BUSINESS LOGIC
            var inputBlock = new BroadcastBlock<int>(a => a);

            //Has 2 task schedulers in it
            //--->ConcurrentScheduler(blocks using it will be executed in parallel) --same as default in TPL Dataflow
            //---> ExclusiveScheduler(Blocks using it will never run async , only one at the time)
            var taskScheduler = new System.Threading.Tasks.ConcurrentExclusiveSchedulerPair();

            Action<int> actionBlockFunction = (int a) =>
            {
                //Simulate complex logic...
                //var counterValue = GetSharedObjectValuer();
                //Task.Delay(_random.Next / (300)).Wait();
                //SetSharedObjectValue(counterValue + 1);
            };
            //Execute same function on x2 AB's
            var incrementingBlock1 = new ActionBlock<int>(actionBlockFunction, new ExecutionDataflowBlockOptions() { TaskScheduler = taskScheduler.ExclusiveScheduler });
            var incrementingBlock2 = new ActionBlock<int>(actionBlockFunction, new ExecutionDataflowBlockOptions() { TaskScheduler = taskScheduler.ExclusiveScheduler });

            inputBlock.LinkToWithPropagation(incrementingBlock1);
            inputBlock.LinkToWithPropagation(incrementingBlock2);

            //Each block shouldp process 10 msgs
            for (int i = 0; i < 10; i++)
            {
                inputBlock.Post(i);
            }

            inputBlock.Complete();
            await incrementingBlock1.Completion;
            await incrementingBlock2.Completion;
            Console.WriteLine($"Current counter value {GetSharedObjectValuer()}");
        }

        private static void SetSharedObjectValue(object p)
        {
            throw new NotImplementedException();
        }

        private static object GetSharedObjectValuer()
        {
            throw new NotImplementedException();
        }
    }
}
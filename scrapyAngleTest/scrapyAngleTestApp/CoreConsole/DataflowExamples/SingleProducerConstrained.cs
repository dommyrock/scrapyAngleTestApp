using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CoreConsole.DataflowExamples
{
    public static class SingleProducerConstrained
    {
        //by default consumer assumes there are more than one producers writing to it ...so here we will specify that there is only one producer !

        public static async Task Run()
        {
            var sw = new Stopwatch();
            //6M iterations
            const int ITERS = 6000000;

            var are = new AutoResetEvent(false);

            var ab = new ActionBlock<int>(i =>
            {
                if (i == ITERS)
                    are.Set();
            }, new ExecutionDataflowBlockOptions() { SingleProducerConstrained = true });//pecify that there is only one producer (each Dataflow Block type acts differently so be carefull!)

            //call 6m itterations x 10 times
            for (int j = 0; j < 10; j++)
            {
                sw.Restart();
                for (int i = 1; i <= ITERS; i++)
                {
                    //Post 6M msgs to actionBlock
                    ab.Post(i);
                }
                //Wait untill ABlock processes last message and calls the "Set" method -->caling it unblocks "WaitOne" Func
                are.WaitOne();

                sw.Stop();

                Console.WriteLine("Messages / Sec: {0:N0}", ITERS / sw.Elapsed.TotalSeconds);
            }
            Console.ReadKey();
        }
    }
}
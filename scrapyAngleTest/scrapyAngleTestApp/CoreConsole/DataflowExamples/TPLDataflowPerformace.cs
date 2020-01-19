using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CoreConsole.DataflowExamples
{
    public static class TPLDataflowPerformace
    {
        public static void Run()
        {
            var sw = new Stopwatch();
            //6M iterations
            const int ITERS = 6000000;

            var are = new AutoResetEvent(false);

            var ab = new ActionBlock<int>(i =>
             {
                 if (i == ITERS)
                     are.Set();
             });

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

        /*TPL Version
         * Messages / Sec: 10,812,710
            Messages / Sec: 17,983,773
            Messages / Sec: 19,423,655
            Messages / Sec: 17,120,361
            Messages / Sec: 19,203,656
            Messages / Sec: 19,063,639
            Messages / Sec: 18,602,182
            Messages / Sec: 18,439,610
            Messages / Sec: 18,302,230
            Messages / Sec: 18,720,778
            */

        public static void RunNonDataflowVersion()
        {
            var sw = new Stopwatch();
            //6M iterations
            const int ITERS = 6000000;

            var are = new AutoResetEvent(false);

            //call 6m itterations x 10 times
            for (int j = 0; j < 10; j++)
            {
                sw.Restart();
                //Replaced with task in which itterating loop is happeninng
                new TaskFactory().StartNew(() =>
                {
                    for (int i = 1; i <= ITERS; i++)
                    {
                        if (i == ITERS)
                            are.Set();
                    }
                });

                //Wait untill ABlock processes last message and calls the "Set" method -->caling it unblocks "WaitOne" Func
                are.WaitOne();

                sw.Stop();

                Console.WriteLine("Messages / Sec: {0:N0}", ITERS / sw.Elapsed.TotalSeconds);
            }
            Console.ReadKey();
        }

        /* non TPL Dataflow version
         * Messages / Sec: 69,527,733
            Messages / Sec: 331,326,964
            Messages / Sec: 275,006,073
            Messages / Sec: 219,642,641
            Messages / Sec: 321,109,755
            Messages / Sec: 343,746,956
            Messages / Sec: 354,918,280
            Messages / Sec: 366,562,196
            Messages / Sec: 298,081,843
            Messages / Sec: 311,583,102
            */

        //******RUNING SEQUENTIAL ALGORITHMS IN PARALLEL WILL MAKE THEM EVEN SLOWER****************
    }
}
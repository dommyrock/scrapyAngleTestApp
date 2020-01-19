using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CoreConsole.DataflowExamples
{
    public static class MultipleProducersSingleConsumer
    {
        public async static Task Run()
        {
            var producer1 = new TransformBlock<string, string>(n =>
            {
                Task.Delay(150);
                return n;
            });
            var producer2 = new TransformBlock<string, string>(n =>
            {
                Task.Delay(500);
                return n;
            });

            var printBlock = new ActionBlock<string>(n => Console.WriteLine(n));

            // extension method
            producer1.LinkToWithPropagation(printBlock);
            producer2.LinkToWithPropagation(printBlock);

            for (int i = 0; i < 10; i++)
            {
                producer1.Post($"Producer 1 Message: {i}");
                producer2.Post($"Producer 2 Message: {i}");
            }
            //await producer block completion
            //producer1.Complete();
            //producer2.Complete();

            //CONSUMER DOESNT KNOW ABOUT PRODUCERS ...SO IF CONSUMERS RECEIVES COMPLETION FROM ANY OF THE BLOCKS IT ASSUMES ALL OTHER ONES ARE COMPLETED AS WELL
            //THATS WHY DEFAULT PropagateCompletion =false

            //Await consumer
            await printBlock.Completion;

            Console.WriteLine("Finished");
            Console.ReadKey();
        }

        //TPL doesn't have default block for scenario where multiple producers write to single consumer so we need custom one

        //***WATCH OUT FOR MULTIPLE PRODUCER SCENARIOS + THEIR COMPLETION
        // ****TEST FOR MESSAGE LOST IN THOSE SCENARIONS !!
        public async static Task RunCustom()
        {
            var producer1 = new TransformBlock<string, string>(n =>
            {
                Task.Delay(150);
                return n;
            });
            var producer2 = new TransformBlock<string, string>(n =>
            {
                Task.Delay(500);
                return n;
            });

            var printBlock = new ActionBlock<string>(n => Console.WriteLine(n));

            // PropagateCompletion =false so we dont signal complete to early !
            producer1.LinkTo(printBlock);
            producer2.LinkTo(printBlock);

            for (int i = 0; i < 10; i++)
            {
                producer1.Post($"Producer 1 Message: {i}");
                producer2.Post($"Producer 2 Message: {i}");
            }
            //await producer block completion
            producer1.Complete();
            producer2.Complete();

            await Task.WhenAll(new[] { producer1.Completion, producer2.Completion }).
               ContinueWith(a => printBlock.Complete());
            //Complete the action block
            //Await consumer
            await printBlock.Completion;

            Console.WriteLine("Finished");
            Console.ReadKey();
        }
    }
}
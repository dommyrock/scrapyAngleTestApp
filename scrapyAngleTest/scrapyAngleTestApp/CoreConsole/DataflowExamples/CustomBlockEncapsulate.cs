using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CoreConsole.DataflowExamples
{
    public static class CustomBlockEncapsulate
    {
        public async static Task Run()
        {
            //Throw exception before encapsulated block
            var inputBlock = new TransformBlock<int, int>(a =>
             {
                 if (a == 2)
                     throw new Exception("What will happen?");
                 return a;
             });

            var increasingBlock = CreateFilteringBlock<int>();

            var printBlock = new ActionBlock<int>(a => Console.WriteLine($"Messge {a} was received !"));

            //push msgs to "inputBlock" with excepton
            increasingBlock.LinkToWithPropagation(inputBlock);

            //send ordered msgs to block
            await inputBlock.SendAsync(1);
            await inputBlock.SendAsync(2);
            await inputBlock.SendAsync(1);
            await inputBlock.SendAsync(3);
            await inputBlock.SendAsync(4);
            await inputBlock.SendAsync(2);

            //await to complete
            inputBlock.Complete();
            await printBlock.Completion;

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }

        //****It has to return the IPropagatorblock , requirement is that our msgs implement IComparable<T> interface !!!
        //filter numbers smaller than last seen value
        private static IPropagatorBlock<T, T> CreateFilteringBlock<T>()
            where T : IComparable<T>, new()
        {
            T maxElement = default(T);
            var source = new BufferBlock<T>();

            //Manualy push messages to buffer block only when msg is larger than max previous value
            var target = new ActionBlock<T>(async item =>
            {
                if (item.CompareTo(maxElement) > 0)
                {
                    await source.SendAsync(item);
                    maxElement = item;
                }
            });
            //Completion logic between action and buffer blocks !!  fror propagating exceptions !!
            //***Check the reason previous task completed ...IF IT FAULTED WE SHOULD PASS FAULTED STATE FORWARD WITH EXCEPTION !!
            target.Completion.ContinueWith(a =>
            {
                if (a.IsFaulted)
                    ((ITargetBlock<T>)source).Fault(a.Exception);
                else
                    source.Complete();
            });

            //Encapsulation
            return DataflowBlock.Encapsulate(target, source);
        }
    }
}
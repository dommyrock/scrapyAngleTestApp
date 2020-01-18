using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CoreConsole.DataflowExamples
{
    public static class BufferBlockSendAsync
    {
        public static void Run()
        {
            var bufferBlock = new BufferBlock<int>(new DataflowBlockOptions() { BoundedCapacity = 1 }); //set max cap at 1 msg(so we dont run out of memory)

            var ab1 = new ActionBlock<int>(a =>
            {
                Console.WriteLine($"Message {a} was processed by Consumer 1");
                Task.Delay(280);
            }, new ExecutionDataflowBlockOptions() { BoundedCapacity = 1 });//set max cap at 1 msg(to load balance consumers)
            var ab2 = new ActionBlock<int>(a =>
            {
                Console.WriteLine($"Message {a} was processed by Consumer 2");
                Task.Delay(150);
            }, new ExecutionDataflowBlockOptions() { BoundedCapacity = 1 });//set max cap at 1 msg(to load balance consumers)

            //Connect producer to consumers (ISourceBlock,ITargetBlock)
            bufferBlock.LinkTo(ab1);
            bufferBlock.LinkTo(ab2);
            for (int i = 0; i < 10; i++)
            {
                //send data to block (not blocking function (does not wait untill block can accept msg --it's discarded and post returns false))
                //bufferBlock.Post(i);
                bufferBlock.SendAsync(i)
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
            //Console.WriteLine("Finished");
            Console.ReadKey();
        }
    }
}
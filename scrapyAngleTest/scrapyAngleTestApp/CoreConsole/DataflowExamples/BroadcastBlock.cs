using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CoreConsole.DataflowExamples
{
    public static class BroadcastBlock
    {
        //Delivers same msgs to consumers --All linked consumers
        //Alll processing is done in memory
        //MSgs ARE CLASS INSTANCES -->(-->passed as reference types, same msg  Instance is delivered to all conusmers  )---> never mutate state of msg ...Return new msg !

        //**BRODCAST DOESNT WAIT FOR CONSUMER TO BE READY TO RECIEVE , IT SENDS MEGS ONLY ONCE AND NEVER OFFERS THEM AGIN**
        //Solutions:
        //ALWAYS SET UNLIMITED "BoundCapacity" in consumers (hard to mantain)
        //Implement custom block implementation
        public static void Run()
        {
            //broadcast requires a cloning functions "a=>a {}"
            var broadcastBlock = new BroadcastBlock<int>(a => a); //set max cap at 1 msg(so we dont run out of memory)

            var ab1 = new ActionBlock<int>(a =>
            {
                Console.WriteLine($"Message {a} was processed by Consumer 1");
                Task.Delay(280);
            });
            var ab2 = new ActionBlock<int>(a =>
            {
                Console.WriteLine($"Message {a} was processed by Consumer 2");
                Task.Delay(150);
            });

            //Connect producer to consumers (ISourceBlock,ITargetBlock)
            broadcastBlock.LinkTo(ab1);
            broadcastBlock.LinkTo(ab2);

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
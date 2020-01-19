using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CoreConsole.DataflowExamples
{
    public static class CustomGuaranteedDeliveryBroadcast
    {
        // TPL dataflow broadcast block is losing messages when consumers input queue cant accept any more messages
        //Solution is tu use BroadcastBlock but have BufferBlock before each consumer !!!
        public async static Task Run()
        {
            var broadcastBlock = new GuaranteedDeliveryBroadcastBlock<int>(a => a); //set max cap at 1 msg(so we dont run out of memory)

            var ab1 = new ActionBlock<int>(a =>
            {
                Console.WriteLine($"Message {a} was processed by Consumer 1");
                Task.Delay(400);
            }, new ExecutionDataflowBlockOptions() { BoundedCapacity = 1 });
            var ab2 = new ActionBlock<int>(a =>
            {
                Console.WriteLine($"Message {a} was processed by Consumer 2");
                Task.Delay(150);
            }, new ExecutionDataflowBlockOptions() { BoundedCapacity = 1 });

            //Connect producer to consumers (ISourceBlock,ITargetBlock)
            broadcastBlock.LinkToWithPropagation(ab1);
            broadcastBlock.LinkToWithPropagation(ab2);

            for (int i = 0; i < 10; i++)
            {
                await broadcastBlock.SendAsync(i);
            }

            broadcastBlock.Complete();
            await ab1.Completion;
            await ab2.Completion;

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }

    //Since we're sending are receiveing msgs ...Implement IPropagatorBlock<T>
    class GuaranteedDeliveryBroadcastBlock<T> : IPropagatorBlock<T, T>
    {
        private BroadcastBlock<T> _broadcastBlock;
        private Task _completion;

        public GuaranteedDeliveryBroadcastBlock(Func<T, T> cloningFunction)
        {
            _broadcastBlock = new BroadcastBlock<T>(cloningFunction);
            //Since broadcastBlock is always first we'l use its completion task as initial value
            _completion = _broadcastBlock.Completion;
        }

        //Task need to wait for all blocks to complete --BroadcastBlock and BufferBlock
        public Task Completion { get { return _completion; } }

        public void Complete()
        {
            //Pass this method calls to our BroadcastBlock !!
            ((ITargetBlock<T>)_broadcastBlock).Complete();
        }

        public void Fault(Exception exception)
        {
            ((ITargetBlock<T>)_broadcastBlock).Fault(exception);
        }

        /// Here we subscribe to our custom broadcast block !!
        public IDisposable LinkTo(ITargetBlock<T> target, DataflowLinkOptions linkOptions)
        {
            var bufferBlock = new BufferBlock<T>();
            //Link new buffer block to broadcastBlock
            var b1 = _broadcastBlock.LinkTo(bufferBlock, linkOptions);
            //Link BufferBlock to subscriber
            var b2 = bufferBlock.LinkTo(target, linkOptions);

            //Each time we create buffer block we need to chain it to current completion Task!!
            _completion.ContinueWith(a => bufferBlock.Completion);

            //Dispose both blocks
            return new DisposableDisposer(b1, b2);
        }

        /// <summary>
        /// Called when block is being offered a Message
        /// </summary>
        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, T messageValue, ISourceBlock<T> source, bool consumeToAccept)
        {
            //Offer MSG directly to broadcast block
            return ((ITargetBlock<T>)_broadcastBlock).OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        }

        //Not used because we used link to manaly
        public T ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target, out bool messageConsumed)
        {
            throw new NotImplementedException("This method should not be called. The producer is a BufferBlock");
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
        {
            throw new NotImplementedException("This method should not be called. The producer is a BufferBlock");
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
        {
            throw new NotImplementedException("This method should not be called. The producer is a BufferBlock");
        }
    }

    //When dispose is called we should unlink the block
    //in our case we need to unlink 2 block's ... SO we need custom class to handle that!!

    class DisposableDisposer : IDisposable
    {
        private readonly IDisposable[] _disposables;

        /// <summary>
        /// Call Dispose on passed objects
        /// </summary>
        public DisposableDisposer(params IDisposable[] disposables)
        {
            _disposables = disposables;
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                //Dispose each obj passed
                disposable.Dispose();
            }
        }
    }
}
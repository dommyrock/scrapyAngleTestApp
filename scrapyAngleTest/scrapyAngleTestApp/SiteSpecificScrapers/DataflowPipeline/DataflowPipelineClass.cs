using ScrapySharp.Network;
using SiteSpecificScrapers.DataflowPipeline.RealTimeFeed;
using SiteSpecificScrapers.Interfaces;
using SiteSpecificScrapers.Messages;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SiteSpecificScrapers.DataflowPipeline
{
    public class DataflowPipelineClass
    {
        //NOTE: TPL DATAFLOW ONLY DEFINES PIPELINE FOR MESSAGE FLOW & TRHOUGHPUT !!! (can extend it with kafka,0mq for load balancing)

        private readonly ISiteSpecific _specificScraper;
        private readonly IRealTimePublisher _realTimeFeedPublisher;
        private readonly IDataConsumer _dataConsumer;
        public ScrapingBrowser Browser { get; set; }

        /// <summary>
        /// Executes specific scraping logic for each passed scraper.
        /// (Only role is message propagation! )
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="scrapers"></param>
        public DataflowPipelineClass(ScrapingBrowser browser,
                                    ISiteSpecific scraper,
                                    IRealTimePublisher realTimePublisher,
                                    IDataConsumer dataConsumer)
        {
            Browser = browser;
            _specificScraper = scraper;
            _realTimeFeedPublisher = realTimePublisher;
            _dataConsumer = dataConsumer;
        }

        public async Task StartPipelineAsync(CancellationToken token)
        {
            //Pipeline config
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            //Block config
            var largeBufferOptions = new ExecutionDataflowBlockOptions() { BoundedCapacity = 600000 };
            var smallBufferOptions = new ExecutionDataflowBlockOptions() { BoundedCapacity = 1000 };
            var realTimeBufferOptions = new ExecutionDataflowBlockOptions() { BoundedCapacity = 6000 };
            var parallelizedOptions = new ExecutionDataflowBlockOptions() { BoundedCapacity = 6000, MaxDegreeOfParallelism = 4 };//was 1000
            var batchOptions = new GroupingDataflowBlockOptions() { BoundedCapacity = 1000 };

            //NOTE: pipeline is only aware of messages it need's to process, not which scrapper called it !

            ///TODO:  When we get <remarks transformBlock ... we skipp it and exit pipeline ...fix that !!!!
            /// check out  await Task.Yield(); to yeald to same context
            /// _specificScraper.Run(browser) --> Run method will have different implementation in each scraper( problem is logic separation which it does in it )

            //Block definitions

            //Download page here---> <site link,downloaded site source>
            //For each message it consumes, it outputs another.
            var transformBlock = new TransformBlock<Message, Message>(async (Message msg) => //SEE"DataBusReader" Class for example !!
            {
                await _specificScraper.Run(this.Browser);//Same browser instance all the way ( Program -->CompositionRoot-->DFPipeline-->method)

                return msg;
            }, largeBufferOptions);

            //It is like the TransformBlock but it outputs an IEnumerable<TOutput> for each message it consumes.
            var scrapeManyBlock = new TransformManyBlock<Message, ProcessedMessage>(
              (Message msg) => /* execute scraping logic for passed site source's  */, largeBufferOptions); //TODO: figure out flow of data in https://jack-vanlightly.com/blog/2018/4/18/processing-pipelines-series-tpl-dataflow and try decide where my scraping logic happens in pipeline (might just happen somwhere else and feed it into pipeline ?)

            //Branches out the messages to other consumer blocks linked!
            //BroadcastBlock has a buffer of one message that gets overwritten by each incoming message.
            //So if the BroadcastBlock cannot forward a message to downstream blocks then the message is lost when the next message arrives. This is load-shedding.
            //Only the blocks up until the first BroadcastBlock could force producer slowdown or load shedding as the BroadcastBlock simply overwrites its buffer on each new message
            //and so neither it or downstream blocks can apply back-pressure to the producer.
            //The broadcast block will make an attempt to pass the message onto all downstream linked blocks before allowing the message to get overwritten.
            //But if a linked block has a bounded buffer which is full, the message gets discarded - load shedding.
            //So as long as the linked blocks have capacity, then the broadcast block ensures all of them get the message.
            var broadcast = new BroadcastBlock<ProcessedMessage>(msg => msg);

            //Real time publish ...
            var realTimeFeedBlock = new ActionBlock<ProcessedMessage>(async
               (ProcessedMessage msg) => await _realTimeFeedPublisher.PublishAsync(msg), parallelizedOptions);

            //Link blocks together
            transformBlock.LinkTo(scrapeManyBlock, linkOptions); //Can add 3rd param , ()=>  filter method msg need to pass to propagate from source to target!!
            scrapeManyBlock.LinkTo(broadcast, linkOptions);
            broadcast.LinkTo(realTimeFeedBlock, linkOptions);

            //Start consuming data
            var consumer = _dataConsumer.StartConsuming(transformBlock, token);

            //Keep going untill CancellationToken is cancelled or block is in the completed state either due to a fault or the completion of the pipeline.
            while (!token.IsCancellationRequested
               && !realTimeFeedBlock.Completion.IsCompleted)
            {
                await Task.Delay(25);
            }

            //the CancellationToken has been cancelled and our producer has stopped producing
            transformBlock.Complete(); // call Complete on the first block, this will propagate down the pipeline

            //Wait for all blocks to finish processing their data
            await Task.WhenAll(realTimeFeedBlock.Completion, consumer);

            // clean up any other resources like ZeroMQ/kafka for example
        }
    }
}
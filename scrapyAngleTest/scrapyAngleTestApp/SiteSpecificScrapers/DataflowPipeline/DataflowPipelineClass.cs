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
            var transformBlock = new TransformBlock<Message, Message>(async (Message msg) => //SEE"DataBusReader" Class for example !!
            {
                await _specificScraper.Run(this.Browser);//Same browser instance all the way ( Program -->CompositionRoot-->DFPipeline-->method)

                return msg;
            }, largeBufferOptions);

            var scrapeManyBlock = new TransformManyBlock<Message, IEnumerable<Message>(//figure out what we pass from transformblock to here
              (Message msg) => /* execute scraping logic for passed site source's  */, largeBufferOptions);

            var broadcast = new BroadcastBlock<IEnumerable<Message>>(msg => msg);

            //Real time publish ...
            var realTimeFeedBlock = new ActionBlock<Message>(async
               (Message msg) => await _realTimeFeedPublisher.PublishAsync(msg), parallelizedOptions); //TODO: check <T> output type and , change it in IRealTimePub, and its class

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
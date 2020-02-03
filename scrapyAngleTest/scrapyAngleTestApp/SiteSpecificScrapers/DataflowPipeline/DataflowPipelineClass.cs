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
        private readonly IFetchSource _fetchSource;

        public ScrapingBrowser Browser { get; set; }

        /// <summary>
        /// Executes specific scraping logic for each passed scraper.
        /// (Only role is message propagation! )
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="scrapers"></param>
        public DataflowPipelineClass(ScrapingBrowser browser, ISiteSpecific scraper)
        {
            _specificScraper = scraper;
            Browser = browser;
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

            //TODO: there should be separate pipe for each scraper(in V2 )FIrst do bellow :
            //4. line 120--_dataBusReader.StartConsuming() --- execute scraping logic here (move my methods inside it)

            //Block definitions

            //Download page here---> <site link,downloaded site source>
            var transformBlock = new TransformBlock<Message, Message>(async (Message msg) => //SEE"DataBusReader" Class for example !!
            {
                //make interface that contains method for scraping site source , + class that implements it ...like "MessageFileWriter"
                //await _fetchSource.NavigateToPage(msg.SourceHtml);
                //await _specificScraper.ScrapeSitemapLinks(); 2nd options is to add  ScrapeSitemapLinks to ISiteSpecific and call it here
                return msg;
            }, largeBufferOptions);

            var scrapeManyBlock = new TransformManyBlock<Message, IEnumerable<Message>(
              (Message msg) =>/* execute scraping logic for passed site source's  */, largeBufferOptions);

            var broadcast = new BroadcastBlock<IEnumerable<Message>>(msg => msg);

            //Real time publish ...
            var realTimeFeedBlock = new ActionBlock<Message>(async
               (Message msg) => await _realTimeFeedPublisher.PublishAsync(msg), parallelizedOptions); //TODO: check <T> output type and , change it in IRealTimePub, and its class

            //Link blocks together
            transformBlock.LinkTo(scrapeManyBlock, linkOptions); //Can add 3rd param , ()=>  filter method msg need to pass to propagate from source to target!!
            scrapeManyBlock.LinkTo(broadcast, linkOptions);
            broadcast.LinkTo(realTimeFeedBlock, linkOptions);

            //Start consuming data
            //var consumer = //consume logic

            //Keep going untill CancellationToken is cancelled or block is in the completed state either due to a fault or the completion of the pipeline.
            while (!token.IsCancellationRequested
               && !realTimeFeedBlock.Completion.IsCompleted
                    )
            {
                await Task.Delay(25);
            }

            //the CancellationToken has been cancelled and our producer has stopped producing
            transformBlock.Complete(); // call Complete on the first block, this will propagate down the pipeline

            //Wait for all blocks to finish processing their data
            await Task.WhenAll(realTimeFeedBlock.Completion);

            // clean up any other resources like ZeroMQ/kafka for example
        }
    }
}
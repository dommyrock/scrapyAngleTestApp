using ScrapySharp.Network;
using SiteSpecificScrapers.DataflowPipeline.RealTimeFeed;
using SiteSpecificScrapers.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SiteSpecificScrapers.DataflowPipeline
{
    public class DataflowPipeline
    {
        //TPL DATAFLOW ONLY DEFINES PIPELINE FOR MESSAGE FLOW & TRHOUGHPUT !!! (can extend it with kafka,0mq for load balancing)

        //All site specific scrapers that implement ISiteSpecific
        private readonly ISiteSpecific[] _specificScrapers;

        private readonly IRealTimePublisher _realTimeFeedPublisher;

        public ScrapingBrowser Browser { get; set; }

        /// <summary>
        /// Executes specific scraping logic for each passed scraper.
        /// (Only role is message propagation! )
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="scrapers"></param>
        public DataflowPipeline(ScrapingBrowser browser, params ISiteSpecific[] scrapers)//no other params can go after params keyword!
        {
            _specificScrapers = scrapers;
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

            //TODO: there should be separate piepe for each scraper(in V2 )FIrst do bellow :
            //4. line 120--_dataBusReader.StartConsuming() --- execute scraping logic here (move my methods inside it)
            //5. nacapsulate whole Pipeline in loop , so i run scraper's after each before completed !!!

            foreach (ISiteSpecific scraper in _specificScrapers)
            {
                //TPL pipeline logic
            }

            //Block definitions

            //Download pages here
            var transformBlock = new TransformBlock<ScraperOutputClass, IEnumerable<string>>(async (ScraperOutputClass msg) => //SEE"DataBusReader" Class for example !!
            {
                //download passed URL, and output parsed links
            }, largeBufferOptions);

            var scrapeManyBlock = new TransformManyBlock<ScraperOutputClass, IEnumerable<string>(
              (ScraperOutputClass msg) =>/* execute scraping logic for passed site source's  */, largeBufferOptions);

            var broadcast = new BroadcastBlock<ScraperOutputClass>(msg => msg);

            //Real time publish ...
            var realTimeFeedBlock = new ActionBlock<ScraperOutputClass>(async
               (ScraperOutputClass msg) => await _realTimeFeedPublisher.PublishAsync(msg), parallelizedOptions); //TODO: check <T> output type and , change it in IRealTimePub, and its class

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

            // clean up any other resources like ZeroMQ for example
        }
    }
}
using ScrapySharp.Network;
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
            var parallelizedOptions = new ExecutionDataflowBlockOptions() { BoundedCapacity = 1000, MaxDegreeOfParallelism = 4 };
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
            var transformBlock = new TransformBlock<string, IEnumerable<string>>(async (ScraperOutputClass msg) => //SEE"DataBusReader" Class for example !!
            {
                //download passed URL, and output parsed links
            }, largeBufferOptions);

            // pass output sites to transformmanyBlock
            //do scraping logic in it

            //pass output data to broadcast block

            //Broadcast output articles to real time service, + new links back to system !
        }
    }
}
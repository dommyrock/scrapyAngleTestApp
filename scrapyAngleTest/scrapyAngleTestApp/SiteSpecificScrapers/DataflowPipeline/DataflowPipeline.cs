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
        private readonly ISiteSpecific[] _specificScrapers;

        /// <summary>
        /// Executes specific scraping logic foreach passed scraper.
        /// </summary>
        /// <param name="scrapers"></param>
        public DataflowPipeline(params ISiteSpecific[] scrapers)//no other params can go after params keyword!
        {
            _specificScrapers = scrapers;
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

            //Block definitions

            //Download pages here
            var transformBlock = new TransformBlock<string, string>()
            {
            };
        }
    }
}
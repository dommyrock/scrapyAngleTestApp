using SiteSpecificScrapers.Messages;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SiteSpecificScrapers.Interfaces
{
    public interface IDataConsumer
    {
        /// <summary>
        /// Sets initial Message to current scraper site
        /// </summary>
        /// <param name="target">Target page</param>
        /// <param name="token">Supports cancellation throughout the pipeline</param>
        /// <returns></returns>
        Task StartConsuming(ITargetBlock<Message> target, CancellationToken token, ISiteSpecific scraper);
    }
}
using SiteSpecificScrapers.Messages;
using System.Threading.Tasks;

namespace SiteSpecificScrapers.DataflowPipeline.RealTimeFeed
{
    public interface IRealTimePublisher
    {
        /// <summary>
        /// Publish output to console in real time .
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        void PublishAsync(Message message);
    }
}
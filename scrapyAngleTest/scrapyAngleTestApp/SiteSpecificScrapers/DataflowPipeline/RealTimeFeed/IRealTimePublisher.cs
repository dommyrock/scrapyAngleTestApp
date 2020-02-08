using SiteSpecificScrapers.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        Task PublishAsync(ProcessedMessage message);
    }
}
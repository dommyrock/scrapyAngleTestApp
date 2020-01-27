using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteSpecificScrapers.DataflowPipeline.RealTimeFeed
{
    public interface IRealTimePublisher
    {
        Task PublishAsync(string message);
    }
}

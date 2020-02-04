using SiteSpecificScrapers.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SiteSpecificScrapers.Interfaces
{
    public interface IDataConsumer
    {
        Task StartConsuming(ITargetBlock<Message> target, CancellationToken token);
    }
}
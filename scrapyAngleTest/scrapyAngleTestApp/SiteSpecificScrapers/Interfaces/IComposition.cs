using SiteSpecificScrapers.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SiteSpecificScrapers.Services
{
    interface IComposition
    {
        Task<List<Task<Message>>> RunAllAsync();
    }
}
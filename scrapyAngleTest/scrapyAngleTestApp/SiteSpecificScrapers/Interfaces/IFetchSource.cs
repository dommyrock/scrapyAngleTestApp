using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteSpecificScrapers.Interfaces
{
    public interface IFetchSource
    {
        Task DownloadSource();
    }
}
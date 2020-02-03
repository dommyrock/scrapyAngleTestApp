using ScrapySharp.Network;
using SiteSpecificScrapers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteSpecificScrapers.Helpers
{
    public class FetchSource : IFetchSource
    {
        public Task<WebPage> NavigateToPage(string url)
        {
            throw new NotImplementedException();
        }
    }
}
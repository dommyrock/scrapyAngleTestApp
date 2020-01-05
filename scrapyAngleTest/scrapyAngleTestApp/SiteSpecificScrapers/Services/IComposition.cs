using ScrapySharp.Network;
using SiteSpecificScrapers.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteSpecificScrapers.Services
{
    interface IComposition
    {
        ValueTask<List<ScraperOutputClass>> RunAll(ScrapingBrowser browser);
    }
}
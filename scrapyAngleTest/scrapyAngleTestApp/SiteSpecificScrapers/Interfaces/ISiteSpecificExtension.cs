using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteSpecificScrapers.Services
{
    /// <summary>
    /// Extends methods that site specific scrapers need to implement.
    /// </summary>
    public interface ISiteSpecificExtension : ISiteSpecific
    {
        Task ScrapeSpecificSite(ScrapingBrowser browser);
    }
}
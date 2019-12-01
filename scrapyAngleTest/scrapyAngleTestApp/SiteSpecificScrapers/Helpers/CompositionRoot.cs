using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScrapySharp.Network;

namespace SiteSpecificScrapers.Helpers
{
    public class CompositionRoot : ISiteSpecific
    {
        public string Url { get; set; }
        public List<string> InputList { get; set; }
        public Dictionary<string, bool> ScrapedKeyValuePairs { get; set; }
        public string SitemapUrl { get; set; }
        public ScrapingBrowser Browser { get; set; }

        /// <summary>
        /// Readonly indicates that assignment to the field can only occur as part of the declaration or in a constructor in the same class
        /// </summary>
        private readonly ISiteSpecific[] _specificScrapers;

        public CompositionRoot(params ISiteSpecific[] scrapers)
        {
            _specificScrapers = scrapers;
        }

        public async Task<bool> ScrapeSitemapLinks(ScrapingBrowser browser)
        {
            foreach (ISiteSpecific scraper in _specificScrapers)
            {
                await scraper.ScrapeSitemapLinks(browser);
            }
            return true;
        }

        public Task<bool> ScrapeSiteLinks()
        {
            throw new NotImplementedException();
        }
    }
}
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using SiteSpecificScrapers.BaseClass;
using SiteSpecificScrapers.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSpecificScrapers
{
    public class AdmScraper : BaseScraperClass, ISiteSpecific, ISiteSpecificExtension
    {
        public string Url { get; set; }
        public List<string> InputList { get; set; }
        public string SitemapUrl { get; set; }
        public ScrapingBrowser Browser { get; set; }
        public Dictionary<string, bool> ScrapedKeyValuePairs { get; set; }

        public AdmScraper()
        {
            this.Url = "https://www.adm.hr";
        }

        public void Run(ScrapingBrowser browser)
        {
            var success = base.ScrapeSitemapLinks(browser, Url);

            if (success.Result)
            {
                ScrapeSpecificSite(browser); //TODO
            }
        }

        public void ScrapeSpecificSite(ScrapingBrowser browser)
        {
            Console.WriteLine("entered Adm Scraper");
        }
    }
}
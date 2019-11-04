using ScrapySharp.Extensions;
using ScrapySharp.Network;
using SiteSpecificScrapers.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSpecificScrapers
{
    public class AdmScraper : BaseScraperClass, ISiteSpecific//TODOd...test this class ...
    {
        public string Url { get; set; }
        public List<string> InputList { get; set; }
        public string SitemapUrl { get; set; }
        public ScrapingBrowser Browser { get; set; }
        public Dictionary<string, bool> ScrapedKeyValuePairs { get; set; }
        private List<string> WebShops { get; set; }

        public AdmScraper()
        {
            this.Url = "testString...";
        }

        /// <summary>
        /// True if Success(Adds webshop urls to "InputList".) / False if no sitemap has been found in robots.txt
        /// </summary>
        public async Task<bool> ScrapeSitemapLinks()
        {
            SitemapUrl = await base.GetSitemap(Browser, Url);

            if (SitemapUrl != string.Empty)
            {
                WebPage document = await Browser.NavigateToPageAsync(new Uri(Url));//might replace with basic downloadstrignasync...
                //Specific  query for nabava.net
                var nodes = document.Html.CssSelect("loc").ToList();

                foreach (var node in nodes)
                {
                    InputList.Add(node.InnerText);
                    Console.WriteLine(node.InnerText + "\n");
                }

                InputList.RemoveAt(0);
                Url = InputList[0];
                return true;//true
            }
            return false;//false
        }
    }
}
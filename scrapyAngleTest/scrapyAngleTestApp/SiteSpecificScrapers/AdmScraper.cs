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
        public bool HasSitemap { get; set; }
        public ScrapingBrowser Browser { get; set; }
        public Dictionary<string, bool> ScrapedKeyValuePairs { get; set; }
        private List<string> WebShops { get; set; }

        /// <summary>
        /// True if Success(Adds webshop urls to "InputList".) / False if no sitemap has been found in robots.txt
        /// </summary>
        public async Task<bool> ScrapeSitemapLinks()
        {
            HasSitemap = await base.GetSitemap(Browser);

            if (HasSitemap)
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
                return HasSitemap;//true
            }
            return HasSitemap;//false
        }
    }
}
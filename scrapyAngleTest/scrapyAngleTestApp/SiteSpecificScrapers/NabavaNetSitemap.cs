using ScrapySharp.Extensions;
using ScrapySharp.Network;
using SiteSpecificScrapers.BaseClass;
using SiteSpecificScrapers.Helpers;
using SiteSpecificScrapers.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSpecificScrapers
{
    public class NabavaNetSitemap : BaseScraperClass, ISiteSpecific
    {
        public string Url { get; set; }
        public List<string> InputList { get; set; }
        public string SitemapUrl { get; set; }
        public ScrapingBrowser Browser { get; set; }
        public Dictionary<string, bool> ScrapedKeyValuePairs { get; set; }
        private List<string> WebShops { get; set; }

        public NabavaNetSitemap()
        {
            this.Url = "http://nabava.net";
            InputList = new List<string>(); //TODO:change to be same instance from main else i lose this
        }

        //This class ment to only fetch sitemap (GetSitemap method --->

        /// <summary>
        /// True if Success(Adds webshop urls to "InputList".) / False if no sitemap has been found in robots.txt
        /// </summary>
        public async Task<bool> ScrapeSitemapLinks(ScrapingBrowser browser)
        {
            this.Browser = browser;

            SitemapUrl = await base.GetSitemap(Browser, Url);

            if (SitemapUrl != string.Empty)
            {
                WebPage document = await Browser.NavigateToPageAsync(new Uri(SitemapUrl));//might replace with basic downloadstrignasync...

                //Specific  query for nabava.net
                var nodes = document.Html.CssSelect("loc").Select(i => i.InnerText).ToList();
                InputList.AddRange(nodes);

                InputList.RemoveAt(0);
                Url = InputList[0];
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds webshops scraped from sitemap to "WebShops" list.
        /// </summary>
        private async Task ScrapeWebshops()
        {
            WebShops = new List<string>();
            ScrapedKeyValuePairs = new Dictionary<string, bool>();

            WebShops = NewtonsoftExtension.GetFromLocalCache();
            if (WebShops.Count == 0)
            {
                while (true)
                {
                    //NOTE: we fetch 1 by 1 node because shop links still need to be scraped from nabava.net(direct Urls-are not in sitemap)
                    WebPage pageSource = await Browser.NavigateToPageAsync(new Uri(Url));//can speed this up by using DownloadStringAsync(but need other filter to extract shop link (regex))

                    var node = pageSource.Html.SelectSingleNode("//b");//get<b> node/Link select all nodes
                    if (node != null)
                    {
                        bool isLink = node.InnerHtml.StartsWith("http");
                        if (isLink)
                        {
                            //InputList.Add(node.InnerHtml);

                            WebShops.Add(node.InnerHtml);

                            //Real link assignment
                            Url = node.InnerHtml;
                            Console.WriteLine(node.InnerHtml);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"All [{WebShops.Count}] Shops scraped from nabava.net/sitemap.xml \n");

                        //Set url to [0] item in "Webshop" queue before exiting
                        Url = WebShops[0];

                        NewtonsoftExtension.CacheWebshopsToLocalCache(WebShops);

                        //Exit loop when all webshops are scraped
                        break;
                    }

                    if (!ScrapedKeyValuePairs.ContainsKey(Url))//TODO : use Hash(set)  or concurrent collection
                    {
                        ScrapedKeyValuePairs.Add(Url, true);//added scraped links from sitemap here...
                    }

                    InputList.RemoveAt(0);//remove scraped links from sitemap
                    Url = InputList[0];
                }
            }
        }

        // Encapsulates scraping logic for each site specific scraper.(Must be async if it encapsulates async code)     OLD METHOD,,,REMOVE WHEN REPLACED
        public async Task RunInitMsg(ScrapingBrowser browser, Message msg)
        {
            var success = ScrapeSitemapLinks(browser).GetAwaiter().GetResult();

            if (success)
            {
                await ScrapeWebshops().ContinueWith(i => Console.WriteLine("Scraping Webshops in Nabava.net DONE"));
            }

            await Task.Yield();
        }

        async Task<IEnumerable<ProcessedMessage>> ISiteSpecific.Run(ScrapingBrowser browser, Message message)
        {
            //TODO: make this method get shops/link data and assigns it to message.Webshops ---TEST FLOW ON "RunInitMsg" FIST HAN REPLACE IT WITH THIS METHOD
            throw new NotImplementedException();
        }
    }
}
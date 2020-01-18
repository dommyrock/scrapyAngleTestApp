using ScrapySharp.Network;
using SiteSpecificScrapers.BaseClass;
using SiteSpecificScrapers.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteSpecificScrapers
{
    public class AbrakadabraScraper : BaseScraperClass, /*ISiteSpecific,*/ ISiteSpecificExtension
    {
        public string Url { get; set; }
        public List<string> InputList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Dictionary<string, bool> ScrapedKeyValuePairs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string SitemapUrl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ScrapingBrowser Browser { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AbrakadabraScraper()
        {
            this.Url = "https://www.abrakadabra.com";
        }

        public async Task Run(ScrapingBrowser browser)
        {
            var success = base.ScrapeSitemapLinks(browser, Url).GetAwaiter().GetResult();

            if (success)
            {
                await ScrapeSpecificSite(browser); //TODO
            }
        }

        public async Task ScrapeSpecificSite(ScrapingBrowser browser)
        {
            Console.WriteLine("Entered Abrakadabra Scraper");
        }
    }
}

//public static void FetchAbrakadabra() moved from Program ->main()
//{
//    string url = "https://www.abrakadabra.com/hr-HR/Katalog/TV%2C-mobiteli-i-elektronika/Televizori-i-dodaci/c/FE100";
//    //Fetch
//    WebPage source = Browser.NavigateToPage(new Uri(url));

//    //Take only script  node with dataLayer inside it (temp solution-specific to to the site)
//    HtmlNode dataLayerNode = source.Html.CssSelect("script").Skip(1).Take(1).SingleOrDefault();

//    //serialize JSON to C# object with Newtonsoft
//    Article article = new Article(dataLayerNode.InnerText);
//    List<Article> articles = article.DeserializeJSON();
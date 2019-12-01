using ScrapySharp.Network;
using SiteSpecificScrapers.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteSpecificScrapers
{
    public class AbrakadabraScraper : BaseScraperClass, ISiteSpecific
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

        public Task<bool> ScrapeSiteLinks()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ScrapeSitemapLinks(ScrapingBrowser browser)
        {
            throw new NotImplementedException();
        }
    }
}

//moved form main ...Reimplement here

//public static void FetchAbrakadabra()
//{
//    string url = "https://www.abrakadabra.com/hr-HR/Katalog/TV%2C-mobiteli-i-elektronika/Televizori-i-dodaci/c/FE100";
//    //Fetch
//    WebPage source = Browser.NavigateToPage(new Uri(url));

//    //Take only script  node with dataLayer inside it (temp solution-specific to to the site)
//    HtmlNode dataLayerNode = source.Html.CssSelect("script").Skip(1).Take(1).SingleOrDefault();

//    //serialize JSON to C# object with Newtonsoft
//    Article article = new Article(dataLayerNode.InnerText);
//    List<Article> articles = article.DeserializeJSON();
}
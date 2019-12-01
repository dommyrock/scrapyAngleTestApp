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
        public AbrakadabraScraper()
        {
            this.Url = "https://www.abrakadabra.com";
        }

        public string Url { get; set; }
        public List<string> InputList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Dictionary<string, bool> ScrapedKeyValuePairs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string SitemapUrl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ScrapingBrowser Browser { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<bool> ScrapeSitemapLinks(ScrapingBrowser browser)
        {
            throw new NotImplementedException();
        }
    }
}
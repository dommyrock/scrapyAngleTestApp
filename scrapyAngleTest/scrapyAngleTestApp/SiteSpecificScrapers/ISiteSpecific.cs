using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteSpecificScrapers
{
    //TODO:  leave only 1 method in base class  , and make children reuse it .....and extract all other props and method for scraping into interface ISiteSpecific !!!

    public interface ISiteSpecific
    {
        string Url { get; set; }
        List<string> InputList { get; set; }
        Dictionary<string, bool> ScrapedKeyValuePairs { get; set; }//refactor this in hashset ? or some other key -value pair (maybe concurrent ?)
        string SitemapUrl { get; set; }
        ScrapingBrowser Browser { get; set; }

        Task<bool> ScrapeSitemapLinks();

        //Task<bool> ScrapeSiteLinks();
    }
}
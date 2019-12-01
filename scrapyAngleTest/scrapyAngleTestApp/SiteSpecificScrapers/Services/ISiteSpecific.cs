using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteSpecificScrapers
{
    //Interface props and methods are abstract by default
    public interface ISiteSpecific
    {
        string Url { get; set; }
        List<string> InputList { get; set; }
        Dictionary<string, bool> ScrapedKeyValuePairs { get; set; }//refactor this in hashset ? or some other key -value pair (maybe concurrent ?)
        string SitemapUrl { get; set; }
        ScrapingBrowser Browser { get; set; }

        Task<bool> ScrapeSitemapLinks(ScrapingBrowser browser);

        //Task<bool> ScrapeSiteLinks();
    }
}
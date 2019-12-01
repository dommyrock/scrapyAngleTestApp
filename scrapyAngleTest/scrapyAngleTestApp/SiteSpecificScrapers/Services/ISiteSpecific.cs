using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteSpecificScrapers
{
    //Interface props and methods are abstract by default

    /// <summary>
    /// All derived classes from "BaseScraperClass" should implement this Interface & non unique methods moved to BaseScraperClass
    /// </summary>
    public interface ISiteSpecific
    {
        string Url { get; set; }
        List<string> InputList { get; set; }
        Dictionary<string, bool> ScrapedKeyValuePairs { get; set; }//refactor this in hashset ? or some other key -value pair (maybe concurrent ?)
        string SitemapUrl { get; set; }
        ScrapingBrowser Browser { get; set; }

        Task<bool> ScrapeSitemapLinks(ScrapingBrowser browser);//this should be moved to Base class

        Task<bool> ScrapeSiteLinks();
    }
}
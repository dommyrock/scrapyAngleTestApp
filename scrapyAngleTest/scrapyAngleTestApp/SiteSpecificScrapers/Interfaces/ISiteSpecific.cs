﻿using ScrapySharp.Network;
using SiteSpecificScrapers.Messages;
using System.Collections.Generic;
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

        Task<bool> ScrapeSitemapLinks(ScrapingBrowser browser);

        /// <summary>
        /// Encapsulates scraping logic for each site specific scraper. (Must be async if it encapsulates async code)
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ProcessedMessage>> Run(ScrapingBrowser browser, Message message);

        Task RunInitMsg(ScrapingBrowser browser, Message message);
    }
}
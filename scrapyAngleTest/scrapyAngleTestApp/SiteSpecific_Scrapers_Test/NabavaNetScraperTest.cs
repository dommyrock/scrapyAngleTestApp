using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScrapySharp.Network;
using SiteSpecificScrapers;

namespace SiteSpecific_Scrapers_Test
{
    [TestClass]
    public class NabavaNetScraperTest
    {
        [TestMethod]
        public async Task NabavaNetScraperTestMethod()
        {
            //Scrape only single site
            NabavaNetSitemap nabava = new NabavaNetSitemap()
            {
                Url = "http://nabava.net",
                Browser = new ScrapingBrowser(),
                InputList = new List<string>(),
            };

            //await nabava.Run(nabava.Browser);
        }
    }
}
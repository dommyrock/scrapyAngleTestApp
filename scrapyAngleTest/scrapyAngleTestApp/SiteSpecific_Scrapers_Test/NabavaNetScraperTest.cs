using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScrapySharp.Network;
using SiteSpecificScrapers;

namespace SiteSpecific_Scrapers_Test
{
    [TestClass]
    public class NabavaNetScraperTest
    {
        [TestMethod]
        public void NabavaNetScraperTestMethod()
        {
            //Scrape only single site
            NabavaNetSitemap nabava = new NabavaNetSitemap()
            {
                Url = "http://nabava.net",
                Browser = new ScrapingBrowser(),
                InputList = new List<string>(),
            };

            nabava.Run(nabava.Browser);
        }
    }
}
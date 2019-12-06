using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScrapySharp.Network;
using SiteSpecificScrapers;

namespace SiteSpecific_Scrapers_Test
{
    [TestClass]
    public class AdmScraperTest
    {
        [TestMethod]
        public void AdmScraperTestMethod()
        {
            //Scrape only single site
            AdmScraper nabava = new AdmScraper()
            {
                Url = "https://www.adm.hr",
                Browser = new ScrapingBrowser(),
                InputList = new List<string>(),
            };

            nabava.Run(nabava.Browser);
        }
    }
}
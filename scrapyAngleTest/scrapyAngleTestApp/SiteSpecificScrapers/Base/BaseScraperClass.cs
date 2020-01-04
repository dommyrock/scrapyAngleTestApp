using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SiteSpecificScrapers.BaseClass
{
    #region Info

    //Abstract Class REASON :Interfaces only have props,methods,struct signatures ... i needed to pass defined method to Children
    //also methods  (could be virtual ... so i can override them in childs if needed )
    //and can have abstract prefix(methods can't have implementations here , only in child class) to note that they need to be inherited & implemented !!

    #endregion Info

    /// <summary>
    /// Common/shared code for derived Scraper Classes(Can't be instanciated)
    /// </summary>
    public abstract class BaseScraperClass
    {
        private ScrapingBrowser Browser { get; set; }

        /// <summary>
        /// Base constructor called before derived constructor
        /// </summary>
        protected BaseScraperClass()
        {
        }

        /// <summary>
        /// Derived classes should call this method to fetch .sitemap file if it exists.
        /// [Protected: only derived class can use this method]
        /// </summary>
        protected async Task<string> GetSitemap(ScrapingBrowser browser, string url)
        {
            Browser = browser;
            if (Browser != null)
            {
                string sitemapSource = url + "/robots.txt";

                var document = await browser.DownloadStringAsync(new Uri(sitemapSource));

                //Global regex (might not be suited for all sites)
                var matchSitemap = Regex.Match(document, @"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                if (matchSitemap.Success && matchSitemap.Value.Contains("sitemap"))
                {
                    url = matchSitemap.Value;
                    return url;
                }
                url = string.Empty;
            }
            return url;
        }

        /// <summary>
        /// Default method for sitemap scraping. (Overridable if needed!)
        /// </summary>
        /// <param name="browser">headless browser instance</param>
        /// <param name="url">Current URI</param>
        /// <returns></returns>
        protected virtual async Task<bool> ScrapeSitemapLinks(ScrapingBrowser browser, string url)
        {
            this.Browser = browser;

            var sitemapUel = await GetSitemap(Browser, url);

            if (sitemapUel != string.Empty)
            {
                WebPage document = await Browser.NavigateToPageAsync(new Uri(url));//might replace with basic downloadstrignasync...

                //TODO: Scrape all links from "document"

                return true;//true
            }
            return false;//false
        }
    }
}

/**********************************************************DI****************************************************************************************
 * when someone requests  IInterface service  we want ASP.Core  to create instance of my Childclass & inject its instance into "controller"
 * By default ASP.core can't do that , we have to register interface , and class into container
 * ---> Startup.cs --->ConfigureServices -->service.AddSingleton<IInterface,childClass>();
 * If someone requests service IInterface than create instance of "childClass" & inject that instance !
 *
 *With DI we exclude tight coupling of classes ...and improve meintainability, also makes unit tests easyer , since we can swap dependencies
 *
 * addSingleton() --single instance of service is created & that instance is used throughout the liftime of app!
 * addTransient() --a new instance of transient service is created each time its requested !(overrides old instance ..same as new Class)
 *  addScoped() -- new instance of scoped service is created once per(http) request within a scope
 *
 *
 *
 *New structure would be :  have each child class implement : ISiteSpecific and its props,methods --->than in Startup.cs
 * include it in DI container and scan all... related classes that implement ISiteSpecific
 *
 *
 *
 */
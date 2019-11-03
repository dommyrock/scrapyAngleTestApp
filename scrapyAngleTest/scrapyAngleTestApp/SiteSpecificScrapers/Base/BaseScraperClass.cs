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

    //Structure : BASE ---> children ---> SUM export(index.js -like )that captures all children and provides access to them .

    #endregion Info

    /// <summary>
    /// Has common/shared code for Scraper Classes(Can't be instanciated)
    /// </summary>
    public abstract class BaseScraperClass
    {
        //Props that sould be inherited/implemented by children:

        public abstract string Url { get; set; }
        public abstract List<string> InputList { get; set; }
        public abstract Dictionary<string, bool> ScrapedKeyValuePairs { get; set; }//refactor this in hashset ? or some other key -value pair (maybe concurrent ?)
        public abstract bool HasSitemap { get; set; }
        public abstract ScrapingBrowser Browser { get; set; }

        //constructor used to pass values to abstract class (has no instance !)
        //protected BaseScraperClass()
        //{
        //    //Init base stuff here (browser ctx .... )
        //    //Browser = new ScrapingBrowser(); dont need this , since i dont want new instance for every child
        //}

        /// <summary>
        /// Derived classes should call this method to fetch .sitemap file if it exists.
        /// [Protected: only derived class can use this method]
        /// </summary>
        protected async Task<bool> GetSitemap(ScrapingBrowser browser)
        {
            Browser = browser;
            //Dont need opening in headless browser for this url
            //Check for /robots.txt
            string sitemapSource = Url + "/robots.txt";

            var document = await browser.DownloadStringAsync(new Uri(sitemapSource));//only need string to parse

            var matchSitemap = Regex.Match(document, @"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);//TODO :check this regex for other sitemaps ...not sure if ok

            if (matchSitemap.Success && matchSitemap.Value.Contains("sitemap"))
            {
                Url = matchSitemap.Value;
                return true;
            }
            return false;
        }
    }
}

/**********************************************************DI****************************************************************************************
 * when someone requests  IInterface service  we want ASP ,Core  to create instance of my Childclass & inject its instance into "controller"
 * By default ASp .core cant do that , we have to register interface , and class into container
 * ---> Startup.cs --->ConfigureServices -->service.AddSingleton<IInterface,childClass>();
 * If someone requests service IInterface than create instance of "childClass" & inject that instance !
 *
 *With DI we exclude tight coupling of classes ...and improve meintainability, also makes unit tests easyer , since we can swap dependencies
 *
 * addSingleton() --single instance of service is created & that instance is used throughout the liftime of app!
 * addTransient() --a new instance of transient service is created each time its requested !
 *addScoped() -- new instance of scoped service is created once per request within a scope
 *
 *
 *
 *
 *
 *
 *
 */
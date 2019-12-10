using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScrapySharp.Network;
using SiteSpecificScrapers.Services;

namespace SiteSpecificScrapers.Helpers
{
    public class CompositionRoot : /*ISiteSpecific,*/ IComposition
    {
        public string Url { get; set; }
        public List<string> InputList { get; set; }
        public Dictionary<string, bool> ScrapedKeyValuePairs { get; set; }
        public string SitemapUrl { get; set; }
        public ScrapingBrowser Browser { get; set; }

        /// <summary>
        /// Readonly indicates that assignment to the field can only occur as part of the declaration or in a constructor in the same class
        /// </summary>
        private readonly ISiteSpecific[] _specificScrapers;

        public CompositionRoot(params ISiteSpecific[] scrapers)
        {
            _specificScrapers = scrapers;
        }

        //REMOVED THIS ONE ...SINCE I DONT NEED IT ...INSTEAD I "RunAll" TASKS AND AWAIT THEM TO COMPLETE !!!
        /// <summary>
        /// Encapsulates scraping logic for each site specific scraper.
        /// </summary>
        /// <param name="browser"></param>
        /// <returns></returns>
        //public async Task Run(ScrapingBrowser browser) //TODO: if i want each scraper class to return an object {scraped lists, items ....} make this method return Task<object> or Task<Dictionary>
        //{
        //    foreach (ISiteSpecific scraper in _specificScrapers)
        //    {
        //        await scraper.Run(browser); //TODO:  call awaitAll() when i call this funition in main
        //    }
        //}

        /// <summary>
        /// Runs all site scraper tasks Async.(Task.WaitAll() should be used)
        /// </summary>
        /// <returns></returns>
        public Task<List<Task>> RunAll(ScrapingBrowser browser) //TODO: return list of completed task ---> results from this method(scraped sites & their articles,items..)make custom class
        {
            var tasklist = new List<Task>();
            try
            {
                foreach (ISiteSpecific scraper in _specificScrapers)
                {
                    //Run each scraper async
                    tasklist.Add(scraper.Run(browser)); //TODO:  call awaitAll() when i call this funition in main
                }

                Task.WaitAll(tasklist.ToArray()); ///<see cref="https://medium.com/@t.masonbarneydev/iterating-asynchronously-how-to-use-async-await-with-foreach-in-c-d7e6d21f89fa"/>  -->Returning Values
            }
            catch (Exception ex)/// exceptions <see cref="https://markheath.net/post/async-antipatterns"/>
            {
                throw ex;
            }
        }

        /// method arhitecture example <see cref="https://stackoverflow.com/questions/25720977/return-list-from-async-await-method"/>
        ///
        /// multyple web requests async <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/how-to-make-multiple-web-requests-in-parallel-by-using-async-and-await"/>
    }
}
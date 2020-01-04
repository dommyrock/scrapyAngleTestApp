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

        // readonly -> indicates that assignment to the field can only occur as part of the declaration or in a constructor in the same class
        private readonly ISiteSpecific[] _specificScrapers;

        public CompositionRoot(params ISiteSpecific[] scrapers)
        {
            _specificScrapers = scrapers;
        }

        /// <summary>
        /// Runs all site scraper tasks Async.(Task.WaitAll() should be used)
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Task>> RunAll(ScrapingBrowser browser)
        {
            //Task ...+await Task.WhenAll(tasklist.ToArray()) [await all tasks to complete ]

            //TODO : ///<see cref="https://medium.com/@t.masonbarneydev/iterating-asynchronously-how-to-use-async-await-with-foreach-in-c-d7e6d21f89fa"/>  -->Returning Values
            //make custom class ..than return Task<Ienumerable<CustomClass>>  from this method  (could reuse "Article" class from main method"
            /* 1.1. than make SINGLE QUEUE per Specific site scraper
             * 1.2. respect politeness policy --delay requests to single domain ..., and NEVER make async requests to same domain (only async other site scrapers)
             * 1.3. make QUEUES SPECIFIC to  sites --ONE QUEUE per site !!
             */

            IEnumerable<Task> tasklist = new List<Task>();
            try
            {
                //Run each scraper in parellel
                foreach (ISiteSpecific scraper in _specificScrapers)
                {
                    //Run each scraper async
                    tasklist.Add(scraper.Run(browser)); //TODO:  call awaitAll() just for testing 2,3 sites , else catch and store/print results as they arive .

                    //Task.Run(() => scraper.Run(browser));
                }

                //Wait all tasks to complete
                //Task.WaitAll(tasklist.ToArray());
            }
            catch (Exception ex)/// exceptions <see cref="https://markheath.net/post/async-antipatterns"/>
            {
                throw ex;
            }

            return await Task.WhenAll<Task>(tasklist);
        }

        /// <remarks The task-Result property is a blocking property. ></remarks>
        /// In most cases, you should access the value by using await instead of accessing the property directly.
        ///
        /// Task.Run(()=> func) arhitecutre <see cref="https://stackoverflow.com/questions/25720977/return-list-from-async-await-method"/>
        ///
        /// multyple web requests async <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/how-to-make-multiple-web-requests-in-parallel-by-using-async-and-await"/>
        ///for less memory alocation (non reference & return types <see cref="https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1?view=netcore-3.0"/>

        ///  yields back to the current context <see cref="https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.yield?view=netframework-4.8"/>
        ///

        ///For ERROR metadata file <see cref="https://stackoverflow.com/questions/1421862/metadata-file-dll-could-not-be-found"/>
    }
}
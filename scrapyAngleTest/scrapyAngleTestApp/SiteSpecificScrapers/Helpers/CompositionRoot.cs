using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScrapySharp.Network;
using SiteSpecificScrapers.Output;
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
        /// Runs all site scrapers in parrallel (each scraper should have its own queue!)
        /// </summary>
        /// <returns></returns>
        public async ValueTask<List<ScraperOutputClass>> RunAll(ScrapingBrowser browser)
        {
            //TODO :
            /* 1.1. than make SINGLE QUEUE per Specific site scraper
             * 1.2. respect politeness policy --delay requests to single domain ..., and NEVER make async requests to same domain (only async other site scrapers)
             * 1.3. make QUEUES SPECIFIC to  sites --ONE QUEUE per site !!
             *
             */
            //

            //List of completed tasks
            List<ValueTask<ScraperOutputClass>> tasklist = new List<ValueTask<ScraperOutputClass>>();

            try
            {
                //TODO: 1.4. CHECK IF LIST IS THREAD SAFE & REPLACE WITH THREAD SAFE COLLECTION INSTEAD (ALSO CHECK OUT QUEUE'S --EACH QUEUE WILL BE SPECIFIC TO SCRAPER AND WILL RUN IT'S SCRAPERS IN NEW THREAD'S)

                //Run each scraper in parellel
                foreach (ISiteSpecific scraper in _specificScrapers)
                {
                    //Run each scraper async
                    tasklist.Add(scraper.Run(browser)); //TODO:  call awaitAll() just for testing 2,3 sites , else catch and store/print results as they arive .

                    //Task.Run(() => scraper.Run(browser));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return await Task.WhenAll<Task>(tasklist); not efficient to wait on all to complete , instead await and print/outptut each result as they arrive
            //
            return await Task.FromResult(tasklist); //TODO: see "NEXT STEP" bellow
        }

        /*NEXT STEP
         * Because Task and Task<TResult> are reference types, memory allocation in performance-critical paths,
         * particularly when allocations occur in tight loops, can adversely affect performance.
         * Support for generalized return types means that you can return a lightweight value type instead of a reference type to avoid additional memory allocations.
         /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/async-return-types" Generalized async return types -at the bottom of page />
         /// <see   https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1?view=netcore-3.1
         */

        /// <remarks The task-Result property is a blocking property. ></remarks>
        /// In most cases, you should access the value by using await instead of accessing the property directly.
        /// exceptions <see cref="https://markheath.net/post/async-antipatterns"/>
        ///
        /// Task.Run(()=> func) arhitecutre <see cref="https://stackoverflow.com/questions/25720977/return-list-from-async-await-method"/>
        ///
        /// multyple web requests async <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/how-to-make-multiple-web-requests-in-parallel-by-using-async-and-await"/>
        ///for less memory alocation (non reference & return types <see cref="https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1?view=netcore-3.0"/>

        ///  yields back to the current context <see cref="https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.yield?view=netframework-4.8"/>
        ///

        ///For ERROR metadata file <see cref="https://stackoverflow.com/questions/1421862/metadata-file-dll-could-not-be-found"/>
        ///

        #region LoopAsyncExample

        /*
         * //Async method to be awaited
        public static Task<string> DoAsyncResult(string item)
        {
        Task.Delay(1000);
        return Task.FromResult(item);
        }

        //Method to iterate over collection and await DoAsyncResult
        public static async Task<IEnumerable<string>> LoopAsyncResult(IEnumerable<string> thingsToLoop)
        {
        List<Task<string>> listOfTasks = new List<Task<string>>();

        foreach (var thing in thingsToLoop)
        {
        listOfTasks.Add(DoAsyncResult(thing));
        }

        return await Task.WhenAll<string>(listOfTasks);
        */

        #endregion LoopAsyncExample
    }
}
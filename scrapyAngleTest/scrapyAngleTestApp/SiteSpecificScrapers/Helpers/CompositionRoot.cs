﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using ScrapySharp.Network;
using SiteSpecificScrapers.DataflowPipeline;
using SiteSpecificScrapers.DataflowPipeline.RealTimeFeed;
using SiteSpecificScrapers.Messages;
using SiteSpecificScrapers.Services;

namespace SiteSpecificScrapers.Helpers
{
    public class CompositionRoot : IComposition
    {
        public string Url { get; set; }
        public List<string> InputList { get; set; }
        public Dictionary<string, bool> ScrapedKeyValuePairs { get; set; }
        public string SitemapUrl { get; set; }
        public ScrapingBrowser Browser { get; set; }
        public int PipeIndex { get; private set; } = 0;

        // readonly -> indicates that assignment to the field can only occur as part of the declaration or in a constructor in the same class
        private readonly ISiteSpecific[] _specificScrapers;

        public CompositionRoot(ScrapingBrowser browser, params ISiteSpecific[] scrapers)
        {
            _specificScrapers = scrapers;
            this.Browser = browser;
        }

        /* NOTES:
         * 1.2. respect politeness policy --delay requests to single domain ..., and NEVER make async requests to same domain (only async other site scrapers)
         * 1.3. make QUEUES SPECIFIC to  sites --ONE QUEUE per site !!
         * 1.4 run scraper and await task completion...than run next one
         */

        protected async Task InitPipeline(ISiteSpecific scraper)
        {
            //TODO:  await completion , than start next scraper (in future if i have more threads ...can make few pipes run in parallel as well)
            var cts = new CancellationTokenSource();
            // init
            var pipeline = new DataflowPipelineClass(Browser, scraper, new RealTimePublisher(), new DataConsumer(), new HubConnectionBuilder());

            Task pipelineTask = Task.Run(async () =>
            {
                try
                {
                    await pipeline.StartPipelineAsync(cts.Token);   //ORDER IS ---> Start consuming ---> ConsumeWithDIscard -->
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Pipeline {PipeIndex} terminated due to error {ex}");
                }
                Console.WriteLine($"Pipe -->[{++PipeIndex}] done processing Messages!");
            });

            await pipelineTask;
        }

        /// <summary>
        /// Runs SINGLE synchronous pipe & await completion , than run next.
        /// </summary>
        /// <returns></returns>
        public void RunAll()
        {
            foreach (ISiteSpecific scraper in _specificScrapers)
            {
                Console.WriteLine($"Scraper [{scraper.Url}] started:");
                try
                {
                    Task task = Task.Run(async () => await InitPipeline(scraper)).ContinueWith((i) => Console.WriteLine("All scrapers completed. [EXITING] Scraper now."));
                    //NOTE: Left InitPipeline async ...so i can reuse it for RunAllAsync
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Runs multiple pipeline's in parallel(not supported yet since i dont have that many threads for this to be efficient.)
        /// </summary>
        /// <returns></returns>
        public async Task<List<Task<Message>>> RunAllAsync()
        {
            //List of completed tasks
            List<Task<Message>> tasklist = new List<Task<Message>>();
            //Run each scraper in parellel
            foreach (ISiteSpecific scraper in _specificScrapers)
            {
                try
                {
                    await InitPipeline(scraper); //I ONLY WANT 1 PIPE FOR NOW (RUN MSG PASSING INSIDE PIPE ASYNC INSTEAD + MAKE ANOTHER SYNC VESION OF "RunAll" since i dont async run pipes atm)

                    //Await completion , than go to next Task
                    //var completedTask = await scraper.Run(browser);
                    ////Run each scraper async
                    //tasklist.Add(scraper.Run(Browser)); //TODO:  call awaitAll() just for testing 2,3 sites , else catch and store/print results as they arive .
                    //await Task.Run(async () => await scraper.Run(Browser));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            //return await Task.WhenAll<Task>(tasklist); not efficient to wait on all to complete , instead await and print/outptut each result as they arrive
            //
            return await Task.FromResult(tasklist); //TODO: see "NEXT STEP" bellow
        }

        //*** Task.WhenAll -->asynchronously awaits the result. calling Task.WaitAll blocks the calling thread until all tasks are completed

        /// ildasm ...> <see cref="https://www.youtube.com/watch?v=eZFtSwh0k4E&list=PLRwVmtr-pp05brRDYXh-OTAIi-9kYcw3r&index=20&frags=wn"/>

        /*NEXT STEP
         * Because Task and Task<TResult> are reference types, memory allocation in performance-critical paths,
         * particularly when allocations occur in tight loops, can adversely affect performance.
         * Support for generalized return types means that you can return a lightweight value type instead of a reference type to avoid additional memory allocations.
         /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/async-return-types" Generalized async return types -at the bottom of page />
         /// ValueTask--->> <see   https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1?view=netcore-3.1
         ///for less memory alocation (non reference & return types <see cref="https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1?view=netcore-3.0"/>
         */

        /// <remarks The task-Result property is a blocking property. ></remarks>
        /// In most cases, you should access the value by using await instead of accessing the property directly.
        /// exceptions <see cref="https://markheath.net/post/async-antipatterns"/>
        ///
        /// Task.Run(()=> func) arhitecutre <see cref="https://stackoverflow.com/questions/25720977/return-list-from-async-await-method"/>
        ///
        /// multyple web requests async <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/how-to-make-multiple-web-requests-in-parallel-by-using-async-and-await"/>

        ///  yields back to the current context <see cref="https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.yield?view=netframework-4.8"/>
        ///

        ///For ERROR metadata file <see cref="https://stackoverflow.com/questions/1421862/metadata-file-dll-could-not-be-found"/>
        ///
        //Parallel.Foreach & For are blocking (like built in await all)... they block code execution untill loop is done iterating !!!

        #region LoopAsyncExample

        /*
         * //Async method to be awaited
         *
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
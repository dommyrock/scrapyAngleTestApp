using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteSpecificScrapers.Services
{
    interface IComposition
    {
        Task<List<Task>> RunAll(ScrapingBrowser browser);
    }
}
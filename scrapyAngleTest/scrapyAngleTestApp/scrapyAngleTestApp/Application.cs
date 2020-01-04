using SiteSpecificScrapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scrapyAngleTestApp
{
    //Cant do this in Program.cs because its static and HAS NO CONSTRUCTOR! (thats why i created helper class)
    public class Application : IApplication
    {
        ISiteSpecific _siteSpecific;

        public Application(ISiteSpecific siteSpecific)
        {
            _siteSpecific = siteSpecific;
        }

        /// <summary>
        ///(Helper class for DI) Called from Program.cs
        /// </summary>
        public void Run()
        {
            //_siteSpecific.ScrapeSitemapLinks(); temp commented while testing reflections
        }
    }
}
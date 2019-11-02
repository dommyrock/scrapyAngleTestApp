using ScrapySharp.Extensions;
using ScrapySharp.Network;
using SiteSpecificScrapers.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSpecificScrapers
{
    public class AdmScraper : BaseScraperClass
    {
        public override string Url { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override List<string> InputList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override Dictionary<string, bool> ScrapedKeyValuePairs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool HasSitemap { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override ScrapingBrowser Browser { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// True if Success(Adds webshop urls to "InputList".) / False if no sitemap has been found in robots.txt
        /// </summary>
        public async Task<bool> ScrapeSitemapLinks()
        {
            HasSitemap = await base.GetSitemap(Browser);

            if (HasSitemap)
            {
                WebPage document = await Browser.NavigateToPageAsync(new Uri(Url));//might replace with basic downloadstrignasync...
                //Specific  query for nabava.net
                var nodes = document.Html.CssSelect("loc").ToList();

                foreach (var node in nodes)
                {
                    InputList.Add(node.InnerText);
                    Console.WriteLine(node.InnerText + "\n");
                }

                InputList.RemoveAt(0);
                Url = InputList[0];
                return HasSitemap;//true
            }
            return HasSitemap;//false
        }
    }
}

//TODO: Redesign my child classes --derived from "BaseScraper" class ...make some kind of interface/container that captures them all
//Othervise i will have Main ,flooded by instances of child classes ??
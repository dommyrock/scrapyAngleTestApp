﻿using AngleSharp;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using SiteSpecificScrapers.BaseClass;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSpecificScrapers
{
    public class NabavaNetSitemap : BaseScraperClass, ISiteSpecific
    {
        public string Url { get; set; }
        public List<string> InputList { get; set; }
        public string SitemapUrl { get; set; }
        public ScrapingBrowser Browser { get; set; }
        public Dictionary<string, bool> ScrapedKeyValuePairs { get; set; }
        private List<string> WebShops { get; set; }

        public NabavaNetSitemap()
        {
            this.Url = "http://nabava.net";
            InputList = new List<string>(); //TODO:change to be same instance from main else i lose this
        }

        //This class ment to only fetch sitemap (GetSitemap method --->

        /// <summary>
        /// True if Success(Adds webshop urls to "InputList".) / False if no sitemap has been found in robots.txt
        /// </summary>
        public async Task<bool> ScrapeSitemapLinks(ScrapingBrowser browser)
        {
            this.Browser = browser;

            SitemapUrl = await base.GetSitemap(Browser, Url);

            if (SitemapUrl != string.Empty)
            {
                WebPage document = await Browser.NavigateToPageAsync(new Uri(SitemapUrl));//might replace with basic downloadstrignasync...

                //Specific  query for nabava.net
                var nodes = document.Html.CssSelect("loc").Select(i => i.InnerText).ToList();
                InputList.AddRange(nodes);
                //old
                //foreach (var node in nodes)//Eventially replace this with parallel.foreach when app is completed
                //{
                //    InputList.Add(node.InnerText);
                //    //Console.WriteLine(node.InnerText + "\n");
                //}

                InputList.RemoveAt(0);
                Url = InputList[0];
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds webshops scraped from sitemap to "WebShops" list.
        /// </summary>
        public async void ScrapeWebshops()
        {
            while (true)
            {
                WebPage pageSource = await Browser.NavigateToPageAsync(new Uri(Url));//can speed this up by using DownloadStringAsync(but need other filter to extract shop link (regex))

                var node = pageSource.Html.SelectSingleNode("//b");//get<b> node/Link
                if (node != null)
                {
                    bool isLink = node.InnerHtml.StartsWith("http");
                    if (isLink)
                    {
                        //InputList.Add(node.InnerHtml);
                        WebShops.Add(node.InnerHtml);//add to separate "Shop" list
                        //Reasign Url to real link
                        Url = node.InnerHtml;
                        Console.WriteLine(node.InnerHtml);
                        //break; dont have loop here so i dont need it !!!
                    }
                }
                else
                {
                    Console.WriteLine($"All [{WebShops.Count}] Shops scraped from nabava.net/sitemap.xml \n");

                    //Set url to [0] item in "Webshop" queue before exiting
                    Url = WebShops[0];
                    break;//to skip bellow code & exit while
                }

                if (!ScrapedKeyValuePairs.ContainsKey(Url))//TODO : use Hash(set)  or concurrent collection
                {
                    ScrapedKeyValuePairs.Add(Url, true);//added scraped links from sitemap here...
                }

                InputList.RemoveAt(0);//remove scraped links from sitemap
                Url = InputList[0];
            }
        }

        public Task<bool> ScrapeSiteLinks()
        {
            throw new NotImplementedException();
        }
    }
}
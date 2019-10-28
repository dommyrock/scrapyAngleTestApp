using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace scrapyAngleTestApp
{
    class Program
    {
        public static string Url { get; set; }
        public static ScrapingBrowser Browser { get; set; }
        public static bool IsActiveNabavaScrape { get; private set; } = true;
        public static List<string> InputList { get; set; }
        public static List<string> WebShops { get; set; }
        public static Dictionary<string, bool> ScrapedDictionary { get; set; }

        // ALWAYS CHECK FOR " robots.txt" BEFORE SCRAPING WHOLE PAGE !

        static void Main(string[] args)
        {
            Url = "http://nabava.net";
            ScrapedDictionary = new Dictionary<string, bool>();
            InputList = new List<string>();
            WebShops = new List<string>();

            //FetchAbrakadabra(); comented for testing
            Browser = new ScrapingBrowser();//class also has async methods for fetching url's

            try
            {
                //NabavaSitemapScrape();

                ////Scrape children
                //while (IsActiveNabavaScrape)
                //{
                //    if (!ScrapedDictionary.ContainsKey(Url))
                //    {
                //        //Extract  shop links here (exit ,nabava.net after nodes.count =0)(continue scraping nabava.net)
                //        var pageSource = Browser.NavigateToPage(new Uri(Url));

                //        #region Extract shop Links

                //        var nodes = pageSource.Html.SelectNodes("//b");//get<b> nodes
                //        if (nodes != null)
                //        {
                //            foreach (var node in nodes)
                //            {
                //                bool isLink = node.InnerHtml.StartsWith("http");
                //                if (isLink)
                //                {
                //                    //InputList.Add(node.InnerHtml);
                //                    WebShops.Add(node.InnerHtml);//add to separate "Shop" list
                //                    Console.WriteLine(node.InnerHtml);
                //                    break;
                //                }
                //            }
                //        }
                //        else
                //        {
                //            Console.WriteLine($"All [{WebShops.Count}] Shops scraped from nabava.net/sitemap.xml \n");
                //            Console.WriteLine($"Remaining links in queue: {InputList.Count}");
                //            //Exit out of nabava.net/sitemap
                //            InputList = null;
                //            IsActiveNabavaScrape = false;

                //            //Set url to [0] item in "Webshop" queue before exiting
                //            Url = WebShops[0];
                //            break;
                //        }

                //        #endregion Extract shop Links
                //    }

                //    ScrapedDictionary.Add(Url, true);//added scraped links from sitemap here...
                //    InputList.RemoveAt(0);//remove scraped links from sitemap
                //    Url = InputList[0];
                //}

                //Start scraping Webshops Queue (check if shop has sitemap ...If it does scrape sitemap, else scrape whole site)

                //TODO: make bellow code into while loop as well , and add else "true" that scrapes sitemap data !!!!!!
                //NOTE : ....could make some kind of enum ...that has values for "SelectNodes()" based on current Url crawled !!!!!!!!!!??????

                //temp set url to adm
                Url = "https://www.adm.hr";
                if (!GetSitemap())
                {
                    var webshopPage = Browser.NavigateToPageAsync(new Uri(Url));

                    //adm.hr nodes
                    //var admNodes = webshopPage.Result.Html.CssSelect("li.subitems"); select all <li>
                    var admNodes = webshopPage.Result.Html.SelectNodes("//li[@class='subitems']/a").Select(a => a.Attributes[0].Value); //select all "href's" in <a> inside <li>

                    List<string> updatedList = new List<string>();

                    Console.WriteLine("\n --------------------------------------------------------------------");

                    //CONSRUCT VALID LINKS
                    var regex = new Regex(Regex.Escape("&amp;"));
                    //  string updated = Regex.Replace(link, @"(\s|&amp;)", "", RegexOptions.Compiled);  replaces both w ""
                    foreach (var link in admNodes)
                    {
                        //replace 1st one with ""
                        var updateOne = regex.Replace(link, "", 1);
                        //Replace 2nd one with &
                        var updateTwo = regex.Replace(updateOne, "&", 1);

                        updatedList.Add(updateTwo);
                        Console.WriteLine(updateTwo);
                    }
                    //CONTINUE SCRAPING FROM "updatedlsit"
                    var url = "";
                    foreach (var item in updatedList)
                    {
                        url = item;
                        webshopPage = Browser.NavigateToPageAsync(new Uri(url));

                        var pNodes = webshopPage.Result.Html.SelectNodes("//div[@class='lista col-12  mb-3']/div"); //select all "href's" in <a> inside <li> col-12 col-md-4 right-block d-flex flex-column align-items-center justify-content-center
                        //var innerNode = pNodes.Descendants();
                        //var css = webshopPage.Result.Html.CssSelect("div.col-12 col-md-4 right-block d-flex flex-column align-items-center justify-content-center >p"); not working  ...
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Console.ReadLine();

            //get user agent if necessary
            //get user agent
            //var useragent = Browser.UserAgent;
            //urlsList.AddRange(goTo.GetResourceUrls());//gets File url's
            //             var link2 = admNode.Descendants("a");
            //var link2 = admNode.DescendantsAndSelf("a");
        }

        /// <summary>
        /// Global method for scraping sitmap. Returns false if it doesnt exist!
        /// </summary>
        /// <returns></returns>
        public static bool GetSitemap()
        {
            //Check for /robots.txt
            string sitemapSource = Url + "/robots.txt";
            WebPage source = Browser.NavigateToPage(new Uri(sitemapSource));

            //Regex -get
            var matcheSitemap = Regex.Match(source.Html.InnerHtml, @"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (matcheSitemap.Success && matcheSitemap.Value.Contains("sitemap"))
            {
                Url = matcheSitemap.Value;
                return true;
            }
            return false;
        }

        public static void NabavaSitemapScrape()
        {
            ScrapedDictionary.Add(Url, true);
            GetSitemap();
            WebPage source = Browser.NavigateToPage(new Uri(Url));
            //Specific  query for nabava.net
            var nodes = source.Html.CssSelect("loc").ToList();
            foreach (var node in nodes)
            {
                //scrapedList.Add(node.InnerText); //Todo: replace with dictionary or hashset .+ check if item already exists
                InputList.Add(node.InnerText);
                Console.WriteLine(node.InnerText + "\n");
            }
            InputList.RemoveAt(0);
            Url = InputList[0];
        }

        //Test method for specific site
        public static void FetchAbrakadabra()
        {
            //TODO : check for robots.txt before scraping site !!!!!(this site has it !!)

            string url = "https://www.abrakadabra.com/hr-HR/Katalog/TV%2C-mobiteli-i-elektronika/Televizori-i-dodaci/c/FE100";//it has robots.txt--(and in it is "sitemap.xml")
            //Fetch
            WebPage source = Browser.NavigateToPage(new Uri(url));

            //Take only script  node with dataLayer inside it (temp solution-specific to to the site)
            HtmlNode dataLayerNode = source.Html.CssSelect("script").Skip(1).Take(1).SingleOrDefault();

            //serialize JSON to C# object with Newtonsoft
            Article article = new Article(dataLayerNode.InnerText);
        }
    }
}

public class Article
{
    public string Name { get; set; }
    public int Id { get; set; }
    public string Brand { get; set; }
    public int Price { get; set; }
    public string Category { get; set; }
    public string CurrencyCode { get; set; }

    public Article(string json)
    {
        //Serialization cancled atm ..might reuse later!!
        JObject jObject = JObject.Parse(json);//Unexpected character encountered while parsing value
        JToken shopObject = jObject["article"];
        Id = (int)shopObject["id"];
        Name = (string)shopObject["name"];
        Brand = (string)shopObject["brand"];
        Price = (int)shopObject["price"];
        Category = (string)shopObject["category"];
        CurrencyCode = (string)shopObject["currencyCode"];
    }
}
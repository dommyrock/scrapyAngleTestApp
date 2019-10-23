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
        public static bool IsActive { get; private set; } = true;
        public static List<string> InputList { get; set; }
        public static List<string> Shops { get; set; }
        public static Dictionary<string, bool> ScrapedDictionary { get; set; }

        // ALWAYS CHECK FOR " robots.txt" BEFORE SCRAPING WHOLE PAGE !

        static void Main(string[] args)
        {
            Url = "http://nabava.net";
            ScrapedDictionary = new Dictionary<string, bool>();
            InputList = new List<string>();
            Shops = new List<string>();

            //FetchAbrakadabra(); comented for testing
            Browser = new ScrapingBrowser();//class also has async methods for fetching url's

            try
            {
                NabavaSitemapScrape();

                //Than scrape children
                while (IsActive)
                {
                    if (!ScrapedDictionary.ContainsKey(Url))
                    {
                        //Extract  shop links here (exit ,nabava.net after nodes.count =0)(continue scraping nabava.net)
                        var pageSource = Browser.NavigateToPage(new Uri(Url));

                        #region Extract shop Links

                        var nodes = pageSource.Html.SelectNodes("//b");//get<b> nodes
                        if (nodes.Count > 0)
                        {
                            foreach (var node in nodes)
                            {
                                bool isLink = node.InnerHtml.StartsWith("http");
                                if (isLink)
                                {
                                    //InputList.Add(node.InnerHtml);
                                    Shops.Add(node.InnerHtml);//add to separate "Shop" list
                                    Console.WriteLine(node.InnerHtml);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"All [{Shops.Count}] Shops scraped from nabava.net/sitemap.xml");
                        }

                        #endregion Extract shop Links
                    }
                    ScrapedDictionary.Add(Url, true);//added scraped links from sitemap here...
                    InputList.RemoveAt(0);//remove scraped links from sitemap
                    Url = InputList[0];
                }
            }
            catch (Exception ex)//TODO : exception happens after shops are scraped when inkom shop is scraped.
            {
                IsActive = false;//temp ...make log instead
                throw ex;
            }

            Console.ReadLine();

            //get user agent if necessary
            //get user agent
            //var useragent = Browser.UserAgent;
            //urlsList.AddRange(goTo.GetResourceUrls());//gets File url's
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
            //Specific  query for nabava.net
            WebPage source = Browser.NavigateToPage(new Uri(Url));
            var nodes = source.Html.CssSelect("loc").ToList();//gets all links from http://nabava.net/sitemap.xml)-(make specific for each one for more precission or global query?? )
            foreach (var node in nodes)
            {
                //scrapedList.Add(node.InnerText); //Todo: replace with dictionary or hashset .+ check if item already exists
                InputList.Add(node.InnerText);
                Console.WriteLine(node.InnerText + "\n");
            }
            InputList.RemoveAt(0);
            Url = InputList[0];
        }

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

//TODO: plan....(since nabava.net already has links to other webshops in its sitemap...
//i can add links to scrape next from it 
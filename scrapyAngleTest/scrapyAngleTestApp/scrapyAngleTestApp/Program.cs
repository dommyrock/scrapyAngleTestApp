using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace scrapyAngleTestApp
{
    class Program
    {
        public static string Url { get; set; }
        public static ScrapingBrowser Browser { get; set; }

        // ALWAYS CHECK FOR " robots.txt" BEFORE SCRAPING WHOLE PAGE !

        static void Main(string[] args)
        {
            Url = "http://nabava.net";
            List<string> urlsList = new List<string>();

            //get user agent
            //var useragent = Browser.UserAgent;
            //urlsList.AddRange(goTo.GetResourceUrls());//gets File url's

            //FetchAbrakadabra(); comented for testing
            Browser = new ScrapingBrowser();
            if (GetSitemap())
            {
                WebPage source = Browser.NavigateToPage(new Uri(Url));
                var nodes = source.Html.CssSelect("loc").ToList();//gets all links(source http://nabava.net/sitemap.xml)

                foreach (var node in nodes)
                {
                    urlsList.Add(node.InnerText);
                    Console.WriteLine(node.InnerText + "\n");
                }
            }

            Console.ReadLine();
        }

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
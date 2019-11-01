using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace scrapyAngleTestApp
{
    class Program
    {
        public static string Url { get; set; }
        public static ScrapingBrowser Browser { get; set; }

        // ALWAYS CHECK FOR " robots.txt" BEFORE SCRAPING WHOLE PAGE !

        static void Main(string[] args)
        {
            var url = "http://nabava.net/robots.txt";
            List<string> urlsList = new List<string>();

            Browser = new ScrapingBrowser();
            WebPage source = Browser.NavigateToPage(new Uri(url));

            var useragent = Browser.UserAgent;
            //urlsList.AddRange(goTo.GetResourceUrls());//gets File url's

            FetchAbrakadabra();

            if (GetSitemap(source))
            {
                WebPage source2 = Browser.NavigateToPage(new Uri(Url));
                var nodes = source2.Html.CssSelect("loc").ToList();//gets all links(source http://nabava.net/sitemap.xml)

                foreach (var node in nodes)
                {
                    urlsList.Add(node.InnerText);
                    Console.WriteLine(node.InnerText + "\n");
                }
            }

            Console.ReadLine();
        }

        public static bool GetSitemap(WebPage source)
        {
            var sitemap = source.Html.InnerHtml.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);//split separator == \n

            string sitemapFound = sitemap.Where(x => x.Contains("Sitemap")).SingleOrDefault().Split(' ')[1];
            if (sitemapFound != null)
            {
                Url = sitemapFound;
                return true;
            }
            return false;
        }

        public static void FetchAbrakadabra()
        {
            string url = "https://www.abrakadabra.com/hr-HR/Katalog/TV%2C-mobiteli-i-elektronika/Televizori-i-dodaci/c/FE100";
            //Fetch
            WebPage source = Browser.NavigateToPage(new Uri(url));

            //Take only script  node with dataLayer inside it (temp solution-specific to to the site)
            HtmlNode dataLayerNode = source.Html.CssSelect("script").Skip(1).Take(1).SingleOrDefault();

            //serialize JSON to C# object with Newtonsoft

            Article article = new Article(dataLayerNode.InnerText);
            List<Article> articles = article.DeserializeJSON();
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
    public string JSON { get; set; }

    public Article(string json)
    {
        this.JSON = json;
        //JObject jObject = JObject.Parse(json);//Unexpected character encountered while parsing value
        //JToken shopObject = jObject["article"];
        //Id = (int)shopObject["id"];
        //Name = (string)shopObject["name"];
        //Brand = (string)shopObject["brand"];
        //Price = (int)shopObject["price"];
        //Category = (string)shopObject["category"];
        //CurrencyCode = (string)shopObject["currencyCode"];
    }

    public List<Article> DeserializeJSON()
    {
        var deserialzedList = JsonConvert.DeserializeObject<List<Article>>(this.JSON, new JsonSerializerSettings//validate json format first before inserting it here
        {
            Formatting = Formatting.Indented,//not usefull
        });

        return deserialzedList;
    }
}
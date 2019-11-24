using Autofac;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using SiteSpecificScrapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace scrapyAngleTestApp
{
    class Program
    {
        public static string Url { get; set; }//TEmp ...remove after refactor
        public static ScrapingBrowser Browser { get; set; }
        public static bool IsActiveNabavaScrape { get; private set; } = true;
        public static List<string> InputList { get; set; }
        public static List<string> WebShops { get; set; }
        public static Dictionary<string, bool> ScrapedDictionary { get; set; }//refactor this in hashset ? or some other key -value pair (maybe concurrent ?), parallel.foreach , caching ...

        // ALWAYS CHECK FOR " robots.txt" BEFORE SCRAPING WHOLE PAGE !

        static void Main(string[] args)
        {
            string url = Url = "http://nabava.net";
            ScrapedDictionary = new Dictionary<string, bool>();
            InputList = new List<string>();
            WebShops = new List<string>();

            Browser = new ScrapingBrowser();//class also has async methods for fetching url's

            #region DI

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////7777
            /// <see cref="https://autofac.readthedocs.io/en/latest/getting-started/index.html#structuring-the-application"/>
            ///
            var container = ContainerConfig.Configure();//holds all our design/DI
            using (var scope = container.BeginLifetimeScope())
            {
                //Retrieve service (manualy get iapplication ctx from container(not good for bigger projects)
                ///By default the type which is registered last will be returned, which is what you are seeing. <see cref="https://stackoverflow.com/questions/45674063/autofac-multiple-classes-inherited-from-the-same-interface"/>
                ///Solution : <see cref="https://stackoverflow.com/questions/22384884/autofac-with-multiple-implementations-of-the-same-interface"/>
                ISiteSpecific app = scope.Resolve<ISiteSpecific>();
                app.ScrapeSitemapLinks();

                //TODO: pass ISiteSpecific as param to constructor of each child class , so it "new It up's"/instantiates needed type(concrete class) -->looks inside container and finds type
            }

            #endregion DI

            try
            {
                NabavaNetSitemap nabavaSitemap = new NabavaNetSitemap(/*url, Browser, InputList, WebShops, ScrapedDictionary*/);

                //If ScrapeSitemapLinks = Success
                if (nabavaSitemap.ScrapeSitemapLinks().Result)
                {
                    #region AWS

                    //TODO cache or local store shops list for set amount of time , + if( data is Stale before re-scrape) -->in new Helper folder->Helpers class
                    //TEMP Store (final one will store shops in one of  AWS data stores--)check date "LastModifued" and if date > 48h ? scrape shops again ... and re-post to DB
                    var tempWebshopCache = new List<string> {
                        "https://www.adm.hr",
                        "https://www.abrakadabra.com",
                        "https://www.links.hr",
                    };

                    WebShops.AddRange(tempWebshopCache);

                    #endregion AWS

                    if (tempWebshopCache.Count == 0)
                    {
                        //re-scrape
                        nabavaSitemap.ScrapeWebshops();
                    }

                    //dispose
                    ///WE're exiting  <param name="http://nabava.net"/> at this point , so remove rest of the links from queue/list

                    InputList = null;
                    nabavaSitemap = null;// TODO : replace this since im using DI : its scope should kill instances (instead of me manualy )
                }
                else
                {
                    Console.WriteLine($"Failed to scrape {url} sitemap.");
                }

                //Continue scraping from "Webshops" here:
                url = WebShops[0];//check if needed after i exit nabavaSitemap.ScrapeWebshops();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            Console.ReadKey();
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////7777

            try//could use try catch finaly (try ->nabava sitemap,shops scrape , catch .., finaly-> scrape each shop (2nd nested try ,catch inside)
            {
                //Start scraping Webshops Queue (check if shop has sitemap ...If it does scrape sitemap, else scrape whole site)

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
                    var tempurl = "";
                    foreach (var item in updatedList)
                    {
                        tempurl = item;
                        webshopPage = Browser.NavigateToPageAsync(new Uri(tempurl));

                        var pNodes = webshopPage.Result.Html.SelectNodes("//div[@class='lista col-12  mb-3']/div"); //select all "href's" in <a> inside <li> col-12 col-md-4 right-block d-flex flex-column align-items-center justify-content-center

                        if (pNodes != null)
                        {
                            //Get child nodes
                            foreach (var node in pNodes)
                            {
                                //Trim text here and ad item to new List<string,string> (article,price) -->reuse Article Class on bottom ???
                                //var child = node.SelectNodes(".//div");//all 11 divs (not needed)
                                var achild = node.SelectNodes(".//div/a"); //all 4/6 outer <a> nodes
                                var pchild = node.SelectNodes(".//div/p");// <p> elements <p class="opis mt-auto"> &  <p class="product-price mt-2">
                                foreach (var p in pchild)//could trim p.innertext (because it copyes \n \r)
                                {
                                    Console.WriteLine(p.InnerText);
                                    //Trim text here and ad item to new List<string,string> (article,price) -->reuse Article Class on bottom ???
                                }
                            }
                            //TODO: remove current item from list when scraped ....(might have problem ... cant update "updateList" while iterating over it ???)
                            //might use for loop instead (https://stackoverflow.com/questions/16497028/how-add-or-remove-object-while-iterating-collection-in-c-sharp)
                        }
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
using Autofac;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using SiteSpecificScrapers;
using SiteSpecificScrapers.BaseClass;
using SiteSpecificScrapers.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace scrapyAngleTestApp
{
    class Program
    {
        #region Properties

        public static string Url { get; set; }//TEmp ...remove after refactor
        public static ScrapingBrowser Browser { get; set; }
        public static List<string> InputList { get; set; }
        public static List<string> WebShops { get; set; }

        //refactor this in hashset ? or some other key -value pair (maybe concurrent ?), parallel.foreach , caching ...
        public static Dictionary<string, bool> ScrapedDictionary { get; set; }

        #endregion Properties

        // ALWAYS CHECK FOR " robots.txt" BEFORE SCRAPING WHOLE PAGE !
        /// ildasm ...> <see cref="https://www.youtube.com/watch?v=eZFtSwh0k4E&list=PLRwVmtr-pp05brRDYXh-OTAIi-9kYcw3r&index=20&frags=wn"/>

        static void Main(string[] args)
        {
            string url = Url = "http://nabava.net";
            ScrapedDictionary = new Dictionary<string, bool>();
            InputList = new List<string>();
            WebShops = new List<string>();

            Browser = new ScrapingBrowser();// Check this class for reusable API

            #region DI(container) testing

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////7777
            /// <see cref="https://autofac.readthedocs.io/en/latest/getting-started/index.html#structuring-the-application"/>
            ///
            //var container = ContainerConfig.Configure();//holds all our design/DI
            //using (var scope = container.BeginLifetimeScope())
            //{
            //    //Retrieve service (manualy get iapplication ctx from container(not good for bigger projects)
            //    ///By default the type which is registered last will be returned, which is what you are seeing. <see cref="https://stackoverflow.com/questions/45674063/autofac-multiple-classes-inherited-from-the-same-interface"/>
            //    ///Solution : <see cref="https://stackoverflow.com/questions/22384884/autofac-with-multiple-implementations-of-the-same-interface"/>
            //    ISiteSpecific app = scope.Resolve<ISiteSpecific>();
            //    app.ScrapeSitemapLinks();

            //    app.ScrapeSitemapLinks();
            //}

            #endregion DI(container) testing

            //TODO:
            //4. (Also make new method for error handling ---> so if one "scraperClass" fails it continues scraping the rest)!!! (later add retry logic...)
            //5. since i have static instances of list , browser ..might be a problem to use async methods with them ??? (could ty concurent bags ...)(or new instances foreach async ) and store from each instance into concurent collection
            //6.  //TODO: Replace async void with Task (because caller of this method has no way to await it ) only valid in event handlers...
            /// <see cref="https://searchcode.com/codesearch/view/125929587/"/> for ScrapySharp async methods source code
            ///

            //Get all clases that implement ISiteSpecific (with Polymorphism)
            var compositionRoot = new CompositionRoot(
                new NabavaNetSitemap(),
                new AdmScraper(),
                new AbrakadabraScraper()
                );
            compositionRoot.RunAll(Browser);

            //TODO: move this code to specific classes
            try
            {
                NabavaNetSitemap nabavaSitemap = new NabavaNetSitemap();

                //If ScrapeSitemapLinks = Success
                if (nabavaSitemap.ScrapeSitemapLinks(Browser).Result)
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
                        //nabavaSitemap.ScrapeWebshops(); refactored this to be private
                    }

                    //dispose
                    ///WE're exiting  <param name="http://nabava.net"/> at this point , so remove rest of the links from queue/list

                    InputList = null;
                    nabavaSitemap = null;
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

            //get user agent
            //var useragent = Browser.UserAgent;
            //urlsList.AddRange(goTo.GetResourceUrls());//gets File url's
            //             var link2 = admNode.Descendants("a");
            //var link2 = admNode.DescendantsAndSelf("a");
        }

        #region Old Methods for site scraping

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

        #endregion Old Methods for site scraping
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
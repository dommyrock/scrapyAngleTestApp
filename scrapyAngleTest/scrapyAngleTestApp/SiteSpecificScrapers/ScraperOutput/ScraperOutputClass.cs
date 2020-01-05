using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteSpecificScrapers.Output
{
    /// <summary>
    /// Contains scraped articles,prices, availibility ,img urls ...
    /// </summary>
    public class ScraperOutputClass
    {
        public ScraperOutputClass()
        {
        }

        //might reuse some of old Article class
        /*
         *
         *
         * public class Article
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
         *
         *
         *
         *
         */
    }
}
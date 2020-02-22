using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace SiteSpecificScrapers.Helpers
{
    public static class NewtonsoftExtension
    {
        /// <summary>
        ///  [SET fromCache = false for fresh scrape!] Gets webshops from local folder
        /// </summary>
        /// <param name="fromCache"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<string> GetFromLocalCache(bool fromCache = true, string fileName = "webshopCache.json")
        {
            string fullpath = Path.GetFullPath(fileName);

            using (StreamReader file = new StreamReader(fullpath))
            {
                string json = file.ReadToEnd();
                if (json != string.Empty && fromCache)
                    return JsonConvert.DeserializeObject<List<string>>(json);
                return new List<string>(); //replace with param webshop list and return same list passed
            }
        }

        /// <summary>
        /// Caches to local bin-debug folder [if you want to cache in memory use "Lazy Cache" nuget]
        /// </summary>
        /// <param name="webshops">Shop list</param>
        /// <param name="fileName">Local file name</param>
        public static void CacheWebshopsToLocalCache(List<string> webshops, string fileName = "webshopCache.json")
        {
            string fullpath = Path.GetFullPath(fileName);

            var json = JsonConvert.SerializeObject(webshops, Formatting.Indented);
            using (StreamWriter file = File.CreateText(fullpath))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                writer.WriteRaw(json);
            }
        }
    }
}

//For generic type extension method see https://stackoverflow.com/questions/4637383/extension-method-return-using-generics
//docs https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods
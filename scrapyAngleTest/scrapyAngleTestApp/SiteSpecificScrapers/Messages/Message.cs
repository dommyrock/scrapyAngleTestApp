using System;

namespace SiteSpecificScrapers.Messages
{
    public class Message
    {
        public string SourceHtml { get; set; }
        public string SiteUrl { get; set; }
        public int Id { get; set; }
        public string Brand { get; set; }
        public int Price { get; set; }
        public string Category { get; set; }
        public string CurrencyCode { get; set; }
        public string JSON { get; set; }
        public DateTime ReadingTime { get; set; }
    }
}
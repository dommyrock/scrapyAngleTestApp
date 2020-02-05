using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteSpecificScrapers.Messages
{
    public class ProcessedMessage
    {
        public string SourceHtml { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string Brand { get; set; }
        public int Price { get; set; }
        public string Category { get; set; }
        public string CurrencyCode { get; set; }
        public string JSON { get; set; }
        public DateTime ReadingTime { get; set; }
    }
}
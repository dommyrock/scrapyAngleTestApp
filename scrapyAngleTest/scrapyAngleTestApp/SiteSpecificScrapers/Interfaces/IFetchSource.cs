using ScrapySharp.Network;
using System.Threading.Tasks;

namespace SiteSpecificScrapers.Interfaces
{
    public interface IFetchSource
    {
        //Note: We inject scrapers into dataflowPipeline , therefore we have access to all its methods and baseScraperClass like "GetSitemap"
        Task<WebPage> NavigateToPage(string site);
    }
}
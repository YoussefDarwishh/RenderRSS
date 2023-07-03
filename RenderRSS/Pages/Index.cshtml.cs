using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml;
using System.Xml.Linq;

namespace RenderRSS.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public List<FeedItem> FeedItems { get; set; } = new List<FeedItem>();

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await FetchXmlContentAsync(httpClient, "http://scripting.com/rss.xml");

            if (response.IsSuccessStatusCode)
            {
                var xmlContent = await response.Content.ReadAsStringAsync();
                FeedItems = ParseXmlContent(xmlContent);

                return Page();
            }
            else
            {
                return RedirectToPage("/Error");
            }
        }

        async Task<HttpResponseMessage> FetchXmlContentAsync(HttpClient httpClient, string url)
        {
            return await httpClient.GetAsync(url);
        }

        List<FeedItem> ParseXmlContent(string xmlContent)
        {
            var feedItems = new List<FeedItem>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);

            XmlNodeList channelNodes = doc.GetElementsByTagName("channel");
            XmlNodeList itemNodes = channelNodes[0].SelectNodes("item");

            foreach (XmlNode itemNode in itemNodes)
            {
                FeedItem feedItem = new FeedItem();

                feedItem.Title = itemNode.SelectSingleNode("title")?.InnerText ?? string.Empty;
                feedItem.Description = itemNode.SelectSingleNode("description")?.InnerText ?? string.Empty;
                feedItem.PubDate = DateTime.Parse(itemNode.SelectSingleNode("pubDate")?.InnerText ?? string.Empty);
                feedItem.Link = itemNode.SelectSingleNode("link")?.InnerText ?? string.Empty;

                feedItems.Add(feedItem);
            }

            return feedItems;
        }
    }

    public class FeedItem
    {
        public string? Title { get; set; }
        public string Description { get; set; }
        public DateTime PubDate { get; set; }
        public string Link { get; set; }
    }
}

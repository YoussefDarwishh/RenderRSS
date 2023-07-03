using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml;
using System.Xml.Linq;

namespace RenderRSS.Pages
{
    public class IndexModel : PageModel
    {
        public List<FeedItem> FeedItems { get; set; } = new List<FeedItem>();

        public async Task<IActionResult> OnGetAsync()
        {
            var xmlContent = await FetchXmlContentAsync("http://scripting.com/rss.xml");
            FeedItems = ParseXmlContent(xmlContent);

            return Page();
        }

        async Task<string> FetchXmlContentAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                return await httpClient.GetStringAsync(url);
            }
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
        public string  Description { get; set; }
        public DateTime  PubDate { get; set; }
        public string  Link { get; set; }
    }
}

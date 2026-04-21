using System.Text.Json;

namespace ComicApp.Core.Services
{
    public class ComicVineResult
    {
        public string? Name { get; set; }
        public string? Deck { get; set; }
        public string? SiteDetailUrl { get; set; }
        public ImageResult? Image { get; set; }
    }

    public class ImageResult
    {
        public string? MediumUrl { get; set; }
    }

    public class ComicVineService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public ComicVineService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<List<ComicVineResult>> SearchAsync(string query)
        {
            string url = $"https://comicvine.gamespot.com/api/search/?api_key={_apiKey}&format=json&resources=volume&query={Uri.EscapeDataString(query)}&field_list=name,deck,site_detail_url,image&limit=5";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "FBZComicApp/1.0");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<ComicVineResult>();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            var results = new List<ComicVineResult>();
            if (doc.RootElement.TryGetProperty("results", out var resultsArray))
            {
                foreach (var item in resultsArray.EnumerateArray())
                {
                    var result = new ComicVineResult
                    {
                        Name = item.TryGetProperty("name", out var name) ? name.GetString() : null,
                        Deck = item.TryGetProperty("deck", out var deck) ? deck.GetString() : null,
                        SiteDetailUrl = item.TryGetProperty("site_detail_url", out var url2) ? url2.GetString() : null,
                    };

                    if (item.TryGetProperty("image", out var imageObj))
                    {
                        result.Image = new ImageResult
                        {
                            MediumUrl = imageObj.TryGetProperty("medium_url", out var imgUrl) ? imgUrl.GetString() : null
                        };
                    }

                    results.Add(result);
                }
            }

            return results;
        }
    }
}
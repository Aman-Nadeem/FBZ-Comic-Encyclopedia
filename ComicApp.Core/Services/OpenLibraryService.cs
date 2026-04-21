using System.Text.Json;

namespace ComicApp.Core.Services
{
    public class OpenLibraryBook
    {
        public string? Title { get; set; }
        public string? CoverUrl { get; set; }
        public List<string> Authors { get; set; } = new();
        public int? FirstPublishYear { get; set; }
        public string? OpenLibraryUrl { get; set; }
    }

    public class OpenLibraryService
    {
        private readonly HttpClient _httpClient;

        public OpenLibraryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<OpenLibraryBook>> SearchAsync(string query)
        {
            string url = $"https://openlibrary.org/search.json?q={Uri.EscapeDataString(query)}&limit=5";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "FBZComicApp/1.0");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<OpenLibraryBook>();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            var results = new List<OpenLibraryBook>();

            if (doc.RootElement.TryGetProperty("docs", out var docs))
            {
                foreach (var item in docs.EnumerateArray())
                {
                    var book = new OpenLibraryBook();

                    if (item.TryGetProperty("title", out var title))
                        book.Title = title.GetString();

                    if (item.TryGetProperty("first_publish_year", out var year))
                        book.FirstPublishYear = year.GetInt32();

                    if (item.TryGetProperty("author_name", out var authors))
                    {
                        foreach (var author in authors.EnumerateArray())
                        {
                            var a = author.GetString();
                            if (a != null) book.Authors.Add(a);
                        }
                    }

                    if (item.TryGetProperty("cover_i", out var coverId))
                    {
                        book.CoverUrl = $"https://covers.openlibrary.org/b/id/{coverId.GetInt32()}-M.jpg";
                    }

                    if (item.TryGetProperty("key", out var key))
                    {
                        book.OpenLibraryUrl = $"https://openlibrary.org{key.GetString()}";
                    }

                    results.Add(book);
                }
            }

            return results;
        }
    }
}
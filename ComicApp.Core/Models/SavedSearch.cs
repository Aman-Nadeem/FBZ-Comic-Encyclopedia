namespace ComicApp.Core.Models
{
    public class SavedSearch
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Query { get; set; } = string.Empty;

        public List<string> ComicIds { get; set; } = new();

        public string? Username { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}

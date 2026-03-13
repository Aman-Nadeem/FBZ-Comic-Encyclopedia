using ComicApp.Core.Interfaces;
using ComicApp.Core.Models;
using ComicApp.Core.DataAccess;
namespace ComicApp.Core.Services
{
    public class SavedSearchService : ISavedSearchService
    {
        private readonly AppDbContext _context;
        public SavedSearchService(AppDbContext context)
        {
            _context = context;
        }
        public void SaveSearch(string query, IEnumerable<Comic> results, string? username = null)
        {
            var search = new SavedSearch
            {
                Query = query,
                ComicIds = results.Select(r => r.BlRecordId ?? "").ToList(),
                Username = username,
                Created = DateTime.UtcNow
            };
            _context.SavedSearches.Add(search);
            _context.SaveChanges();
        }
        public IReadOnlyList<SavedSearch> GetSavedSearches(string? username)
        {
            return _context.SavedSearches
                .Where(s => s.Username == username)
                .OrderByDescending(s => s.Created)
                .ToList();
        }
    }
}
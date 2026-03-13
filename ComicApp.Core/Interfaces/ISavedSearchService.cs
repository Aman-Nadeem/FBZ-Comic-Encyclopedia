using ComicApp.Core.Models;
namespace ComicApp.Core.Interfaces
{
    public interface ISavedSearchService
    {
        void SaveSearch(string query, IEnumerable<Comic> results, string? username = null);
        IReadOnlyList<SavedSearch> GetSavedSearches(string? username);
    }
}
using ComicApp.Core.Interfaces;
using ComicApp.Core.Models;

namespace ComicApp.Tests
{
    // STUB: Simulates search service without real filtering logic
    // Used to test controllers and UI in isolation
    public class StubComicSearchService : IComicSearchService
    {
        public IEnumerable<Comic> BasicTitleSearch(IEnumerable<Comic> comics, string titleTerm)
        {
            return comics.Where(c =>
                c.Title != null &&
                c.Title.Contains(titleTerm, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Comic> AdvancedSearch(
            IEnumerable<Comic> comics,
            string? title,
            string? author,
            int? year,
            string? genre,
            string? language,
            string? nameType)
        {
            return comics.ToList();
        }

        public IEnumerable<IGrouping<string, Comic>> GroupByAuthor(IEnumerable<Comic> comics)
        {
            return comics.GroupBy(c => c.Creators.FirstOrDefault()?.Name ?? "Unknown");
        }

        public IEnumerable<IGrouping<int?, Comic>> GroupByYear(IEnumerable<Comic> comics)
        {
            return comics.GroupBy(c => c.PublicationYear);
        }

        public void RegisterSearch(string queryKey, IEnumerable<Comic> results) { }

        public IReadOnlyList<(string Query, int Count)> GetTopQueries(int top)
        {
            return new List<(string, int)>
            {
                ("Batman", 5),
                ("Watchmen", 3)
            };
        }

        public IReadOnlyList<(string ComicId, int Count)> GetTopResults(int top)
        {
            return new List<(string, int)>
            {
                ("1", 10),
                ("2", 7)
            };
        }

        public IEnumerable<string> GetComicIdsWithMoreThan(int minResultCount)
        {
            return new List<string>();
        }

        public IReadOnlyList<(string Query, int Count)> GetTop10Queries()
        {
            return new List<(string, int)>
            {
                ("Batman", 5),
                ("Watchmen", 3)
            };
        }

        public IReadOnlyList<(string ComicId, int Count)> GetTop10Results()
        {
            return new List<(string, int)>
            {
                ("1", 10),
                ("2", 7)
            };
        }

        public IEnumerable<string> GetComicsAppearingInMoreThan100Searches()
        {
            return new List<string>();
        }
    }
}
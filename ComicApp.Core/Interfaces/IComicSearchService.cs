using System.Collections.Generic;
using ComicApp.Core.Models;

namespace ComicApp.Core.Interfaces
{
    // SRP: This interface ONLY defines searching + statistics behaviour.
    // DIP: UI layers depend on THIS instead of a concrete implementation.
    public interface IComicSearchService
    {
        IEnumerable<Comic> BasicTitleSearch(IEnumerable<Comic> comics, string titleTerm);


    IEnumerable<Comic> AdvancedSearch(
        IEnumerable<Comic> comics,
        string? title,
        string? author,
        int? year,
        string? genre,
        string? language,
        string? nameType);

        IEnumerable<IGrouping<string, Comic>> GroupByAuthor(IEnumerable<Comic> comics);
        IEnumerable<IGrouping<int?, Comic>> GroupByYear(IEnumerable<Comic> comics);

        // Statistics tracking for reporting (top searches, popular results)
        void RegisterSearch(string queryKey, IEnumerable<Comic> results);

        IReadOnlyList<(string Query, int Count)> GetTopQueries(int top);
        IReadOnlyList<(string ComicId, int Count)> GetTopResults(int top);
        IEnumerable<string> GetComicIdsWithMoreThan(int minResultCount);

        // -------- Dashboard helper methods --------
        // These simplify controller usage

        IReadOnlyList<(string Query, int Count)> GetTop10Queries();
        IReadOnlyList<(string ComicId, int Count)> GetTop10Results();
        IEnumerable<string> GetComicsAppearingInMoreThan100Searches();
    }


}

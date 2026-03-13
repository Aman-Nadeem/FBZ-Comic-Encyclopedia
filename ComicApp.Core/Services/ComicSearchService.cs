using System;
using System.Collections.Generic;
using System.Linq;
using ComicApp.Core.Models;
using ComicApp.Core.Interfaces;

namespace ComicApp.Core.Services
{
// SRP (Single Responsibility Principle)
// This class performs comic searches AND tracks search analytics.


// DIP (Dependency Inversion Principle)
// UI depends on IComicSearchService instead of this concrete class.

public class ComicSearchService : IComicSearchService
    {
        // Tracks analytics for reports dashboard
        //  • Top search queries
        //  • Top returned comics
        //  • Comics appearing in more than 100 searches

        private readonly Dictionary<string, int> _queryCounts = new();
        private readonly Dictionary<string, int> _resultCounts = new(); // key = BL Record ID


        // ================= BASIC TITLE SEARCH =================

        public IEnumerable<Comic> BasicTitleSearch(IEnumerable<Comic> comics, string titleTerm)
        {
            if (string.IsNullOrWhiteSpace(titleTerm))
                return Enumerable.Empty<Comic>();

            return comics
                .Where(c =>
                    !string.IsNullOrWhiteSpace(c.Title) &&
                    c.Title.Contains(titleTerm, StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c.Title);
        }


        // ================= ADVANCED SEARCH =================

        public IEnumerable<Comic> AdvancedSearch(
            IEnumerable<Comic> comics,
            string? title,
            string? author,
            int? year,
            string? genre,
            string? language,
            string? nameType)
        {
            var query = comics;

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(c =>
                    !string.IsNullOrWhiteSpace(c.Title) &&
                    c.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(author))
            {
                query = query.Where(c =>
                    c.Creators.Any(cr =>
                        !string.IsNullOrWhiteSpace(cr.Name) &&
                        cr.Name.Contains(author, StringComparison.OrdinalIgnoreCase)));
            }

            if (year.HasValue)
            {
                query = query.Where(c => c.PublicationYear == year.Value);
            }

            if (!string.IsNullOrWhiteSpace(genre))
            {
                query = query.Where(c =>
                    c.Genres.Any(g =>
                        g.Contains(genre, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrWhiteSpace(language))
            {
                query = query.Where(c =>
                    c.Languages.Any(l =>
                        l.Contains(language, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrWhiteSpace(nameType))
            {
                query = query.Where(c =>
                    c.Creators.Any(cr =>
                        !string.IsNullOrWhiteSpace(cr.TypeOfName) &&
                        cr.TypeOfName.Contains(nameType, StringComparison.OrdinalIgnoreCase)));
            }

            return query.OrderBy(c => c.Title).ToList();
        }


        // ================= GROUP BY AUTHOR =================

        public IEnumerable<IGrouping<string, Comic>> GroupByAuthor(IEnumerable<Comic> comics)
        {
            return comics
                .SelectMany(
                    c => c.Creators.DefaultIfEmpty(new Creator { Name = "Unknown" }),
                    (comic, creator) => new { comic, creator })
                .GroupBy(
                    pair => pair.creator.Name ?? "Unknown",
                    pair => pair.comic);
        }


        // ================= GROUP BY YEAR =================

        public IEnumerable<IGrouping<int?, Comic>> GroupByYear(IEnumerable<Comic> comics)
        {
            return comics.GroupBy(c => c.PublicationYear);
        }


        // ================= ANALYTICS TRACKING =================

        public void RegisterSearch(string queryKey, IEnumerable<Comic> results)
        {
            if (string.IsNullOrWhiteSpace(queryKey) || results == null)
                return;

            var resultList = results.ToList();

            _queryCounts.TryGetValue(queryKey, out int q);
            _queryCounts[queryKey] = q + 1;

            foreach (var comic in resultList)
            {
                if (string.IsNullOrWhiteSpace(comic.BlRecordId))
                    continue;

                _resultCounts.TryGetValue(comic.BlRecordId, out int r);
                _resultCounts[comic.BlRecordId] = r + 1;
            }
        }


        // ================= TOP SEARCH QUERIES =================

        public IReadOnlyList<(string Query, int Count)> GetTopQueries(int top)
        {
            return _queryCounts
                .OrderByDescending(kv => kv.Value)
                .Take(top)
                .Select(kv => (kv.Key, kv.Value))
                .ToList();
        }

        // Convenience method for reports dashboard
        public IReadOnlyList<(string Query, int Count)> GetTop10Queries()
        {
            return GetTopQueries(10);
        }


        // ================= TOP RETURNED COMICS =================

        public IReadOnlyList<(string ComicId, int Count)> GetTopResults(int top)
        {
            return _resultCounts
                .OrderByDescending(kv => kv.Value)
                .Take(top)
                .Select(kv => (kv.Key, kv.Value))
                .ToList();
        }

        // Convenience method for reports dashboard
        public IReadOnlyList<(string ComicId, int Count)> GetTop10Results()
        {
            return GetTopResults(10);
        }


        // ================= FREQUENT COMICS =================

        public IEnumerable<string> GetComicIdsWithMoreThan(int minResultCount)
        {
            return _resultCounts
                .Where(kv => kv.Value > minResultCount)
                .Select(kv => kv.Key);
        }

        // Required by brief: comics appearing in more than 100 searches
        public IEnumerable<string> GetComicsAppearingInMoreThan100Searches()
        {
            return GetComicIdsWithMoreThan(100);
        }
    }


}

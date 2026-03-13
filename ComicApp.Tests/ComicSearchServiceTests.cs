using ComicApp.Core.Models;
using ComicApp.Core.Services;

namespace ComicApp.Tests
{
    public class ComicSearchServiceTests
    {
        private readonly ComicSearchService _service;
        private readonly List<Comic> _testComics;

        public ComicSearchServiceTests()
        {
            _service = new ComicSearchService();

            _testComics = new List<Comic>
            {
                new Comic
                {
                    BlRecordId = "1",
                    Title = "Batman: Year One",
                    PublicationYear = 1987,
                    Genres = new List<string> { "Genre [1]: Superhero" },
                    Languages = new List<string> { "Language [1]: English" },
                    Creators = new List<Creator>
                    {
                        new Creator { Name = "Frank Miller", Role = "writer" }
                    }
                },
                new Comic
                {
                    BlRecordId = "2",
                    Title = "Watchmen",
                    PublicationYear = 1986,
                    Genres = new List<string> { "Genre [1]: Superhero" },
                    Languages = new List<string> { "Language [1]: English" },
                    Creators = new List<Creator>
                    {
                        new Creator { Name = "Alan Moore", Role = "writer" }
                    }
                },
                new Comic
                {
                    BlRecordId = "3",
                    Title = "Saga Volume 1",
                    PublicationYear = 2012,
                    Genres = new List<string> { "Genre [1]: Science Fiction" },
                    Languages = new List<string> { "Language [1]: English" },
                    Creators = new List<Creator>
                    {
                        new Creator { Name = "Brian K Vaughan", Role = "writer" }
                    }
                }
            };
        }

        // BASIC SEARCH TESTS
        [Fact]
        public void BasicTitleSearch_ValidTerm_ReturnsMatchingComics()
        {
            var results = _service.BasicTitleSearch(_testComics, "Batman");
            Assert.Single(results);
        }

        [Fact]
        public void BasicTitleSearch_EmptyTerm_ReturnsNoResults()
        {
            var results = _service.BasicTitleSearch(_testComics, "");
            Assert.Empty(results);
        }

        [Fact]
        public void BasicTitleSearch_CaseInsensitive_ReturnsMatch()
        {
            var results = _service.BasicTitleSearch(_testComics, "batman");
            Assert.Single(results);
        }

        [Fact]
        public void BasicTitleSearch_NoMatch_ReturnsEmpty()
        {
            var results = _service.BasicTitleSearch(_testComics, "Spider-Man");
            Assert.Empty(results);
        }

        // ADVANCED SEARCH TESTS
        [Fact]
        public void AdvancedSearch_ByAuthor_ReturnsCorrectComic()
        {
            var results = _service.AdvancedSearch(_testComics, null, "Alan Moore", null, null, null, null);
            Assert.Single(results);
        }

        [Fact]
        public void AdvancedSearch_ByYear_ReturnsCorrectComic()
        {
            var results = _service.AdvancedSearch(_testComics, null, null, 1987, null, null, null);
            Assert.Single(results);
        }

        [Fact]
        public void AdvancedSearch_ByGenre_ReturnsMultipleComics()
        {
            var results = _service.AdvancedSearch(_testComics, null, null, null, "Superhero", null, null);
            Assert.Equal(2, results.Count());
        }

        // GROUPING TESTS
        [Fact]
        public void GroupByYear_ReturnsCorrectGroups()
        {
            var groups = _service.GroupByYear(_testComics).ToList();
            Assert.Equal(3, groups.Count);
        }

        [Fact]
        public void GroupByAuthor_ReturnsCorrectGroups()
        {
            var groups = _service.GroupByAuthor(_testComics).ToList();
            Assert.Equal(3, groups.Count);
        }

        // ANALYTICS TESTS
        [Fact]
        public void RegisterSearch_TracksQueryCount()
        {
            _service.RegisterSearch("batman", _testComics.Take(1));
            _service.RegisterSearch("batman", _testComics.Take(1));

            var top = _service.GetTopQueries(10);
            Assert.Equal(2, top.First().Count);
        }

        [Fact]
        public void GetTopResults_ReturnsCorrectCount()
        {
            _service.RegisterSearch("test", _testComics);
            var top = _service.GetTopResults(10);
            Assert.Equal(3, top.Count);
        }
    }
}
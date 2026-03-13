using ComicApp.Core.DataAccess;
using ComicApp.Core.Models;
using ComicApp.Core.Services;
using Microsoft.EntityFrameworkCore;
namespace ComicApp.Tests
{
    public class SavedSearchServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }
        private List<Comic> GetTestComics()
        {
            return new List<Comic>
            {
                new Comic { BlRecordId = "1", Title = "Batman: Year One" },
                new Comic { BlRecordId = "2", Title = "Watchmen" }
            };
        }
        [Fact]
        public void SaveSearch_ValidSearch_SavedToDatabase()
        {
            var context = CreateInMemoryContext();
            var service = new SavedSearchService(context);
            service.SaveSearch("batman", GetTestComics(), "testuser");
            Assert.Equal(1, context.SavedSearches.Count());
        }
        [Fact]
        public void SaveSearch_StoresCorrectQuery()
        {
            var context = CreateInMemoryContext();
            var service = new SavedSearchService(context);
            service.SaveSearch("batman", GetTestComics(), "testuser");
            Assert.Equal("batman", context.SavedSearches.First().Query);
        }
        [Fact]
        public void SaveSearch_StoresCorrectUsername()
        {
            var context = CreateInMemoryContext();
            var service = new SavedSearchService(context);
            service.SaveSearch("batman", GetTestComics(), "testuser");
            Assert.Equal("testuser", context.SavedSearches.First().Username);
        }
        [Fact]
        public void SaveSearch_StoresCorrectComicIds()
        {
            var context = CreateInMemoryContext();
            var service = new SavedSearchService(context);
            service.SaveSearch("batman", GetTestComics(), "testuser");
            var saved = context.SavedSearches.First();
            Assert.Contains("1", saved.ComicIds);
            Assert.Contains("2", saved.ComicIds);
        }
        [Fact]
        public void SaveSearch_NullUsername_SavedWithNullUsername()
        {
            var context = CreateInMemoryContext();
            var service = new SavedSearchService(context);
            service.SaveSearch("batman", GetTestComics(), null);
            Assert.Null(context.SavedSearches.First().Username);
        }
        [Fact]
        public void GetSavedSearches_ReturnsOnlyUsersSearches()
        {
            var context = CreateInMemoryContext();
            var service = new SavedSearchService(context);
            service.SaveSearch("batman", GetTestComics(), "testuser");
            service.SaveSearch("watchmen", GetTestComics(), "testuser");
            service.SaveSearch("xmen", GetTestComics(), "otheruser");
            Assert.Equal(2, service.GetSavedSearches("testuser").Count);
        }
        [Fact]
        public void GetSavedSearches_ReturnsMostRecentFirst()
        {
            var context = CreateInMemoryContext();
            var service = new SavedSearchService(context);
            service.SaveSearch("first", GetTestComics(), "testuser");
            service.SaveSearch("second", GetTestComics(), "testuser");
            var results = service.GetSavedSearches("testuser");
            Assert.Equal("second", results.First().Query);
        }
    }
}
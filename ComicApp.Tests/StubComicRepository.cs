using ComicApp.Core.Interfaces;
using ComicApp.Core.Models;

namespace ComicApp.Tests
{
    // STUB: Simulates the data layer without reading real CSV files
    // Used to test search and filtering logic in isolation
    public class StubComicRepository : IComicRepository
    {
        public List<Comic> LoadComics(string dataFolderPath)
        {
            return new List<Comic>
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
                    },
                    Isbns = new List<string> { "ISBN [1]: 978-1401207526" }
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
                    },
                    Isbns = new List<string> { "missing" }
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
                    },
                    Isbns = new List<string> { "ISBN [1]: 978-1607066019" }
                },
                new Comic
                {
                    BlRecordId = "4",
                    Title = "Sandman Volume 1",
                    PublicationYear = 1991,
                    Genres = new List<string> { "Genre [1]: Fantasy" },
                    Languages = new List<string> { "Language [1]: English" },
                    Creators = new List<Creator>
                    {
                        new Creator { Name = "Neil Gaiman", Role = "writer" }
                    },
                    Isbns = new List<string> { "missing" }
                }
            };
        }
    }
}
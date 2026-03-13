using ComicApp.Core.Models;
using System.Collections.Generic;

namespace ComicApp.Core.Interfaces
{
    // SRP: This interface has ONE job – describe how I load comics.
    // DIP: UI layers depend on this abstraction, not on any CSV details.
    public interface IComicRepository
    {
        // I pass in the folder that contains:
        //   - titles.csv
        //   - records.csv
        //   - names.csv
        // and I get back a list of fully-built Comics.
        List<Comic> LoadComics(string dataFolderPath);
    }
}

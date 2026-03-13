using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ComicApp.Core.Models;
using ComicApp.Core.Interfaces;
using Microsoft.VisualBasic.FileIO;

namespace ComicApp.Core.Repositories
{
    public class ComicRepository : IComicRepository
    {
        public List<Comic> LoadComics(string dataFolderPath)
        {
            var comicsById = new Dictionary<string, Comic>();

            string titlesPath = Path.Combine(dataFolderPath, "titles.csv");
            string recordsPath = Path.Combine(dataFolderPath, "records.csv");
            string namesPath = Path.Combine(dataFolderPath, "names.csv");

            if (!File.Exists(titlesPath) || !File.Exists(recordsPath) || !File.Exists(namesPath))
            {
                throw new FileNotFoundException(
                    "Dataset files were not found in the selected folder.");
            }

            // ---------- TITLES ----------
            using (var parser = CreateParser(titlesPath))
            {
                bool header = true;
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    if (header) { header = false; continue; }

                    string title = CleanText(GetField(fields, 0));
                    string otherTitle = CleanText(GetField(fields, 1));
                    string blId = GetField(fields, 2);
                    string genres = GetField(fields, 24);
                    string languages = GetField(fields, 25);

                    if (string.IsNullOrWhiteSpace(blId))
                        continue;

                    var comic = GetOrCreateComic(comicsById, blId);

                    if (string.IsNullOrWhiteSpace(comic.Title))
                        comic.Title = title;

                    if (!string.IsNullOrWhiteSpace(otherTitle))
                        comic.VariantTitles.Add(otherTitle);

                    AddMultiValues(comic.Genres, genres, "Genre");
                    AddMultiValues(comic.Languages, languages, "Language");
                }
            }

            // ---------- RECORDS ----------
            using (var parser = CreateParser(recordsPath))
            {
                bool header = true;
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    if (header) { header = false; continue; }

                    string blId = GetField(fields, 0);
                    string isbnField = GetField(fields, 5);
                    string dateField = GetField(fields, 18);
                    string genres = GetField(fields, 24);
                    string languages = GetField(fields, 25);

                    if (string.IsNullOrWhiteSpace(blId))
                        continue;

                    var comic = GetOrCreateComic(comicsById, blId);

                    if (!string.IsNullOrWhiteSpace(dateField))
                    {
                        var digits = new string(dateField.Where(char.IsDigit).ToArray());
                        if (digits.Length >= 4 &&
                            int.TryParse(digits.Substring(0, 4), out int year))
                        {
                            comic.PublicationYear = year;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(isbnField))
                    {
                        if (!comic.Isbns.Contains("missing"))
                            comic.Isbns.Add("missing");
                    }
                    else
                    {
                        AddMultiValues(comic.Isbns, isbnField, "ISBN");
                    }

                    AddMultiValues(comic.Genres, genres, "Genre");
                    AddMultiValues(comic.Languages, languages, "Language");
                }
            }

            // ---------- NAMES ----------
            using (var parser = CreateParser(namesPath))
            {
                bool header = true;
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    if (header) { header = false; continue; }

                    string name = CleanText(GetField(fields, 0));
                    string typeOfName = CleanText(GetField(fields, 2));
                    string role = CleanText(GetField(fields, 3));
                    string blId = GetField(fields, 5);

                    if (string.IsNullOrWhiteSpace(blId) || string.IsNullOrWhiteSpace(name))
                        continue;

                    var comic = GetOrCreateComic(comicsById, blId);

                    comic.Creators.Add(new Creator
                    {
                        Name = name,
                        Role = role,
                        TypeOfName = typeOfName
                    });
                }
            }

            // ---------- ENSURE EVERY COMIC HAS AT LEAST ONE ISBN ----------
            foreach (var comic in comicsById.Values)
            {
                if (comic.Isbns.Count == 0)
                    comic.Isbns.Add("missing");
            }

            // Filter to Fantasy, Horror and Science Fiction genres only
            string[] allowedGenres = { "Fantasy", "Horror", "Science Fiction" };

            return comicsById.Values
                .Where(c => c.Genres.Any(g =>
                    allowedGenres.Any(ag =>
                        g.IndexOf(ag, StringComparison.OrdinalIgnoreCase) >= 0)))
                .ToList();
        }

        private static TextFieldParser CreateParser(string path)
        {
            var parser = new TextFieldParser(path);
            parser.SetDelimiters(",");
            parser.HasFieldsEnclosedInQuotes = true;
            return parser;
        }

        private static Comic GetOrCreateComic(Dictionary<string, Comic> dict, string blId)
        {
            if (!dict.TryGetValue(blId, out var comic))
            {
                comic = new Comic { BlRecordId = blId };
                dict[blId] = comic;
            }
            return comic;
        }

        private static string GetField(string[] fields, int index)
        {
            return (index >= 0 && index < fields.Length) ? fields[index] : string.Empty;
        }

        private static void AddMultiValues(List<string> target, string raw, string description)
        {
            if (string.IsNullOrWhiteSpace(raw)) return;

            var parts = raw
                .Split(';')
                .Select(p => CleanText(p))
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();

            for (int i = 0; i < parts.Count; i++)
            {
                string formatted = $"{description} [{i + 1}]: {parts[i]}";
                if (!target.Contains(formatted))
                {
                    target.Add(formatted);
                }
            }
        }

        private static string CleanText(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            string cleaned = new string(value.Where(ch => !char.IsControl(ch)).ToArray());
            return cleaned.Trim();
        }
    }
}
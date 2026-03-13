namespace ComicApp.Core.Models;

// This class represents a single comic record from the dataset.

public class Comic
{
    // This is the British Library record ID. Each comic has one.
    public string? BlRecordId { get; set; }

    // Main title of the comic.
    public string? Title { get; set; }

    // Some comics have extra or alternative titles, so I'm keeping them as a list.
    public List<string> VariantTitles { get; set; } = new List<string>();

    // Comics can have multiple genres (e.g. Fantasy, Horror).
    public List<string> Genres { get; set; } = new List<string>();

    // The dataset includes languages, so I might as well store them too.
    public List<string> Languages { get; set; } = new List<string>();

    // Some entries don't have a clear year, so I allow this to be empty.
    public int? PublicationYear { get; set; }

    // Some comics have one or more reference numbers, so I’m just storing them in a list
    public List<string> Isbns { get; set; } = new List<string>();

    // Comics can have multiple creators (e.g. writer, illustrator).
    public List<Creator> Creators { get; set; } = new List<Creator>();

    // NEW — Staff can flag records for review 
    public bool IsFlagged { get; set; } = false;
}

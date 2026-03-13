namespace ComicApp.Core.Models;

// This class represents a creator (e.g. writer, illustrator) associated with a comic.
public class Creator
{
    // Name of the creator.
    public string? Name { get; set; }

    // Role of the creator (e.g. writer, illustrator).
    public string? Role { get; set; }

    // How the name appears in the dataset.
    public string? TypeOfName { get; set; }
}

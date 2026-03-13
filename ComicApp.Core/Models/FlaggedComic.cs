using System;

namespace ComicApp.Core.Models
{
    public class FlaggedComic
    {
        public int Id { get; set; }

        public string ComicId { get; set; } = "";

        public string? FlaggedBy { get; set; }

        public DateTime FlaggedAt { get; set; } = DateTime.Now;
    }
}
namespace ComicApp.Core.Models
{
    public class User
    {
        public int Id { get; set; }  // Primary key required by Entity Framework

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "Public"; // Public or Staff
    }
}
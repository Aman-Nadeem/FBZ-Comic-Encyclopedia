using ComicApp.Core.Models;

namespace ComicApp.Core.Interfaces
{
    // Defines user account operations for the application
    public interface IUserService
    {
        // Registers a new user
        bool Register(string username, string password, string role);

        // Authenticates a user and returns the user if valid
        User? Login(string username, string password);

        // Returns the currently logged-in user
        User? GetCurrentUser();

        // Logs the current user out
        void Logout();
    }
}

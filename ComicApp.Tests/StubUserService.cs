using ComicApp.Core.Interfaces;
using ComicApp.Core.Models;

namespace ComicApp.Tests
{
    // STUB: Simulates user service without a real database
    // Used to test account controller logic in isolation
    public class StubUserService : IUserService
    {
        private readonly List<User> _users = new List<User>
        {
            new User { Username = "staffuser", Password = "password123", Role = "Staff" },
            new User { Username = "publicuser", Password = "password123", Role = "Public" }
        };

        private User? _currentUser;

        public bool Register(string username, string password, string role)
        {
            if (_users.Any(u => u.Username == username))
                return false;

            _users.Add(new User
            {
                Username = username,
                Password = password,
                Role = role
            });

            return true;
        }

        public User? Login(string username, string password)
        {
            _currentUser = _users.FirstOrDefault(u =>
                u.Username == username &&
                u.Password == password);

            return _currentUser;
        }

        public User? GetCurrentUser()
        {
            return _currentUser;
        }

        public void Logout()
        {
            _currentUser = null;
        }
    }
}
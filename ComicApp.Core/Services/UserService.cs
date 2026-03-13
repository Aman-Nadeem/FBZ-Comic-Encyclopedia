using ComicApp.Core.Interfaces;
using ComicApp.Core.Models;
using ComicApp.Core.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;

namespace ComicApp.Core.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private User? _currentUser;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public bool Register(string username, string password, string role)
        {
            if (_context.Users.Any(u => u.Username == username))
                return false;

            var user = new User
            {
                Username = username,
                Password = password,
                Role = role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return true;
        }

        public User? Login(string username, string password)
        {
            _currentUser = _context.Users.FirstOrDefault(u =>
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
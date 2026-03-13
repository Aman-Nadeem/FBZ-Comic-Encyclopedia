using ComicApp.Core.DataAccess;
using ComicApp.Core.Models;
using ComicApp.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace ComicApp.Tests
{
    public class UserServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public void Register_NewUser_ReturnsTrue()
        {
            var context = CreateInMemoryContext();
            var service = new UserService(context);

            var result = service.Register("testuser", "password123", "Public");

            Assert.True(result);
        }

        [Fact]
        public void Register_DuplicateUsername_ReturnsFalse()
        {
            var context = CreateInMemoryContext();
            var service = new UserService(context);

            service.Register("testuser", "password123", "Public");
            var result = service.Register("testuser", "password456", "Public");

            Assert.False(result);
        }

        [Fact]
        public void Login_ValidCredentials_ReturnsUser()
        {
            var context = CreateInMemoryContext();
            var service = new UserService(context);

            service.Register("testuser", "password123", "Public");
            var user = service.Login("testuser", "password123");

            Assert.NotNull(user);
        }

        [Fact]
        public void Login_InvalidPassword_ReturnsNull()
        {
            var context = CreateInMemoryContext();
            var service = new UserService(context);

            service.Register("testuser", "password123", "Public");
            var user = service.Login("testuser", "wrongpassword");

            Assert.Null(user);
        }

        [Fact]
        public void Login_NonExistentUser_ReturnsNull()
        {
            var context = CreateInMemoryContext();
            var service = new UserService(context);

            var user = service.Login("nobody", "password123");

            Assert.Null(user);
        }

        [Fact]
        public void Register_StaffRole_StoredCorrectly()
        {
            var context = CreateInMemoryContext();
            var service = new UserService(context);

            service.Register("staffuser", "password123", "Staff");
            var user = service.Login("staffuser", "password123");

            Assert.Equal("Staff", user?.Role);
        }

        [Fact]
        public void Register_PublicRole_StoredCorrectly()
        {
            var context = CreateInMemoryContext();
            var service = new UserService(context);

            service.Register("publicuser", "password123", "Public");
            var user = service.Login("publicuser", "password123");

            Assert.Equal("Public", user?.Role);
        }

        [Fact]
        public void Logout_ClearsCurrentUser()
        {
            var context = CreateInMemoryContext();
            var service = new UserService(context);

            service.Register("testuser", "password123", "Public");
            service.Login("testuser", "password123");
            service.Logout();

            Assert.Null(service.GetCurrentUser());
        }
    }
}
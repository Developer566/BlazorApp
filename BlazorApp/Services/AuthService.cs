using BlazorApp.Data;
using BlazorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Services
{
    public class AuthService
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        // 👆 Sirf DbFactory chahiye — kuch nahi

        public AuthService(IDbContextFactory<AppDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public User? CurrentUser { get; private set; }
        // 👆 private set = sirf is class change kar sake

        public bool IsLoggedIn => CurrentUser != null;

        public bool Login(string username, string password)
        {
            using var db = _dbFactory.CreateDbContext();
            var user = db.Users.FirstOrDefault(u =>
                u.Username == username && u.Password == password);

            if (user != null)
            {
                CurrentUser = user;
                return true;
            }
            return false;
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
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

            var user = db.Users.FirstOrDefault(u => u.Username == username);
            // 👆 Pehle sirf username se dhundo
            // Password compare yahan nahi karein ge — BCrypt se karein ge

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            // 👆 BCrypt.Verify = 
            //    password = user ne jo type kiya (plain text)
            //    user.Password = database mein jo hash hai
            //    Verify dono ko compare karta hai → true/false
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
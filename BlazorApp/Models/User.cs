// Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Models
{

    public class User
    {
        public int Id { get; set; }
        // 👆 Primary key — database automatically 1,2,3 assign karega

        public string Username { get; set; } = "";
        // 👆 Login username — "" matlab default empty string hai

        public string Password { get; set; } = "";
        // 👆 Password — abhi simple text, baad mein hash karenge

        public string Role { get; set; } = "User";
        // 👆 Role — "Admin" ya "User" — future mein permissions ke liye
    }
}
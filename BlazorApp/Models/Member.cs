using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; } = "";

        [Required(ErrorMessage = "Please select membership type")]
        public string MembershipType { get; set; } = "";

        public DateTime RegisteredDate { get; set; } = DateTime.Now;
        public string? PhotoPath { get; set; } = "";
    }
}
using MailKit.Net.Smtp;
using MimeKit;

namespace BlazorApp.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        // 👆 wwwroot path ke liye — template files yahan hain

        public EmailService(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        // ✅ Core Send Method
        public async Task SendEmailAsync(string toEmail, string toName, string subject, string body)
        {
            var settings = _config.GetSection("EmailSettings");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(settings["SenderName"], settings["SenderEmail"]));
            email.To.Add(new MailboxAddress(toName, toEmail));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(settings["SmtpHost"], int.Parse(settings["SmtpPort"]!), MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(settings["SenderEmail"], settings["AppPassword"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        // ✅ Template Load Helper
        private async Task<string> LoadTemplateAsync(string templateName)
        {
            var path = Path.Combine(_env.WebRootPath, "EmailTemplates", templateName);

            Console.WriteLine($"Template path: {path}");
            // 👆 Yeh add karo — terminal mein path dikhega

            if (!File.Exists(path))
            {
                Console.WriteLine("Template file NOT FOUND!");
                return "<p>Template not found</p>";
            }

            return await File.ReadAllTextAsync(path);
        }

        // ✅ Member Welcome Email
        public async Task SendMemberWelcomeAsync(string toEmail, string memberName, string membershipType)
        {
            var body = await LoadTemplateAsync("MemberWelcome.html");
            // 👆 Template load karo

            body = body
                .Replace("{{MemberName}}", memberName)
                .Replace("{{MemberEmail}}", toEmail)
                .Replace("{{MembershipType}}", membershipType)
                .Replace("{{Date}}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            // 👆 Placeholders ko actual values se replace karo

            await SendEmailAsync(toEmail, memberName, "Welcome to BlazorApp! 🎉", body);
        }

        // ✅ User Welcome Email
        public async Task SendUserWelcomeAsync(string toEmail, string username, string role)
        {
            var body = await LoadTemplateAsync("UserWelcome.html");

            body = body
                .Replace("{{Username}}", username)
                .Replace("{{Email}}", toEmail)
                .Replace("{{Role}}", role)
                .Replace("{{Date}}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

            await SendEmailAsync(toEmail, username, "Your Account is Ready! 🔐", body);
        }

        // ✅ Admin Notification Email
        public async Task SendAdminNotificationAsync(string username, string email, string role)
        {
            var adminEmail = _config["EmailSettings:SenderEmail"]!;

            var body = await LoadTemplateAsync("AdminNotification.html");

            body = body
                .Replace("{{Username}}", username)
                .Replace("{{Email}}", email)
                .Replace("{{Role}}", role)
                .Replace("{{Date}}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

            await SendEmailAsync(adminEmail, "Admin", "New User Registered 🔔", body);
        }
    }
}
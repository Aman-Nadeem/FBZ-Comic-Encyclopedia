using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace ComicApp.Core.Services
{
    public class EmailService
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(string apiKey, string fromEmail, string fromName)
        {
            _apiKey = apiKey;
            _fromEmail = fromEmail;
            _fromName = fromName;
        }

        public async Task SendSavedSearchEmailAsync(string toEmail, string username, string searchTerm)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail);
            var subject = $"FBZ - Your saved search: {searchTerm}";
            var plainText = $"Hi {username},\n\nYour search for '{searchTerm}' has been saved successfully in the FBZ Comic Encyclopedia.\n\nVisit the app to view your results.\n\nFBZ Team";
            var htmlContent = $@"
                <h2>FBZ Comic Encyclopedia</h2>
                <p>Hi <strong>{username}</strong>,</p>
                <p>Your search for <strong>'{searchTerm}'</strong> has been saved successfully.</p>
                <p>Visit the app to view your results.</p>
                <br/>
                <p>FBZ Team</p>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainText, htmlContent);
            await client.SendEmailAsync(msg);
        }
    }
}
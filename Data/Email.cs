using Domain;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;

namespace Data
{
    public class Email : IEmail
    {
        private readonly ILogger _logger;

        public Email(ILogger log)
        {
            _logger = log;
        }

        public void SendEmail(Exception exception)
        {
            EmailLog("Start");

            string apiKey = Environment.GetEnvironmentVariable("SendgridAPIKey");
            SendGridClient client = new SendGridClient(apiKey);

            string functionEmail = Environment.GetEnvironmentVariable("FunctionEmailAddress");
            SendGridMessage emailMessage = new SendGridMessage()
            {
                From = new EmailAddress(functionEmail, "RedditTopWeekly_Function"),
                Subject = "ERROR: RedditTopWeekly",
                PlainTextContent = $"RedditTopWeekly_Function ran into a problem.\n\nMessage: {exception.Message}\n\nStackTrace: {exception.StackTrace}"
            };

            string receivingEmail = Environment.GetEnvironmentVariable("MyEmailAddress");
            emailMessage.AddTo(new EmailAddress(receivingEmail));

            EmailLog("Send");
            Response _ = client.SendEmailAsync(emailMessage).Result;

            EmailLog("Return");
        }

        private void EmailLog(string logString)
        {
            _logger.LogInformation($"SendEmail - {logString} - {DateTime.Now}");
        }
    }
}

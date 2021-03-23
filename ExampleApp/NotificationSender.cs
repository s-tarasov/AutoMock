using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MimeKit;

namespace ExampleApp
{
    public interface INotificationSender
    {
        void Send(string message);
    }

    public class NotificationSender : INotificationSender
    {
        private readonly ILogger<NotificationSender> _logger;

        public NotificationSender(ILogger<NotificationSender> logger)
        {
            _logger = logger;
        }

        public void Send(string message)
        {
            _logger.LogInformation("Сообщение '{0}' отправлено.", message);

            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Me", "testrateapp765@gmail.com"));
            mailMessage.To.Add(new MailboxAddress("Me", "testrateapp765@gmail.com"));
            mailMessage.Subject = "subject";
            mailMessage.Body = new TextPart("plain")
            {
                Text = message
            };

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;

                smtpClient.Connect("localhost", 1025, false);
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }
            
        }
    }
}

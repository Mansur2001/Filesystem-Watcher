using System.Net;
using System.Net.Mail;

namespace FilesystemWatcher.Service
{
    /// <summary>
    /// Sends emails via SMTP, with support for attachments.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public class EmailService
    {
        /// <summary>
        /// Client used to send SMTP messages.
        /// </summary>
        private readonly SmtpClient smtpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class,
        /// configured to use Mailtrap's SMTP endpoint.
        /// </summary>
        public EmailService()
        {
            smtpClient = new SmtpClient("live.smtp.mailtrap.io", 587)
            {
                Credentials = new NetworkCredential("api", "15141f996469bd6ee2e9a7bc29f4f986"),
                EnableSsl = true
            };
        }

        /// <summary>
        /// Sends an email with the specified recipient, subject, and attachment file.
        /// </summary>
        /// <param name="to">The recipient's email address. Cannot be null or whitespace.</param>
        /// <param name="subject">The subject line of the email. Cannot be null or whitespace.</param>
        /// <param name="attachmentPath">The file path of the attachment. Must exist.</param>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="to"/> or <paramref name="subject"/> is null or empty.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Thrown if <paramref name="attachmentPath"/> is null, empty, or does not point to an existing file.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the SMTP client fails to send the message.
        /// </exception>
        public void SendEmail(string to, string subject, string attachmentPath)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Recipient email address is required.", nameof(to));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Email subject is required.", nameof(subject));

            if (string.IsNullOrWhiteSpace(attachmentPath) || !File.Exists(attachmentPath))
                throw new FileNotFoundException("Attachment file not found.", attachmentPath);

            var message = new MailMessage("hello@demomailtrap.co", to, subject, "Attached is the requested file.");
            message.Attachments.Add(new Attachment(attachmentPath));

            try
            {
                smtpClient.Send(message);
            }
            catch (SmtpException ex)
            {
                throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
            }
        }
    }
}

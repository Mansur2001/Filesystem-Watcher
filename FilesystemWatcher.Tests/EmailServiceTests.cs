using System;
using System.IO;
using FilesystemWatcher.Service;
using Xunit;

namespace FilesystemWatcher.Tests
{
    public class EmailServiceTests
    {
        [Fact]
        public void SendEmail_ThrowsIfRecipientIsEmpty()
        {
            var emailService = new EmailService();
            var tmpFile = Path.GetTempFileName();

            var ex = Assert.Throws<ArgumentException>(() =>
                emailService.SendEmail("", "Subject", tmpFile));

            Assert.Contains("Recipient", ex.Message);
        }

        [Fact]
        public void SendEmail_ThrowsIfAttachmentPathMissing()
        {
            var emailService = new EmailService();

            var ex = Assert.Throws<FileNotFoundException>(() =>
                emailService.SendEmail("user@example.com", "Subject", "nonexistent.csv"));

            Assert.Contains("Attachment", ex.Message);
        }

        [Fact]
        public void SendEmail_ThrowsIfSmtpFails()
        {
            var emailService = new EmailService();
            var tmp = Path.GetTempFileName();

            // This will fail if smtp.example.com is not valid
            var ex = Record.Exception(() =>
                emailService.SendEmail("user@example.com", "Subject", tmp));

            Assert.NotNull(ex);
            Assert.IsType<InvalidOperationException>(ex);
        }
    }
}
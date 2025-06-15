namespace FilesystemWatcher.View
{
    /// <summary>
    /// Represents the email parameters collected from the email dialog.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public class EmailResult
    {
        /// <summary>
        /// Gets or sets the recipient email address.
        /// </summary>
        public string To { get; set; } = "";

        /// <summary>
        /// Gets or sets the subject line of the email.
        /// </summary>
        public string Subject { get; set; } = "";

        /// <summary>
        /// Gets or sets the file path of the attachment.
        /// </summary>
        public string AttachmentPath { get; set; } = "";
    }
}
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Controls.ApplicationLifetimes;

namespace FilesystemWatcher.View
{
    /// <summary>
    /// A modal dialog window for composing and sending emails with an attachment.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public partial class EmailDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailDialog"/> class
        /// and loads its XAML content.
        /// </summary>
        public EmailDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the XAML definition for this window.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Handler for the OK button. Closes the dialog and returns the entered email data.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Event data for the click.</param>
        private void OnOk(object? sender, RoutedEventArgs e)
        {
            var emailBox      = this.FindControl<TextBox>("EmailBox");
            var subjectBox    = this.FindControl<TextBox>("SubjectBox");
            var attachmentBox = this.FindControl<TextBox>("AttachmentBox");

            var result = new EmailDialogResult
            {
                To             = emailBox.Text,
                Subject        = subjectBox.Text,
                AttachmentPath = attachmentBox.Text
            };

            Close(result);
        }

        /// <summary>
        /// Handler for the Cancel button. Closes the dialog without returning a result.
        /// </summary>
        /// <param name="sender">The Cancel button.</param>
        /// <param name="e">Event data for the click.</param>
        private void OnCancel(object? sender, RoutedEventArgs e)
        {
            Close(null);
        }

        /// <summary>
        /// Handler for the Browse button. Opens a file picker to select an attachment.
        /// </summary>
        /// <param name="sender">The Browse button.</param>
        /// <param name="e">Event data for the click.</param>
        private async void OnBrowse(object? sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Title         = "Select file to attach"
            };

            var lifetime   = Avalonia.Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var mainWindow = lifetime?.MainWindow;

            var result = mainWindow != null
                ? await dialog.ShowAsync(mainWindow)
                : null;

            if (result != null && result.Length > 0)
            {
                var attachmentBox = this.FindControl<TextBox>("AttachmentBox");
                attachmentBox.Text = result[0];
            }
        }

        /// <summary>
        /// Shows this dialog modally and returns the user's input.
        /// </summary>
        /// <param name="owner">The owner window.</param>
        /// <returns>
        /// A task that completes with an <see cref="EmailDialogResult"/> if OK was pressed,
        /// or <c>null</c> if cancelled.
        /// </returns>
        public Task<EmailDialogResult?> ShowDialogAsync(Window owner)
            => ShowDialog<EmailDialogResult?>(owner);
    }

    /// <summary>
    /// Represents the data entered in an <see cref="EmailDialog"/>.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public class EmailDialogResult
    {
        /// <summary>
        /// Gets or sets the recipient email address.
        /// </summary>
        public string? To { get; set; }

        /// <summary>
        /// Gets or sets the email subject line.
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Gets or sets the full path to the attachment file.
        /// </summary>
        public string? AttachmentPath { get; set; }
    }
}

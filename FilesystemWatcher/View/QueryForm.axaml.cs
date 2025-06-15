using Avalonia.Controls;
using Avalonia.Interactivity;
using FilesystemWatcher.Service;
using FilesystemWatcher.ViewModel;

namespace FilesystemWatcher.View
{
    /// <summary>
    /// A window for querying, filtering, and exporting file system events.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public partial class QueryForm : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryForm"/> class,
        /// wiring up the data context to a <see cref="QueryCriteriaViewModel"/>.
        /// </summary>
        /// <param name="db">The database manager to query stored events.</param>
        /// <param name="csv">The CSV exporter for exporting query results.</param>
        /// <param name="email">The email service for sending exported CSVs.</param>
        /// <param name="dialogs">The dialog service for user prompts and alerts.</param>
        public QueryForm(
            DatabaseManager db,
            CSVExporter     csv,
            EmailService    email,
            IDialogService  dialogs)
        {
            InitializeComponent();
            DataContext = new QueryCriteriaViewModel(db, csv, email, dialogs);
        }

        /// <summary>
        /// Handles the Close button click and closes this window.
        /// </summary>
        /// <param name="sender">The control that raised the event.</param>
        /// <param name="e">Event data.</param>
        private void OnClose(object? sender, RoutedEventArgs e)
            => Close();
    }
}
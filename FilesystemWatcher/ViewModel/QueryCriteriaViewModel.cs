using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using FilesystemWatcher.Service;
using FilesystemWatcher.View;
using ReactiveUI;

namespace FilesystemWatcher.ViewModel
{
    /// <summary>
    /// ViewModel that handles querying, filtering, exporting, and emailing
    /// of file system events stored in the database.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public class QueryCriteriaViewModel : ViewModelBase
    {
        private readonly DatabaseManager _db;
        private readonly CSVExporter     _csv;
        private readonly EmailService    _email;
        private readonly IDialogService  _dialogs;

        /// <summary>
        /// Available file extensions to filter on.
        /// </summary>
        public ObservableCollection<string> AvailableExtensions { get; }
            = new() { "", ".txt", ".csv", ".log", ".xml", ".json", ".md" };

        /// <summary>
        /// Available event types to filter on.
        /// </summary>
        public ObservableCollection<string> AvailableEvents { get; }
            = new() { "", "Created", "Changed", "Deleted", "Renamed" };

        private string? _fileNameQuery;
        /// <summary>
        /// The partial file name to filter on.
        /// </summary>
        public string? FileNameQuery
        {
            get => _fileNameQuery;
            set => this.RaiseAndSetIfChanged(ref _fileNameQuery, value);
        }

        /// <summary>
        /// The selected extension filter.
        /// </summary>
        public string? SelectedExtension { get; set; }

        /// <summary>
        /// The selected event type filter.
        /// </summary>
        public string? SelectedEventType { get; set; }

        /// <summary>
        /// The collection of query results for binding to the UI.
        /// </summary>
        public ObservableCollection<FileEventViewModel> QueryResults { get; }
            = new();

        private string _statusMessage = "Ready.";
        /// <summary>
        /// Status message displayed in the UI.
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            private set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }

        /// <summary>
        /// Command to execute the query with the current filters.
        /// </summary>
        public ReactiveCommand<Unit, Unit> RunQueryCommand { get; }

        /// <summary>
        /// Command to clear all filter inputs and results.
        /// </summary>
        public ReactiveCommand<Unit, Unit> ClearFiltersCommand { get; }

        /// <summary>
        /// Command to export the current query results to CSV.
        /// </summary>
        public ReactiveCommand<Unit, Unit> ExportCsvCommand { get; }

        /// <summary>
        /// Command to email the current query results as a CSV attachment.
        /// </summary>
        public ReactiveCommand<Unit, Unit> EmailCsvCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCriteriaViewModel"/> class.
        /// </summary>
        /// <param name="db">The database manager for querying events.</param>
        /// <param name="csv">The CSV exporter for exporting results.</param>
        /// <param name="email">The email service for sending attachments.</param>
        /// <param name="dialogs">Optional dialog service for prompts and alerts.</param>
        public QueryCriteriaViewModel(
            DatabaseManager db,
            CSVExporter     csv,
            EmailService    email,
            IDialogService? dialogs = null)
        {
            _db      = db;
            _csv     = csv;
            _email   = email;
            _dialogs = dialogs ?? new AvaloniaDialogService();

            RunQueryCommand     = ReactiveCommand.Create(RunQuery);
            ClearFiltersCommand = ReactiveCommand.Create(ClearFilters);
            ExportCsvCommand    = ReactiveCommand.CreateFromTask(ExportCsvAsync);
            EmailCsvCommand     = ReactiveCommand.CreateFromTask(EmailCsvAsync);
        }

        /// <summary>
        /// Executes the database query with the selected filters,
        /// deduplicates the results, and populates <see cref="QueryResults"/>.
        /// </summary>
        private void RunQuery()
        {
            QueryResults.Clear();
            var results = _db.QueryAll();

            if (!string.IsNullOrWhiteSpace(FileNameQuery))
                results = results
                    .Where(e => e.FileName.Contains(FileNameQuery, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            if (!string.IsNullOrWhiteSpace(SelectedExtension))
                results = results.Where(e => e.Extension == SelectedExtension).ToList();

            if (!string.IsNullOrWhiteSpace(SelectedEventType))
                results = results.Where(e => e.EventType == SelectedEventType).ToList();

            // Deduplicate exact path/type/timestamp combinations
            var distinct = results
                .GroupBy(e => (e.FilePath, e.EventType, e.Timestamp))
                .Select(g => g.First())
                .ToList();

            for (int i = 0; i < distinct.Count; i++)
            {
                QueryResults.Add(new FileEventViewModel(distinct[i]) { RowNumber = i + 1 });
            }

            StatusMessage = $"Found {QueryResults.Count} row(s).";
        }

        /// <summary>
        /// Clears all filter inputs and current query results.
        /// </summary>
        private void ClearFilters()
        {
            FileNameQuery     = null;
            SelectedExtension = null;
            SelectedEventType = null;
            QueryResults.Clear();
            StatusMessage     = "Filters cleared.";
        }

        /// <summary>
        /// Prompts the user for a save location and exports current results to CSV.
        /// </summary>
        private async Task<Unit> ExportCsvAsync()
        {
            var path = await _dialogs.SaveFileDialog("Save CSV", "query.csv");
            if (!string.IsNullOrWhiteSpace(path))
            {
                var toExport = QueryResults.Select(vm => vm.ToModel()).ToList();
                _csv.Export(toExport, path);
                StatusMessage = $"Exported {toExport.Count} row(s) to CSV.";
            }
            return Unit.Default;
        }

        /// <summary>
        /// Exports the results to a temporary CSV, prompts for email details,
        /// and sends the file as an attachment.
        /// </summary>
        private async Task<Unit> EmailCsvAsync()
        {
            // 1) Export filtered results
            var tmp = Path.GetTempFileName() + ".csv";
            var toExport = QueryResults.Select(vm => vm.ToModel()).ToList();
            _csv.Export(toExport, tmp);

            // 2) Show the email dialog
            var lifetime = Avalonia.Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var owner = lifetime?.MainWindow;
            if (owner == null)
                return Unit.Default;

            var dlg = new EmailDialog();
            dlg.FindControl<TextBox>("AttachmentBox").Text = tmp;

            EmailDialogResult? result;
            try
            {
                result = await dlg.ShowDialog<EmailDialogResult>(owner);
            }
            catch (Exception ex)
            {
                await _dialogs.Alert("Dialog Error", ex.Message);
                return Unit.Default;
            }

            if (result == null)
                return Unit.Default;  // user cancelled

            // 3) Validate input fields
            if (string.IsNullOrWhiteSpace(result.To))
            {
                await _dialogs.Alert("Validation Error", "Recipient is required.");
                return Unit.Default;
            }
            if (string.IsNullOrWhiteSpace(result.Subject))
            {
                await _dialogs.Alert("Validation Error", "Subject is required.");
                return Unit.Default;
            }

            // 4) Attempt to send email
            try
            {
                _email.SendEmail(result.To, result.Subject, result.AttachmentPath);
                await _dialogs.Alert("Success", $"Email sent to {result.To}.");
                StatusMessage = $"Email sent to {result.To}.";
            }
            catch (Exception ex)
            {
                await _dialogs.Alert("Send Error", ex.Message);
            }

            return Unit.Default;
        }
    }
}

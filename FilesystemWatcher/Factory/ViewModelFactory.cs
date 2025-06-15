using FilesystemWatcher.Service;
using FilesystemWatcher.ViewModel;

namespace FilesystemWatcher.Factory
{
    /// <summary>
    /// Factory for creating view model instances.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public class ViewModelFactory
    {
        /// <summary>
        /// Service for watching file system changes.
        /// </summary>
        private readonly FileWatcherService _watcherService;

        /// <summary>
        /// Exporter for CSV functionality.
        /// </summary>
        private readonly CSVExporter _csvExporter;

        /// <summary>
        /// Manager for database operations.
        /// </summary>
        private readonly DatabaseManager _databaseManager;

        /// <summary>
        /// Service for sending emails.
        /// </summary>
        private readonly EmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelFactory"/> class.
        /// </summary>
        /// <param name="watcherService">The file watcher service.</param>
        /// <param name="csvExporter">The CSV exporter.</param>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="emailService">The email service.</param>
        public ViewModelFactory(
            FileWatcherService watcherService,
            CSVExporter        csvExporter,
            DatabaseManager    databaseManager,
            EmailService       emailService)
        {
            _watcherService  = watcherService;
            _csvExporter     = csvExporter;
            _databaseManager = databaseManager;
            _emailService    = emailService;
        }

        /// <summary>
        /// Creates a new <see cref="FileWatcherViewModel"/>.
        /// </summary>
        /// <returns>A configured <see cref="FileWatcherViewModel"/> instance.</returns>
        public FileWatcherViewModel CreateFileWatcherViewModel()
            => new FileWatcherViewModel(
                _watcherService,
                _csvExporter,
                _databaseManager,
                _emailService
            );

        /// <summary>
        /// Creates a new <see cref="QueryCriteriaViewModel"/>.
        /// </summary>
        /// <returns>A configured <see cref="QueryCriteriaViewModel"/> instance.</returns>
        public QueryCriteriaViewModel CreateQueryCriteriaViewModel()
            => new QueryCriteriaViewModel(
                _databaseManager,
                _csvExporter,
                _emailService
            );
    }
}

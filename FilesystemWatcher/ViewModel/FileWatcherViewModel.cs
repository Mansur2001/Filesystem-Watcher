using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using FilesystemWatcher.Model;
using FilesystemWatcher.Service;
using ReactiveUI;

namespace FilesystemWatcher.ViewModel
{
    /// <summary>
    /// ViewModel that manages file system monitoring, manual file creation,
    /// and database export functionality for the main application window.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public class FileWatcherViewModel : ViewModelBase
    {
        /// <summary>
        /// Filename for persisting user settings.
        /// </summary>
        private const string SettingsFile = "user.settings";

        /// <summary>
        /// Tracks which files are currently considered "existing" for CRUD semantics.
        /// </summary>
        private readonly HashSet<string> _existingFiles = new();

        /// <summary>
        /// Used to debounce rapid successive Changed events for the same file.
        /// </summary>
        private readonly Dictionary<string, DateTime> _lastChangedTimes = new();

        private readonly FileWatcherService _watcherService;
        private readonly CSVExporter        _csvExporter;
        private readonly DatabaseManager    _dbManager;
        private readonly EmailService       _emailService;
        private readonly IDialogService     _dialogs;

        /// <summary>
        /// Collection of file events displayed in the UI.
        /// </summary>
        public ObservableCollection<FileEventViewModel> FileEvents { get; } = new();

        /// <summary>
        /// List of file extensions available for manual file creation.
        /// </summary>
        public ObservableCollection<string> AvailableExtensions { get; } =
            new() { "",".txt", ".json", ".csv", ".log", ".xml", ".md" };

        /// <summary>
        /// The extension selected from the preset dropdown.
        /// </summary>
        public string? SelectedPresetExtension { get; set; }

        /// <summary>
        /// A custom extension entered by the user.
        /// </summary>
        public string? CustomExtension { get; set; }

        private string? _directoryPath;
        /// <summary>
        /// The directory path currently being monitored.
        /// </summary>
        public string? DirectoryPath
        {
            get => _directoryPath;
            set => this.RaiseAndSetIfChanged(ref _directoryPath, value);
        }

        private string? _manualFileName;
        /// <summary>
        /// The file name entered by the user for manual creation.
        /// </summary>
        public string? ManualFileName
        {
            get => _manualFileName;
            set => this.RaiseAndSetIfChanged(ref _manualFileName, value);
        }

        private string _statusMessage = "Ready.";
        /// <summary>
        /// Status text displayed in the status bar.
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }

        /// <summary>
        /// Command to browse for and select a directory to watch.
        /// </summary>
        public ReactiveCommand<Unit, Unit> BrowseDirectoryCommand { get; }

        /// <summary>
        /// Command to create a new file in the monitored directory.
        /// </summary>
        public ReactiveCommand<Unit, Unit> CreateFileCommand { get; }

        /// <summary>
        /// Command to start file system monitoring.
        /// </summary>
        public ReactiveCommand<Unit, Unit> StartCommand { get; }

        /// <summary>
        /// Command to stop file system monitoring.
        /// </summary>
        public ReactiveCommand<Unit, Unit> StopCommand { get; }

        /// <summary>
        /// Command to save all pending events to the database.
        /// </summary>
        public ReactiveCommand<Unit, Unit> SaveToDatabaseCommand { get; }

        /// <summary>
        /// Command to clear the in-memory event list.
        /// </summary>
        public ReactiveCommand<Unit, Unit> ClearEventsCommand { get; }

        /// <summary>
        /// Command to export all database events to CSV.
        /// </summary>
        public ReactiveCommand<Unit, Unit> ExportToCsvCommand { get; }

        /// <summary>
        /// Command to delete all records from the database.
        /// </summary>
        public ReactiveCommand<Unit, Unit> ClearDatabaseCommand { get; }

        /// <summary>
        /// Command to open the query form window.
        /// </summary>
        public ReactiveCommand<Unit, Unit> OpenQueryWindowCommand { get; }

        /// <summary>
        /// Command to exit the application.
        /// </summary>
        public ReactiveCommand<Unit, Unit> ExitCommand { get; }

        /// <summary>
        /// Command to show the About dialog.
        /// </summary>
        public ReactiveCommand<Unit, Unit> AboutCommand { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="FileWatcherViewModel"/>,
        /// wiring up services, commands, and seeding the existing-files set.
        /// </summary>
        /// <param name="watcherService">The service to monitor file system events.</param>
        /// <param name="csvExporter">CSV exporter for writing files.</param>
        /// <param name="databaseManager">Database manager for persistence.</param>
        /// <param name="emailService">Email service for sending CSV attachments.</param>
        /// <param name="dialogs">Dialog service for alerts and prompts.</param>
        public FileWatcherViewModel(
            FileWatcherService watcherService,
            CSVExporter        csvExporter,
            DatabaseManager    databaseManager,
            EmailService       emailService,
            IDialogService?    dialogs = null)
        {
            _watcherService = watcherService;
            _csvExporter    = csvExporter;
            _dbManager      = databaseManager;
            _emailService   = emailService;
            _dialogs        = dialogs ?? new AvaloniaDialogService();

            // Seed existing files from DB (ignore Deleted)
            foreach (var evt in _dbManager.QueryAll())
                if (evt.EventType != "Deleted")
                    _existingFiles.Add(evt.FilePath);

            LoadSettings();

            BrowseDirectoryCommand  = ReactiveCommand.CreateFromTask(BrowseDirectoryAsync);
            CreateFileCommand       = ReactiveCommand.CreateFromTask(CreateFileAsync);
            StartCommand            = ReactiveCommand.Create(StartWatching);
            StopCommand             = ReactiveCommand.Create(StopWatching);
            SaveToDatabaseCommand   = ReactiveCommand.CreateFromTask(SaveToDatabaseAsync);
            ClearEventsCommand      = ReactiveCommand.Create(ClearEvents);
            ExportToCsvCommand      = ReactiveCommand.CreateFromTask(ExportFromDatabaseAsync);
            ClearDatabaseCommand    = ReactiveCommand.Create(ClearDatabase);
            OpenQueryWindowCommand  = ReactiveCommand.Create(OpenQueryWindow);
            ExitCommand             = ReactiveCommand.Create(ExitApp);
            AboutCommand            = ReactiveCommand.CreateFromTask(ShowAboutAsync);

            _watcherService.OnFileEvent += (_, e) =>
            {
                var path = e.FilePath;

                if (e.EventType == "Created")
                {
                    if (_existingFiles.Contains(path))
                    {
                        e.EventType = "Changed";
                    }
                    else
                    {
                        _existingFiles.Add(path);
                        AddEvent(e);
                        return;
                    }
                }

                if (e.EventType == "Changed")
                {
                    var now = DateTime.Now;
                    if (_lastChangedTimes.TryGetValue(path, out var last)
                     && (now - last).TotalMilliseconds < 1000)
                        return;
                    _lastChangedTimes[path] = now;
                    AddEvent(e);
                    return;
                }

                if (e.EventType == "Deleted")
                {
                    AddEvent(e);
                    _existingFiles.Remove(path);
                    return;
                }

                AddEvent(e);
            };
        }

        /// <summary>
        /// Opens a folder browser and updates <see cref="DirectoryPath"/>.
        /// </summary>
        private async Task<Unit> BrowseDirectoryAsync()
        {
            var dlg = new OpenFolderDialog { Title = "Select Directory to Monitor" };
            var lifetime = Avalonia.Application.Current.ApplicationLifetime
                               as IClassicDesktopStyleApplicationLifetime;
            var owner = lifetime?.MainWindow;
            var result = await dlg.ShowAsync(owner!);
            if (!string.IsNullOrWhiteSpace(result))
                DirectoryPath = result;
            return Unit.Default;
        }

        /// <summary>
        /// Creates a new file in the monitored directory and injects a Created event.
        /// </summary>
        private async Task<Unit> CreateFileAsync()
        {
            if (string.IsNullOrWhiteSpace(ManualFileName))
            {
                await _dialogs.Alert("Error", "Enter a file name first.");
                return Unit.Default;
            }

            var ext = !string.IsNullOrWhiteSpace(CustomExtension)
                ? CustomExtension!
                : SelectedPresetExtension ?? string.Empty;

            var fileName = ManualFileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)
                ? ManualFileName
                : ManualFileName + ext;

            var dir = !string.IsNullOrWhiteSpace(DirectoryPath)
                ? DirectoryPath!
                : Directory.GetCurrentDirectory();
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var path = Path.Combine(dir, fileName);
            try
            {
                using var fs = new FileStream(path,
                    FileMode.Create, FileAccess.Write, FileShare.None);
            }
            catch (Exception ex)
            {
                await _dialogs.Alert("File Error", ex.Message);
                return Unit.Default;
            }

            _existingFiles.Add(path);
            AddEvent(new FileEvent
            {
                FileName  = fileName,
                Extension = ext,
                FilePath  = path,
                EventType = "Created",
                Timestamp = DateTime.Now
            });

            ManualFileName = null;
            return Unit.Default;
        }

        /// <summary>
        /// Adds a <see cref="FileEvent"/> to the UI list and updates the status message.
        /// </summary>
        /// <param name="e">The event to display.</param>
        private void AddEvent(FileEvent e)
        {
            FileEvents.Add(new FileEventViewModel(e)
            {
                RowNumber = FileEvents.Count + 1
            });

            var verb = e.EventType switch
            {
                "Created" => "Created",
                "Changed" => "Updated",
                "Deleted" => "Deleted",
                "Renamed" => "Renamed",
                _         => "Detected"
            };
            StatusMessage = $"{verb}: {e.FileName}";
        }

        /// <summary>
        /// Loads saved settings (directory and extension) from disk.
        /// </summary>
        private void LoadSettings()
        {
            if (!File.Exists(SettingsFile)) return;
            var lines = File.ReadAllLines(SettingsFile);
            if (lines.Length > 0) DirectoryPath           = lines[0];
            if (lines.Length > 1) SelectedPresetExtension = lines[1];
        }

        /// <summary>
        /// Saves current settings (directory and extension) to disk.
        /// </summary>
        private void SaveSettings()
            => File.WriteAllLines(SettingsFile, new[]
            {
                DirectoryPath           ?? string.Empty,
                SelectedPresetExtension ?? string.Empty
            });

        /// <summary>
        /// Starts watching the configured directory for file events.
        /// </summary>
        private void StartWatching()
        {
            if (string.IsNullOrWhiteSpace(DirectoryPath))
            {
                _dialogs.Alert("Error", "Select a directory first.");
                return;
            }
            var ext = !string.IsNullOrWhiteSpace(CustomExtension)
                ? CustomExtension!
                : SelectedPresetExtension ?? string.Empty;

            _watcherService.StartWatching(DirectoryPath, ext);
            StatusMessage = "Monitoring started.";
        }

        /// <summary>
        /// Stops the file system watcher.
        /// </summary>
        private void StopWatching()
        {
            _watcherService.StopWatching();
            StatusMessage = "Monitoring stopped.";
        }

        /// <summary>
        /// Saves all in-memory file events into the database and resynchronizes the existing-files set.
        /// </summary>
        private async Task<Unit> SaveToDatabaseAsync()
        {
            if (FileEvents.Count == 0)
            {
                await _dialogs.Alert("No Events", "No events to save.");
                return Unit.Default;
            }
            foreach (var ev in FileEvents)
                _dbManager.Insert(ev.ToModel());

            _existingFiles.Clear();
            foreach (var evt in _dbManager.QueryAll())
                if (evt.EventType != "Deleted")
                    _existingFiles.Add(evt.FilePath);

            StatusMessage = $"Saved {FileEvents.Count} event(s).";
            return Unit.Default;
        }

        /// <summary>
        /// Clears the currently displayed file events.
        /// </summary>
        private void ClearEvents()
        {
            FileEvents.Clear();
            StatusMessage = "Events cleared.";
        }

        /// <summary>
        /// Exports the entire database to a CSV file.
        /// </summary>
        private async Task<Unit> ExportFromDatabaseAsync()
        {
            var path = await _dialogs.SaveFileDialog("Save CSV", "events.csv");
            if (!string.IsNullOrWhiteSpace(path))
            {
                var all = _dbManager.QueryAll();
                _csvExporter.Export(all, path);
                StatusMessage = $"Exported {all.Count} rows.";
            }
            return Unit.Default;
        }

        /// <summary>
        /// Clears all records from the database.
        /// </summary>
        private void ClearDatabase()
        {
            _dbManager.ClearAll();
            _existingFiles.Clear();
            StatusMessage = "Database cleared.";
        }

        /// <summary>
        /// Opens the QueryForm window for advanced filtering and export.
        /// </summary>
        private void OpenQueryWindow()
        {
            var lifetime = Avalonia.Application.Current.ApplicationLifetime
                               as IClassicDesktopStyleApplicationLifetime;
            var owner = lifetime?.MainWindow;
            if (owner != null)
            {
                var dlg = new View.QueryForm(_dbManager, _csvExporter, _emailService, _dialogs);
                dlg.Show(owner);
            }
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        private void ExitApp()
            => (Avalonia.Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?
                   .Shutdown();

        /// <summary>
        /// Displays the About dialog.
        /// </summary>
        private async Task<Unit> ShowAboutAsync()
        {
            var msg = @"
Welcome to Filesystem Watcher v1.0! 
By: Mansur Yassin and Tairan Zhang

Usage:
1. Click “Browse…” to select the folder you want to monitor.
2. Pick one of the preset extensions or type a custom one.
3. Click Start to begin watching file events will appear live.
4. Use “Create File” to drop a new empty file into that folder.
5. Click “Save to Database” to persist all displayed events.
6. Open “Query Database” to filter events, then export to CSV or email.

 SMTP NOTICE:
  The built-in EmailService is pre-configured to use Mailtrap’s demo server.
  This means that the User should create a mail trap account and copy/paste their
  information in the EmailService.cs file
    • Host (this is the demo domain)   
    • Username, Password is the API token
  Edit the constructor in EmailService.cs and paste in your credentials.

Enjoy, and happy monitoring!
";
            await _dialogs.Alert("About Filesystem Watcher", msg);
            return Unit.Default;
        }

    }
}


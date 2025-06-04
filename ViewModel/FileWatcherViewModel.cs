using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using FilesystemWatcher.Model;
using FilesystemWatcher.Service;
using ReactiveUI;

namespace FilesystemWatcher.ViewModel
{
    public class FileWatcherViewModel : ReactiveObject
    {
        private readonly DatabaseManager _dbManager;
        private readonly CSVExporter _csvExporter;
        private readonly EmailService _emailService;

        private FileSystemWatcher? _watcher;
        private bool _isWatching;

        private string _directoryPath = string.Empty;
        public string DirectoryPath
        {
            get => _directoryPath;
            set => this.RaiseAndSetIfChanged(ref _directoryPath, value);
        }

        private string _extension = string.Empty;
        public string Extension
        {
            get => _extension;
            set => this.RaiseAndSetIfChanged(ref _extension, value);
        }

        public ObservableCollection<FileEventViewModel> FileEvents { get; } = new();

        public bool CanStart => !_isWatching && !string.IsNullOrWhiteSpace(DirectoryPath);
        public bool CanStop => _isWatching;
        public bool CanSave => FileEvents.Any();

        public ReactiveCommand<Unit, Unit> StartCommand { get; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveToDatabaseCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportToCSVCommand { get; }
        public ReactiveCommand<Unit, Unit> SendEmailCommand { get; }
        public ReactiveCommand<Unit, Unit> ExitCommand { get; }
        public ReactiveCommand<Unit, Unit> AboutCommand { get; }

        public FileWatcherViewModel(DatabaseManager dbManager, CSVExporter csvExporter, EmailService emailService)
        {
            _dbManager = dbManager;
            _csvExporter = csvExporter;
            _emailService = emailService;

            StartCommand = ReactiveCommand.Create(StartWatching, this.WhenAnyValue(vm => vm.CanStart));
            StopCommand = ReactiveCommand.Create(StopWatching, this.WhenAnyValue(vm => vm.CanStop));
            SaveToDatabaseCommand = ReactiveCommand.Create(SaveEventsToDB, this.WhenAnyValue(vm => vm.CanSave));
            ExportToCSVCommand = ReactiveCommand.CreateFromTask(ExportToCsvAsync, this.WhenAnyValue(vm => vm.CanSave));
            SendEmailCommand = ReactiveCommand.CreateFromTask(SendCSVEmailAsync, this.WhenAnyValue(vm => vm.CanSave));
            ExitCommand = ReactiveCommand.Create(ExitApp);
            AboutCommand = ReactiveCommand.Create(ShowAbout);

            this.WhenAnyValue(vm => vm.DirectoryPath)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(CanStart)));

            this.WhenAnyValue(vm => vm._isWatching)
                .Subscribe(_ =>
                {
                    this.RaisePropertyChanged(nameof(CanStart));
                    this.RaisePropertyChanged(nameof(CanStop));
                });
        }

        private void StartWatching()
        {
            if (_watcher != null)
                StopWatching();

            if (!Directory.Exists(DirectoryPath))
            {
                ShowMessageBox("Invalid path.");
                return;
            }

            _watcher = new FileSystemWatcher(DirectoryPath, string.IsNullOrWhiteSpace(Extension) ? "*.*" : $"*.{Extension}")
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };

            _watcher.Created += (s, e) => OnFileEvent(e, "Created");
            _watcher.Changed += (s, e) => OnFileEvent(e, "Changed");
            _watcher.Deleted += (s, e) => OnFileEvent(e, "Deleted");
            _watcher.Renamed += (s, e) => OnFileEvent(e, "Renamed");

            _isWatching = true;
            this.RaisePropertyChanged(nameof(CanStart));
            this.RaisePropertyChanged(nameof(CanStop));
        }

        private void StopWatching()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }
            _isWatching = false;
            this.RaisePropertyChanged(nameof(CanStart));
            this.RaisePropertyChanged(nameof(CanStop));
        }

        private void OnFileEvent(FileSystemEventArgs e, string eventType)
        {
            var ext = Path.GetExtension(e.Name);
            if (!string.IsNullOrEmpty(Extension) && !ext.Equals("." + Extension, StringComparison.OrdinalIgnoreCase))
                return;

            var fileEvent = new FileEvent
            {
                FileName = Path.GetFileName(e.Name),
                Extension = ext ?? string.Empty,
                Path = e.FullPath,
                EventType = eventType,
                Timestamp = DateTime.Now
            };
            var vm = new FileEventViewModel(fileEvent)
            {
                RowNumber = FileEvents.Count + 1
            };

            Dispatcher.UIThread.Post(() =>
            {
                FileEvents.Add(vm);
                this.RaisePropertyChanged(nameof(CanSave));
            });
        }

        private void SaveEventsToDB()
        {
            foreach (var ev in FileEvents)
            {
                _dbManager.InsertEvent(ev.Model);
            }
            ShowMessageBox("Events saved to database.");
        }

        // Modern StorageProvider API (Avalonia 11+)
        private async Task ExportToCsvAsync()
        {
            var window = GetMainWindow();
            if (window?.StorageProvider is null)
            {
                ShowMessageBox("File save dialog is not available.");
                return;
            }

            var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export to CSV",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("CSV Files") { Patterns = new[] { "*.csv" } }
                },
                SuggestedFileName = "events.csv"
            });

            if (file != null)
            {
                var path = file.Path.LocalPath;
                _csvExporter.Export(FileEvents.Select(vm => vm.Model).ToList(), path);
                ShowMessageBox("CSV export complete.");
            }
        }

        private async Task SendCSVEmailAsync()
        {
            var recipient = await ShowInputDialog("Send CSV", "Enter recipient email:");
            if (string.IsNullOrWhiteSpace(recipient))
                return;

            var tempCsv = Path.GetTempFileName() + ".csv";
            _csvExporter.Export(FileEvents.Select(vm => vm.Model).ToList(), tempCsv);
            _emailService.SendEmail(recipient, tempCsv);
            ShowMessageBox("Email sent.");
        }

        private void ExitApp()
        {
            if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }

        private void ShowAbout()
        {
            ShowMessageBox("Filesystem Watcher\nVersion 1.0\nDeveloper: Mansur Yassin & Tairan Zhang");
        }

        private void ShowMessageBox(string message)
        {
            // Ideally, connect to a dialog service or UI notification system.
            Console.WriteLine(message);
        }

        private async Task<string?> ShowInputDialog(string title, string prompt)
        {
            // Replace with your own dialog service or custom input window for Avalonia.
            // For now, just return null.
            return await Task.FromResult<string?>(null);
        }

        private Window? GetMainWindow()
        {
            if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                return desktop.MainWindow;
            return null;
        }
    }
}


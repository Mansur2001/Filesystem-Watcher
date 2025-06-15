using ReactiveUI;
using System.Reactive;

namespace FilesystemWatcher.ViewModel
{
    /// <summary>
    /// ViewModel for the settings dialog, allowing the user to choose a directory,
    /// default file extension, and remembered email address. Exposes Save, Cancel,
    /// and Clear commands.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public class SettingsDialogViewModel : ViewModelBase
    {
        private string? _directoryPath;

        /// <summary>
        /// Gets or sets the directory path to monitor.
        /// </summary>
        public string? DirectoryPath
        {
            get => _directoryPath;
            set => this.RaiseAndSetIfChanged(ref _directoryPath, value);
        }

        private string? _defaultExtension;

        /// <summary>
        /// Gets or sets the default file extension to watch.
        /// </summary>
        public string? DefaultExtension
        {
            get => _defaultExtension;
            set => this.RaiseAndSetIfChanged(ref _defaultExtension, value);
        }

        private string? _rememberedEmail;

        /// <summary>
        /// Gets or sets the email address remembered for notifications.
        /// </summary>
        public string? RememberedEmail
        {
            get => _rememberedEmail;
            set => this.RaiseAndSetIfChanged(ref _rememberedEmail, value);
        }

        /// <summary>
        /// Alias for <see cref="DefaultExtension"/>, used in the UI binding.
        /// </summary>
        public string? SelectedExtension
        {
            get => DefaultExtension;
            set => DefaultExtension = value;
        }

        /// <summary>
        /// Gets the list of available file extensions for the dropdown.
        /// </summary>
        public string[] AvailableExtensions { get; } = new[] { ".txt", ".json", ".csv", ".log", ".xml" };

        /// <summary>
        /// Action to close the dialog, assigned by the view.
        /// </summary>
        public Action? CloseAction { get; set; }

        /// <summary>
        /// Command to save the current settings and close the dialog.
        /// </summary>
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        /// <summary>
        /// Command to cancel without saving and close the dialog.
        /// </summary>
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        /// <summary>
        /// Command to clear all fields in the settings dialog.
        /// </summary>
        public ReactiveCommand<Unit, Unit> ClearCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsDialogViewModel"/> class,
        /// wiring up Save, Cancel, and Clear commands.
        /// </summary>
        public SettingsDialogViewModel()
        {
            SaveCommand   = ReactiveCommand.Create(Save);
            CancelCommand = ReactiveCommand.Create(Cancel);
            ClearCommand  = ReactiveCommand.Create(Clear);
        }

        /// <summary>
        /// Saves the current settings by invoking the <see cref="CloseAction"/>.
        /// </summary>
        private void Save()
        {
            CloseAction?.Invoke();
        }

        /// <summary>
        /// Cancels changes by invoking the <see cref="CloseAction"/>.
        /// </summary>
        private void Cancel()
        {
            CloseAction?.Invoke();
        }

        /// <summary>
        /// Clears all settings fields.
        /// </summary>
        private void Clear()
        {
            DirectoryPath    = string.Empty;
            DefaultExtension = string.Empty;
            RememberedEmail  = string.Empty;
        }
    }
}

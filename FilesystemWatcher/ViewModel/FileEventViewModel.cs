using FilesystemWatcher.Model;

namespace FilesystemWatcher.ViewModel
{
    /// <summary>
    /// ViewModel wrapper around <see cref="FileEvent"/>, exposing properties
    /// and formatted data for data-binding in the UI.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public class FileEventViewModel : ViewModelBase
    {
        /// <summary>
        /// Exposes the underlying <see cref="FileEvent"/> model for inspection or persistence.
        /// </summary>
        public FileEvent Model { get; }

        /// <summary>
        /// Gets or sets the row number in the UI list.
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets the file extension, including the leading dot.
        /// </summary>
        public string Extension { get; }

        /// <summary>
        /// Gets the full path to the file.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the type of event (e.g., "Created", "Changed", "Deleted", "Renamed", "Manual").
        /// </summary>
        public string EventType { get; }

        /// <summary>
        /// Gets the timestamp when the event occurred.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Gets a formatted timestamp string (ISO sortable format) for display or testing.
        /// </summary>
        public string FormattedTimestamp => Timestamp.ToString("s");

        /// <summary>
        /// Initializes a new instance of the <see cref="FileEventViewModel"/> class
        /// from the given <see cref="FileEvent"/> model.
        /// </summary>
        /// <param name="model">The file event model to wrap.</param>
        public FileEventViewModel(FileEvent model)
        {
            Model     = model;
            FileName  = model.FileName;
            Extension = model.Extension;
            FilePath  = model.FilePath;
            EventType = model.EventType;
            Timestamp = model.Timestamp;
        }

        /// <summary>
        /// Converts this ViewModel back into a <see cref="FileEvent"/> model instance.
        /// </summary>
        /// <returns>A new <see cref="FileEvent"/> populated from this ViewModel.</returns>
        public FileEvent ToModel()
            => new FileEvent
            {
                FileName  = FileName,
                Extension = Extension,
                FilePath  = FilePath,
                EventType = EventType,
                Timestamp = Timestamp
            };
    }
}

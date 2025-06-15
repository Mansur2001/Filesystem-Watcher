namespace FilesystemWatcher.Model
{
    /// <summary>
    /// Represents a file system event record, intended for persistence in the database.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public class FileEventModel
    {
        /// <summary>
        /// Gets or sets the name of the file involved in the event.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file's extension (including the leading dot).
        /// </summary>
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the full path to the file.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of event (e.g., "Created", "Changed", "Deleted", "Renamed").
        /// </summary>
        public string EventType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timestamp when the event occurred.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
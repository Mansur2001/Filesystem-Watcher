namespace FilesystemWatcher.Model
{
    /// <summary>
    /// Represents the criteria for querying file events, including extension, event type,
    /// directory path, and date range.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public class QueryCriteria
    {
        /// <summary>
        /// Gets or sets the file extension to filter by (including the leading dot).
        /// </summary>
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of event to filter by (e.g., "Created", "Changed", "Deleted", "Renamed").
        /// </summary>
        public string EventType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the directory path in which to search for file events.
        /// </summary>
        public string DirectoryPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the start date of the query range.
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Gets or sets the end date of the query range.
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.Today;
    }
}
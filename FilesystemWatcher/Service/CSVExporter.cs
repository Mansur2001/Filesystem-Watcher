using System.Text;
using FilesystemWatcher.Model;

namespace FilesystemWatcher.Service
{
    /// <summary>
    /// Exports a sequence of <see cref="FileEvent"/> objects to a CSV file.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public class CSVExporter
    {
        /// <summary>
        /// Writes the provided file-system events into a CSV file at the given path.
        /// </summary>
        /// <param name="events">The collection of <see cref="FileEvent"/> to export.</param>
        /// <param name="filePath">The full path where the CSV should be written.</param>
        public void Export(IEnumerable<FileEvent> events, string filePath)
        {
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            writer.WriteLine("FileName,Extension,FilePath,EventType,Timestamp");
            foreach (var e in events)
            {
                writer.WriteLine($"{Escape(e.FileName)},{Escape(e.Extension)},{Escape(e.FilePath)},{Escape(e.EventType)},{e.Timestamp:o}");
            }
        }

        /// <summary>
        /// Escapes a field value by surrounding it with quotes and doubling any embedded quotes
        /// if it contains a comma; otherwise returns it unchanged.
        /// </summary>
        /// <param name="v">The raw field value to escape.</param>
        /// <returns>The escaped field value safe for CSV output.</returns>
        private static string Escape(string v)
            => v.Contains(',') ? $"\"{v.Replace("\"", "\"\"")}\"" : v;
    }
}
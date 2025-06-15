using System;
using System.Collections.Generic;
using FilesystemWatcher.Model;
using Microsoft.Data.Sqlite;

namespace FilesystemWatcher.Service
{
    /// <summary>
    /// Manages a SQLite database of file-system events, providing methods to ensure
    /// the schema exists, upsert records, query all records, and clear the table.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public class DatabaseManager
    {
        /// <summary>
        /// The SQLite connection string pointing at the database file.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManager"/> class,
        /// using the specified database file name.
        /// </summary>
        /// <param name="dbFile">The filename of the SQLite database. Defaults to "events.db".</param>
        public DatabaseManager(string dbFile = "events.db")
        {
            _connectionString = $"Data Source={dbFile}";
            EnsureDatabase();
        }

        /// <summary>
        /// Ensures that the FileEvents table exists in the database, creating it if necessary.
        /// </summary>
        private void EnsureDatabase()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS FileEvents (
                    FilePath   TEXT    NOT NULL PRIMARY KEY,
                    FileName   TEXT    NOT NULL,
                    Extension  TEXT    NOT NULL,
                    EventType  TEXT    NOT NULL,
                    Timestamp  TEXT    NOT NULL
                );";
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Inserts or replaces a file event record in the FileEvents table.
        /// </summary>
        /// <param name="evt">The <see cref="FileEvent"/> to upsert into the database.</param>
        public void Insert(FileEvent evt)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT OR REPLACE INTO FileEvents
                  (FilePath, FileName, Extension, EventType, Timestamp)
                VALUES
                  ($path, $name, $ext, $type, $ts);";
            cmd.Parameters.AddWithValue("$path", evt.FilePath);
            cmd.Parameters.AddWithValue("$name", evt.FileName);
            cmd.Parameters.AddWithValue("$ext",  evt.Extension);
            cmd.Parameters.AddWithValue("$type", evt.EventType);
            cmd.Parameters.AddWithValue("$ts",   evt.Timestamp.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Retrieves all file event records from the database, ordered by file name.
        /// </summary>
        /// <returns>
        /// A <see cref="List{FileEvent}"/> containing all current records in the FileEvents table.
        /// </returns>
        public List<FileEvent> QueryAll()
        {
            var results = new List<FileEvent>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT FilePath, FileName, Extension, EventType, Timestamp
                  FROM FileEvents
                ORDER BY FileName;";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                results.Add(new FileEvent
                {
                    FilePath  = reader.GetString(0),
                    FileName  = reader.GetString(1),
                    Extension = reader.GetString(2),
                    EventType = reader.GetString(3),
                    Timestamp = DateTime.Parse(
                        reader.GetString(4),
                        null,
                        System.Globalization.DateTimeStyles.RoundtripKind)
                });
            }
            return results;
        }

        /// <summary>
        /// Deletes all records from the FileEvents table.
        /// </summary>
        public void ClearAll()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM FileEvents;";
            cmd.ExecuteNonQuery();
        }
    }
}

// DatabaseManager.cs
// Mansur Yassin & Tairan Zhang
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace FileWatcherApp.Services
{
    /// <summary>
    /// Handles database operations for file event storage and retrieval.
    /// </summary>
    public class DatabaseManager
    {
        private readonly string myConnectionString;

        /// <summary>
        /// Initializes the database manager with a connection string.
        /// </summary>
        public DatabaseManager(string theConnectionString)
        {
            myConnectionString = theConnectionString;
        }

        /// <summary>
        /// Inserts a new file event into the database.
        /// </summary>
        public void InsertEvent(FileEvent theEvent)
        {
            using var connection = new SQLiteConnection(myConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO FileEvents (FileName, Extension, Path, EventType, Timestamp)
                                    VALUES (@FileName, @Extension, @Path, @EventType, @Timestamp);";
            command.Parameters.AddWithValue("@FileName", theEvent.FileName);
            command.Parameters.AddWithValue("@Extension", theEvent.Extension);
            command.Parameters.AddWithValue("@Path", theEvent.Path);
            command.Parameters.AddWithValue("@EventType", theEvent.EventType);
            command.Parameters.AddWithValue("@Timestamp", theEvent.Timestamp);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Queries file events based on filter criteria.
        /// </summary>
        public List<FileEvent> QueryEvents(QueryCriteria theCriteria)
        {
            var results = new List<FileEvent>();
            using var connection = new SQLiteConnection(myConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM FileEvents WHERE 
                                    Timestamp BETWEEN @StartDate AND @EndDate AND
                                    Extension = @Extension AND
                                    EventType = @EventType AND
                                    Path LIKE @Path;";
            command.Parameters.AddWithValue("@StartDate", theCriteria.StartDate);
            command.Parameters.AddWithValue("@EndDate", theCriteria.EndDate);
            command.Parameters.AddWithValue("@Extension", theCriteria.Extension);
            command.Parameters.AddWithValue("@EventType", theCriteria.EventType);
            command.Parameters.AddWithValue("@Path", theCriteria.DirectoryPath + "%");

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                results.Add(new FileEvent
                {
                    FileName = reader["FileName"].ToString(),
                    Extension = reader["Extension"].ToString(),
                    Path = reader["Path"].ToString(),
                    EventType = reader["EventType"].ToString(),
                    Timestamp = DateTime.Parse(reader["Timestamp"].ToString())
                });
            }
            return results;
        }

        /// <summary>
        /// Clears all file events from the database.
        /// </summary>
        public void ClearDatabase()
        {
            using var connection = new SQLiteConnection(myConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM FileEvents;";
            command.ExecuteNonQuery();
        }
    }
}

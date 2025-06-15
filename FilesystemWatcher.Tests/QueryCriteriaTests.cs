using FilesystemWatcher.Model;

namespace FilesystemWatcher.Tests
{
    public class QueryCriteriaTests
    {
        [Fact]
        public void QueryCriteria_HasCorrectDefaultValues()
        {
            var criteria = new QueryCriteria();

            Assert.Equal(DateTime.Today, criteria.StartDate.Date);
            Assert.Equal(DateTime.Today, criteria.EndDate.Date);
            Assert.Equal(string.Empty, criteria.Extension);
            Assert.Equal(string.Empty, criteria.EventType);
            Assert.Equal(string.Empty, criteria.DirectoryPath);
        }

        [Fact]
        public void QueryCriteria_CanAssignValues()
        {
            var start = DateTime.Today.AddDays(-7);
            var end = DateTime.Today;
            var criteria = new QueryCriteria
            {
                StartDate = start,
                EndDate = end,
                Extension = ".txt",
                EventType = "Created",
                DirectoryPath = "/var/log"
            };

            Assert.Equal(start, criteria.StartDate);
            Assert.Equal(end, criteria.EndDate);
            Assert.Equal(".txt", criteria.Extension);
            Assert.Equal("Created", criteria.EventType);
            Assert.Equal("/var/log", criteria.DirectoryPath);
        }
    }
}
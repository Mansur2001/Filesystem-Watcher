using FilesystemWatcher.Model;
using FilesystemWatcher.Service;
using FilesystemWatcher.ViewModel;

namespace FilesystemWatcher.Factory
{
    public class ViewModelFactory
    {
        public FileEventViewModel VisitFileEvent(FileEvent fileEvent)
        {
            return new FileEventViewModel(fileEvent);
        }

        // Fix here: Accept a DatabaseManager and return the correct ViewModel
        public QueryCriteriaViewModel CreateQueryCriteriaViewModel(DatabaseManager dbManager)
        {
            return new QueryCriteriaViewModel(dbManager);
        }
    }
}
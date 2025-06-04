using FilesystemWatcher.Model;
using FilesystemWatcher.Service;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;

namespace FilesystemWatcher.ViewModel
{
    public class QueryCriteriaViewModel : ReactiveObject
    {
        private readonly DatabaseManager _dbManager;

        public QueryCriteria Model { get; }
        public ObservableCollection<FileEventViewModel> QueryResults { get; } = new();

        private string _extension;
        public string Extension
        {
            get => _extension;
            set => this.RaiseAndSetIfChanged(ref _extension, value);
        }

        // Commands
        public ReactiveCommand<Unit, Unit> SubmitQueryCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearDatabaseCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseCommand { get; }

        public QueryCriteriaViewModel(DatabaseManager dbManager)
        {
            _dbManager = dbManager;
            Model = new QueryCriteria();

            SubmitQueryCommand = ReactiveCommand.Create(SubmitQuery);
            ClearDatabaseCommand = ReactiveCommand.Create(ClearDatabase);
            CloseCommand = ReactiveCommand.Create(CloseWindow);
        }

        private void SubmitQuery()
        {
            Model.Extension = Extension;
            Model.StartDate = System.DateTime.MinValue;
            Model.EndDate = System.DateTime.MaxValue;

            var results = _dbManager.QueryEvents(Model);
            QueryResults.Clear();
            int row = 1;
            foreach (var ev in results)
            {
                QueryResults.Add(new FileEventViewModel(ev) { RowNumber = row++ });
            }
        }

        private void ClearDatabase()
        {
            _dbManager.ClearDatabase();
            QueryResults.Clear();
        }

        private void CloseWindow()
        {
            // Window close logic to be called from View
        }
    }
}
using FilesystemWatcher.Model;
using ReactiveUI;
using System;

namespace FilesystemWatcher.ViewModel
{
    public class FileEventViewModel : ReactiveObject
    {
        public FileEvent Model { get; }
        public int RowNumber { get; set; } // For DataGrid

        public FileEventViewModel(FileEvent model)
        {
            Model = model;
        }

        public string FileName => Model.FileName;
        public string Extension => Model.Extension;
        public string Path => Model.Path;
        public string EventType => Model.EventType;
        public DateTime Timestamp => Model.Timestamp;
        public string FormattedTimestamp => Timestamp.ToString("g");
    }
}
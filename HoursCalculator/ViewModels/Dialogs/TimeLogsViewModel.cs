using HoursCalculator.Model;
using HoursCalculator.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HoursCalculator.ViewModels.Dialogs
{
    public class TimeLogsViewModel : BindableBase, IDialogAware
    {
        public string Title => "History of Time Logs";
        string location = "";
        private readonly string fileName = "TimeLogs.xml";
        public event Action<IDialogResult> RequestClose;
        public FileService<TimeLog> FileService;

        public List<TimeLog> TimeLogsCollection { get; set; }

        public DelegateCommand<object> DeleteRow { get; set; }

        public TimeLogsViewModel()
        {
            TimeLogsCollection = new List<TimeLog>();
            FileService = new FileService<TimeLog>();

            TimeLogsCollection = FileService.GetData(fileName);

            DeleteRow = new DelegateCommand<object>(DeleteItem)
                .ObservesProperty(() => TimeLogsCollection);

            string binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            location = Path.Combine(binPath, "TimeLogs.xml");
        }

        private void DeleteItem(object selectedItem)
        {
            var item = selectedItem;
            TimeLogsCollection.Remove(item as TimeLog);
            TimeLogsCollection = new List<TimeLog>(TimeLogsCollection);
            RaisePropertyChanged(nameof(TimeLogsCollection));
            FileService.SetData(fileName, TimeLogsCollection);
            EnableDelete = TimeLogsCollection.Count > 0 ? true : false;
        }

        private bool enableDelete;
        public bool EnableDelete
        {
            get
            {
                return enableDelete = TimeLogsCollection.Count > 0 ? true : false;
            }
            set
            {
                value = TimeLogsCollection.Count > 0 ? true : false;
                SetProperty(ref enableDelete, value);
            }
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        { }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var from = parameters.GetValue<string>("from");
            var to = parameters.GetValue<string>("to");
            var spent = parameters.GetValue<string>("spent");

            if (TimeLogsCollection.Where(r => r.Date == DateTime.Now.ToString("d") || r.Date == "Today").Any())
            {
                TimeLogsCollection.RemoveAll(r => r.Date == DateTime.Now.ToString("d") || r.Date == "Today");
                TimeLogsCollection.Add(new TimeLog() { Date = "Today", From = from, To = to, TotalSpent = spent });
            }
            else
            {
                TimeLogsCollection.Add(new TimeLog() { Date = DateTime.Now.ToString("d"), From = from, To = to, TotalSpent = spent });
            }

            FileService.SetData(fileName, TimeLogsCollection);
        }
    }
}

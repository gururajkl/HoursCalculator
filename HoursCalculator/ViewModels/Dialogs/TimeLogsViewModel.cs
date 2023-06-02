using HoursCalculator.Model;
using HoursCalculator.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HoursCalculator.ViewModels.Dialogs
{
    public class TimeLogsViewModel : BindableBase, IDialogAware
    {
        public string Title => "History of Time Logs";
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
        }

        private void DeleteItem(object selectedItem)
        {
            var item = selectedItem;
            TimeLogsCollection.Remove(item as TimeLog);
            RaisePropertyChanged("TimeLogsCollection");
            FileService.SetData(fileName, TimeLogsCollection);
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

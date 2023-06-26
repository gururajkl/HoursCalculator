using HoursCalculator.Model;
using HoursCalculator.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HoursCalculator.ViewModels.Dialogs
{
    public class TimeLogsViewModel : BindableBase, IDialogAware
    {
        public string Title => "History of Time Logs";
        private readonly string fileName = "TimeLogs.xml";
        public event Action<IDialogResult> RequestClose;
        public FileService<TimeLog> FileService;
        private readonly IDialogService dialogService;
        private readonly IEventAggregator eventAggregator;

        public List<TimeLog> TimeLogsCollection { get; set; }

        public DelegateCommand<object> DeleteRow { get; set; }
        public DelegateCommand<object> AddComment { get; set; }
        public DelegateCommand<object> LeftDoubleClick { get; set; }
        public DelegateCommand DownloadXML { get; set; }

        public TimeLogsViewModel(IDialogService dialogService, IEventAggregator eventAggregator)
        {
            TimeLogsCollection = new List<TimeLog>();
            FileService = new FileService<TimeLog>(eventAggregator);
            this.dialogService = dialogService;

            TimeLogsCollection = FileService.GetData(fileName);

            DeleteRow = new DelegateCommand<object>(DeleteItem)
                .ObservesProperty(() => TimeLogsCollection);

            AddComment = new DelegateCommand<object>(AddComments);

            LeftDoubleClick = new DelegateCommand<object>(DoubleClickGrid);
            DownloadXML = new DelegateCommand(Download);
        }

        private void Download()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = fileName;
            saveFileDialog.Filter = "XML Files (*.xml)|*.xml";
            bool result = saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK;
            if (result)
            {
                string filePath = saveFileDialog.FileName;
                string selectedDirectory = Path.GetDirectoryName(filePath);
                string destinationFilePath = Path.Combine(selectedDirectory, fileName);

                var source = AppDomain.CurrentDomain.BaseDirectory;
                source = Path.Combine(source, fileName);
                File.Copy(source, destinationFilePath, true);
            }
        }

        private TimeLog selectedItem;
        public TimeLog SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value); }
        }

        private void DoubleClickGrid(object obj)
        {
            var parameters = new DialogParameters()
            {
                {"comment", selectedItem.Comments},
                { "hour", selectedItem.TotalSpent },
                { "date", selectedItem.Date},
            };

            dialogService.ShowDialog("OverView", parameters, r => { });
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

        private void AddComments(object selectedItem)
        {
            var preEnteredComment = ((TimeLog)selectedItem).Comments;
            var dialogParameterForCommentsWindow = new DialogParameters();
            dialogParameterForCommentsWindow.Add("commentText", preEnteredComment.ToString());

            dialogService.ShowDialog("CommentsWindow", dialogParameterForCommentsWindow, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    TimeLog selectedTimeLog = TimeLogsCollection.Find(t => t.Date == ((TimeLog)selectedItem).Date);
                    selectedTimeLog.Comments = r.Parameters.GetValue<string>("comment");
                    TimeLogsCollection = new List<TimeLog>(TimeLogsCollection);
                    RaisePropertyChanged(nameof(TimeLogsCollection));
                    FileService.SetData(fileName, TimeLogsCollection);
                    EnableDelete = TimeLogsCollection.Count > 0 ? true : false;
                }
            });
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
            var comment = parameters.GetValue<string>("comment");

            if (TimeLogsCollection.Where(r => r.Date == DateTime.Now.ToString("d")).Any())
            {
                TimeLogsCollection.RemoveAll(r => r.Date == DateTime.Now.ToString("d"));
                TimeLogsCollection.Add(new TimeLog()
                {
                    Date = DateTime.Now.ToString("d"),
                    From = from,
                    To = to,
                    TotalSpent = spent,
                    Comments = comment
                });
            }
            else
            {
                TimeLogsCollection.Add(new TimeLog()
                {
                    Date = DateTime.Now.ToString("d"),
                    From = from,
                    To = to,
                    TotalSpent = spent,
                    Comments = comment
                });
            }

            FileService.SetData(fileName, TimeLogsCollection);
        }
    }
}

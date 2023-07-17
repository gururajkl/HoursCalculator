using HoursCalculator.Events;
using HoursCalculator.Model;
using HoursCalculator.Services;
using HoursCalculator.ViewModels.Dialogs;
using HoursCalculator.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HoursCalculator.ViewModels
{
    public class MainWindowViewModel : BindableBase, INotifyDataErrorInfo
    {
        #region Commands
        public DelegateCommand SubmitCommand { get; private set; }
        public DelegateCommand NowCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }
        public DelegateCommand SettingClick { get; private set; }
        public DelegateCommand TimeLogCommand { get; private set; }
        public DelegateCommand TimeLogSaveCommand { get; private set; }
        public DelegateCommand CloseAppCommand { get; private set; }
        #endregion

        #region Helper fields
        private readonly ITimerService timerService;
        private readonly IDialogService dialogService;
        private readonly Dictionary<string, List<string>> propertyErrors = new();
        private readonly FileService<TimeLog> fileService;
        private readonly string formatForTime = "h:mm tt";
        private readonly TimeLogsViewModel timeLogsViewModel;
        private readonly IEventAggregator eventAggregator;
        private int errorCount = 0;
        string comments = "";
        #endregion

        public MainWindowViewModel(ITimerService timerService, IDialogService dialogService, IEventAggregator eventAggregator)
        {
            timeLogsViewModel = new TimeLogsViewModel(dialogService, eventAggregator);

            SubmitCommand = new DelegateCommand(CalculateHours, CanExecuteSubmitCommand)
                .ObservesProperty(() => HoursResult)
                .ObservesProperty(() => FromTime)
                .ObservesProperty(() => ToTime);

            NowCommand = new DelegateCommand(CurrentTime);

            ClearCommand = new DelegateCommand(ClearTextBoxes);

            SettingClick = new DelegateCommand(ShowSettingsDialog);

            TimeLogCommand = new DelegateCommand(ShowTimeLogs);

            TimeLogSaveCommand = new DelegateCommand(SaveTimeLog);

            CloseAppCommand = new DelegateCommand(CloseAppPublishCommand);

            this.timerService = timerService;
            this.dialogService = dialogService;
            timerService.Tick += TimerService_Tick;
            this.eventAggregator = eventAggregator;
            fileService = new FileService<TimeLog>(eventAggregator);
            eventAggregator.GetEvent<DataSetEvent>().Subscribe(() => DisplayLineCount());
            DisplayLineCount();
        }

        #region Properties
        private string fromTime = "8:30 AM";
        public string FromTime
        {
            get { return fromTime; }
            set
            {
                RemoveErrors(nameof(FromTime));
                if (string.IsNullOrEmpty(value))
                {
                    AddErrors(nameof(FromTime), "Value cannot be null");
                }
                SetProperty(ref fromTime, value);
            }
        }

        private string toTime = DateTime.Now.ToString("hh:mm tt");
        public string ToTime
        {
            get { return toTime; }
            set
            {
                RemoveErrors(nameof(ToTime));
                if (string.IsNullOrEmpty(value))
                {
                    AddErrors(nameof(ToTime), "Value cannot be null");
                }

                SetProperty(ref toTime, value);
            }
        }

        private string hoursResult;
        public string HoursResult
        {
            get { return hoursResult; }
            set { SetProperty(ref hoursResult, value); }
        }

        private string minutesResult;
        public string MinutesResult
        {
            get { return minutesResult; }
            set { SetProperty(ref minutesResult, value); }
        }

        private string syncText = "Sync is off";
        public string SyncText
        {
            get { return syncText; }
            set { SetProperty(ref syncText, value); }
        }

        private bool sync;
        public bool Sync
        {
            get { return sync; }
            set
            {
                SetProperty(ref sync, value);
                if (value == true)
                {
                    timerService.Start();
                    StatusBar = "Syncing with time";
                    SyncText = "Sync is on";
                    ChangeStatusSyncronously();
                }
                else
                {
                    timerService.Stop();
                    StatusBar = "Not Syncing with time";
                    SyncText = "Sync is off";
                    ChangeStatusSyncronously();
                }
            }
        }

        private Visibility enableSync = Properties.Settings.Default.EnableSync ? Visibility.Visible : Visibility.Collapsed;
        public Visibility EnableSync
        {
            get { return enableSync; }
            set
            {
                if (SetProperty(ref enableSync, value))
                {
                    RaisePropertyChanged(nameof(EnableSync));
                }
            }
        }

        private Visibility showDays = Properties.Settings.Default.ShowDays ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ShowDays
        {
            get { return showDays; }
            set
            {
                if (SetProperty(ref showDays, value))
                {
                    RaisePropertyChanged(nameof(ShowDays));
                }
            }
        }

        private string hoursAndMinutes;
        public string HoursAndMinutes
        {
            get { return hoursAndMinutes; }
            set { SetProperty(ref hoursAndMinutes, value); }
        }

        private string groupBoxHeader;
        public string GroupBoxHeader
        {
            get { return groupBoxHeader; }
            set { SetProperty(ref groupBoxHeader, value); }
        }

        private string statusBar = "Ready";
        public string StatusBar
        {
            get { return statusBar; }
            set { SetProperty(ref statusBar, value); }
        }

        private string dayCount;
        public string DayCount
        {
            get { return dayCount; }
            set { SetProperty(ref dayCount, value); }
        }
        #endregion

        private bool CanExecuteSubmitCommand()
        {
            return !string.IsNullOrEmpty(FromTime) && !string.IsNullOrEmpty(ToTime);
        }

        public void CalculateHours()
        {
            DateTime fromTime24 = DateTime.Now;
            DateTime toTime24 = DateTime.Now;

            try
            {
                try
                {
                    fromTime24 = DateTime.ParseExact(FromTime, formatForTime, null);
                    RemoveErrors(nameof(FromTime));
                }
                catch (Exception)
                {
                    AddErrors(nameof(FromTime), "Format: hh:mm AM/PM");
                    errorCount++;
                }

                try
                {
                    toTime24 = DateTime.ParseExact(ToTime, formatForTime, null);
                    RemoveErrors(nameof(ToTime));
                }
                catch (Exception)
                {
                    AddErrors(nameof(ToTime), "Format: hh:mm AM/PM");
                    errorCount++;
                }

                if (errorCount <= 0 && !(Convert.ToDateTime(fromTime) > Convert.ToDateTime(ToTime)))
                {
                    TimeSpan timeDiff = toTime24 - fromTime24;
                    double totalHours = timeDiff.TotalHours;
                    double totalMinutes = timeDiff.TotalMinutes;

                    double hours = timeDiff.Hours;
                    double minutes = timeDiff.Minutes;
                    string hoursAndMinutes = $"{hours} hour {minutes} minute";

                    HoursResult = $"{totalHours.ToString("0.00")} Hours";
                    MinutesResult = $"{totalMinutes} Minutes";
                    HoursAndMinutes = hoursAndMinutes;

                    GroupBoxHeader = $"Time between {FromTime} and {ToTime} is";
                }
                else
                {
                    HoursResult = "Invalid Inputs...";
                    MinutesResult = "Please provide proper inputs";
                    HoursAndMinutes = "";
                    errorCount = 0;
                }
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid value");
            }
        }

        public void SaveTimeLog()
        {
            if (fileService.GetData("TimeLogs.xml").Count > 0)
            {
                if (fileService.GetData("TimeLogs.xml").Find(t => t.Date == DateTime.Now.ToString("d")) != null)
                    comments = (fileService.GetData("TimeLogs.xml").Find(t => t.Date == DateTime.Now.ToString("d")).Comments);
            }

            DialogParameters timeLogsParamas = new()
            {
                { "from", FromTime },
                { "to", ToTime },
                { "spent", HoursResult },
                { "comment", comments }
            };
            timeLogsViewModel.OnDialogOpened(timeLogsParamas);
            StatusBar = "Log Saved!";
            ChangeStatusSyncronously();
        }

        public async void ChangeStatusSyncronously()
        {
            await Task.Delay(2000);
            StatusBar = "Ready";
        }

        private void ShowSettingsDialog()
        {
            dialogService.Show("Options", r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    EnableSync = r.Parameters.GetValue<bool>("show") ? Visibility.Visible : Visibility.Collapsed;
                    ShowDays = r.Parameters.GetValue<bool>("showDays") ? Visibility.Visible : Visibility.Collapsed;
                    Properties.Settings.Default.EnableSync = r.Parameters.GetValue<bool>("show");
                    Properties.Settings.Default.ShowDays = r.Parameters.GetValue<bool>("showDays");
                    Properties.Settings.Default.AutoStartEnable = r.Parameters.GetValue<bool>("autoStart");
                    Properties.Settings.Default.DarkMode = r.Parameters.GetValue<bool>("darkMode");
                    Properties.Settings.Default.Save();
                    new MainWindow().ChangeTheme();
                }
            });
        }

        public void ShowTimeLogs()
        {
            comments = "";
            var count = fileService.GetData("TimeLogs.xml").Count;
            if (fileService.GetData("TimeLogs.xml").Count > 0)
            {
                if (fileService.GetData("TimeLogs.xml").Find(t => t.Date == DateTime.Now.ToString("d")) != null)
                    comments = (fileService.GetData("TimeLogs.xml").Find(t => t.Date == DateTime.Now.ToString("d")).Comments);
            }

            DialogParameters timeLogsParamas = new()
            {
                { "from", FromTime },
                { "to", ToTime },
                { "spent", HoursResult },
                { "comment", comments }
            };

            dialogService.ShowDialog("TimeLogs", timeLogsParamas, callBack =>
            {
                // TODO implement the call back
            });
        }

        private void DisplayLineCount()
        {
            DayCount = fileService.GetData("TimeLogs.xml").Count.ToString();
            if (Convert.ToInt32(DayCount) >= 40) DayCount = "9+";
        }

        private void TimerService_Tick(object sender, EventArgs e)
        {
            ToTime = DateTime.Now.ToString(formatForTime);
            CalculateHours();
        }

        private void CurrentTime()
        {
            ToTime = DateTime.Now.ToString(formatForTime);
        }

        private void ClearTextBoxes()
        {
            FromTime = ": AM"; ToTime = ": PM";
        }

        public void CloseAppPublishCommand()
        {
            eventAggregator.GetEvent<CloseApplicationEvent>().Publish();
        }

        #region ErrorHandling
        public bool HasErrors => propertyErrors.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return propertyErrors.GetValueOrDefault(propertyName, null);
        }

        public void AddErrors(string propertyName, string errorMessage)
        {
            if (!propertyErrors.ContainsKey(propertyName))
            {
                propertyErrors.Add(propertyName, new List<string>());
            }
            propertyErrors[propertyName].Add(errorMessage);
            OnErrorChanged(propertyName);
        }

        public void RemoveErrors(string propertyName)
        {
            propertyErrors.Remove(propertyName);
            OnErrorChanged(propertyName);
        }

        private void OnErrorChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        #endregion
    }
}

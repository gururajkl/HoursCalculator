using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace HoursCalculator.ViewModels.Dialogs
{
    public class OptionsViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand<string> ButtonCommand { get; set; }

        public event Action<IDialogResult> RequestClose;
        private readonly IEventAggregator eventAggregator;

        public OptionsViewModel(MainWindowViewModel mainWindowViewModel, IEventAggregator eventAggregator)
        {
            ButtonCommand = new DelegateCommand<string>(CloseCommand);
            this.eventAggregator = eventAggregator;
        }

        public string Title => "Options";

        private bool enableCheckBox = Properties.Settings.Default.EnableSync;
        public bool EnableCheckBox
        {
            get { return enableCheckBox; }
            set { SetProperty(ref enableCheckBox, value); }
        }

        private bool showDaysCount = Properties.Settings.Default.ShowDays;
        public bool ShowDaysCount
        {
            get { return showDaysCount; }
            set { SetProperty(ref showDaysCount, value); }
        }

        private bool autoStart = Properties.Settings.Default.AutoStartEnable;
        public bool AutoStart
        {
            get { return autoStart; }
            set
            {
                SetProperty(ref autoStart, value);
            }
        }

        private void CloseCommand(string parameter)
        {
            ButtonResult buttonResult;
            if (parameter == "save") buttonResult = ButtonResult.OK;
            else buttonResult = ButtonResult.Cancel;
            RaiseRequestClose(new DialogResult(buttonResult, new DialogParameters()
            {
                { "show", EnableCheckBox },
                { "autoStart", AutoStart },
                { "showDays", ShowDaysCount }
            }));
        }

        public void RaiseRequestClose(DialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        { }

        public void OnDialogOpened(IDialogParameters parameters)
        { }
    }
}

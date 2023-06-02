using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace HoursCalculator.ViewModels.Dialogs
{
    public class OptionsViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand<string> ButtonCommand { get; set; }

        public event Action<IDialogResult> RequestClose;

        public OptionsViewModel(MainWindowViewModel mainWindowViewModel)
        {
            ButtonCommand = new DelegateCommand<string>(CloseCommand);
        }

        public string Title => "Options";

        private bool enableCheckBox = Properties.Settings.Default.EnableSync;
        public bool EnableCheckBox
        {
            get { return enableCheckBox; }
            set { SetProperty(ref enableCheckBox, value); }
        }

        private bool autoStart = false;
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
                { "autoStart", AutoStart }
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

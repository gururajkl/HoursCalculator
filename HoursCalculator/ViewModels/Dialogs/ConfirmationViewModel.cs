using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoursCalculator.ViewModels.Dialogs
{
    public class ConfirmationViewModel : BindableBase, IDialogAware
    {
        public string Title => "Are you sure?";

        public event Action<IDialogResult> RequestClose;

        private DelegateCommand<string> closeDialogCommand;
        public DelegateCommand<string> CloseDialogCommand => closeDialogCommand ?? (closeDialogCommand = new DelegateCommand<string>(CloseDialog));

        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private void CloseDialog(string parameter)
        {
            ButtonResult result;
            if (parameter?.ToLower() == "yes") result = ButtonResult.Yes;
            else result = ButtonResult.No;
            RaiseRequestClose(new DialogResult(result));
        }

        private void RaiseRequestClose(DialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        { }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
        }
    }
}

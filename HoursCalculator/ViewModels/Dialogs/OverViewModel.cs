using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace HoursCalculator.ViewModels.Dialogs
{
    public class OverViewModel : BindableBase, IDialogAware
    {
        public string Title => "Details";

        private string comment;
        public string Comment
        {
            get { return comment; }
            set { SetProperty(ref comment, value); }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set { SetProperty(ref text, value); }
        }

        public event Action<IDialogResult> RequestClose;
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
        {
            Comment = parameters.GetValue<string>("comment");
            Text = $"You spent {parameters.GetValue<string>("hour")} and spent for";
        }
    }
}

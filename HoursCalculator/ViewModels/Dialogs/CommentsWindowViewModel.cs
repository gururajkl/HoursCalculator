using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace HoursCalculator.ViewModels.Dialogs
{
    public class CommentsWindowViewModel : BindableBase, IDialogAware
    {
        public string Title => "Edit Comments";
        public DelegateCommand<string> ButtonCommand { get; set; }
        public event Action<IDialogResult> RequestClose;

        public CommentsWindowViewModel()
        {
            ButtonCommand = new DelegateCommand<string>(CloseCommand);
        }

        private string commentText;
        public string CommentText
        {
            get
            {
                return commentText;
            }
            set { SetProperty(ref commentText, value); }
        }

        private void CloseCommand(string obj)
        {
            ButtonResult buttonResult;
            if (obj == "save") buttonResult = ButtonResult.OK;
            else buttonResult = ButtonResult.Cancel;

            RaiseRequestClose(new DialogResult(buttonResult, new DialogParameters()
            {
                { "comment", CommentText }
            }));
        }

        public void RaiseRequestClose(DialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            CommentText = parameters.GetValue<string>("commentText");
        }

        public void OnDialogClosed()
        { }
    }
}

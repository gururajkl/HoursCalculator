using HoursCalculator.Events;
using HoursCalculator.Services;
using HoursCalculator.ViewModels.Dialogs;
using HoursCalculator.Views;
using HoursCalculator.Views.Dialogs;
using Prism.Events;
using Prism.Ioc;
using System.Windows;

namespace HoursCalculator
{
    public partial class App
    {
        private IEventAggregator eventAggregator;

        protected override Window CreateShell()
        {
            CloseApp();
            eventAggregator.GetEvent<RegisterAutoStart>().Publish();
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ITimerService, TimerService>();
            containerRegistry.RegisterDialog<Options, OptionsViewModel>();
            containerRegistry.RegisterDialog<TimeLogs, TimeLogsViewModel>();
            containerRegistry.RegisterDialog<CommentsWindow, CommentsWindowViewModel>();
            containerRegistry.RegisterDialog<OverView, OverViewModel>();
            containerRegistry.RegisterDialog<Confirmation, ConfirmationViewModel>();
        }

        /* AutoStart app logic (Legacy)
        public void RegisterAutoStart()
        {
            string appName = Assembly.GetEntryAssembly().GetName().Name;
            string runKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(runKey, true))
            {
                if (registryKey != null)
                {
                    if (HoursCalculator.Properties.Settings.Default.AutoStartEnable)
                    {
                        if (registryKey.GetValue(appName) == null)
                            registryKey.SetValue(appName, Process.GetCurrentProcess().MainModule.FileName);
                    }
                    else
                    {
                        if (registryKey.GetValue(appName) != null)
                            registryKey.DeleteValue(appName, false);
                    }
                }
            }
        }
        */

        private void CloseApp()
        {
            eventAggregator = ((App)Current).Container.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<CloseApplicationEvent>().Subscribe(() =>
            {
                Current.Dispatcher.Invoke(() =>
                {
                    Current.Shutdown();
                });
            });
        }
    }
}

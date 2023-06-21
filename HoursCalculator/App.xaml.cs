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
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ITimerService, TimerService>();
            containerRegistry.RegisterDialog<Options, OptionsViewModel>();
            containerRegistry.RegisterDialog<TimeLogs, TimeLogsViewModel>();
            containerRegistry.RegisterDialog<CommentsWindow, CommentsWindowViewModel>();
        }

        /* AutoStart Enable logic
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var appName = "CheckTime";
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DllFiles", "Hours Calculator.dll");
            var runKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);


            if (CheckTime.Properties.Settings.Default.AutoStartEnable)
            {
                if (runKey.GetValue(appName) == null)
                {
                    // Set the path to your application executable
                    var appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    runKey.SetValue(appName, appPath);
                }
            }
            else
            {
                if (runKey.GetValue(appName) != null)
                    runKey.DeleteValue(appName);
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

using System;
using System.Windows.Threading;

namespace HoursCalculator.Services
{
    public class TimerService : ITimerService
    {
        public event EventHandler Tick;
        private DispatcherTimer dispatcherTimer;

        public TimerService()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(3);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            Tick?.Invoke(sender, e);
        }

        public void Start()
        {
            dispatcherTimer.Start();
        }

        public void Stop()
        {
            dispatcherTimer.Stop();
        }
    }
}

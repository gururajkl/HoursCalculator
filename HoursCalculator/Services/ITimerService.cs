using System;

namespace HoursCalculator.Services
{
    public interface ITimerService
    {
        event EventHandler Tick;
        void Start();
        void Stop();
    }
}

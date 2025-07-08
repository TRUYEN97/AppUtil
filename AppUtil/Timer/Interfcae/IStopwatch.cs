using System;

namespace AppUtil.Timer.Interfcae
{
    public interface IStopwatch
    {
        void Reset();
        void Start(long interval);
        bool IsOntime();
        bool IsOutOfTime();
        long GetCurrentTime();
        long Interval { get; set; }
    }
}

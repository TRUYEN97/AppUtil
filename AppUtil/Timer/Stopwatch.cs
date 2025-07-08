
using System;
using AppUtil.Timer.Interfcae;

namespace AppUtil.Timer
{
    internal class Stopwatch : IStopwatch
    {
        private readonly DateTimeOffset _now;
        private long _interval;
        private long _startTime;
        public Stopwatch(long interval)
        {
            _now = DateTimeOffset.Now;
            Start(interval);
        }

        public long Interval { get => _interval; set { _interval = value < 0 ? 0 : value; } }

        public long GetCurrentTime()
        {
            return _now.Millisecond - _startTime;
        }

        public bool IsOntime()
        {
            return GetCurrentTime() < _interval;
        }

        public bool IsOutOfTime()
        {
            return !IsOntime();
        }

        public void Reset()
        {
            _startTime = _now.Millisecond;
        }

        public void Start(long interval)
        {
            Interval = interval;
            Reset();
        }
    }
}

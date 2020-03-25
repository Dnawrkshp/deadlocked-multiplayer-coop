using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DLMC.Shared
{
    public class LogicTimer
    {
        public const float FramesPerSecond = 64.0f;
        public const float FixedDelta = 1.0f / FramesPerSecond;

        protected double _time = 0d;
        protected double _accumulator;
        protected long _lastTime;

        private readonly Stopwatch _stopwatch;
        private readonly Action _action;

        public float LerpAlpha => (float)_accumulator / FixedDelta;

        public virtual double Time => _time;

        public LogicTimer(Action action)
        {
            _stopwatch = new Stopwatch();
            _action = action;
        }

        public void Start()
        {
            _lastTime = 0;
            _accumulator = 0.0;
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public virtual void Update()
        {
            long elapsedTicks = _stopwatch.ElapsedTicks;
            _accumulator += (double)(elapsedTicks - _lastTime) / Stopwatch.Frequency;
            _lastTime = elapsedTicks;

            while (_accumulator >= FixedDelta)
            {
                _action();
                _accumulator -= FixedDelta;
                _time += FixedDelta;
            }
        }
    }
}

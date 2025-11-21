using System;
using System.Collections.Generic;
using Game.Core;

// TODO
namespace Game.Core
{
    public sealed class TickMgr
    {
        private const int WheelSize = 256;

        private static TickMgr _instance;
        public static TickMgr Instance => _instance ??= new TickMgr();

        private readonly List<ScheduledCall>[] _wheel = new List<ScheduledCall>[WheelSize];
        private readonly Dictionary<ulong, ScheduledCall> _activeCalls = new();

        private double _accumulator;
        private ulong _nextHandle = 1;

        private TickMgr(int framesPerSecond = 60)
        {
            TickLength = 1.0 / framesPerSecond;

            for (int i = 0; i < WheelSize; i++)
            {
                _wheel[i] = new List<ScheduledCall>();
            }
        }

        public double TickLength { get; }
        public long TickCount { get; private set; }

        private sealed class ScheduledCall
        {
            public ulong Handle;
            public Action Callback;
            public bool Repeat;
            public int Interval;
            public long TargetTick;
            public bool Cancelled;
        }

        public ulong Schedule(Action callback, int delayTicks = 1, bool repeat = false, int repeatInterval = 0)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (delayTicks < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(delayTicks), "Delay must be zero or greater.");
            }

            if (repeat)
            {
                if (repeatInterval <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(repeatInterval), "Repeat interval must be at least 1 tick.");
                }
            }

            var call = new ScheduledCall
            {
                Handle = _nextHandle++,
                Callback = callback,
                Repeat = repeat,
                Interval = repeat ? repeatInterval : 0
            };

            _activeCalls[call.Handle] = call;
            PlaceCall(call, delayTicks);
            return call.Handle;
        }

        public bool Cancel(ulong handle)
        {
            if (_activeCalls.TryGetValue(handle, out var call))
            {
                call.Cancelled = true;
                _activeCalls.Remove(handle);
                return true;
            }

            return false;
        }

        private void PlaceCall(ScheduledCall call, int delayTicks)
        {
            int safeDelay = Math.Max(delayTicks, 1);

            call.TargetTick = TickCount + safeDelay;
            int slotIndex = (int)(call.TargetTick % WheelSize);
            _wheel[slotIndex].Add(call);
        }

        public void Update(double delta)
        {
            if (delta <= 0)
            {
                return;
            }

            _accumulator += delta;
            while (_accumulator >= TickLength)
            {
                _accumulator -= TickLength;
                AdvanceWheel();
            }
        }

        public void AdvanceTicks(int ticks)
        {
            if (ticks <= 0)
            {
                return;
            }

            for (int i = 0; i < ticks; i++)
            {
                AdvanceWheel();
            }
        }

        private void AdvanceWheel()
        {
            int slotIndex = (int)(TickCount % WheelSize);
            var bucket = _wheel[slotIndex];

            if (bucket.Count > 0)
            {
                for (int i = bucket.Count - 1; i >= 0; i--)
                {
                    var call = bucket[i];

                    if (call.Cancelled)
                    {
                        bucket.RemoveAt(i);
                        continue;
                    }

                    if (call.TargetTick > TickCount)
                    {
                        continue;
                    }

                    bucket.RemoveAt(i);

                    try
                    {
                        call.Callback();
                    }
                    catch (Exception ex)
                    {
                        GameLogger.LogError($"Error in scheduled call: {ex}");
                    }

                    if (call.Repeat && !call.Cancelled)
                    {
                        PlaceCall(call, call.Interval);
                    }
                    else
                    {
                        _activeCalls.Remove(call.Handle);
                    }
                }
            }

            TickCount++;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StoneToolkit.Common.Timers
{
    public class IntervalTriggerManager
    {
        public Dictionary<object, IntervalTrigger> IntervalTriggerMap { get; set; } = new Dictionary<object, IntervalTrigger>();

        private IntervalTriggerManager()
        {
        }

        private static IntervalTriggerManager _current;

        public static IntervalTriggerManager Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new IntervalTriggerManager();
                }

                return _current;
            }
        }

        public IntervalTrigger GetTimerIntervalTrigger(object id)
        {
            IntervalTrigger intervalTrigger;
            if (!IntervalTriggerMap.TryGetValue(id, out intervalTrigger))
            {
                return intervalTrigger;
            }

            return null;
        }

        public TimerIntervalTrigger CreateOrGetTimerIntervalTrigger(object id)
        {
            IntervalTrigger intervalTrigger;
            if (IntervalTriggerMap.TryGetValue(id, out intervalTrigger))
            {
                if (intervalTrigger is TimerIntervalTrigger)
                {
                    return intervalTrigger as TimerIntervalTrigger;
                }

                intervalTrigger.Dispose();
                IntervalTriggerMap.Remove(id);
            }

            TimerIntervalTrigger timerIntervalTrigger = new TimerIntervalTrigger();
            IntervalTriggerMap.Add(id, timerIntervalTrigger);
            timerIntervalTrigger.StartTimer();

            return timerIntervalTrigger;
        }

        public void DestoryIntervalTrigger(object id)
        {
            IntervalTrigger intervalTrigger;
            if (IntervalTriggerMap.TryGetValue(id, out intervalTrigger))
            {
                intervalTrigger.Dispose();
                IntervalTriggerMap.Remove(id);
            }
        }
    }
}

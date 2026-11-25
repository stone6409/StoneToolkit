using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StoneToolkit.Common.Timers
{
    public class TimerIntervalTrigger : IntervalTrigger
    {
        public TimerIntervalTrigger()
        {
        }

        #region timer

        public override void StartTimer()
        {
            DisposeTimer();

            _timer = new Timer(TimerCallback, this, 0, Interval);
        }

        private Timer _timer;

        bool _hasEntered = false;

        private void TimerCallback(Object stateInfo)
        {
            if (_hasEntered)
            {
                return;
            }

            _hasEntered = true;

            // Do Somthing
            OnTick();

            _hasEntered = false;
        }

        public override void DisposeTimer()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        #endregion
    }
}

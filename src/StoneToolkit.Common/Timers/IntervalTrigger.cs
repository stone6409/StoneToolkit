using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.Common.Timers
{
    public abstract class IntervalTrigger
    {
        public int Interval { get; set; } = 100;

        public TriggedHandler _triggedHandler;

        public IntervalTrigger()
        {
        }

        public void AddTriggedHandler(TriggedHandler triggedHandler)
        {
            _triggedHandler += triggedHandler;
        }

        public void RemoveTriggedHandler(TriggedHandler triggedHandler)
        {
            _triggedHandler -= triggedHandler;
        }

        protected void OnTick()
        {
            if (_triggedHandler != null)
            {
                _triggedHandler();
            }
        }

        public abstract void StartTimer();

        public abstract void DisposeTimer();

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Release managed resources
                }

                // Release unmanaged resources

                DisposeTimer();

                _disposed = true;
            }
        }
    }
}

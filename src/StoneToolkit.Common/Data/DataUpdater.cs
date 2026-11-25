using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StoneToolkit.Common.Data
{
    public class DataUpdater
    {
        public DataUpdaterCallback DataUpdaterCallback { get; private set; }

        private Thread _runThread;

        private ManualResetEvent _startedEvent = new ManualResetEvent(false);

        private AutoResetEvent _stopEvent = new AutoResetEvent(false);

        public DataUpdater(DataUpdaterCallback dataUpdaterCallback)
        {
            DataUpdaterCallback = dataUpdaterCallback;
        }

        public void Start()
        {
            _runThread = new Thread(new ThreadStart(Run));
            _runThread.Start();

            _startedEvent.WaitOne();
        }

        public void Run()
        {
            _startedEvent.Set();

            var waitHandles = new WaitHandle[1];
            waitHandles[0] = _stopEvent;

            bool isShutdown = false;

            while (!isShutdown)
            {

                int index = WaitHandle.WaitAny(waitHandles, 50);
                switch (index)
                {
                    case 0:
                        isShutdown = true;
                        break;
                    case WaitHandle.WaitTimeout:
                        {
                            DataUpdaterCallback();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public void Stop()
        {
            _stopEvent.Set();
            _runThread.Join();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal delegate void ThreadCompletedEventHandler();

    internal class ThreadJob
    {
        Thread _thread = null;
        System.Windows.Forms.Timer _endTimer;

        public event ThreadStart DoWork;
        public event ThreadCompletedEventHandler RunWorkerCompleted;

        public bool IsAlive
        {
            get
            {
                if (null == _thread)
                    return false;
                else
                    return _thread.IsAlive;
            }
        }

        public void Start()
        {
            _thread = new Thread(DoWork);
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Priority = ThreadPriority.Normal;
            _thread.IsBackground = false;
            _thread.Start();
            _endTimer = new System.Windows.Forms.Timer();
            _endTimer.Interval = 1000;
            _endTimer.Tick += new EventHandler(_endTimer_Tick);
            _endTimer.Enabled = true;
        }

        public void Abort()
        {
            if (null != _thread)
            _thread.Abort();
        }

        private void _endTimer_Tick(object sender, EventArgs e)
        {
            if ((null != _thread) && (false == _thread.IsAlive))
            {
                _endTimer.Enabled = false;
                _thread = null;

                if (null != RunWorkerCompleted)
                    RunWorkerCompleted();
            }
        }
    }
}

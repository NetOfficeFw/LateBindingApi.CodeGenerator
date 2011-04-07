using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Text;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer
{
    public delegate void ThreadProgressHandler();
    public delegate void ThreadCompletedEventHandler();

    internal class ThreadJob
    {
        Thread _thread = null;
        private System.Windows.Forms.Timer _endTimer;

        public event ThreadStart DoWork;
        public event ThreadCompletedEventHandler RunWorkerCompleted;

        public bool IsAlive
        {
            get 
            {
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
            _endTimer.Interval = 100;
            _endTimer.Tick += new EventHandler(_endTimer_Tick);
            _endTimer.Enabled = true;
        }

        public void Abort()
        {
            _thread.Abort();
        }

        private void _endTimer_Tick(object sender, EventArgs e)
        {
            if (false == _thread.IsAlive)
            {
                if (null != RunWorkerCompleted)
                    RunWorkerCompleted();

                _endTimer.Enabled = false;
            }
        }
    }
}

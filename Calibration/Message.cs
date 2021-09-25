using System;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using System.Collections.Concurrent;
using AlarmLibrary;
using System.Threading;

namespace Calibration
{
    public partial class Message : DockContent
    {
        private ConcurrentQueue<string> queueMessage = new ConcurrentQueue<string>();
        public Message()
        {
            InitializeComponent();
        }

        private void Message_Load(object sender, EventArgs e)
        {
            Log.ReadOnly = true;
            NotifyG.EventHandlerNotify += Receive;
            timerUpdate.Enabled = true;
        }

        public void Receive(string args)
        {
            queueMessage.Enqueue(args);
        }
        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            while (queueMessage.Count > 0)
            {
                string result;
                queueMessage.TryDequeue(out result);
                ShowMessage(result);
            }
        }

        private void ShowMessage(string args)
        {
            {
                if (Log.Text.Length > 500 * 1024) { Log.Clear();}      
                Log.AppendText(args + "\r\n");
            }
        }

        private void ClearLog_Click(object sender, EventArgs e)
        {
            Log.Clear();
        }
    }
}

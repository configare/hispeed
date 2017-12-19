using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public class ClockLabel :Label, IDisposable
    {
        private Timer _timer;
        private string _dateFormat = "HH:mm:ss" + Environment.NewLine + "yyyy年MM月dd日";

        public ClockLabel()
        {
            this.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Font = new System.Drawing.Font("微软雅黑", 14.25f);
            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(UpdateTime));
            else
                UpdateTime();
        }

        [DefaultValue("HH:mm:ss " + "yyyy年MM月dd日")]
        public string DateFormat
        {
            get { return _dateFormat; }
            set { _dateFormat = value; }
        }

        /// <summary>
        /// 14:46 2012年10月12日
        /// </summary>
        private void UpdateTime()
        {
            this.Text = DateTime.Now.ToString(_dateFormat);
        }

        private void Close()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            this.Close();
            base.Dispose(disposing);
        }
    }
}

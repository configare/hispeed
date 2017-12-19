using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading;

namespace CodeCell.AgileMap.WebComponent
{
    internal class SyncCallStatus:IDisposable
    {
        private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        private object _result = null;

        public SyncCallStatus()
        {
        }

        public object Result
        {
            get { return _result; }
            set { _result = value; }
        }

        public void WaitOne()
        {
            _autoResetEvent.WaitOne();
        }

        public void Set()
        {
            _autoResetEvent.Set();
        }

        public bool Reset()
        {
            return _autoResetEvent.Reset();
        }

        public void Dispose()
        {
            if (_autoResetEvent != null)
            {
                _autoResetEvent.Dispose();
                _autoResetEvent = null;
            }
        }
    }
}

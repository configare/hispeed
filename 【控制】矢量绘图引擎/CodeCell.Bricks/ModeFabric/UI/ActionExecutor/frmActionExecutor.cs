using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using CodeCell.Bricks.Runtime;
using CodeCell.Bricks.UIs;

namespace CodeCell.Bricks.ModelFabric
{
    public partial class frmActionExecutor : Form,IActionExecutor,IProgressTracker,ILog,IContextMessage
    {
        private const string cstInfoString = "[{0}] 消息:\n      {1}";
        private const string cstWarningString = "[{0}] 警告:\n      {1}";
        private const string cstErrorString = "[{0}] 错误:\n      {1}";
        private const string cstExceptionString = "[{0}] 异常:\n      {1}";
        private const string cstResultString = "[{0}] {1}";
        private IAction _action = null;
        private bool _running = false;
        private bool _isOK = false;
        private List<string> _errorInfos = new List<string>();

        private enum enumLogType
        {
            Info,
            Error,
            Waring,
            Exception,
            Result
        }
        private enum enumProgressType
        {
            StartTracking,
            Tracking,
            Stop
        }
        private delegate void WriteLogHandler(enumLogType logType, object data);
        private delegate void ProgressHandler(enumProgressType prgType, string text, int time);
        private delegate void FinishedHandler();
        private WriteLogHandler _writeLogHandler = null;
        private ProgressHandler _progressHandler = null;
        private FinishedHandler _finishedHandler = null;
  
        public frmActionExecutor()
        {
            InitializeComponent();
            InitControls();
        }

        private void InitControls()
        {
            txtMsg.Text = string.Empty;
            progressBar1.Visible = false;
            _progressHandler = new ProgressHandler(DoProgress);
            _writeLogHandler = new WriteLogHandler(DoLog);
            _finishedHandler = new FinishedHandler(Finished);
            FormClosing += new FormClosingEventHandler(frmActionExecutor_FormClosing);
        }

        void frmActionExecutor_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = _running;
        }

        private void btnDo_Click(object sender, EventArgs e)
        {
            if (_action == null)
            {
                MsgBox.ShowError("没有活动Action。");
                return;
            }
            _errorInfos.Clear();
            _running = true;
            _isOK = false;
            progressBar1.Visible = true;
            btnDo.Enabled = btnClose.Enabled = false;
            _action.SetLog(this);
            _action.SetTracker(this);
            WaitCallback callback = new WaitCallback(AsyncDoAction);
            ThreadPool.QueueUserWorkItem(callback, _action);
        }

        private void AsyncDoAction(object action)
        {
            try
            {
                _isOK =  (action as IAction).Do(this);
            }
            catch (Exception ex)
            {
                this.Invoke(_writeLogHandler, enumLogType.Exception, ex);
            }
            finally 
            {
                this.Invoke(_finishedHandler);
            }
        }

        private void Finished()
        {
            if (!_isOK)
            {
                if (_errorInfos.Count > 0)
                    WriterError(string.Concat(_errorInfos.ToArray()));
                WriterResult("工具\"" + _action.Name + "\"执行失败。");
            }
            else
                WriterResult("工具\"" + _action.Name + "\"执行成功。");
            progressBar1.Value = progressBar1.Maximum;
            btnClose.Enabled = true;
            _running = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DoProgress(enumProgressType prgType, string text, int time)
        {
            switch (prgType)
            {
                case enumProgressType.StartTracking:
                    if (text != null)
                    {
                        txtMsg.Text = text;
                        DoLog(enumLogType.Info, text);
                    }
                    if (time < 1)
                        return;
                    progressBar1.Maximum = time;
                    break;
                case enumProgressType.Tracking:
                    if (!string.IsNullOrEmpty(text))
                    {
                        txtMsg.Text = text;
                        DoLog(enumLogType.Info, text);
                    }
                    if (time > progressBar1.Minimum && time < progressBar1.Maximum)
                    {
                        progressBar1.Value = time;
                    }
                    break;
                case enumProgressType.Stop:
                    progressBar1.Value = progressBar1.Maximum;
                    break;
            }
        }

        private void DoLog(enumLogType logType, object data)
        {
            switch (logType)
            {
                case enumLogType.Info:
                    if (data == null)
                        return;
                    txtLog.AppendText(string.Format(cstInfoString, GetCurrentTime(), data.ToString()));
                    txtLog.AppendText("\n");
                    Application.DoEvents();
                    break;
                case enumLogType.Result:
                    if (data == null)
                        return;
                    txtLog.AppendText(string.Format(cstResultString, GetCurrentTime(), data.ToString()));
                    txtLog.AppendText("\n");
                    Application.DoEvents();
                    break;
                case enumLogType.Waring:
                    if (data == null)
                        return;
                    txtLog.AppendText(string.Format(cstWarningString, GetCurrentTime(), data.ToString()));
                    txtLog.AppendText("\n");
                    Application.DoEvents();
                    break;
                case enumLogType.Error:
                    if (data == null)
                        return;
                    txtLog.AppendText(string.Format(cstErrorString, GetCurrentTime(), data.ToString()));
                    txtLog.AppendText("\n");
                    Application.DoEvents();
                    break;
                case enumLogType.Exception:
                    if (data == null)
                        return;
                    txtLog.AppendText(string.Format(cstExceptionString, GetCurrentTime(), (data as Exception).Message));
                    txtLog.AppendText("\n");
                    txtLog.SelectionStart = txtLog.Text.Length;
                    Application.DoEvents();
                    break;
            }
        }

        #region IProgressTracker 成员

        public bool IsBusy
        {
            get { return false; }
        }

        public void StartTracking(string text, int estimateTotalTime)
        {
            this.Invoke(_progressHandler, enumProgressType.StartTracking, text, estimateTotalTime);
        }

        public void StartTracking(string text)
        {
            this.Invoke(_progressHandler, enumProgressType.Tracking,text, -1);
        }

        public void StopTracking()
        {
            this.Invoke(_progressHandler, enumProgressType.Stop, string.Empty, -1);
        }

        public void Tracking(string text, int currentTime)
        {
            this.Invoke(_progressHandler, enumProgressType.Tracking, text, currentTime);
        }

        public void Tracking(string text)
        {
            this.Invoke(_progressHandler, enumProgressType.Tracking, text, -1);
        }

        #endregion

        #region ILog 成员

        public void WriterWarning(string sWarning)
        {
            this.Invoke(_writeLogHandler, enumLogType.Waring, sWarning);
        }

        public void WriterError(string sError)
        {
            this.Invoke(_writeLogHandler, enumLogType.Error, sError);
        }

        public void WriterInfo(string sInfo)
        {
            this.Invoke(_writeLogHandler, enumLogType.Info, sInfo);
        }

        public void WriterResult(string sInfo)
        {
            this.Invoke(_writeLogHandler, enumLogType.Result, sInfo);
        }

        public void WriterException(Exception Ex)
        {
            this.Invoke(_writeLogHandler, enumLogType.Exception, Ex);
        }

        #endregion

        private string GetCurrentTime()
        {
            return DateTime.Now.ToLongDateString()+" "  +DateTime.Now.ToLongTimeString();
        }

        #region IActionExecutor 成员

        public void Queue(IAction action)
        {
            _action = action;
            if (_action != null)
                Text = "执行 \"" + _action.Name + "\"...";
        }

        #endregion

        #region IContextMessage 成员

        public void Reset()
        {
            //
        }

        public string GetErrorInfoString()
        {
            return string.Empty;
        }

        public void AddError(string errorInfo)
        {
            _errorInfos.Add(errorInfo);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.AppEnv
{
    public class AppArgsIsSetedChecker:IQueueTaskItem,IStartChecker
    {
        protected bool _isOK = false;
        protected object _data = null;

        #region IQueueTaskItem 成员

        public string Name
        {
            get { return "检查系统运行环境参数是否已经正确配置"; }
        }

        public void Do(IProgressTracker tracker)
        {
            try
            {
                //string wksDir = Configer.GetWorkspaceDir();
                //if (Directory.Exists(wksDir))
                //{
                //    string tempDir = Configer.GetTemporalFileDir();
                //    if (Directory.Exists(tempDir))
                //    {
                //        _isOK = true;
                //        return;
                //    }
                //}
                _isOK = DoConfig();
            }
            catch (Exception ex)
            {
                //_isOK = DoConfig();
                throw;
            }
            finally
            {
            }
        }

        private bool DoConfig()
        {
            //MsgBox.ShowInfo("系统运行环境参数没有设置或者设置错误,按[确定]按钮现在进行配置。");
            ////
            //using (frmConfigWnd frm = new frmConfigWnd())
            //{
            //    if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //    {
            //        return true;
            //    }
            //}
            return true;
        }

        #endregion

        #region IStartChecker 成员

        public bool IsCanContinue
        {
            get { return false; }
        }

        public bool IsOK
        {
            get { return _isOK; }
        }

        public object Data
        {
            get { return _data; }
        }

        public Exception Exception
        {
            get { return new Exception(string.Empty); }
        }

        public string Message
        {
            get { return "系统运行环境参数没有设置或者设置错误,终止系统启动。"; }
        }

        #endregion
    }
}

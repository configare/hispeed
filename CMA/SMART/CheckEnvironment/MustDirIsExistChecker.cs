using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.AppEnv
{
    public class MustDirIsExistChecker : IQueueTaskItem,IStartChecker
    {
        private Dictionary<string, string> _dirs = null;
        private object _data = null;
        private bool _isOK = true;
        private bool _isCanContinue = true;

        public MustDirIsExistChecker()
        {
            _dirs = new Dictionary<string, string>();
            //BEGIN GDAL
            _dirs.Add(@"gdal-data", "");
            _dirs.Add(@"gdalplugins", "");
            _dirs.Add(@"projlib", "");
            //END GDAL
            //BEGIN COORDINATE SYSTEM
            _dirs.Add(@"坐标系统\自定义", "");
            _dirs.Add(@"坐标系统\预定义", "");
            _dirs.Add(@"SystemData\Scripts", "");
            _dirs.Add(@"SystemData\RasterTemplate", "");
            //END SYSTEM
            _dirs.Add(@"Python25\Lib", "Python脚本可能无法运行");
            _dirs.Add(@"数据引用", "数据引用功能可能无法使用");
            _dirs.Add(@"LayoutTemplate", "专题图模板可能无法使用");
            _dirs.Add(@"MapResource", "矢量地图符号库无法使用");
        }

        #region IQueueTaskItem 成员

        public string Name
        {
            get { return "检查必要的文件是否存在?"; }
        }

        public void Do(IProgressTracker tracker)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory ;
            List<string> lostDirKeys = new List<string>();
            string tipFrt = "正在检查文件夹\"{0}\"......";
            foreach (string subDir in _dirs.Keys)
            {
                tracker.Tracking(string.Format(tipFrt, subDir));
                try
                {
                    if (!Directory.Exists(dir + subDir))
                        lostDirKeys.Add(subDir);
                }
                catch 
                {
                    lostDirKeys.Add(subDir);
                }
            }
            if (lostDirKeys.Count > 0)
            {
                _isOK = false;
                _data = lostDirKeys.ToArray();
            }
        }

        #endregion

        #region IStartChecker 成员

        public bool IsOK
        {
            get { return _isOK; }
        }

        public object Data
        {
            get { return _data ; }
        }

        public bool IsCanContinue
        {
            get 
            {
                return _isCanContinue; ; 
            }
        }

        public string Message
        {
            get
            {
                string[] dirs = _data as string[];
                foreach (string dir in dirs)
                {
                    if (_dirs[dir] == string.Empty)
                    {
                        _isCanContinue = false;
                        break;
                    }
                }
                if (!_isCanContinue)
                    return "应用程序运行必须的部分文件丢失,终止启动。";
                else
                    return "应用程序运行必须的部分文件夹丢失,某些功能将不能正确执行,但不影响程序启动。";
            }
        }

        public Exception Exception
        {
            get 
            {
                string str = "丢失的文件夹列表:\n";
                string[] dirs = _data as string[];
                foreach (string dir in dirs)
                {
                    string sLine = dir + ":" + _dirs[dir] + "\n";
                    str += sLine;
                }
                return new Exception(str);
            }
        }

        #endregion
    }
}

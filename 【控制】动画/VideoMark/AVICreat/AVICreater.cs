using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.VideoMark
{
    public class AVICreater
    {
        private int _interval = 300;
        private Image[] _res = null;
        private string _filename = string.Empty;
        private Action<int, string> _progress = null;

        public AVICreater(string filename, int interval, Image[] res, Action<int, string> progress)
        {
            _interval = interval;
            _res = res;
            _filename = filename;
            _progress = progress;
        }

        public void AVICreate()
        {
            if (_res == null || _res.Length == 0 || _interval == 0)
                return;
            if (_progress != null)
                _progress(0, "正在生成AVI动画,请稍候...");
            Bitmap bitmap = null;
            if (_res[0] != null)
            {
                bitmap = new Bitmap(_res[0]);
                _res[0].Dispose();
            }
            AviManager aviManager = new AviManager(_filename, false);
            VideoStream aviStream = aviManager.AddVideoStream(true, GetFrame(_interval), bitmap);
            for (int n = 1; n < _res.Length; n++)
            {
                if (_progress != null)
                    _progress(100 * n / _res.Length, "正在生成AVI动画,请稍候...");
                if (_res[n] == null)
                    continue;
                bitmap = new Bitmap(_res[n]);
                aviStream.AddFrame(bitmap);
                _res[n].Dispose();
            }
            aviManager.Close();
        }

        private double GetFrame(int _interval)
        {
            return Math.Round((double)1000 / _interval, 1);
        }
    }
}

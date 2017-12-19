using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace GeoDo.RSS.VideoMark
{
    public class GIFCreater
    {
        private AnimatedGifEncoder coder = new AnimatedGifEncoder();
        private Stream stream = new MemoryStream();
        private int _interval = 300;
        private Image[] _res = null;
        private Action<int, string> _progress = null;

        public GIFCreater(Image[] res, Action<int, string> progress)
        {
            _res = res;
            _progress = progress;
        }

        public GIFCreater(int width, int height, Image[] res, Action<int, string> progress)
        {
            coder.SetSize(width, height);
            _res = res;
            _progress = progress;
        }

        public int Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        public Stream Create(Stream stream)
        {
            coder.Start(stream);
            Process();
            return stream;
        }

        private void Process()
        {
            if (_progress != null)
                _progress(0, "正在生成GIF动画,请稍候...");
            coder.SetRepeat(0);
            for (int i = 0; i < _res.Length; i++)
            {
                if (_progress != null)
                    _progress(100 * i / _res.Length, "正在生成GIF动画,请稍候...");
                if (_res[i] == null)
                    continue;
                coder.SetDelay(_interval);
                coder.AddFrame(_res[i]);
                _res[i].Dispose();
            }
            coder.Finish();
        }


        public void Create(string path)
        {
            //coder.Start(path);用它的这个方法,比用 stream 生成的要大!
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                coder.Start(fs);
                Process();
                fs.Close();
            }
            //FileStream fs = new FileStream(path, FileMode.Create);
        }
    }
}

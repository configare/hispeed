using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace GeoDo.RSS.VideoMark
{
    public class ViedoMarkFactory : IViedoMarker
    {
        public ViedoMarkFactory()
        {

        }

        #region IViedoMarker 成员

        /// <summary>
        /// 保存avi文件到指定路径
        /// </summary>
        /// <param name="avifilename">保存avi文件的完整路径</param>
        /// <param name="fileType">保存的文件类型，[avi|gif]</param>
        /// <param name="size">输出尺寸</param>
        /// <param name="interval">动画时间间隔</param>
        /// <param name="res">生成动画的图片组</param>
        /// <param name="progress">进度条</param>
        /// <returns></returns>
        public bool Mark(string avifilename, string fileType, Size size, int interval, Image[] res, Action<int, string> progress)
        {
            if (res == null || res.Length == 0)
                return false;
            if (String.IsNullOrEmpty(fileType))
                return false;
            if (fileType.ToLower() == "avi")
                return CreatAVIFile(avifilename, size, interval, res, progress);
            else if (fileType.ToLower() == "gif")
                return CreatGIFFile(avifilename, size, interval, res, progress);
            else
                return false;
        }

        private bool CreatGIFFile(string avifilename, Size size, int interval, Image[] res, Action<int, string> progress)
        {
            try
            {
                IViedoMarkProcesser processer = new ViedoMarkerToGIF();
                Size oSize = size;
                if (processer.Support(avifilename))
                {
                    ImageProcessClass.ImageProcess(res, size, out oSize);
                    return processer.Mark(avifilename, oSize, interval, res, progress);
                }
                return false;
            }
            finally
            {
                if (progress != null)
                    progress(100, "AVI动画生成完成！");
            }
        }

        private bool CreatAVIFile(string avifilename, Size size, int interval, Image[] res, Action<int, string> progress)
        {
            try
            {
                IViedoMarkProcesser processer = new ViedoMarkerToAVI();
                Size oSize = size;
                if (processer.Support(avifilename))
                {
                    ImageProcessClass.ImageProcess(res, size, out oSize);
                    return processer.Mark(avifilename, oSize, interval, res, progress);
                }
                return false;
            }
            finally
            {
                if (progress != null)
                    progress(100, "gif动画生成完成！");
            }
        }

        #endregion
    }
}

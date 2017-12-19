using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RasterProject;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public class FileBaseInfo
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string Filename;
        /// <summary>
        /// 附属文件
        /// </summary>
        public string[] AttachmentFiles;
        /// <summary>
        /// 缩略图文件
        /// </summary>
        public string Thumbnail;
        /// <summary>
        /// 文件坐标范围
        /// </summary>
        public PrjEnvelope Envelope;
        /// <summary>
        /// 分辨率
        /// </summary>
        public double ResolutionX;
        /// <summary>
        /// 分辨率
        /// </summary>
        public double ResolutionY;
        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long Length;
    }
}

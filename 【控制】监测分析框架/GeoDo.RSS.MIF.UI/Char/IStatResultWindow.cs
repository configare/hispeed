using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geodo.RSS.MIF.UI
{
    //by chennan 2011-4-21 增加Excel统计合计行
    public interface IStatResultWindow
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowText"></param>
        /// <param name="result"></param>
        /// <param name="isTotal"></param>
        /// <param name="StatImage">1：显示统计图；0：不显示统计图</param>
        void Add(bool singleFile, string windowText, IStatResult result, bool isTotal, int StatImage);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowText"></param>
        /// <param name="result"></param>
        /// <param name="displayCol"></param>
        /// <param name="isTotal"></param>
        /// <param name="StatImage">1：显示统计图；0：不显示统计图</param>
        void Add(bool singleFile, string windowText, IStatResult result, int displayCol, bool isTotal, int StatImage);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowText"></param>
        /// <param name="result"></param>
        /// <param name="displayCol"></param>
        /// <param name="displayDataLabel"></param>
        /// <param name="isTotal"></param>
        /// <param name="StatImage">1：显示统计图；0：不显示统计图</param>
        void Add(bool singleFile, string windowText, IStatResult result, int displayCol, bool displayDataLabel, bool isTotal, int StatImage);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowText"></param>
        /// <param name="result"></param>
        /// <param name="displayDataLabel"></param>
        /// <param name="isTotal"></param>
        /// <param name="StatImage">1：显示统计图；0：不显示统计图</param>
        void Add(bool singleFile, string windowText, IStatResult result, bool displayDataLabel, bool isTotal, int StatImage);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="isTotal"></param>
        /// <param name="StatImage">1：显示统计图；0：不显示统计图</param>
        void Append(bool singleFile, IStatResult result, bool isTotal, int StatImage);
    }
}

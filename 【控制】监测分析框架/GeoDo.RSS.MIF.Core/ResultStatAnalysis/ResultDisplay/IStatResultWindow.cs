using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
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
        //1:全部合计；1：仅合计统计图列
        void Add(bool singleFile, string windowText, IStatResult result, int displayCol, bool isTotal, int totalAll, int StatImage);
       
        void Add(bool singleFile, string windowText, IStatResult result, int displayCol, bool isTotal, int totalAll, int StatImage,byte baseCol);
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

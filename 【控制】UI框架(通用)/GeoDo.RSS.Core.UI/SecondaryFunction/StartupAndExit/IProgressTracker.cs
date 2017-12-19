using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public interface IProgressTracker
    {
        bool IsBusy { get; }
        /// <summary>
        /// 开始进度跟踪
        /// </summary>
        /// <param name="estimateTotalTime">-1为估计不出时间</param>
        void StartTracking(string text,int estimateTotalTime);
        void StartTracking(string text);
        /// <summary>
        /// 结束进度跟踪
        /// </summary>
        void StopTracking();
        /// <summary>
        /// 正在跟踪进度
        /// </summary>
        /// <param name="currentTime">-1为估计不出准确的时间点</param>
        void Tracking(string text,int currentTime);
        void Tracking(string text);
    }
}

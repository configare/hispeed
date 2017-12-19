using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 进度条
    /// </summary>
    public interface IProgressTrackerSupport
    {
        /// <summary>0
        /// 初始化或重置进度条
        /// </summary>
        /// <param name="text">本次任务提示</param>
        /// <param name="totalTimes">进度总刻度</param>
        /// <param name="segments">分段进度定义,不传递则不分段</param>
        void Reset(string text, int totalTimes);
        /// <summary>
        /// 启动进度条
        /// </summary>
        /// <param name="isAuto">是否自动运行</param>
        void Start(bool isAuto);
        /// <summary>
        /// 推进进度条
        /// </summary>
        /// <param name="iTimes">推进到第i个刻度</param>
        void Boost(int iTimes);
        /// <summary>
        /// 推进进度条
        /// </summary>
        /// <param name="iTimes">推进到第i个刻度</param>
        /// <param name="segmentText">第i个刻度所在的进度段提示</param>
        void Boost(int iTimes, string segmentText);
        /// <summary>
        /// 推进进度条
        /// </summary>
        /// <param name="iTimes">推进到第i个刻度</param>
        /// <param name="text">本次任务提示</param>
        /// <param name="segmentText">第i个刻度所在的进度段提示</param>
        void Boost(int iTimes, string text, string segmentText);
        /// <summary>
        /// 结束进度条,同时关闭进度条
        /// </summary>
        void Finish();
    }
}

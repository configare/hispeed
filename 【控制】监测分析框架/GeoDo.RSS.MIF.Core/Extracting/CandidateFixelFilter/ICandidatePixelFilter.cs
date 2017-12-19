using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 判识前过滤无效像元
    /// </summary>
    public interface ICandidatePixelFilter
    {
        /// <summary>
        /// [GET,SET]名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// [GET,SET]过滤器是否有效
        /// </summary>
        bool IsEnabled { get; set; }
        /// <summary>
        /// [GET]过滤器是否已经执行过
        /// </summary>
        bool IsFiltered { get; }
        /// <summary>
        /// 复位(复位后IsFiltered为False)
        /// </summary>
        void Reset();
        /// <summary>
        /// 执行过滤
        /// </summary>
        /// <param name="dataProvider">待判识影像(栅格数据提供者)</param>
        /// <param name="aoi">前面过滤器过滤后的有效像元索引数组(可为空)</param>
        /// <returns>该过滤器过滤后的有效像元索引数组</returns>
        int[] Filter(IRasterDataProvider dataProvider, Rectangle aoiRect, int[] aoi);
        /// <summary>
        /// 执行过滤
        /// </summary>
        /// <param name="dataProvider">待判识影像(栅格数据提供者)</param>
        /// <param name="aoi">前面过滤器过滤后的有效像元索引数组(可为空)</param>
        /// <returns>该过滤器过滤后的有效像元索引数组</returns>
        /// <param name="assistInfo">辅助信息</param>
        /// <returns></returns>
        int[] Filter(IRasterDataProvider dataProvider, Rectangle aoiRect, int[] aoi, byte[] assistInfo);
    }
}

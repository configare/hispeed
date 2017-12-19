using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;

namespace GeoDo.Radiation
{
    /// <summary>
    /// 亮温转换提供者
    /// </summary>
    public interface IRadiationProvider
    {
        string Name { get; }

        /// <summary>
        /// 初始化亮温转换参数
        /// </summary>
        void InitRadiationArgs(IRasterDataProvider srcRaster, bool isSolarZenith);

        /// <summary>
        /// 执行亮温转换
        /// </summary>
        /// <param name="srcBandData">通道数据</param>
        /// <param name="srcSize">数据大小</param>
        /// <param name="bandNumber">通道号</param>
        /// <param name="isRadiation"></param>
        /// <param name="isSolarZenith"></param>
        void DoRadiation(ushort[] srcBandData, int xOffset, int yOffset, int srcWidth, int srcHeight,
            int bandNumber, bool isRadiation, bool isSolarZenith);
    }
}

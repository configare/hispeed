using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IGrid:IDisposable,IIdentifyFeatures
    {
        /// <summary>
        /// 是否转换过坐标
        /// </summary>
        bool CoordIsConverted { get; set; }
        /// <summary>
        /// 估计该块占的字节数
        /// </summary>
        int EstimateSize { get; }
        /// <summary>
        /// 网格编号
        /// </summary>
        int GridNo { get; }
        /// <summary>
        /// 网格封套
        /// </summary>
        Envelope GridEnvelope { get; }
        /// <summary>
        /// 网格内矢量要素的实际封套，并非网格的封套
        /// </summary>
        Envelope Envelope { get; }
        /// <summary>
        /// 网格内的矢量要素
        /// </summary>
        List<Feature> VectorFeatures { get; }
        /// <summary>
        /// 判断网格内要素是否为空
        /// </summary>
        /// <returns></returns>
        bool IsEmpty();
    }
}

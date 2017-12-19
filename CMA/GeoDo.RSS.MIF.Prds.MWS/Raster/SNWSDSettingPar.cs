using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.VectorDrawing;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 封装反演雪深雪水当量的参数对象
    /// </summary>
    public class SNWSDSettingPar
    {
        /// <summary>
        /// 选择区域名称
        /// </summary>
        public string SelectRegionName { get; set; }
        /// <summary>
        /// 选择区域矢量层
        /// </summary>
        public AOIContainerLayer AoiContainer { get; set; }
        /// <summary>
        /// 算法参数
        /// </summary>
        public float[] AlgorithmPars { get; set; }
    }
}

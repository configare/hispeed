using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class BandnameItem
    {
        /// <summary>
        /// 波段序号从1开始
        /// </summary>
        public int Index = 0;
        /// <summary>
        /// 原始序号
        /// </summary>
        public int OriginalIndex = 0;
        /// <summary>
        /// 波长范围
        /// </summary>
        public BandWaveLength WaveLength = null;
        /// <summary>
        /// 波段名称或描述
        /// </summary>
        public string Name = null;
        /// <summary>
        /// 波段分辨率,单位:米
        /// </summary>
        public float Resolution = 0;
        /// <summary>
        /// 中心波数
        /// </summary>
        public float CenterWaveNumber = 0;
        public string Type;
    }
}

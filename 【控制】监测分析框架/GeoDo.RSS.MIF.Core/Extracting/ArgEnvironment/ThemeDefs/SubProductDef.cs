using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    public class SubProductDef
    {
        /// <summary>
        /// 子产品名称,"判识"，“能见度计算”
        /// </summary>
        public string Name;
        /// <summary>
        /// 子产品唯一标识，“binary”
        /// </summary>
        public string Identify;
        /// <summary>
        /// 子产品算法集合
        /// </summary>
        public AlgorithmDef[] Algorithms;
        /// <summary>
        /// 子产品所属产品
        /// </summary>
        public ProductDef ProductDef;

        /// <summary>
        /// 是否使用感兴趣区域模板
        /// </summary>
        public bool IsUseAoiTemplate;

        /// <summary>
        /// 是否维持窗体不被关闭
        /// </summary>
        public bool IsKeepUserControl;

        /// <summary>
        /// 默认使用的感兴趣区域模板
        /// </summary>
        public string AoiTemplates;

        /// <summary>
        /// 子产品的颜色。
        /// 格式为"R,G,B,A"四个数字的字符串
        /// </summary>
        public Color Color;

        /// <summary>
        /// isdisplaypanel:
        /// true or false
        /// </summary>
        public bool IsDisplayPanel;
        /// <summary>
        /// 是否依赖当前打开的影像
        /// </summary>
        public bool IsNeedCurrentRaster = false;
        /// <summary>
        /// 子产品实例(主要用于专题图和统计分析)
        /// </summary>
        public SubProductInstanceDef[] SubProductInstanceDefs;

        public string AOISecondaryInfoFromArg;
        /// <summary>
        /// 是否进行快速生成（默认为是）
        /// </summary>
        public bool IsAutoGenerate = true;
        /// <summary>
        /// 参与快速生成的组标识
        /// </summary>
        public string[] AutoGenerateGroup;
        /// <summary>
        /// 面板是否需要显示保存按钮，默认需要
        /// </summary>
        public bool VisiableSaveBtn = true;


        public AlgorithmDef GetAlgorithmDefByIdentify(string identify)
        {
            if (Algorithms == null || Algorithms.Length == 0 || identify == null)
                return null;
            foreach (AlgorithmDef prd in Algorithms)
                if (prd.Identify != null && prd.Identify.ToUpper() == identify.ToUpper())
                    return prd;
            return null;
        }

        public AlgorithmDef GetAlgorithmDefByAlgorithmIdentify(ExtractAlgorithmIdentify id)
        {
            if (Algorithms == null || Algorithms.Length == 0 || id == null)
                return null;
            foreach (AlgorithmDef prd in Algorithms)
            {
                if (Array.IndexOf(prd.Satellites, id.Satellite) >= 0)
                {
                    if (id.Sensor == null)
                        return prd;
                    if (Array.IndexOf(prd.Sensors, id.Sensor) >= 0)
                    {
                        if (id.CustomIdentify == null)
                            return prd;
                        if (prd.CustomIdentify == id.CustomIdentify)
                            return prd;
                    }
                }
            }
            return null;
        }
    }
}

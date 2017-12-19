using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class SubProductInstanceDef
    {
        public string Name;
        public string FileProvider;
        public string OutFileIdentify;
        public string AOIProvider;
        /// <summary>
        /// 仅对专题图子产品实例有效
        /// </summary>
        public string LayoutName;
        /// <summary>
        /// 对统计分析有效
        /// Prd=BAG,SubPrd=DBLV,Days=10
        /// </summary>
        public string Argument;
        /// <summary>
        /// 针对不同付色方案但是相同数据生产的专题图，可为空
        /// </summary>
        public string ColorTableName;
        public bool isautogenerate = true;
        /// <summary>
        /// 文件生成时的扩展信息
        /// 例如：频次统计 旬 _POTD 月 _POAM 季 _POAQ 年 _POAY
        /// </summary>
        public string extInfo;
        /// <summary>
        /// 是否原始分辨率输出
        /// </summary>
        public bool isOriginal;
        /// <summary>
        /// 是否保持模板尺寸
        /// </summary>
        public bool isFixTempleSize;
        /// <summary>
        /// 是否根据当前影像生成Bitmap（用于多通道合成图）
        /// </summary>
        public bool isCurrentView;
        /// <summary>
        /// 自动生成组标识
        /// </summary>
        public string[] AutoGenerateGroup;
        /// <summary>
        /// 专题图导出路径
        /// </summary>
        public string OutDir;
        /// <summary>
        /// 是否支持扩展方法,默认为true，现主要用于专题图辅助信息叠加显示（如 云、烟）控制
        /// </summary>
        public bool isExtMethod;
        /// <summary>
        /// 默认投影方式，现主要用于专题图输出时默认采用何种投影，例如 陆表温度采用 “阿尔伯斯等面积投影”
        /// </summary>
        public string DefaultProj;
    }
}

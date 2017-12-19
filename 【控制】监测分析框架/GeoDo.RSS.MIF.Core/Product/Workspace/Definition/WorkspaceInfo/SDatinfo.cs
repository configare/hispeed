namespace GeoDo.RSS.MIF.Core
{
    public class SDatinfo
    {
        /// <summary>
        /// 源文件文件全名
        /// </summary>
        public string SourceFileName { get; set; }
        /// <summary>
        /// 产品标识
        /// </summary>
        public string ProductIdentify { get; set; }
        /// <summary>
        /// 产品类别
        /// </summary>
        public string SubProductIdentify { get; set; }
        /// <summary>
        /// 文件名（不包含路径）
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string FileDir { get; set; }
        /// <summary>
        /// 数据集定义
        /// </summary>
        public string CatalogDef { get; set; }
        /// <summary>
        /// 卫星
        /// </summary>
        public string Satellite { get; set; }
        /// <summary>
        /// 传感器
        /// </summary>
        public string Sensor { get; set; }
        /// <summary>
        /// 轨道时间
        /// </summary>
        public string OrbitDateTime { get; set; }
        /// <summary>
        /// 轨道时间分组
        /// </summary>
        public string OrbitTimeGroup { get; set; }
        /// <summary>
        /// 类别中文
        /// </summary>
        public string CatalogItemCN { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// 扩展信息
        /// </summary>
        public string ExtInfos { get; set; }
        public string CycFlagCN { get; set; }
        /// <summary>
        /// 轨道时间段
        /// </summary>
        public string OrbitTimes { get; set; }
    }
}
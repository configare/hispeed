using System;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    /// <summary>
    /// Hdf4 拼接为 Hdf5 的参数结构
    /// </summary>
    public class SHdf4To5
    {
        public SHdf4To5(string[] hdf4Names, string hdf5Name)
        {
            Hdf4Names = hdf4Names;
            Hdf5Name = hdf5Name;
        }

        /// <summary>
        /// 要合并的 Hdf4 文件列表
        /// </summary>
        public string[] Hdf4Names { get; private set; }
        /// <summary>
        /// 要生成的 hdf5 文件名
        /// </summary>
        public string Hdf5Name { get; private set; }
        /// <summary>
        /// 进度函数，第一个是信息，说明当前处理的数据集名；第二个是数据集所在的索引号；第三个是当前处理的Hdf4文件索引号
        /// </summary>
        public Action<string, int, int> MessageAction { get; set; }
        /// <summary>
        /// 状态函数，获取hdf4数据集个数
        /// </summary>
        public Action<int> SdsCountAction { get; set; }
    }
}
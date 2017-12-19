using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.Core.DF
{
    /// <summary>
    /// 波段提供者
    /// 对于像HDF这种类型的科学数据集，大多数数据集是非图像数据集并不需要显示
    /// 该接口仅仅返回可以显示为图像的数据集中的所有波段
    /// </summary>
    public interface IBandProvider:IDisposable
    {
        /// <summary>
        /// 卫星和传感器标识
        /// </summary>
        DataIdentify DataIdentify { get; set; }
        /// <summary>
        /// 初始化波段提供者
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="access"></param>
        /// <param name="provider"></param>
        void Init(string fname, enumDataProviderAccess access, IRasterDataProvider provider);
        /// <summary>
        /// 复位
        /// </summary>
        void Reset();
        /// <summary>
        /// 获取默认的波段列表(一般情况下是图像数据波段，或依据配置文件)
        /// </summary>
        /// <returns></returns>
        IRasterBand[] GetDefaultBands();
        /// <summary>
        /// 获取指定数据集名称的波段
        /// </summary>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        IRasterBand[] GetBands(string datasetName);
        /// <summary>
        /// 获取所有波段名称
        /// </summary>
        /// <returns></returns>
        string[] GetDatasetNames();
        /// <summary>
        /// 获取文件属性
        /// </summary>
        /// <returns></returns>
        Dictionary<string,string> GetAttributes();
        /// <summary>
        /// 获取指定数据集的属性
        /// </summary>
        /// <param name="datasetNames"></param>
        /// <returns></returns>
        Dictionary<string, string> GetDatasetAttributes(string datasetNames);
        /// <summary>
        /// 判读输入的文件是否是波段提供者支持的类型
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="header1024"></param>
        /// <returns></returns>
        bool IsSupport(string fname, byte[] header1024,Dictionary<string,string> datasetNames);
    }
}

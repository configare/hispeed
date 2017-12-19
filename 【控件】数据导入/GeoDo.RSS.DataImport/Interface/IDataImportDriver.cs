using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.DI
{
    public interface IDataImportDriver
    {
        /// <summary>
        /// 自动查找待导入数据
        /// </summary>
        /// <param name="dataProvider"></param>
        /// <param name="dir">搜索路径</param>
        /// <returns></returns>
        ImportFilesObj[] AutoFindFilesByDirver(string productIdentify, string subProIdentify, IRasterDataProvider dataProvider, string dir);

        /// <summary>
        /// 判断是否支持当前数据的转换
        /// </summary>
        /// <param name="productIdentify">产品名</param>
        /// <param name="subProductIdentify">子产品名</param>
        /// <param name="filename">待转换数据</param>
        /// <param name="error">错误信息</param>
        /// <returns>是否支持</returns>
        bool CanDo(string productIdentify, string subProductIdentify, string filename, out string error);

        /// <summary>
        /// 转换当前数据
        /// </summary>
        /// <param name="productIdentify">产品名</param>
        /// <param name="subProductIdentify">子产品名</param>
        /// <param name="dataProvider">影像数据：null 表示转换；非空 表示结果需叠加（需裁剪数据）</param>
        /// <param name="filename">待转换数据</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        IExtractResult Do(string productIdentify, string subProductIdentify, IRasterDataProvider dataProvider, string filename, out string error);

        /// <summary>
        /// 转换当前数据,直接输出文件
        /// </summary>
        /// <param name="productIdentify">产品名</param>
        /// <param name="subProductIdentify">子产品名</param>
        /// <param name="filename">待转换数据</param>
        /// <param name="dstFilename">转换结果文件名</param>
        /// <param name="error">错误信息</param>
        /// <returns>是否成功</returns>
        bool Do(string productIdentify, string subProductIdentify, string filename, string dstFilename, out string error);
    }
}

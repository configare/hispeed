using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using GeoDo.RasterProject;

namespace GeoDo.FileProject
{
    public interface IFileProjector:IDisposable
    {
        /// <summary>
        /// 文件投影器的Name
        /// </summary>
        string Name { get; }
        string FullName { get; }
        
        /// <summary>
        /// 启动一个预处理
        /// 1、如果调用了该方法，则初始化一个srcRaster，
        /// 2、下面每次调用Project方法，则拿Project方法中的srcRaster与这里记录的做比较，
        ///    如果一致，则不计算公共数据，例如，计算后的经纬度数据、最大范围等。
        ///    如果不一致，则先调用EndSession()，再继续。
        /// 一般用于投影多个分块时候使用。
        /// </summary>
        /// <param name="srcRaster"></param>
        void BeginSession(IRasterDataProvider srcRaster);
        
        /// <summary>
        /// 清除BeginSession纪录的所有信息。
        /// </summary>
        void EndSession();
        /// <summary>
        /// 投影到文件或内存
        /// </summary>
        /// <param name="srcRaster">原始栅格数据</param>
        /// <param name="prjSettings">投影参数</param>
        /// <param name="dstSpatial">目标投影</param>
        /// <param name="progressCallback">进度条</param>
        void Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatial, Action<int, string> progressCallback);
        
        //拼接投影到目标，如果目标为空则创建新的
        IRasterDataProvider Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, IRasterDataProvider dstRaster, int beginBandIndex, Action<int, string> progressCallback);
        
        //计算目标投影下范围
        void ComputeDstEnvelope(IRasterDataProvider srcRaster, ISpatialReference dstSpatialRef,out PrjEnvelope maxPrjEnvelope, Action<int, string> progressCallback);

        FilePrjSettings CreateDefaultPrjSettings();

        bool HasVaildEnvelope(IRasterDataProvider locationRaster, PrjEnvelope validEnv, ISpatialReference envSpatialReference);

        bool ValidEnvelope(IRasterDataProvider locationRaster, PrjEnvelope validEnv, SpatialReference envSpatialReference, out double validRate, out PrjEnvelope outEnv);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Project;
using GeoDo.RasterProject;

namespace GeoDo.FileProject
{
    public class PrjOutArg
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectionRef"></param>
        /// <param name="envelopes"></param>
        /// <param name="resolutonX"></param>
        /// <param name="resolutionY"></param>
        /// <param name="prjBands"></param>
        /// <param name="outDirFile"></param>
        /// <param name="args">eg:NotRadiation,NotSolarZenith</param>
        public PrjOutArg(ISpatialReference projectionRef, PrjEnvelopeItem[] envelopes, float resolutonX, float resolutionY,string outDirFile)
        {
            ProjectionRef = projectionRef;
            Envelopes = envelopes;
            ResolutionX = resolutonX;
            ResolutionY = resolutionY;
            OutDirOrFile = outDirFile;
        }

        public PrjOutArg(string projectionIdentify, PrjEnvelopeItem[] envelopes, float resolutonX, float resolutionY,string outDirFile)
        {
            ISpatialReference projectionRef = GetSpatialReference(projectionIdentify);
            ProjectionRef = projectionRef;
            Envelopes = envelopes;
            ResolutionX = resolutonX;
            ResolutionY = resolutionY;
            OutDirOrFile = outDirFile;
        }
        /// <summary>
        /// 输出文件投影定义
        /// </summary>
        public ISpatialReference ProjectionRef;
        /// <summary>
        /// 定义输出范围
        /// null代表整文件输出
        /// </summary>
        public PrjEnvelopeItem[] Envelopes;
        /// <summary>
        /// 输出的分辨率
        /// 小于0代表原分辨率
        /// </summary>
        public float ResolutionX;
        /// <summary>
        /// 输出的分辨率
        /// 小于0代表原分辨率
        /// </summary>
        public float ResolutionY;
        /// <summary>
        /// 指定输出目录
        /// </summary>
        public string OutDirOrFile;
        /// <summary>
        /// 是否输出角度数据集数据
        /// true：输出
        /// false：不输出
        /// </summary>
        public bool OutAngle = false;
        /// <summary>
        /// 扩展参数
        /// </summary>
        public object[] Args;
        /// <summary>
        /// 有同名文件时候是否覆盖
        /// [暂未使用]
        /// </summary>
        public bool IsOverwrite = false;

        /// <summary>
        /// 要投影的通道
        /// [从1开始的波段编号]
        /// </summary>
        public int[] SelectedBands;
        
        private ISpatialReference GetSpatialReference(string projectionIdentify)
        {
            switch (projectionIdentify)
            {
                case "ABS":
                    return SpatialReferenceFactory.GetSpatialReferenceByPrjFile("\\");
                    //return new SpatialReference(new GeographicCoordSystem(), new ProjectionCoordSystem());
                case "GLL":
                default:
                    return SpatialReference.GetDefault();
            }
        }
    }
}

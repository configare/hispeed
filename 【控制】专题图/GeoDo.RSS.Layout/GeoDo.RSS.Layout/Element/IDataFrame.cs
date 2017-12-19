using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;

namespace GeoDo.RSS.Layout
{
    public interface IDataFrame:ISizableElement
    {
        /*
         * 以下四个参数用于背景层
         * 注：下次重构时修改
         */
        Color LandColor { get; set; }
        Color SeaColor { get; set; }
        bool IsUseDefaultBackgroudLayer { get; set; }
        string[] InterestRegions { get;set; }
        //
        string SpatialRef { get; }
        object Provider { get; }
        /// <summary>
        /// 比例尺(分母)
        /// </summary>
        float LayoutScale { get; set; }
        Color BorderColor { get; set; }
        Color BackColor { get; set; }
        int BorderWidth { get; set; }
        bool IsShowBorderLine { get; set; }
        void Update(ILayoutHost host);
        object Data { get; set; }
        /// <summary>
        /// 有DataFrame的类中静态初始化函数调用，表示VectorHost以正确初始化
        /// </summary>
        void NotifyInitFinished();
        XElement GetGridXml();
        XElement GeoGridXml { get; }
        XElement GetDocumentableLayersHostXml();
        /// <summary>
        /// 从IDataFrameDataProvider中同步属性
        /// </summary>
        void SyncAttrbutes();
        void UpdateCanves();
    }
}

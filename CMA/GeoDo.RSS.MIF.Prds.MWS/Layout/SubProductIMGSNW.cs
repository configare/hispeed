#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-2-11 15:25:34
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.Layout;
using GeoDo.RSS.UI.AddIn.DataPro;
using GeoDo.FileProject;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using GeoDo.RasterProject;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：SubProductIMGSNW
    /// 属性描述：
    /// 创建者：lxj   创建日期：2014-2-11 15:25:34
    /// 修改者：lxj             修改日期：2014-3-14
    /// 修改描述：扩大数据框的范围，数据可以不紧贴着数据框显示。
    /// 备注：
    /// </summary>
   
    public class SubProductIMGSNW : CmaMonitoringSubProduct
    {  
        public SubProductIMGSNW(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "0IMGAlgorithm")
            {
                return IMGAlgorithm();
            }
            return null;
        }

        private IExtractResult IMGAlgorithm()
        {
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (string.IsNullOrWhiteSpace(instanceIdentify))
                return null;
            SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
            if (instance == null)
                return ThemeGraphyResult(null);
            if (instanceIdentify == "MCSI" || instanceIdentify == "OMCS")
            {
                return ThemeGraphyResult(null);
            }
            if (instanceIdentify == "NMCS")
            {
                return ThemeGraphyMCSIDBLV(instance);
            }
            return ThemeGraphyByInstance(instance);
        }

        GxdEnvelope _envelope = null;

        protected override void ApplyAttributesOfLayoutTemplate(ILayoutTemplate template)
        {
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            string[] files = GetStringArray("SelectedPrimaryFiles");
            ILayout layout = template.Layout;
            double minx ;
            double miny;
            double maxx;
            double maxy;
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(files[0],"") as IRasterDataProvider)
            {
                minx = dataPrd.CoordEnvelope.MinX - 1;   //扩大数据框范围
                maxx = dataPrd.CoordEnvelope.MaxX + 1;
                miny = dataPrd.CoordEnvelope.MinY - 1;
                maxy = dataPrd.CoordEnvelope.MaxY + 1;
            }
                _envelope = new Layout.GxdEnvelope(minx, maxx, miny,maxy);
        }
        protected override void ApplyAttributesOfDataFrame(IGxdDataFrame gxdDataFrame, IDataFrame dataFrame, ILayout layout)
        {
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;

                Layout.GxdEnvelope evp = ToPrjEnvelope(_envelope, gxdDataFrame, dataFrame);
                if (evp != null)
                {
                    FitSizeToTemplateWidth(layout, (float)(evp.MaxX - evp.MinX ), (float)(evp.MaxY - evp.MinY ));
                    gxdDataFrame.Envelope = evp;
                    _envelope = null;
                }
        }
        private GxdEnvelope ToPrjEnvelope(GxdEnvelope env, IGxdDataFrame gxdDataFrame, IDataFrame dataFrame)
        {
            if (env == null)
                return null;
            GeoDo.Project.IProjectionTransform tran = GetProjectionTransform(gxdDataFrame.SpatialRef);
            if (tran == null)
                return null;
            double[] xs = new double[] { env.MinX, env.MaxX };
            double[] ys = new double[] { env.MaxY, env.MinY };
            try
            {
                tran.Transform(xs, ys);
                return new Layout.GxdEnvelope(xs[0], xs[1], ys[1], ys[0]);
            }
            catch
            {
                return null;
            }
        }

        private void ChangeTemplateSize(ILayout layout, int width, int height)
        {
            IElement[] dfs = layout.QueryElements((e) => { return e is IDataFrame; });
            if (dfs == null || dfs.Length == 0)
                return;
            IDataFrame df = (dfs[0] as IDataFrame);
            if (df.Size.Width == width && df.Size.Height == height)
                return;
            float yOffset = height - df.Size.Height;
            float xOffset = width - df.Size.Width;
            df.IsLocked = false;
            df.ApplySize(xOffset, yOffset);
            df.IsLocked = true;
            layout.Size = new System.Drawing.SizeF(width, height);
            List<IElement> eles = layout.Elements;
            for (int i = 0; i < eles.Count; i++)
            {
                if (eles[i].Name == "标题" ||
                    eles[i].Name.Contains("Time") ||
                    eles[i].Name.Contains("Date"))
                    continue;
                if (eles[i] is IBorder ||
                    eles[i] is IDataFrame)
                    continue;
                if (eles[i] is ISizableElement)
                {
                    (eles[i] as ISizableElement).ApplyLocation(xOffset, yOffset);
                }
            }
        }

        private Project.IProjectionTransform GetProjectionTransform(string spatialRef)
        {
            if (spatialRef == null || !spatialRef.Contains("PROJCS"))
                return Project.ProjectionTransformFactory.GetDefault();
            else
                return Project.ProjectionTransformFactory.GetProjectionTransform(GeoDo.Project.SpatialReference.GetDefault(), GeoDo.Project.SpatialReferenceFactory.GetSpatialReferenceByWKT(spatialRef, Project.enumWKTSource.EsriPrjFile));
        }
    }
}

#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-2-11 17:59:50
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
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Layout;
using GeoDo.RSS.UI.AddIn.DataPro;
using GeoDo.FileProject;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using GeoDo.RasterProject;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：SubProductCustomIMG
    /// 属性描述：ldf格式积雪参数专题图，单次计算的雪深
    /// 创建者：lxj  创建日期：2014-2-11 17:59:50
    /// 修改者：             修改日期：
    /// 修改描述：增加根据查询检索计算好的历史统计数据
    /// 备注：
    /// </summary>
    public class SubProductCustomIMG : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private Action<int, string> _progressTracker = null;

        public SubProductCustomIMG(SubProductDef subProductDef)
            :base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker,null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _progressTracker = progressTracker;
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "SDSWEAlgorithm")
            {
                return SDSWEAlgorithm(progressTracker);
            }
            return null;
        }
        private IExtractResult SDSWEAlgorithm(Action<int, string> progressTracker)
        {
            string arguments = _argumentProvider.GetArg("RasterFile").ToString();
            if (arguments.Contains("MFSD"))
            {
                _argumentProvider.SetArg("OutFileIdentify", "MSDI");
            }
            if (arguments.Contains("MFVI"))
            {
                _argumentProvider.SetArg("OutFileIdentify", "MSVI");
            }
            if (arguments.Contains("MFWE"))
            {
                _argumentProvider.SetArg("OutFileIdentify", "MSWI");
            }
            //历史雪水当量数据
            if (arguments.Contains("HFWE"))
            {
                _argumentProvider.SetArg("OutFileIdentify", "HSWI");
            }
            if (arguments.Contains("JPEA"))
            {
                _argumentProvider.SetArg("OutFileIdentify", "JPEI");
            }
            //历史雪深数据
            if (arguments.Contains("HFSD"))
            {
                _argumentProvider.SetArg("OutFileIdentify", "HSDI");
            }
            if (arguments.Contains("JPDA"))
            {
                _argumentProvider.SetArg("OutFileIdentify", "JPDI");
            }
            _argumentProvider.SetArg("SelectedPrimaryFiles", arguments);
            _argumentProvider.SetArg("fileOpenArgs", arguments);
            return ThemeGraphyResult(null);
        }
        GxdEnvelope _envelope = null;
        protected override void ApplyAttributesOfLayoutTemplate(ILayoutTemplate template)
        {
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            string[] files = GetStringArray("SelectedPrimaryFiles");
            ILayout layout = template.Layout;
            double minx;
            double miny;
            double maxx;
            double maxy;
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(files[0], "") as IRasterDataProvider)
            {
                minx = dataPrd.CoordEnvelope.MinX - 1;   //扩大数据框范围
                maxx = dataPrd.CoordEnvelope.MaxX + 1;
                miny = dataPrd.CoordEnvelope.MinY - 1;
                maxy = dataPrd.CoordEnvelope.MaxY + 1;
            }
            _envelope = new Layout.GxdEnvelope(minx, maxx, miny, maxy);
            Dictionary<string, string> vars = new Dictionary<string, string>();
            vars.Add("{NAME}", "请输入标题");
            template.ApplyVars(vars);
        }

        protected override void ApplyAttributesOfDataFrame(IGxdDataFrame gxdDataFrame, IDataFrame dataFrame, ILayout layout)
        {
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;

            Layout.GxdEnvelope evp = ToPrjEnvelope(_envelope, gxdDataFrame, dataFrame);
            if (evp != null)
            {
                FitSizeToTemplateWidth(layout, (float)(evp.MaxX - evp.MinX), (float)(evp.MaxY - evp.MinY));
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

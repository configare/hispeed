using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.Layout;
using GeoDo.RSS.UI.AddIn.DataPro;
using GeoDo.FileProject;

namespace GeoDo.RSS.MIF.Prds.SNW
{
    /// <summary>
    /// 积雪二值图
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
            if (instanceIdentify == "MCSI"||instanceIdentify=="OMCS")
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
            if (instanceIdentify == "NCSI"||instanceIdentify=="NMCS")
            {
                string[] files = GetStringArray("SelectedPrimaryFiles");
                RasterIdentify rstId = new RasterIdentify(files[0]);
                if (!string.IsNullOrWhiteSpace(rstId.RegionIdentify))
                {
                    DefinedRegionParse reg = new DefinedRegionParse();
                    BlockItemGroup blockGroup = reg.BlockDefined.FindGroup("积雪");
                    PrjEnvelopeItem envItem = blockGroup.GetPrjEnvelopeItem(rstId.RegionIdentify);
                    if (envItem != null)
                    {
                        RasterProject.PrjEnvelope prjEnvelope = RasterProject.PrjEnvelope.CreateByCenter(envItem.PrjEnvelope.CenterX, envItem.PrjEnvelope.CenterY, 10, 10);
                        ILayout layout = template.Layout;
                        if(layout.Size.Width!=1000||layout.Size.Height!=1000)
                        {
                            ChangeTemplateSize(layout,1000,1000);
                        }
                        _envelope=new Layout.GxdEnvelope(prjEnvelope.MinX, prjEnvelope.MaxX, prjEnvelope.MinY, prjEnvelope.MaxY);   
                    }
                }
            }
        }

        protected override void ApplyAttributesOfDataFrame(IGxdDataFrame gxdDataFrame, IDataFrame dataFrame,ILayout layout)
        {
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (instanceIdentify == "NCSI"||instanceIdentify=="NMCS")
            {
                Layout.GxdEnvelope evp = ToPrjEnvelope(_envelope, gxdDataFrame, dataFrame);
                if (evp != null)
                {
                    FitSizeToTemplateWidth(layout, (float)(evp.MaxX - evp.MinX), (float)(evp.MaxY - evp.MinY));
                    gxdDataFrame.Envelope = evp;
                    _envelope = null;
                }
            }
            base.ApplyAttributesOfDataFrame(gxdDataFrame, dataFrame, layout);
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

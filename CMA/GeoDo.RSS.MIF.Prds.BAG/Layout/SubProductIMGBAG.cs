using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public class SubProductIMGBAG : CmaMonitoringSubProduct
    {
        private bool _isMCSI = false;

        public SubProductIMGBAG(SubProductDef subProductDef)
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
            if (instance.Name.Contains("多通道合成图"))
            {
                _isMCSI = true;
                return ThemeGraphyResult(null);
            }
            return ThemeGraphyByInstance(instance);
        }

        protected override void ApplyAttributesOfDataFrame(IGxdDataFrame gxdDataFrame, IDataFrame dataFrame, ILayout layout)
        {
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            string envelope = null;
            if (instanceIdentify == "TFRI")
                envelope = _argumentProvider.GetArg("OldThemeEnvelope").ToString();
            else
                envelope = _argumentProvider.GetArg("ThemeEnvelope").ToString();
            if (string.IsNullOrEmpty(envelope))
                return;
            string[] latlon = envelope.Split(',');
            if (latlon == null || latlon.Length < 4)
                return;
            ISmartSession session = _argumentProvider.GetArg("SmartSession") as ISmartSession;
            if (session == null)
                return;
            double[] geoRange;
            if (!IsSetRange(session, latlon, out geoRange))
                return;
            GxdEnvelope env = new GxdEnvelope(double.Parse(latlon[0]), double.Parse(latlon[1]), double.Parse(latlon[2]), double.Parse(latlon[3]));
            Layout.GxdEnvelope evp = ToPrjEnvelope(env, gxdDataFrame, dataFrame);
            if (evp != null)
            {
                FitSizeToTemplateWidth(layout, (float)(evp.MaxX - evp.MinX), (float)(evp.MaxY - evp.MinY));
                gxdDataFrame.Envelope = evp;
            }
            base.ApplyAttributesOfDataFrame(gxdDataFrame, dataFrame, layout);
        }

        private Layout.GxdEnvelope ToPrjEnvelope(Layout.GxdEnvelope env, Layout.IGxdDataFrame gxdDataFrame, Layout.IDataFrame dataFrame)
        {
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

        private Project.IProjectionTransform GetProjectionTransform(string spatialRef)
        {
            if (spatialRef == null || !spatialRef.Contains("PROJCS"))
                return Project.ProjectionTransformFactory.GetDefault();
            else
                return Project.ProjectionTransformFactory.GetProjectionTransform(GeoDo.Project.SpatialReference.GetDefault(), GeoDo.Project.SpatialReferenceFactory.GetSpatialReferenceByWKT(spatialRef, Project.enumWKTSource.EsriPrjFile));
        }

        public bool IsSetRange(ISmartSession session, string[] latlon, out double[] geoRange)
        {
            geoRange = new double[4];
            if (latlon == null || latlon.Length != 4)
                return false;

            double num;
            for (int i = 0; i < latlon.Length; i++)
            {
                if (double.TryParse(latlon[i], out num))
                    geoRange[i] = num;
                else
                    return false;
            }
            if (!_isMCSI)
            {
                string[] fnames = _argumentProvider.GetArg("SelectedPrimaryFiles") as string[];
                if (fnames == null || fnames.Length == 0)
                    return false;
                IRasterDataProvider dataPrd = null;
                try
                {
                    dataPrd = GeoDataDriver.Open(fnames[0]) as IRasterDataProvider;
                    if (dataPrd.CoordEnvelope.MinX <= geoRange[0] && dataPrd.CoordEnvelope.MaxX >= geoRange[1] &&
                        dataPrd.CoordEnvelope.MinY <= geoRange[2] && dataPrd.CoordEnvelope.MaxY >= geoRange[3])
                        return true;
                    return false;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    if (dataPrd != null)
                        dataPrd.Dispose();
                }
            }
            else
            {
                if (session == null)
                    return false;
                ICanvasViewer viewer = session.SmartWindowManager.ActiveCanvasViewer;
                if (viewer == null)
                    return false;
                ICanvas canvas = viewer.Canvas;
                if (canvas == null)
                    return false;
                IRasterDrawing drawing = canvas.PrimaryDrawObject as IRasterDrawing;
                if (drawing == null)
                    return false;
                if (drawing.DataProvider == null)
                    return false;
                IRasterDataProvider prd = drawing.DataProviderCopy;
                if (prd.CoordEnvelope.MinX <= geoRange[0] && prd.CoordEnvelope.MaxX >= geoRange[1] &&
                       prd.CoordEnvelope.MinY <= geoRange[2] && prd.CoordEnvelope.MaxY >= geoRange[3])
                    return true;
                return false;
            }
        }
    }
}

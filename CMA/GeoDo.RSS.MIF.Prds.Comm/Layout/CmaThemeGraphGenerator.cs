using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public class CmaThemeGraphGenerator : ThemeGraphGenerator
    {
        private Layout.GxdEnvelope _env = null;

        public CmaThemeGraphGenerator(ISmartSession session)
            : base(session)
        {
            _env = null;
        }

        public ISmartSession SmartSession
        {
            get
            {
                return _session;
            }
        }

        public void SetEnvelope(Layout.GxdEnvelope env)
        {
            _env = env;
        }

        protected override void ApplyAttributesOfDataFrame(Layout.IGxdDataFrame gxdDataFrame, Layout.IDataFrame dataFrame, ILayout layout)
        {
            Layout.GxdEnvelope evp = ToPrjEnvelope(_env, gxdDataFrame.SpatialRef);
            if (evp != null)
            {
                FitTemplateWidth(layout, (float)(evp.MaxX - evp.MinX), (float)(evp.MaxY - evp.MinY));
                gxdDataFrame.Envelope = evp;
            }
            base.ApplyAttributesOfDataFrame(gxdDataFrame, dataFrame, layout);
        }

        private Layout.GxdEnvelope ToPrjEnvelope(Layout.GxdEnvelope env, string spatialRef)
        {
            if (_env == null)
                return null;
            GeoDo.Project.IProjectionTransform tran = GetProjectionTransform(spatialRef);
            if (tran == null)
                return null;
            double[] xs = new double[] { env.MinX,env.MaxX};
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
                return Project.ProjectionTransformFactory.GetProjectionTransform(GeoDo.Project.SpatialReference.GetDefault(), GeoDo.Project.SpatialReferenceFactory.GetSpatialReferenceByWKT(spatialRef,Project.enumWKTSource.EsriPrjFile));
        }

        protected override void ApplyAttributesOfElement(string name, Layout.IElement ele)
        {
            base.ApplyAttributesOfElement(name, ele);
        }

    }
}

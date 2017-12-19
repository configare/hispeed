using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.MIF.Core;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    internal class CanvasContextMenuHandler : ICanvasViewerContextMenuHandler
    {
        private ISmartSession _session;
        private IAOIContainerLayer _aoiContainer;
        private ICanvasViewerMenuHandlerManager _handlerManager;

        public CanvasContextMenuHandler(ISmartSession session, IAOIContainerLayer aoiContainer, ICanvasViewerMenuHandlerManager handlerManager)
        {
            _session = session;
            _aoiContainer = aoiContainer;
            _handlerManager = handlerManager;
        }

        public string GetArgProviderUI(enumCanvasViewerMenu menuItem)
        {
            foreach (object[] obj in _handlerManager.Handlers)
            {
                if (obj[1].ToString() == enumCanvasViewerMenu.MagicWand.ToString())
                {
                    return obj[3] != null ? obj[3].ToString() : null;
                }
            }
            return null;
        }

        public bool HandleImportAOI()
        {
            using (frmStatSubRegionTemplates frm = new frmStatSubRegionTemplates())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    Feature[] fets = frm.GetSelectedFeatures();
                    if (fets == null || fets.Length == 0)
                        return false;
                    foreach (Feature fet in fets)
                        _aoiContainer.AddAOI(fet);
                    return true;
                }
            }
            return false;
        }

        public bool HandleSelectAOIFromFeatures(GeometryOfDrawed result)
        {
            if (result == null)
                return false;
            double bRow = double.MaxValue;
            double bCol = double.MaxValue;
            double eRow = double.MinValue;
            double eCol = double.MinValue;
            for (int i = 0; i < result.RasterPoints.Length; i++)
            {
                if (result.RasterPoints[i].X < bCol)
                    bCol = result.RasterPoints[i].X;
                if (result.RasterPoints[i].X > eCol)
                    eCol = result.RasterPoints[i].X;
                if (result.RasterPoints[i].Y < bRow)
                    bRow = result.RasterPoints[i].Y;
                if (result.RasterPoints[i].Y > eRow)
                    eRow = result.RasterPoints[i].Y;
            }
            //
            ICanvas canvas = _session.SmartWindowManager.ActiveCanvasViewer.Canvas;
            double prjX1, prjY1, prjX2, prjY2;
            canvas.CoordTransform.Raster2Prj((int)eRow, (int)bCol, out prjX1, out prjY1);//left-upper
            canvas.CoordTransform.Raster2Prj((int)bRow, (int)eCol, out prjX2, out prjY2);//right-bottom
            CoordEnvelope evp = new CoordEnvelope(prjX1, prjX2, prjY1, prjY2);
            Feature[] fets = SelectFeatures(evp, canvas);
            if (fets != null && fets.Length > 0)
            {
                foreach (Feature fet in fets)
                {
                    Feature feature = fet.Clone() as Feature;
                    feature.Geometry = fet.Geometry.Clone() as Shape;
                    feature.Projected = true;
                    _aoiContainer.AddAOI(feature);
                }
                return true;
            }
            return false;
        }

        private Feature[] SelectFeatures(CoordEnvelope evp, ICanvas canvas)
        {
            IVectorHostLayer hostLayer = canvas.LayerContainer.VectorHost as IVectorHostLayer;
            if (hostLayer == null || hostLayer.Map == null)
                return null;
            IMap map = hostLayer.Map as IMap;
            IMapRuntime runtime = hostLayer.MapRuntime as IMapRuntime;
            Envelope aoiEnvelope = new Envelope(evp.MinX, evp.MinY, evp.MaxX, evp.MaxY);
            Feature[] features = runtime.HitTestByPrj(aoiEnvelope);
            return features;
        }


        public void HandleErase(GeometryOfDrawed result)
        {
            foreach (object[] obj in _handlerManager.Handlers)
            {
                if (obj[1].ToString() == enumCanvasViewerMenu.Erase.ToString())
                {
                    (obj[2] as Action<object, Dictionary<string, object>>)(result, null);
                    break;
                }
            }
        }

        public void HandleAdsorb(GeometryOfDrawed result)
        {
            foreach (object[] obj in _handlerManager.Handlers)
            {
                if (obj[1].ToString() == enumCanvasViewerMenu.Adsorb.ToString())
                {
                    (obj[2] as Action<object, Dictionary<string, object>>)(result, null);
                    break;
                }
            }
        }

        public void HandleMagicWandExtracting(GeometryOfDrawed result, Dictionary<string, object> args)
        {
            foreach (object[] obj in _handlerManager.Handlers)
            {
                if (obj[1].ToString() == enumCanvasViewerMenu.MagicWand.ToString())
                {
                    (obj[2] as Action<object, Dictionary<string, object>>)(result, args);
                    break;
                }
            }
        }

        public void HandleClearAOI()
        {
            _session.SmartWindowManager.ActiveCanvasViewer.AOIProvider.Reset();
        }

        public void HandleFlash()
        {
            foreach (object[] obj in _handlerManager.Handlers)
            {
                if (obj[1].ToString() == enumCanvasViewerMenu.Flash.ToString())
                {
                    (obj[2] as Action<object, Dictionary<string, object>>)(null, null);
                    break;
                }
            }
        }

        public void HandleUnDo()
        {
            foreach (object[] obj in _handlerManager.Handlers)
            {
                if (obj[1].ToString() == enumCanvasViewerMenu.UnDo.ToString())
                {
                    (obj[2] as Action<object, Dictionary<string, object>>)(null, null);
                    break;
                }
            }
        }

        public void HandleReDo()
        {
            foreach (object[] obj in _handlerManager.Handlers)
            {
                if (obj[1].ToString() == enumCanvasViewerMenu.ReDo.ToString())
                {
                    (obj[2] as Action<object, Dictionary<string, object>>)(null, null);
                    break;
                }
            }
        }

        public void HandleRemoveAll()
        {
            foreach (object[] obj in _handlerManager.Handlers)
            {
                if (obj[1].ToString() == enumCanvasViewerMenu.RemoveAll.ToString())
                {
                    (obj[2] as Action<object, Dictionary<string, object>>)(null, null);
                    break;
                }
            }
        }
    }
}

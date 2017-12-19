using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;
using System.Drawing;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.Layout.DataFrm
{
    [Export(typeof(IElement)), Category("图例")]
    public class FeatureDrawingElement : DrawingElement
    {
        public FeatureDrawingElement()
        {
            _name = "矢量图例";
            _icon = Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("GeoDo.RSS.Layout.DataFrm.Elements.Vector.png"));
        }

        protected override CodeCell.AgileMap.Core.ISymbol GetSymbol()
        {
            if (string.IsNullOrEmpty(_layerName) || _dataFrame == null || _dataFrame.Provider == null)
                return null;
            IDataFrameDataProvider provider = _dataFrame.Provider as IDataFrameDataProvider;
            if (provider == null || provider.Canvas == null)
                return null;
            IVectorHostLayer hostLayer = provider.Canvas.LayerContainer.VectorHost as IVectorHostLayer;
            if (hostLayer == null)
                return null;
            IMap map = hostLayer.Map as IMap;
            if (map == null)
                return null;
            CodeCell.AgileMap.Core.ILayer layer = map.LayerContainer.GetLayerByName(_layerName);
            if (layer == null)
                return null;
            CodeCell.AgileMap.Core.IFeatureLayer fetLayer = layer as CodeCell.AgileMap.Core.IFeatureLayer;
            if (fetLayer == null || fetLayer.Renderer == null)
                return null;
            return fetLayer.Renderer.CurrentSymbol;
        }

        protected override System.Drawing.Drawing2D.GraphicsPath GetGraphicsPathFromFeatureType(float x, float y, float width, float height)
        {
            return null;
        }
    }
}

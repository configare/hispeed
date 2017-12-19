using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    public class LabelLayer:ILabelLayer,IDisposable
    {
        private GeoDo.RSS.Core.VectorDrawing.ISimpleVectorObjectHost _vecthorHost;
        private ICanvas _canvas;

        public LabelLayer(ICanvas canvas, string name, string[] fieldNames)
        {
            _canvas = canvas;
            _vecthorHost = new SimpleVectorObjectHost(name, canvas, fieldNames);
        }

        public CodeCell.AgileMap.Core.Feature[] GetAllFeature()
        {
            return _vecthorHost.GetAllFeatures();
        }

        public void AddFeature(CodeCell.AgileMap.Core.Feature feature)
        {
            _vecthorHost.Add(new SimpleVectorObject(feature.FieldValues, feature.Geometry));
        }

        public void RemoveFeature(CodeCell.AgileMap.Core.Feature feature)
        {
            _vecthorHost.RemoveFeature(feature.OID);
        }

        public void RemoveAll()
        {
            _vecthorHost.Remove((obj) => { return true; });
        }

        public CodeCell.AgileMap.Core.LabelDef LabelDef
        {
            get
            {
                return _vecthorHost.LabelSetting;
            }
        }

        public void Refresh()
        {
            _canvas.Refresh(enumRefreshType.All);
        }

        public void SaveToFile(string shpFileName)
        {
            throw new NotImplementedException();
        }

        public CodeCell.AgileMap.Core.ISymbol Symbol
        {
            get { return _vecthorHost.Symbol; }
            set { _vecthorHost.Symbol = value; }
        }

        public void Dispose()
        {
            _canvas = null;
            if (_vecthorHost != null)
            {
                RemoveAll();
                _vecthorHost.Dispose();
                //_vecthorHost = null;
            }
        }
    }
}

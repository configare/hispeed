using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    public class LabelService:ILabelService,IDisposable
    {
        private ICanvasViewer _canvasViewer;
        private Dictionary<string, ILabelLayer> _layers = new Dictionary<string, ILabelLayer>();

        public LabelService(ICanvasViewer canvasViewer)
        {
            _canvasViewer = canvasViewer;
        }

        public void Reset()
        {
            if (_layers != null && _layers.Count > 0)
            {
                foreach (ILabelLayer lyr in _layers.Values)
                    (lyr as IDisposable).Dispose();
                _layers.Clear();
            }
        }

        public ILabelLayer GetLabelLayer(string name,string[] fieldNames)
        {
            if (_layers.ContainsKey(name))
                return _layers[name];
            ILabelLayer lyr = CreateLabelLayer(name, fieldNames);
            if (lyr != null)
            {
                _layers.Add(name, lyr);
                return _layers[name];
            }
            return null;
        }

        public ILabelLayer FindLabelLayer(string name)
        {
            if (_layers.ContainsKey(name))
                return _layers[name];
            return null;
        }

        private ILabelLayer CreateLabelLayer(string name, string[] fieldNames)
        {
            return new LabelLayer(_canvasViewer.Canvas, name, fieldNames);
        }

        public void Dispose()
        {
            if (_layers != null)
            {
                Reset();
                //_layers = null;
            }
            _canvasViewer = null;
        }
    }
}

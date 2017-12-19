using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.Layout.DataFrm
{
    internal class LayerObjectContainer:ILayerObjectContainer
    {
        private IDataFrame _dataFrame;

        public LayerObjectContainer(IDataFrame dataFrame)
        {
            _dataFrame = dataFrame;
        }

        public List<ILayerObjectBase> LayerObjects
        {
            get 
            {
                return GetLayerObjects();
            }
        }

        private List<ILayerObjectBase> GetLayerObjects()
        {
            if (_dataFrame == null || _dataFrame.Provider == null)
                return null;
            IDataFrameDataProvider prd = _dataFrame.Provider as IDataFrameDataProvider;
            if (prd == null)
                return null;
            ILayerObjectBase[] layers = GetLayerFromCanvas(prd.Canvas);
            if (layers != null && layers.Length > 0)
                return new List<ILayerObjectBase>(layers);
            return null;
        }

        private ILayerObjectBase[] GetLayerFromCanvas(ICanvas canvas)
        {
            if (canvas == null || canvas.LayerContainer.IsEmpty())
                return null;
            List<ILayerObjectBase> layerObjects = new List<ILayerObjectBase>();
            foreach (GeoDo.RSS.Core.DrawEngine.ILayer lyr in canvas.LayerContainer.Layers)
            {
                ILayerObjectBase obj = GetLayerObjects(lyr);
                if (obj != null)
                    layerObjects.Add(obj);
            }
            return layerObjects.Count > 0 ? layerObjects.ToArray() : null;
        }

        private ILayerObjectBase GetLayerObjects(GeoDo.RSS.Core.DrawEngine.ILayer lyr)
        {
            if (lyr is IVectorHostLayer)
                return GetLayerObject(lyr as IVectorHostLayer);
            else if (lyr is GeoDo.RSS.Core.DrawEngine.ILayerGroup)
                return GetLayerObject(lyr as GeoDo.RSS.Core.DrawEngine.ILayerGroup);
            else
                return GetLayerObject(lyr as GeoDo.RSS.Core.DrawEngine.ILayer);
        }

        private ILayerObjectBase GetLayerObject(GeoDo.RSS.Core.DrawEngine.ILayer layer)
        {
            ILayerObject obj = new LayerObject(layer.Alias ?? layer.Name, layer);
            return obj;
        }

        private ILayerObjectBase GetLayerObject(GeoDo.RSS.Core.DrawEngine.ILayerGroup layerGroup)
        {
            ILayerObjecGroup g = new LayerObjectGroup(layerGroup.Alias ?? layerGroup.Name);
            if (layerGroup.Layers == null || layerGroup.Layers.Count == 0)
                return g;
            GetLayerObjects(layerGroup, g);
            return null;
        }

        private void GetLayerObjects(GeoDo.RSS.Core.DrawEngine.ILayerGroup layerGroup, ILayerObjecGroup g)
        {
            foreach (GeoDo.RSS.Core.DrawEngine.ILayer lyr in layerGroup.Layers)
            {
                ILayerObjectBase obj = GetLayerObjects(lyr);
                if (obj != null)
                    g.Children.Add(obj);
                if (lyr is ILayerGroup)
                    GetLayerObjects(lyr as ILayerGroup, obj as ILayerObjecGroup);
            }
        }

        private ILayerObjectBase GetLayerObject(IVectorHostLayer vectorHostLayer)
        {
            if (vectorHostLayer == null || vectorHostLayer.Map == null)
                return null;
            IMap map = vectorHostLayer.Map as IMap;
            if (map == null)
                return null;
            ILayerObjecGroup g = new LayerObjectGroup("矢量层");
            GetLayerObjects(map.LayerContainer.Layers, g);
            return g;
        }

        private void GetLayerObjects(CodeCell.AgileMap.Core.ILayer[] layers, ILayerObjecGroup g)
        {
            if (layers == null || layers.Length == 0)
                return;
            foreach (CodeCell.AgileMap.Core.ILayer lyr in layers)
            {
                ILayerObject obj = new LayerObject(lyr.Name ?? string.Empty, lyr);
                g.Children.Add(obj);
            }
        }
    }
}

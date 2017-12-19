using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public class SimpleVectorObject : ISimpleVectorObject
    {
        private ISimpleVectorObjectHost _host;
        private List<string> _attributeValues = new List<string>();
        private object _geometry;
        private bool _visible = true;
        private bool _selected = true;
        private bool _waitingProcess = true;
        private int _oid = 0;

        public SimpleVectorObject()
        {
        }

        public SimpleVectorObject(string name, CoordEnvelope geometry)
        {
            if(name != null)
                _attributeValues.Add(name);
            _geometry = geometry;
        }

        public SimpleVectorObject(string[] attValues, Shape geometry)
        {
            if(attValues != null)
                _attributeValues.AddRange(attValues);
            _geometry = geometry;
        }

        internal void SetHost(ISimpleVectorObjectHost host)
        {
            _host = host;
        }

        public int OID
        {
            get { return _oid; }
            set { _oid = value; }
        }

        public string Name
        {
            get
            {
                if(_attributeValues.Count >0)
                    return _attributeValues[0];
                return string.Empty;
            }
        }

        public string[] AttValues
        {
            get { return _attributeValues.Count > 0 ? _attributeValues.ToArray() : null; }
        }

        public void SetGeometry(DrawEngine.CoordEnvelope geometry)
        {
            _geometry = geometry;
            if (_host != null)
                _host.Add(this);
        }

        public void SetGeometry(Shape geoShape)
        {
            _geometry = geoShape;
            if (_host != null)
                _host.Add(this);
        }

        public CodeCell.AgileMap.Core.Shape Geometry
        {
            get
            {
                if (_geometry == null)
                    return null;
                if (_geometry is CoordEnvelope)
                {
                    CoordEnvelope evp = _geometry as CoordEnvelope;
                    ShapePoint[] points = new ShapePoint[] 
                    {
                        new ShapePoint(evp.MinX,evp.MaxY),
                        new ShapePoint(evp.MaxX,evp.MaxY),
                        new ShapePoint(evp.MaxX,evp.MinY),
                        new ShapePoint(evp.MinX,evp.MinY),
                        new ShapePoint(evp.MinX,evp.MaxY)
                    };
                    ShapeRing ring = new ShapeRing(points);
                    ShapePolygon ply = new ShapePolygon(new ShapeRing[] { ring });
                    return ply;
                }
                else if (_geometry is ShapePolyline)
                {
                    return _geometry as ShapePolyline;
                }
                else if (_geometry is ShapePolygon)
                {
                    return _geometry as ShapePolygon;
                }
                else if (_geometry is Shape)
                {
                    return _geometry as Shape;
                }
                return null;
            }
        }

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public bool IsSelected
        {
            get { return _selected; }
            set { _selected = value; }
        }

        public bool IsWaitingProcess
        {
            get { return _waitingProcess; }
            set { _waitingProcess = value; }
        }
    }
}

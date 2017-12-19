using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DrawEngine;
using System.ComponentModel;

namespace GeoDo.RSS.Core.VectorDrawing
{
    /// <summary>
    /// 冰缘线图层
    /// 包含冰缘线，和冰缘线拐点数据
    /// </summary>
    public class IceEdgeLayer:Layer,IVectorLayer
    {
        private PointF[] _controlPoint = null;
        private PointF[] _iceEdgeLine = null;
        private bool _visible = true;

        public IceEdgeLayer()
        {
            _name = _alias = "冰缘线";
        }

        [DisplayName("是否显示"), Category("状态")]
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            //if (_geometrys.Count == 0)
            //    return;
            //Graphics g = drawArgs.Graphics as Graphics;
            //ICanvas canvas = sender as ICanvas;
            //foreach (object geo in _geometrys)
            //{
            //    GraphicsPath path = ToGraphicsPath(geo, canvas);
            //    g.DrawPath(_pen, path);
            //    //
            //    EditObject eobj;
            //    if (!_editObjects.ContainsKey(geo))
            //    {
            //        eobj = new EditObject();
            //        eobj.Geometry = geo;
            //        _editObjects.Add(geo, eobj);
            //    }
            //    else
            //        eobj = _editObjects[geo];
            //    if (eobj.Path != null)
            //        eobj.Path.Dispose();
            //    eobj.Path = path;
            //    //
            //    if (_isAllowEdit)
            //    {
            //        Brush boxBrush = eobj.IsSelected ? Brushes.Green : Brushes.Red;
            //        for (int i = 0; i < path.PointCount; i++)
            //        {
            //            g.FillRectangle(boxBrush,
            //                path.PathPoints[i].X - EDIT_BOX_HALF_WIDTH,
            //                path.PathPoints[i].Y - EDIT_BOX_HALF_WIDTH,
            //                EDIT_BOX_WIDTH, EDIT_BOX_WIDTH);
            //        }
            //    }
            //}
        }
    }
}

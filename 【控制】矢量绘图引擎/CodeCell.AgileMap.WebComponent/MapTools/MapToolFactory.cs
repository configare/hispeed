using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;


namespace CodeCell.AgileMap.WebComponent
{
    internal static class MapToolFactory
    {
        private static Dictionary<enumMapTools, IMapCommand> _regedTools = new Dictionary<enumMapTools, IMapCommand>();

        static MapToolFactory()
        {
            _regedTools.Add(enumMapTools.ZoomInOut, new MapToolWheel());
            _regedTools.Add(enumMapTools.ZoomIn, new MapToolZoomIn());
            _regedTools.Add(enumMapTools.ZoomOut, new MapToolZoomOut());
            _regedTools.Add(enumMapTools.Pan, new MapToolPan());
            _regedTools.Add(enumMapTools.SelectToolRect, new SelectToolRect());
            _regedTools.Add(enumMapTools.SelectToolCirle, new SelectToolEllipse());
            _regedTools.Add(enumMapTools.SelectToolPolygon, new SelectToolPolygon());
            _regedTools.Add(enumMapTools.FullView, new MapCommandFullView());
            _regedTools.Add(enumMapTools.Refresh, new MapCommandRefresh());
            _regedTools.Add(enumMapTools.Identify, new MapToolIdentify());
            _regedTools.Add(enumMapTools.MeasureByPolyline, new MeasureToolByPolyline());
            _regedTools.Add(enumMapTools.MeasureByPolygon, new MeasureToolByPolygon());
        }

        internal static void Init(IMapControl mapcontrol)
        {
            foreach (IMapCommand cmd in _regedTools.Values)
                (cmd as MapCommand).SetMapControl(mapcontrol);
        }

        public static IMapCommand GetMapTool(enumMapTools maptool)
        {
            if(_regedTools.ContainsKey(maptool))
                return _regedTools[maptool];
            return null ;
        }
    }
}

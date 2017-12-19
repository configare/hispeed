using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    internal class MapZoomOutTool : MapZoomToolBase
    {
        public MapZoomOutTool()
            : base()
        { 
        }

        protected override void HandleZoom(IMapControl mapcontrol, RectangleF rect)
        {
            RectangleF oldrect = mapcontrol.ExtentPrj;
            float panzoomFactor = Math.Min(oldrect.Width / rect.Width, oldrect.Height / rect.Height);
            float zoomWidthAmount = oldrect.Width * panzoomFactor;
            float zoomHeightAmount = oldrect.Height * panzoomFactor;
            //
            rect.Inflate(zoomWidthAmount, zoomHeightAmount);
            //
            mapcontrol.ExtentPrj = rect;
        }
    }
}

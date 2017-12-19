using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CodeCell.AgileMap.Core;
using System.Windows.Forms;

namespace CodeCell.AgileMap.Components
{
    internal class MapZoomInTool : MapZoomToolBase
    {
        public MapZoomInTool()
            : base()
        { 
        }

        protected override void HandleZoom(IMapControl mapcontrol, RectangleF rect)
        {
            if (mapcontrol.MapRuntime.IsExceedMaxResolution(rect))
                return;
            //int level = TileComputer.GetLevelOfDetail(new SizeF(40075020f, 39986840f), new SizeF(rect.Width, rect.Height), 20);
            //(mapcontrol as Control).FindForm().Text = "LevelOfDetail:"+level.ToString();
            mapcontrol.ExtentPrj = rect;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class CanvasSetting
    {
        private string _name;
        private RenderSetting _renderSetting;
        private ZoomSetting _zoomSetting;
        private TileSetting _tileSetting;

        public CanvasSetting(string name,TileSetting tileSetting,RenderSetting renderSetting,ZoomSetting zoomSetting)
        {
            _name = name;
            _tileSetting = tileSetting;
            _renderSetting = renderSetting;
            _zoomSetting = zoomSetting;
        }

        public string Name
        {
            get { return _name; }
        }

        public TileSetting TileSetting
        {
            get { return _tileSetting; }
        }

        public RenderSetting RenderSetting
        {
            get { return _renderSetting; }
        }

        public ZoomSetting ZoomSetting
        {
            get { return _zoomSetting; }
        }
    }
}

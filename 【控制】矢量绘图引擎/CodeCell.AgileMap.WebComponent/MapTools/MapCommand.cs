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
using System.Windows.Media.Imaging;

namespace CodeCell.AgileMap.WebComponent
{
    public abstract class MapCommand:IMapCommand
    {
        protected IMapControl _mapcontrol = null;
        protected Canvas _canvas = null;
        protected string _name = null;
        protected string _tooltips = null;
        protected string _image = null;

        public MapCommand()
        { 
        }

        public string Name
        {
            get { return _name; }
        }

        public string ToolTips
        {
            get { return _tooltips; }
        }

        public string Image
        {
            get { return _image; }
        }

        internal void SetMapControl(IMapControl mapcontrol)
        {
            _mapcontrol = mapcontrol;
            _canvas = mapcontrol.Canvas;
        }

        public virtual void Click()
        { 
            //
        }
    }
}

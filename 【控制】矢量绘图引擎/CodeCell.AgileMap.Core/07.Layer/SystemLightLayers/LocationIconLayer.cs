using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CodeCell.AgileMap.Core
{
    internal class LocationIconLayer:ILocationIconLayer
    {
        private string _name = "地标图标";
        private IMapRuntime _mapRuntime = null;
        private List<LocationIcon> _locIcons = new List<LocationIcon>();
        private Size _iconSize = new Size(24, 20);
        private int _wndHalf = 3;
        private Image _img = null;
 
        public LocationIconLayer()
        {
        }

        #region ILocationIconLayer 成员

        public void Clear()
        {
            _locIcons.Clear();
        }

        public void Add(LocationIcon locationIcon)
        {
            _locIcons.Add(locationIcon);
        }

        #endregion

        #region ILightLayer 成员

        public string Name
        {
            get { return _name; }
        }

        public bool Enabled
        {
            get { return true; }
        }

        public void Init(IMapRuntime runtime)
        {
            _mapRuntime = runtime;
        }

        public void Render(RenderArgs arg)
        {
            if (_locIcons == null || _locIcons.Count == 0)
                return;
            if (_img == null)
                CreateBackgroudImage();
            using (Font font = new Font("微软雅黑", 9))
            {
                ICoordinateTransform tran = (_mapRuntime as IFeatureRenderEnvironment).CoordinateTransform;
                foreach (LocationIcon icon in _locIcons)
                {
                    if (icon == null || string.IsNullOrEmpty(icon.Text) || icon.Feature == null)
                        continue;
                    SizeF fontsize = arg.Graphics.MeasureString(icon.Text, font);
                    ShapePoint prjpt = GetLocationByFeature(icon.Feature);
                    PointF[] pts = tran.PrjCoord2PixelCoord(new ShapePoint[] {  prjpt});
                    pts[0].Y -= (_iconSize.Height + 5);
                    pts[0].X -= _iconSize.Width / 2;
                    arg.Graphics.DrawImage(_img, pts[0]);
                    arg.Graphics.DrawString(icon.Text, font, Brushes.Red, pts[0].X + (_iconSize.Width - fontsize.Width) / 2, pts[0].Y - (_iconSize.Height - fontsize.Height) / 2);
                }
            }
        }

        private void CreateBackgroudImage()
        {
            _img = new Bitmap(_iconSize.Width, _iconSize.Height);
            using (Graphics g = Graphics.FromImage(_img))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                using (Pen pen = new Pen(Color.FromArgb(64, 64, 64)))
                {
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddLine(0, 0, _iconSize.Width - 1, 0);
                        path.AddLine(_iconSize.Width - 1, 0, _iconSize.Width - 1, _iconSize.Height - 2 * _wndHalf);
                        path.AddLine(_iconSize.Width / 2 + _wndHalf, _iconSize.Height - 2 * _wndHalf, _iconSize.Width - 1, _iconSize.Height - 2 * _wndHalf);
                        path.AddLine(_iconSize.Width / 2 + _wndHalf, _iconSize.Height - 2 * _wndHalf, _iconSize.Width / 2, _iconSize.Height - 1);
                        path.AddLine( _iconSize.Width / 2, _iconSize.Height - 1,_iconSize.Width / 2 - _wndHalf, _iconSize.Height - 2 * _wndHalf);
                        path.AddLine(_iconSize.Width / 2 - _wndHalf, _iconSize.Height - 2 * _wndHalf,0, _iconSize.Height - 2 * _wndHalf);
                        path.AddLine( 0, _iconSize.Height - 2 * _wndHalf,0, 0);
                        using (Brush brsh = new SolidBrush(Color.FromArgb(255, 210, 225))) //Color.FromArgb(245, 240, 230)
                        {
                            g.FillPath(brsh, path);
                        }
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        public ShapePoint GetLocationByFeature(Feature fet)
        {
            lock (fet)
            {
                ShapePoint pt = fet.Geometry.Centroid.Clone() as ShapePoint;
                ICoordinateTransform tran = (_mapRuntime as IFeatureRenderEnvironment).CoordinateTransform;
                if ((fet.Projected && fet.FeatureClass.OriginalCoordinateType == enumCoordinateType.Geographic) ||
                       fet.FeatureClass.OriginalCoordinateType == enumCoordinateType.Projection)
                {
                    return pt;
                }
                else if (!fet.Projected && fet.FeatureClass.OriginalCoordinateType == enumCoordinateType.Geographic)
                {
                    tran.GeoCoord2PrjCoord(new ShapePoint[] { pt });
                }
                return pt;
            }
        }

        #endregion
    }
}

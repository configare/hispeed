using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CodeCell.AgileMap.WebComponent.AgileMapServiceProxy;
using System.Windows.Media.Imaging;

namespace CodeCell.AgileMap.WebComponent
{
    public partial class FullviewMapControl : UserControl
    {
        private IMapControl _mapcontrol = null;
        private readonly string cstInsideRequestIdentify = null;
        private PrjRectangleF _viewport;
        private bool _dragingBox = false;
        private Point _previousPoint;
        private Point _startPoint;
        private double _resolutionX = 0;
        private double _resolutionY = 0;
        private const int cstTopBanks = 0;
        private const int cstLeftBanks = 0;

        public FullviewMapControl()
        {
            InitializeComponent();
            cstInsideRequestIdentify = "FullviewMap_" + Guid.NewGuid();
            SizeChanged += new SizeChangedEventHandler(FullviewMapControl_SizeChanged);
        }

        void FullviewMapControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mapImage.Width = ActualWidth - cstLeftBanks;
            mapImage.Height = ActualHeight - cstTopBanks;
            Canvas.SetTop(mapImage, cstTopBanks / 2);
            Canvas.SetLeft(mapImage, cstLeftBanks / 2);
            cliprect.Rect = new Rect(0, 0, ActualWidth, ActualHeight);
            Refresh();
        }

        public void SetBuddy(IMapControl mapcontrol)
        {
            _mapcontrol = mapcontrol;
            _mapcontrol.MapServerAgent.MapServiceClient.GeoEnvelope2PrjEnvelopeCompleted += new EventHandler<AgileMapServiceProxy.GeoEnvelope2PrjEnvelopeCompletedEventArgs>(MapServiceClient_GeoEnvelope2PrjEnvelopeCompleted);
            _mapcontrol.MapServerAgent.MapServiceClient.GetMapImageCompleted += new EventHandler<GetMapImageCompletedEventArgs>(MapServiceClient_GetMapImageCompleted);
            _mapcontrol.OnMapControlViewportChanged += new OnMapControlViewportChangedHandler(OnMapControlViewportChanged);
        }

        public void Refresh()
        {
            if (mapImage.Width < double.Epsilon || mapImage.Height < double.Epsilon)
                return;
            _mapcontrol.MapServerAgent.MapServiceClient.GeoEnvelope2PrjEnvelopeAsync(-180, 180, -90, 90, cstInsideRequestIdentify);
        }

        void OnMapControlViewportChanged(object sender, PrjRectangleF oldviewport, PrjRectangleF newviewport)
        {
            if (_viewport.Width < double.Epsilon || _viewport.Height < double.Epsilon)
                return;
            _resolutionX = _viewport.Width / mapImage.ActualWidth;
            _resolutionY = _viewport.Height / mapImage.ActualHeight;
            double res = _resolutionY;
            double x = (newviewport.MinX - _viewport.MinX) / res;
            double y = (_viewport.MaxY - newviewport.MaxY) / res;
            double w = newviewport.Width / res;
            double h = newviewport.Height / res;
            Canvas.SetLeft(foucusBox, x + cstLeftBanks / 2);
            Canvas.SetTop(foucusBox, y + cstTopBanks / 2 + 4);
            foucusBox.Width = w;
            if (h > 8)
                foucusBox.Height = h - 8;
            else
                foucusBox.Height = h;
        }

        bool IsResponseFromInsideRequested(object userState)
        {
            if (userState == null || userState.ToString() != cstInsideRequestIdentify)
                return false;
            return true;
        }

        void MapServiceClient_GeoEnvelope2PrjEnvelopeCompleted(object sender, AgileMapServiceProxy.GeoEnvelope2PrjEnvelopeCompletedEventArgs e)
        {
            if (!IsResponseFromInsideRequested(e.UserState))
                return;
            string rect = e.Result;
            if (string.IsNullOrEmpty(rect))
                return;
            string[] ds = rect.Split(',');
            PrjRectangleF fullViewport = new PrjRectangleF();
            fullViewport.MinX = double.Parse(ds[0]);
            fullViewport.MaxX = fullViewport.MinX + double.Parse(ds[2]);
            fullViewport.MinY = double.Parse(ds[1]);
            fullViewport.MaxY = fullViewport.MinY + double.Parse(ds[3]);
            RequestMap(fullViewport);
        }

        void MapServiceClient_GetMapImageCompleted(object sender, GetMapImageCompletedEventArgs e)
        {
            if (!IsResponseFromInsideRequested(e.UserState))
                return;
            MapImage retMapImage = e.Result;
            if (retMapImage != null)
            {
                BitmapImage bi = new BitmapImage(new Uri(retMapImage.ImageUrl));
                mapImage.Source = bi;
                _viewport.MinX = retMapImage.Left;
                _viewport.MinY = retMapImage.Bottom;
                _viewport.MaxX = retMapImage.Left + retMapImage.Width;
                _viewport.MaxY = retMapImage.Bottom + retMapImage.Height;
            }
        }

        private void RequestMap(PrjRectangleF fullViewport)
        {
            _mapcontrol.MapServerAgent.MapServiceClient.GetMapImageAsync(fullViewport.MinX,
                fullViewport.MinY, fullViewport.Width, fullViewport.Height, (int)ActualWidth, (int)ActualHeight,null, cstInsideRequestIdentify);
        }

        private void foucusBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragingBox = true;
            _previousPoint = e.GetPosition(foucusBox);
            _startPoint = _previousPoint;
        }

        private void foucusBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragingBox)
            {
                Point pt = e.GetPosition(foucusBox);
                Point thispt = e.GetPosition(this);
                if (thispt.X > ActualWidth - 10 || thispt.X < 0 || thispt.Y < 0 || thispt.Y > ActualHeight - 10)
                {
                    return;
                }
                double offsetX = pt.X - _previousPoint.X;
                double offsetY = pt.Y - _previousPoint.Y;
                Canvas.SetLeft(foucusBox, Canvas.GetLeft(foucusBox) + offsetX);
                Canvas.SetTop(foucusBox, Canvas.GetTop(foucusBox) + offsetY);
            }
        }

        private void foucusBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_dragingBox)
            {
                //
                PrjRectangleF viewport = new PrjRectangleF();
                double x = Canvas.GetLeft(foucusBox);
                double y = Canvas.GetTop(foucusBox);
                viewport.MinX = x * _resolutionX + _viewport.MinX;
                viewport.MaxY = _viewport.MaxY - y * _resolutionY;
                viewport.MaxX = viewport.MinX + foucusBox.Width * _resolutionX;
                viewport.MinY = viewport.MaxY - foucusBox.Height * _resolutionY;
                //
                _mapcontrol.SetViewportByPrj(viewport);
            }
            _dragingBox = false;
        }

        private void mapImage_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Refresh();
        }
    }
}

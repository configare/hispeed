using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class DefaultControlLayer : Layer, IControlLayer
    {
        private bool _beginWheel = false;
        private System.Timers.Timer _timerForWheel = null;
        private int _lastTicks = 0;
        private const int cstDelayTicksForCheckWheelIsFinished = 600;
        private const int cstCheckRequenceForWheelIsFinished = 20;
        private delegate void CheckMouseWheelIsFinished();
        private CheckMouseWheelIsFinished _checkMouseWheelIsFinished = null;
        protected PointF _preLocation = PointF.Empty;
        protected bool _enabledPan = true;
        protected bool _hasChanged = false;
        protected ICanvas _canvas = null;
        protected ZoomStepsCalculator _zoomStepsCalculator = null;

        public DefaultControlLayer()
            : base()
        {
            _name = "Pan";
            _alias = "漫游";
            InitForDummy();
            BuildZoomStepsCalculator();
        }

        private void BuildZoomStepsCalculator()
        {
            _zoomStepsCalculator = new ZoomStepsCalculator();
        }

        private void InitForDummy()
        {
            _checkMouseWheelIsFinished = new CheckMouseWheelIsFinished(checkMouseWheelIsFinishedFun);
            _timerForWheel = new System.Timers.Timer(cstCheckRequenceForWheelIsFinished);
            _timerForWheel.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
        }

        public bool EnabledPan
        {
            get { return _enabledPan; }
            set { _enabledPan = value; }
        }

        #region ICanvasEvent 成员

        public virtual void Event(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
            if (_canvas == null)
                _canvas = sender as ICanvas;
            switch (eventType)
            {
                case enumCanvasEventType.MouseDown:
                    if (_enabledPan)
                    {
                        _preLocation.X = e.ScreenX;
                        _preLocation.Y = e.ScreenY;
                        _canvas.DummyRenderModeSupport.SetToDummyRenderMode();
                    }
                    break;
                case enumCanvasEventType.MouseMove:
                    if (_enabledPan)
                    {
                        if (_preLocation.IsEmpty)
                            return;
                        float offsetX = e.ScreenX - _preLocation.X;
                        float offsetY = e.ScreenY - _preLocation.Y;
                        if ((_canvas as IDummyRenderModeSupport).IsDummyModel)
                            _hasChanged = true;
                        ApplyOffset(sender as ICanvas, offsetX, offsetY);
                        _preLocation.X = e.ScreenX;
                        _preLocation.Y = e.ScreenY;
                    }
                    break;
                case enumCanvasEventType.MouseUp:
                    if (_enabledPan)
                    {
                        _hasChanged = false;
                        _preLocation = PointF.Empty;
                        _canvas.DummyRenderModeSupport.SetToNomralRenderMode();
                        //if (_hasChanged)
                            _canvas.Refresh(enumRefreshType.All);
                    }
                    break;
                case enumCanvasEventType.MouseWheel:
                    if (!_beginWheel /*&& (_canvas as IDummyRenderModeSupport).IsEnabledDummyCache*/)
                    {
                        _timerForWheel.Start();
                        (_canvas as IDummyRenderModeSupport).SetToDummyRenderMode();
                        _hasChanged = true;
                    }
                    ICanvas canvas = sender as ICanvas;
                    //Console.WriteLine("Wheel:" + e.WheelDelta.ToString());
                    int steps = 0;
                    if (e.WheelDelta > 0)
                    {
                        steps = _zoomStepsCalculator.GetZoomSteps(_canvas, e.WheelDelta, -1);
                        canvas.ZoomOutByStep(e.ScreenX, e.ScreenY, steps);
                    }
                    else
                    {
                        steps = _zoomStepsCalculator.GetZoomSteps(_canvas, e.WheelDelta, 1);
                        canvas.ZoomInByStep(e.ScreenX, e.ScreenY, steps);
                    }
                    //Console.WriteLine("Steps:" + steps.ToString());
                    _lastTicks = Environment.TickCount;
                    break;
            }
        }

        private void ApplyOffset(ICanvas canvas, float offsetX, float offsetY)
        {
            canvas.ApplyOffset(offsetX, offsetY);
        }

        #endregion

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Environment.TickCount - _lastTicks > cstDelayTicksForCheckWheelIsFinished)
            {
                _beginWheel = false;
                _timerForWheel.Stop();
                (_canvas.Container as Control).Invoke(_checkMouseWheelIsFinished);
            }
        }

        private void checkMouseWheelIsFinishedFun()
        {
            if ((_canvas as IDummyRenderModeSupport).IsDummyModel)
            {
                (_canvas as IDummyRenderModeSupport).SetToNomralRenderMode();
                _canvas.Refresh(enumRefreshType.All);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _canvas = null;
            _checkMouseWheelIsFinished = null;
            _zoomStepsCalculator = null;
        }
    }
}

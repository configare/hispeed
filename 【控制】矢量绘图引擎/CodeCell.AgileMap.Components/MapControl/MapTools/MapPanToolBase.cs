using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class MapPanToolBase:MapToolBase
    {
        private bool _beginWheel = false;
        private System.Timers.Timer _timerForWheel = null;
        private int _lastTicks = 0;
        private const int cstDelayTicksForCheckWheelIsFinished = 600;
        private const int cstCheckRequenceForWheelIsFinished = 20;
        private delegate void CheckMouseWheelIsFinished();
        private CheckMouseWheelIsFinished _checkMouseWheelIsFinished = null;
        private IMapControl _mapcontrol = null;
        private float panzoomFactor = 0.1f;

        public MapPanToolBase()
            : base()
        {
            InitForMouseWheelHandle();
        }


        private void InitForMouseWheelHandle()
        {
            _checkMouseWheelIsFinished = new CheckMouseWheelIsFinished(checkMouseWheelIsFinishedFun);
            _timerForWheel = new System.Timers.Timer(cstCheckRequenceForWheelIsFinished);
            _timerForWheel.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
        }

        protected override void MouseWheel(IMapControl mapcontrol, MouseEventArgs e)
        {
            if (_mapcontrol == null)
                _mapcontrol = mapcontrol;
            if (!_beginWheel)
            {
                _beginWheel = true;
                _timerForWheel.Start();
                (mapcontrol as IMapControlDummySupprot).SetToDummyRenderMode();
            }
            if (e.Delta < 0)
                ZoomOut();
            else
                ZoomIn();
            _lastTicks = Environment.TickCount;
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Environment.TickCount - _lastTicks > cstDelayTicksForCheckWheelIsFinished)
            {
                _beginWheel = false;
                _timerForWheel.Stop();
                (_mapcontrol as Control).Invoke(_checkMouseWheelIsFinished);
            }
        }

        private void checkMouseWheelIsFinishedFun()
        {
            (_mapcontrol as IMapControlDummySupprot).ResetToNormalRenderMode();
            _mapcontrol.ReRender();
        }

        public void ZoomIn()
        {
            RectangleF _viewport = _mapcontrol.ExtentPrj;
            float zoomWidthAmount = -_viewport.Width * panzoomFactor;
            float zoomHeightAmount = -_viewport.Height * panzoomFactor;
            //
            RectangleF newViewport = _viewport;
            newViewport.Inflate(zoomWidthAmount, zoomHeightAmount);
            //
            if (_mapcontrol.OperationStack.Enabled)
            {
                OprChangeExtent opr = new OprChangeExtent(_mapcontrol, newViewport);
                _mapcontrol.OperationStack.Do(opr);
            }
            else
            {
                _mapcontrol.ExtentPrj = newViewport;
                _mapcontrol.ReRender();
            }
        }

        public void ZoomOut()
        {
            RectangleF _viewport = _mapcontrol.ExtentPrj;
            float zoomWidthAmount = _viewport.Width * panzoomFactor;
            float zoomHeightAmount = _viewport.Height * panzoomFactor;
            RectangleF newViewport = _viewport;
            newViewport.Inflate(zoomWidthAmount, zoomHeightAmount);
            //
            if (_mapcontrol.OperationStack.Enabled)
            {
                OprChangeExtent opr = new OprChangeExtent(_mapcontrol, newViewport);
                _mapcontrol.OperationStack.Do(opr);
            }
            else
            {
                _mapcontrol.ExtentPrj = newViewport;
                _mapcontrol.ReRender();
            }
        }
    }
}

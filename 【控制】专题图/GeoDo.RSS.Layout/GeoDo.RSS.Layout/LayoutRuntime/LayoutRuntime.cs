using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Diagnostics;

namespace GeoDo.RSS.Layout
{
    public class LayoutRuntime : ILayoutRuntime, ILayoutCoordinateTransform, IDisposable
    {
        public const float INCH_TO_CENTIMETER = 2.539999918f;
        private const float MAX_SCALE = 100;   //100倍
        private const float MIN_SCALE = 0.01f; //100倍 
        protected ILayout _layout;
        protected ILayoutHost _host;
        private Matrix _matrix = new Matrix();
        private Matrix _invertMatrix = new Matrix();
        private float _scale = 1f;
        private int _offsetX = 0;
        private int _offsetY = 0;
        private ISelectedEditBoxManager _selectedEditBoxManager;

        public LayoutRuntime(ILayout layout, ILayoutHost host)
        {
            _layout = layout;
            _host = host;
            _selectedEditBoxManager = host.SelectedEditBoxManager;
            _host.CanvasSizeChanged += new EventHandler(CanvasSizeChanged);
            UpdateMatrix();
        }

        public void ChangeLayout(ILayout layout)
        {
            if (layout == null)
                return;
            if (_layout != null)
            {
                _layout.Dispose();
            }
            _layout = layout;
            UpdateMatrix();
        }

        void CanvasSizeChanged(object sender, EventArgs e)
        {
            UpdateMatrix();
        }

        public void Render(IDrawArgs drawArgs)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            TryUnpdateSelectedEditBoxes();
            if (_layout.Visible)
                _layout.Render(this, drawArgs);
            //sw.Stop();
            //Console.WriteLine(sw.ElapsedMilliseconds.ToString());
        }

        private void TryUnpdateSelectedEditBoxes()
        {
            _selectedEditBoxManager.Update(this);
        }

        public ISelectedEditBoxManager SelectedEditBoxManager
        {
            get { return _selectedEditBoxManager; }
        }

        public IElement[] Selection
        {
            get
            {
                return QueryElements((ele) => { return ele.IsSelected; }, false);
            }
        }

        public void ClearSelection()
        {
            IElement[] selection = Selection;
            if (selection == null || selection.Length == 0)
                return;
            foreach (IElement ele in selection)
                ele.IsSelected = false;
        }

        public IElement[] LockedElements
        {
            get
            {
                return QueryElements((ele) => { return ele.IsLocked; }, false);
            }
        }

        public IElement[] Hit(float layoutX, float layoutY)
        {
            return QueryElements((ele) => { return ele.IsHited(layoutX, layoutY); }, true);
        }

        public IElement[] Hit(RectangleF layoutRect)
        {
            return QueryElements((ele) => { return ele.IsHited(layoutRect); }, false);
        }

        public IElement[] QueryElements(Func<IElement, bool> filter, bool returnOnlyOne)
        {
            //先反转元素顺序再查询,保证了层顺序的优先级
            List<IElement> eles = ReserveElements();
            List<IElement> retEles = new List<IElement>();
            foreach (IElement e in eles)
            {
                if (filter(e))
                {
                    retEles.Add(e);
                    if (returnOnlyOne)
                        break;
                }
            }
            return retEles.Count > 0 ? retEles.ToArray() : null;
        }

        public List<IElement> ReserveElements()
        {
            List<IElement> eles = new List<IElement>();
            ReserveElements(_layout, eles);
            eles.Reverse();
            return eles;
        }

        private void ReserveElements(IElement beginElement, List<IElement> eles)
        {
            eles.Add(beginElement);
            if (beginElement is IElementGroup)
                foreach (IElement sub in (beginElement as IElementGroup).Elements)
                    ReserveElements(sub, eles);
        }

        public ILayout Layout
        {
            get { return _layout; }
        }

        public void ApplyOffset(int offsetX, int offsetY)
        {
            _offsetX += offsetX;
            _offsetY += offsetY;
            UpdateMatrix();
        }

        public void ResetOffets()
        {
            _offsetX = 0;
            _offsetY = 0;
        }

        public float Scale
        {
            get { return _scale; }
            set
            {
                if (value > MAX_SCALE)
                    value = MAX_SCALE;
                else if (value < MIN_SCALE)
                    value = MIN_SCALE;
                _scale = value;
                UpdateMatrix();
            }
        }

        public void Layout2Screen(ref float x, ref float y)
        {
            Layout2Pixel(ref x, ref y);
            PointF[] ptfs = new PointF[1] { new PointF(x, y) };
            _matrix.TransformPoints(ptfs);
            x = ptfs[0].X;
            y = ptfs[0].Y;
        }

        public ILayoutHost GetHost()
        {
            return _host;
        }

        public void Layout2Screen(ref float v)
        {
            Layout2Pixel(ref v);
            v *= _scale;
        }

        public void Screen2Layout(ref float x, ref float y)
        {
            PointF[] pts = new PointF[1] { new PointF(x, y) };
            _invertMatrix.TransformPoints(pts);
            x = pts[0].X;
            y = pts[0].Y;
            Pixel2Layout(ref x, ref y);
        }

        public void UpdateMatrix()
        {
            if (_layout == null)
                return;
            float w = _layout.Size.Width;
            float h = _layout.Size.Height;
            Layout2Pixel(ref w, ref h);
            float offsetXAutoToCenter = (_host.CanvasSize.Width - w * _scale) / 2f;
            float offsetYAutoToCenter = (_host.CanvasSize.Height - h * _scale) / 2f;
            _matrix.Reset();
            _matrix.Translate(offsetXAutoToCenter + _offsetX, offsetYAutoToCenter + _offsetY);
            _matrix.Scale(_scale, _scale);
            _invertMatrix.Dispose();
            _invertMatrix = _matrix.Clone();
            _invertMatrix.Invert();
        }

        public void Layout2Pixel(ref float x, ref float y)
        {
            switch (_layout.Unit)
            {
                case enumLayoutUnit.Pixel:
                    break;
                case enumLayoutUnit.Centimeter:
                    x = this.Centimeter2Pixel(x);
                    y = this.Centimeter2Pixel(y);
                    break;
                default:
                    throw new NotSupportedException(_layout.Unit.ToString());
            }
        }

        public void Layout2Pixel(ref float v)
        {
            switch (_layout.Unit)
            {
                case enumLayoutUnit.Pixel:
                    break;
                case enumLayoutUnit.Centimeter:
                    v = this.Centimeter2Pixel(v);
                    break;
                default:
                    throw new NotSupportedException(_layout.Unit.ToString());
            }
        }

        public void Pixel2Layout(ref float x, ref float y)
        {
            switch (_layout.Unit)
            {
                case enumLayoutUnit.Pixel:
                    break;
                case enumLayoutUnit.Centimeter:
                    x = this.Pixel2Centimeter((int)x);
                    y = this.Pixel2Centimeter((int)y);
                    break;
                default:
                    throw new NotSupportedException(_layout.Unit.ToString());
            }
        }

        public float Pixel2Centimeter(int pixels)
        {
            return INCH_TO_CENTIMETER * Pixel2Inch(pixels, _host.DPI);
        }

        public float Pixel2Centimeter(int pixels, float dpi)
        {
            return INCH_TO_CENTIMETER * Pixel2Inch(pixels, dpi);
        }

        public int Centimeter2Pixel(float centimeters)
        {
            return (int)(Centimeter2Inch(centimeters) * _host.DPI);
        }

        public int Centimeter2Pixel(float centimeters, float dpi)
        {
            return (int)(Centimeter2Inch(centimeters) * dpi);
        }

        private float Pixel2Inch(int pixels, float dpi)
        {
            return pixels / (float)dpi;
        }

        private float Centimeter2Inch(float centimeters)
        {
            return centimeters / INCH_TO_CENTIMETER;
        }

        public void Dispose()
        {
            if (_layout != null)
            {
                _layout.Dispose();
                _layout = null;
            }
            if (_matrix != null)
            {
                _matrix.Dispose();
                _matrix = null;
                _invertMatrix.Dispose();
                _invertMatrix = null;
            }
            _host = null;
        }
    }
}

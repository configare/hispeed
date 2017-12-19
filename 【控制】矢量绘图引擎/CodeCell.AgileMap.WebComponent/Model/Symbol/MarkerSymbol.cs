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

namespace CodeCell.AgileMap.WebComponent
{
    public abstract class MarkerSymbol:Symbol
    {
        internal static IMapControl mapControl = null;
        protected UIElement _uiElement = null;
        protected Point _prjLocation;
        protected AdjustSizeArgument _adjustSizeArgument = null;

        public MarkerSymbol()
            : base()
        { 
        }

        public Point PrjLocation
        {
            get { return _prjLocation; }
            internal set { _prjLocation = value; }
        }

        public AdjustSizeArgument AdjustSizeArgument
        {
            get { return _adjustSizeArgument; }
            set { _adjustSizeArgument = value; }
        }

        public virtual void ResetToDefaultSize()
        { 
        }

        public virtual Size ActualSize
        {
            get { return Size.Empty; }
        }

        public UIElement UIElement
        {
            get { return _uiElement; }
        }

        public void UpatePixelLocation(double resolution)
        {
            double res = 0;
            if (_adjustSizeArgument != null &&
                _adjustSizeArgument.IsEnabled)
            {
                _adjustSizeArgument.IsValidRange(resolution, out res);
                double newsize = _adjustSizeArgument.MaxSize -
                    _adjustSizeArgument.Factor * res;
                AdjustSizeForSuitResolution(newsize);
            }
            Point pt = mapControl.Prj2PixelTransform.Transform(_prjLocation);
            Canvas.SetLeft(UIElement, pt.X - ActualSize.Width / 2);
            Canvas.SetTop(UIElement, pt.Y - ActualSize.Height / 2);
        }

        public virtual void AdjustSizeForSuitResolution(double resolution)
        { 
        }

        public abstract MarkerSymbol Clone();
    }

    public class TrueTypeFontMarkerSymbol : MarkerSymbol
    {
        protected double _fontSize = 60;
        protected double _originalFontSize = 60;
        protected string _fontFamily = "Wingdings";
        protected int _charCode = 0;
        protected TextBlock _textBlock = null;
        protected Color _foreground = Colors.Red;
        protected FontSource _fontSource = null;

        public TrueTypeFontMarkerSymbol()
            : base()
        {
            BuildUIElement();
        }

        public TrueTypeFontMarkerSymbol(string fontFamily, int charCode, double fontSize)
        {
            _fontFamily = fontFamily;
            _charCode = charCode;
            _fontSize = fontSize;
            _originalFontSize = _fontSize;
            BuildUIElement();
        }

        public TrueTypeFontMarkerSymbol(FontSource fontSource,string fontFamily, int charCode, double fontSize)
        {
            _fontSource = fontSource;
            _fontFamily = fontFamily;
            _charCode = charCode;
            _fontSize = fontSize;
            _originalFontSize = _fontSize;
            BuildUIElement();
        }

        public TrueTypeFontMarkerSymbol(string fontFamily, int charCode, double fontSize,Color foreground)
        {
            _fontFamily = fontFamily;
            _charCode = charCode;
            _fontSize = fontSize;
            _foreground = foreground;
            _originalFontSize = _fontSize;
            BuildUIElement();
        }

        public TrueTypeFontMarkerSymbol(FontSource fontsource, string fontFamily, int charCode, double fontSize, Color foreground)
        {
            _fontSource = fontsource;
            _fontFamily = fontFamily;
            _charCode = charCode;
            _fontSize = fontSize;
            _foreground = foreground;
            _originalFontSize = _fontSize;
            BuildUIElement();
        }

        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                _originalFontSize = _fontSize;
                BuildUIElement();
            }
        }

        public override void ResetToDefaultSize()
        {
            _fontSize = _originalFontSize;
            BuildUIElement();
        }

        public string FontFamily
        {
            get { return _fontFamily; }
            set 
            {
                _fontFamily = value;
                BuildUIElement();
            }
        }

        public FontSource FontSource
        {
            get { return _fontSource; }
            set
            {
                _fontSource = value;
                BuildUIElement();
            }
        }

        public int CharCode
        {
            get { return _charCode; }
            set
            {
                _charCode = value; 
                BuildUIElement();
            }
        }

        public Color Foreground
        {
            get { return _foreground; }
            set 
            {
                _foreground = value;
                BuildUIElement();
            }
        }

        public override Size ActualSize
        {
            get
            {
                return new Size(_textBlock.ActualWidth, _textBlock.ActualHeight);
            }
        }

        private void BuildUIElement()
        {
            if (_textBlock == null)
                _textBlock = new TextBlock();
            if (_fontSource != null)
                _textBlock.FontSource = _fontSource;
            if (_fontFamily != null)
                _textBlock.FontFamily = new FontFamily(_fontFamily);
            _textBlock.FontSize = _fontSize;
            _textBlock.Text = new string(Convert.ToChar(_charCode), 1);
            _textBlock.Foreground = new SolidColorBrush(_foreground);
            //
            _uiElement = _textBlock;
        }

        public override void AdjustSizeForSuitResolution(double newsize)
        {
            _textBlock.FontSize = (int)newsize;
        }

        public override MarkerSymbol Clone()
        {
            TrueTypeFontMarkerSymbol sym = new TrueTypeFontMarkerSymbol(_fontSource, _fontFamily, _charCode,_fontSize,_foreground);
            sym.AdjustSizeArgument = _adjustSizeArgument;
            return sym;
        }
    }
}

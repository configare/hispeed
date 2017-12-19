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
    public class FillSymbol:Symbol
    {
        protected LineSymbol _outlineSymbol = null;
        protected Brush _fillBrush = null;

        public FillSymbol()
            : base()
        {
            _outlineSymbol = new LineSymbol(new SolidColorBrush(Colors.Red));
            _fillBrush = new SolidColorBrush(Color.FromArgb(80, 255, 0, 0));
        }

        public LineSymbol OutlineSymbol
        {
            get { return _outlineSymbol; }
            set { _outlineSymbol = value;  }
        }

        public Brush FillBrush
        {
            get { return _fillBrush; }
            set { _fillBrush = value; }
        }
    }
}

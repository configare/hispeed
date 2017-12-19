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
    public class LineSymbol:Symbol
    {
        protected int _lineWidth = 1;
        protected Brush _lineBrush = null;

        public LineSymbol()
            : base()
        {
            _lineBrush = new SolidColorBrush(Colors.Blue);
        }

        public LineSymbol(Brush lineBrush)
            :base()
        {
            _lineBrush = lineBrush;
        }

        public int LineWidth
        {
            get { return _lineWidth; }
            set { _lineWidth = value; }
        }

        public Brush LineBrush
        {
            get { return _lineBrush; }
            set { _lineBrush = value; }
        }
    }
}

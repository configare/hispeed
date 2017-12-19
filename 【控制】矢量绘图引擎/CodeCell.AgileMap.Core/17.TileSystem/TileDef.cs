using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Core
{
    public class TileDef
    {
        private uint _row = 0;
        private uint _col = 0;
        private RectangleF _rect = RectangleF.Empty;
        private string _quadkey = null;

        public TileDef(uint row, uint col,RectangleF rect,string quadkey)
        {
            _row = row;
            _col = col;
            _rect = rect;
            _quadkey = quadkey;
        }

        public RectangleF Rect
        {
            get { return _rect; }
        }

        public string Quadkey
        {
            get { return _quadkey; }
        }

        public override string ToString()
        {
            return string.Format("(Col:{0},Row{1},Key:{2})", _col, _row, _quadkey);
        }
    }
}

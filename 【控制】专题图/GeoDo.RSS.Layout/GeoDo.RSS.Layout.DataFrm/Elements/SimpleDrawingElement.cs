using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.Layout.DataFrm
{
    //[Export(typeof(IElement)), Category("图例")]
    public class SimpleDrawingElement:DrawingElement
    {
        private ISymbol _symbol;

        public SimpleDrawingElement()
            : base()
        {
            _name = "简单点矢量图例";
        }

        [DisplayName("符号"),Category("外观"), TypeConverter(typeof(ExpandableObjectConverter))]
        public ISymbol Symbol
        {
            get { return _symbol; }
            set
            {
                _symbol = value;
            }
        }

        protected override CodeCell.AgileMap.Core.ISymbol GetSymbol()
        {
            return _symbol;
        }
    }
}

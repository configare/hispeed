using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Linq;
using System.IO;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class SimpleFeatureRenderer:BaseFeatureRenderer, IFeatureRenderer
    {
        public SimpleFeatureRenderer(ISymbol symbol)
        {
            _currentSymbol = symbol;
        }

        [DisplayName("符号"), TypeConverter(typeof(ExpandableObjectConverter))]
        public ISymbol Symbol
        {
            get { return _currentSymbol; }
            set 
            {
                _currentSymbol = value;
            }
        }

        protected override void SetCurrentSymbolFromFeature(Feature feature)
        {
        }

        public override PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("Renderer");
            obj.AddAttribute("type", Path.GetFileName(this.GetType().Assembly.Location)+"," +this.GetType().ToString());
            //symbol
            if (_currentSymbol != null)
            {
                PersistObject symObj = (_currentSymbol as IPersistable).ToPersistObject();
                obj.AddSubNode(symObj);
            }
            //
            return obj;
        }

        public static IFeatureRenderer FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            ISymbol symbol = PersistObject.ReflectObjFromXElement(ele.Element("Symbol")) as ISymbol;
            return new SimpleFeatureRenderer(symbol);
        }
    }
}

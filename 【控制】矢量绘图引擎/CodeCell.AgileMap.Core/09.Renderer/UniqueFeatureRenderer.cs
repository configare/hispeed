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
    public class UniqueFeatureRenderer:BaseFeatureRenderer
    {
        private ISymbol _defaultSymbol = null;
        private Dictionary<string, ISymbol> _symbols = null;
        private string _field = null;

        public UniqueFeatureRenderer(string field)
        {
            _field = field;
        }

        public UniqueFeatureRenderer(string field,Dictionary<string, ISymbol> symbols)
        {
            _field = field;
            _symbols = symbols;
        }

        [Browsable(false)]
        public string FieldName
        {
            get { return _field; }
        }

        [DisplayName("缺省符号")]
        public ISymbol DefaultSymbol
        {
            get { return _defaultSymbol; }
            set 
            {
                _defaultSymbol = value;
            }
        }

        [DisplayName("唯一值符号")]
        public Dictionary<string, ISymbol> Symbol
        {
            get { return _symbols; }
            set 
            {
                _symbols = value;
            }
        }

        protected override void SetCurrentSymbolFromFeature(Feature feature)
        {
            if (Array.IndexOf(feature.FieldNames, _field) < 0)
            {
                _currentSymbol = _defaultSymbol;
                return;
            }
            if (_symbols != null)
            {
                string fieldValue = feature.GetFieldValue(_field).Trim();
                if (_symbols.ContainsKey(fieldValue))
                {
                    _currentSymbol = _symbols[fieldValue];
                    return;
                }
            }
            _currentSymbol = _defaultSymbol;
        }

        public override void Dispose()
        {
            if (_symbols != null)
            {
                if (_symbols.Count > 0)
                    foreach (ISymbol sym in _symbols.Values)
                        sym.Dispose();
                _symbols.Clear();
                _symbols = null;
            }
            if (_defaultSymbol != null)
            {
                _defaultSymbol.Dispose();
                _defaultSymbol = null;
            }
            base.Dispose();
        }

        public override PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("Renderer");
            obj.AddAttribute("type", Path.GetFileName(this.GetType().Assembly.Location) + "," + this.GetType().ToString());
            obj.AddAttribute("field", _field != null ? _field : string.Empty);
            //default symbol
            if (_defaultSymbol != null)
            {
                PersistObject defaultObj = new PersistObject("DefaultSymbol");
                obj.AddSubNode(defaultObj);
                defaultObj.AddSubNode((_defaultSymbol as IPersistable).ToPersistObject());
            }
            //symbols
            if (_symbols != null)
            {
                PersistObject symsObj = new PersistObject("UniqueSymbols");
                obj.AddSubNode(symsObj);
                symsObj.AddAttribute("UniqueValues", string.Join(",", _symbols.Keys.ToArray()));
                foreach (ISymbol sym in _symbols.Values)
                {
                    symsObj.AddSubNode((sym as IPersistable).ToPersistObject());
                }
            }
            return obj;
        }

        public static IFeatureRenderer FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            ISymbol defaultSym = null;
            Dictionary<string, ISymbol> dic = null;
            if (ele.Element("DefaultSymbol") != null)
                defaultSym = PersistObject.ReflectObjFromXElement(ele.Element("DefaultSymbol")) as ISymbol;
            if (ele.Element("UniqueSymbols") != null)
            {
                string[] values = ele.Element("UniqueSymbols").Attribute("UniqueValues").Value.Split(',');
                var result = ele.Element("UniqueSymbols").Elements("Symbol");
                int i = 0;
                List<ISymbol> symbols = new List<ISymbol>();
                foreach (XElement r in result)
                {
                    symbols.Add(PersistObject.ReflectObjFromXElement(r) as ISymbol);
                    i++;
                }
                dic = new Dictionary<string, ISymbol>();
                /*
                 * 如果保存mxd是采用的是其他投影(例如：Mercator)，那么要素数量可能
                 * 多余真是的要素数量，对应的符号也多。
                 * 使用不同的投影打开后，可能符号数和要素数不一致，因此取了最小值
                 */
                int n = Math.Min(values.Length, symbols.Count);
                for (i = 0; i < n; i++)
                {
                    if(!dic.ContainsKey(values[i]))
                        dic.Add(values[i], symbols[i]);
                }
            }
            return new UniqueFeatureRenderer(ele.Attribute("field").Value, dic);
        }
    }
}

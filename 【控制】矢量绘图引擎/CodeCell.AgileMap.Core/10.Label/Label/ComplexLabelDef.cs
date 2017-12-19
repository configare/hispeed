using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class ComplexLabelDef : LabelDef,IDisposable, IFormattable, IFieldNamesProvider, ILabelDef
    {
        private Dictionary<LogicExpressTuple[], ISymbol> _symbols = new Dictionary<LogicExpressTuple[], ISymbol>();
        private ISymbol _defaultSymbol = null;

        public ComplexLabelDef()
        { 
        }

        public ComplexLabelDef(Dictionary<LogicExpressTuple[], ISymbol> symbols)
        {
            _symbols = symbols;
        }

        public ISymbol DefaultSybol
        {
            get { return _defaultSymbol; }
            set { _defaultSymbol = value; }
        }

        public ISymbol GetSymbol(Feature feature)
        {
            if (_symbols == null || _symbols.Count == 0 || feature == null)
                return _defaultSymbol;
            string fldValue = feature.GetFieldValue(_fieldname);
            foreach (LogicExpressTuple[] keys in _symbols.Keys)
            {
                int okCount = 0;
                foreach (LogicExpressTuple key in keys)
                {
                    if (key.IsTrue(fldValue))
                        okCount++;
                }
                if (okCount == keys.Length)
                    return _symbols[keys];
            }
            return _defaultSymbol;
        }
    }
}
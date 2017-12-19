using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CodeCell.AgileMap.Core
{
    public interface ICompositeSymbol
    {
        List<ISymbol> Symbols { get; }
    }

    [Serializable]
    public class CompositeSymbol:Symbol,ICompositeSymbol,IPersistable
    {
        protected List<ISymbol> _symbols = new List<ISymbol>();

        public CompositeSymbol()
        { 
        }

        public CompositeSymbol(ISymbol[] symbols)
        {
            if (_symbols == null)
                _symbols = new List<ISymbol>();
            _symbols.AddRange(symbols);
        }

        #region ICompositeSymbol Members

        public List<ISymbol> Symbols
        {
            get { return _symbols; }
        }

        #endregion

        public override void Draw(Graphics g, GraphicsPath path)
        {
            SmoothingMode oldModel = g.SmoothingMode;
            try
            {
                //g.SmoothingMode = SmoothingMode.HighQuality;
                if (_symbols == null || _symbols.Count == 0)
                    return;
                foreach (ISymbol sym in _symbols)
                    sym.Draw(g, path);
            }
            finally
            {
                g.SmoothingMode = oldModel;
            }
        }

        public override void Draw(Graphics g, PointF point)
        {
           SmoothingMode oldModel = g.SmoothingMode;
           try
           {
               g.SmoothingMode = SmoothingMode.HighQuality;
               if (_symbols == null || _symbols.Count == 0)
                   return;
               foreach (ISymbol sym in _symbols)
                   sym.Draw(g, point);
           }
           finally 
           {
               g.SmoothingMode = oldModel;
           }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_symbols.Count > 0)
            {
                foreach (ISymbol sym in _symbols)
                    sym.Dispose();
                _symbols = null;
            }
        }

        public override PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("Symbol");
            obj.AddAttribute("type", this.GetType().ToString());
            //symbols
            if (_symbols != null)
            {
                PersistObject symsObj = new PersistObject("CompositeSymbols");
                obj.AddSubNode(symsObj);
                foreach (ISymbol sym in _symbols)
                    symsObj.AddSubNode((sym as IPersistable).ToPersistObject());
            }
            return obj;
        }
    }
}

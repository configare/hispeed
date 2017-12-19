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
    public class SimpleTwoStepFeatureRenderer:BaseFeatureRenderer, IFeatureRenderer,IFeatureTwoStepRenderer
    {
        private enumTwoStepType _stepType = enumTwoStepType.Outline;

        public SimpleTwoStepFeatureRenderer(ISymbol symbol)
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
            return new SimpleTwoStepFeatureRenderer(symbol);
        }

        #region IFeatureTwoStepRenderer Members

        [Browsable(false)]
        public enumTwoStepType StepType
        {
            get { return _stepType; }
            set {_stepType = value;}
        }

        #endregion

        protected override void DrawPathUseCurrentSymbol(Graphics g)
        {
            switch (_stepType)
            { 
                case enumTwoStepType.Outline:
                    (_currentSymbol as ITwoStepDrawSymbol).DrawOutline(g, _path);
                    break;
                case enumTwoStepType.Fill:
                    (_currentSymbol as ITwoStepDrawSymbol).Fill(g, _path);
                    break;
            }
        }
    }
}

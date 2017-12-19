using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.CA
{
    public class SelectableColorArgItem
    {
        private enumSelectableColor _targetColor = enumSelectableColor.Red;
        private int _cyanAdjustValue = 0;
        private int _magentaAdjustValue = 0;
        private int _yellowAdjustValue = 0;
        private int _blackAdjustValue = 0;

        public SelectableColorArgItem(enumSelectableColor color)
        {
            _targetColor = color;
        }

        public enumSelectableColor TargetColor
        {
            get { return _targetColor; }
        }

        public int CyanAdjustValue
        {
            get { return _cyanAdjustValue; }
            set { _cyanAdjustValue = value; }
        }

        public int MagentaAdjustValue
        {
            get { return _magentaAdjustValue; }
            set { _magentaAdjustValue = value; }
        }

        public int YellowAdjustValue
        {
            get { return _yellowAdjustValue; }
            set { _yellowAdjustValue = value; }
        }

        public int BlackAdjustValue
        {
            get { return _blackAdjustValue; }
            set { _blackAdjustValue = value; }
        }

        public bool IsEmpty()
        {
            return _cyanAdjustValue == 0 && _magentaAdjustValue == 0 && _yellowAdjustValue == 0 && _blackAdjustValue == 0;
        }

        public void SetEmpty()
        {
            _cyanAdjustValue = 0;
            _magentaAdjustValue = 0;
            _yellowAdjustValue = 0;
            _blackAdjustValue = 0;
        }

        public SelectableColorArgItem Clone()
        {
            SelectableColorArgItem it = new SelectableColorArgItem(_targetColor);
            it.BlackAdjustValue = _blackAdjustValue;
            it.CyanAdjustValue = _cyanAdjustValue;
            it.MagentaAdjustValue = _magentaAdjustValue;
            it.YellowAdjustValue = _yellowAdjustValue;
            return it;
        }

    }
}

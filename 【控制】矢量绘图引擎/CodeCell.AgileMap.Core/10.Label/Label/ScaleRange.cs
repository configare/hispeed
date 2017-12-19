using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class ScaleRange:IPersistable
    {
        protected bool _enabled = false;
        //开始显示标注的区间,最小比例尺
        protected int _displayScaleOfMin = -1;  //1 :  _displayScaleOfMin
        //开始显示标注的区间,最大比例尺
        protected int _displayScaleOfMax = 1; // 1 : _displayScaleOfMax

        public ScaleRange(int displayScaleOfMin, int displayScaleOfMax)
        {
            _displayScaleOfMin = displayScaleOfMin;
            _displayScaleOfMax = displayScaleOfMax;
        }

        public ScaleRange(int displayScaleOfMin, int displayScaleOfMax,bool isEnable)
        {
            _displayScaleOfMin = displayScaleOfMin;
            _displayScaleOfMax = displayScaleOfMax;
            _enabled = isEnable;
        }

        [DisplayName("是否启用")]
        public bool Enable
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        [DisplayName("最小比例 1:"), Category("按比例显示")]
        public int DisplayScaleOfMin
        {
            get
            {
                return _displayScaleOfMin;
            }
            set
            {
                if (value == 0)
                    return;
                _displayScaleOfMin = value; 
            }
        }

        [DisplayName("最大比例 1:"), Category("按比例显示")]
        public int DisplayScaleOfMax
        {
            get { return _displayScaleOfMax; }
            set
            {
                if (value == 0)
                    return;
                _displayScaleOfMax = value;
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("DisplayScaleRange");
            obj.AddAttribute("enabled", _enabled.ToString());
            obj.AddAttribute("minscale", _displayScaleOfMin.ToString());
            obj.AddAttribute("maxscale", _displayScaleOfMax.ToString());
            return obj;
        }

        #endregion

        public static ScaleRange FromXElement(XElement ele)
        {
            if (ele == null)
                return new ScaleRange(-1, 1);
            ScaleRange range = new ScaleRange(-1,1);
            range.DisplayScaleOfMax = int.Parse(ele.Attribute("maxscale").Value);
            range.DisplayScaleOfMin = int.Parse(ele.Attribute("minscale").Value);
            range.Enable = bool.Parse(ele.Attribute("enabled").Value);
            return range;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class GridDefinition:IPersistable
    {
        private float _spanX = 360f;
        private float _spanY = 180f;
        //一个读取任务允许读多少块
        private int _maxGridsOneTimes = 4;

        public GridDefinition()
        { 
        }

        public GridDefinition(float spanX, float spanY)
        {
            _spanX = spanX;
            _spanY = spanY;
        }

        [DisplayName("网格经度方向大小")]
        public float SpanX 
        {
            get { return _spanX; }
            set { _spanX = value; }
        }

        [DisplayName("网格纬度方向大小")]
        public float SpanY
        {
            get { return _spanY; }
            set { _spanY = value; }
        }

        [DisplayName("同时读取网格数")]
        public int MaxGridsOneTimes
        {
            get { return _maxGridsOneTimes; }
            set { _maxGridsOneTimes = value; }
        }

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("GridDef");
            obj.AddAttribute("spanx", _spanX.ToString());
            obj.AddAttribute("spany", _spanY.ToString());
            obj.AddAttribute("gridsonetimes", _maxGridsOneTimes.ToString());
            return obj;
        }

        #endregion

        public static GridDefinition FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            int gridCount = 4;
            if (ele.Attribute("gridsonetimes") != null)
                gridCount = int.Parse(ele.Attribute("gridsonetimes").Value);
            GridDefinition gf = new GridDefinition(float.Parse(ele.Attribute("spanx").Value),
                                                    float.Parse(ele.Attribute("spany").Value));
            gf.MaxGridsOneTimes = gridCount;
            return gf;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}

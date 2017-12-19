using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

namespace CodeCell.AgileMap.Core
{
    public class ArgOfLeveling:IPersistable
    {
        private bool _enabled = true;
        private int _gridSize = 80; //pixels
        private int _beginLevel = 0; //0 - 15

        public ArgOfLeveling()
        { 
        }

        public ArgOfLeveling(bool enabled)
        {
            _enabled = enabled;
        }

        public ArgOfLeveling(bool enabled, int gridSize, int beginLevel)
        {
            _enabled = enabled;
            _gridSize = gridSize;
            _beginLevel = beginLevel;
        }

        [DisplayName("启动(读取时)")]
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        [DisplayName("网格大小(像素)")]
        public int GridSize
        {
            get { return _gridSize; }
            set 
            {
                if (value > 5)
                {
                    _gridSize = value;
                }
            }
        }

        [DisplayName("开始级别(0-15)")]
        public int BeginLevel
        {
            get { return _beginLevel; }
            set
            {
                if (value >= 0 && value <= 15)
                {
                    _beginLevel = value;
                }
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("ArgsOfLeveling");
            obj.AddAttribute("enabled", _enabled.ToString());
            obj.AddAttribute("gridsize", _gridSize.ToString());
            obj.AddAttribute("beginlevel", _beginLevel.ToString());
            return obj;
        }

        #endregion

        public static ArgOfLeveling FromXElement(XElement ele)
        {
            if (ele == null)
                return new ArgOfLeveling(false);
            ArgOfLeveling arg = new ArgOfLeveling();
            arg.Enabled = bool.Parse(ele.Attribute("enabled").Value);
            arg.GridSize = int.Parse(ele.Attribute("gridsize").Value);
            arg.BeginLevel = int.Parse(ele.Attribute("beginlevel").Value);
            return arg;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml.Linq;

namespace CodeCell.AgileMap.Core
{
    public class ConflictDefinition:IPersistable
    {
        private Size _gridSize = new Size(20, 20);
        private bool _enabled = true;

        public ConflictDefinition()
        { 
        }

        public ConflictDefinition(Size gridSize,bool enabled)
        {
            _gridSize = gridSize;
            _enabled = enabled;
        }

        [DisplayName("是否启用")]
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        [DisplayName("网格大小")]
        public Size GridSize
        {
            get { return _gridSize; }
            set { _gridSize = value; }
        }

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("ConflictDefinition");
            obj.AddAttribute("gridsize", _gridSize.Width.ToString() + "," + _gridSize.Height.ToString());
            obj.AddAttribute("enabled", _enabled.ToString());
            return obj;
        }

        #endregion

        public static ConflictDefinition FromXElement(XElement ele)
        {
            if (ele == null)
                return new ConflictDefinition();
            string sizestr = ele.Attribute("gridsize").Value;
            string[] sizestrs = sizestr.Split(',');
            ConflictDefinition def = new ConflictDefinition();
            def.GridSize = new Size(int.Parse(sizestrs[0]), int.Parse(sizestrs[1]));
            def.Enabled = bool.Parse(ele.Attribute("enabled").Value);
            return def;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Components
{
    public class ContextOprItem
    {
        private string _name = null;
        private Image _image = null;
        private enumContextKeys _enumKey = enumContextKeys.None;

        public ContextOprItem(string name, enumContextKeys enumKey)
        {
            _name = name;
            _enumKey = enumKey;
        }

        public ContextOprItem(string name, Image image, enumContextKeys enumKey)
            :this(name,enumKey)
        {
            _image = image;
        }

        public string Name
        {
            get { return _name != null ? _name : string.Empty; }
        }

        public Image Image
        {
            get { return _image; }
        }

        public enumContextKeys EnumKey
        {
            get { return _enumKey; }
        }
    }
}

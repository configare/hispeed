using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.Bricks.UIs
{
    public class HierItem
    {
        private Image _image = null;
        private string _title = null;
        private List<HierItem> _children = new List<HierItem>();
        private bool _isHotlink = true;
        internal bool _isclosed = false;
        internal RectangleF _imgBounds = RectangleF.Empty;
        internal RectangleF _titleBounds = RectangleF.Empty;
        private int _imageIndex = -1;
        private object _tag = null;
        private Dictionary<string, string> _properties = new Dictionary<string, string>();

        public HierItem(string title)
        {
            _title = title;
        }

        public HierItem(string title, Image image)
            :this(title)
        {
            _image = image;
        }

        public HierItem(string title, Image image, HierItem[] hierItems)
            : this(title, image)
        {
            if(hierItems != null && hierItems.Length>0)
                _children = new List<HierItem>(hierItems);
        }

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public string Title
        {
            get { return _title != null ?_title : string.Empty; }
            set { _title = value; }
        }

        public Image Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public int ChildCount 
        {
            get { return _children != null ? _children.Count : 0; }
        }

        public List<HierItem> Children
        {
            get { return _children; }
        }

        public bool IsHotlink
        {
            get { return _isHotlink; }
            set { _isHotlink = true; }
        }

        public int ImageIndex
        {
            get { return _imageIndex; }
            set { _imageIndex = value; }
        }

        public Dictionary<string, string> Properties
        {
            get { return _properties; }
        }
    }
}

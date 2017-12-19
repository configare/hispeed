using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    //<TabItem name="" text="" page="" font="" image="" imagealignment="" textalignment=""/>
    public class UITabItem:UICommand
    {
        private string _page;
        private string _font;
        private string _image;
        private string _selected;
        private string _textImageRelation;
        private string _argument;
        private string _textAligment;
        private string _imageAligment;

        public UITabItem()
        { }

        public UITabItem(string name, string text, string page, string image)
            : base(name, text)
        {
            _page = page;
            _image = image;
        }

        public string Page
        {
            get { return _page; }
            set { _page = value; }
        }

        public string Font
        {
            get { return _font; }
            set { _font = value; }
        }

        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public string Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }

        public string TextAligment
        {
            get { return _textAligment; }
            set { _textAligment = value; }
        }

        public string ImageAligment
        {
            get { return _imageAligment; }
            set { _imageAligment = value; }
        }

        public string TextImageRelation
        {
            get { return _textImageRelation; }
            set { _textImageRelation = value; }
        }

        public string Argument
        {
            get { return _argument; }
            set { _argument = value; }
        }
    }
}

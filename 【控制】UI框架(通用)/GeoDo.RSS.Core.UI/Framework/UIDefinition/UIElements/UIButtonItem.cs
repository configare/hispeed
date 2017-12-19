using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    // <ButtonItem name="" text="" image="" font="" identify="" textimagerelation=""/>
    public class UIButtonItem:UICommand
    {
        private string _image;
        private int _identify;
        private string _font;
        protected string _textAligment;
        protected string _imageAligment;
        private string _argument;

        public UIButtonItem()
        { }

        public UIButtonItem(string name, string text, int identify, string image)
            : base(name, text)
        {
            _identify = identify;
            _image = image;
        }

        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public int Identify
        {
            get { return _identify; }
            set { _identify = value; }
        }

        public string Font
        {
            get { return _font; }
            set { _font = value; }
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

        public string Argument
        {
            get { return _argument; }
            set { _argument = value; }
        }

    }
}

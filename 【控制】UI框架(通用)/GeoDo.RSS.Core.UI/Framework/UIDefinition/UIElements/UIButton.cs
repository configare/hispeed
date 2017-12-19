using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    // <UIButton name="" text="" image="" identify="" imagealignment="" textalignment=""/>
    public class UIButton:UICommand
    {
        protected string _image;
        protected int _identify;
        protected string _argument;
        protected string _textAligment;
        protected string _imageAligment;
        protected string _textImageRelation;

        public UIButton()
        { }

        public UIButton(string name, string text, int identify, string image)
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

        public string TextImageRelation
        {
            get { return _textImageRelation; }
            set { _textImageRelation = value; }
        }
    }
}

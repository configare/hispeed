using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    /*
     * <UIDropDownButon name="toscale" text="10%" textaligment="" dropdowndirection="" arrowpositon="" expandarrowbutton="false">
     */
    public class UIDropDownButton:UICommand
    {
        private string _textAligment = null;
        private string _image = null;
        private string _imageAligment = null;
        private string _dropDownDirection = null;
        private string _arrowPostion = null;
        private bool _expandArrowButton = false;
        private UIItem[] _menuItems = null;
        private string _font = null;

        public UIDropDownButton()
        { 
        }

        public UIDropDownButton(string text,string textAligment, string dropDownDirection, string arrowPosition, bool expandArrowButton)
            : base(text,text)
        {
            _textAligment = textAligment;
            _dropDownDirection = dropDownDirection;
            _arrowPostion = arrowPosition;
            _expandArrowButton = expandArrowButton;
        }

        public string Image
        {
            get { return _image; }
            set { _image = value; }
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

        public string DropDownDirection
        {
            get { return _dropDownDirection; }
            set { _dropDownDirection = value; }
        }

        public string ArrowPosition
        {
            get { return _arrowPostion; }
            set { _arrowPostion = value; }
        }

        public bool ExpandArrowButton
        {
            get { return _expandArrowButton; }
            set { _expandArrowButton = value; }
        }

        public UIItem[] MenuItems
        {
            get { return _menuItems; }
            set { _menuItems = value; }
        }

        public string Font
        {
            get { return _font; }
            set { _font = value; }
        }
    }
}

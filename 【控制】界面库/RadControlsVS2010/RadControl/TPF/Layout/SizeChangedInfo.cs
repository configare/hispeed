using System;
using System.Drawing;
using Telerik.WinControls;

namespace Telerik.WinControls.Layouts
{
    public class SizeChangedInfo
    {
        public SizeChangedInfo(RadElement element, Size previousSize, bool widthChanged, bool heightChanged)
        {
            this._element = element;
            this._previousSize = previousSize;
            this._widthChanged = widthChanged;
            this._heightChanged = heightChanged;
        }

        internal void Update(bool widthChanged, bool heightChanged)
        {
            this._widthChanged |= widthChanged;
            this._heightChanged |= heightChanged;
        }


        internal RadElement Element
        {
            get
            {
                return this._element;
            }
        }

        public bool HeightChanged
        {
            get
            {
                return this._heightChanged;
            }
        }

        /*public Size NewSize
        {
            get
            {
                return this._element.RenderSize;
            }
        }*/

        public Size PreviousSize
        {
            get
            {
                return this._previousSize;
            }
        }

        public bool WidthChanged
        {
            get
            {
                return this._widthChanged;
            }
        }


        private RadElement _element;
        private bool _heightChanged;
        private Size _previousSize;
        private bool _widthChanged;
        internal SizeChangedInfo Next;
    }
}


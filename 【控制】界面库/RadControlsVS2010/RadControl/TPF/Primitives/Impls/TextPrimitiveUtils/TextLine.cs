using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Text;
using Telerik.WinControls.Paint;
using System.Globalization;
using Telerik.WinControls.UI;
using System.ComponentModel;

namespace Telerik.WinControls.TextPrimitiveUtils
{
    public class TextLine
    {
        //members
        private List<FormattedText> list;
        private SizeF lineSize;
        private float baseLine;       

        //default constructor
        public TextLine()
        {
            this.list = new List<FormattedText>();
            this.lineSize = SizeF.Empty;
        }

        //properties

        //get ot sets the Size of the line
        public SizeF LineSize
        {
            get { return lineSize; }
            set { lineSize = value; }
        }

        /// <summary>
        /// BaseLine Property
        /// </summary>
        public float BaseLine
        {
            get
            {
                return baseLine;
            }
            set
            {
                baseLine = value;
            }
        }

        //get ot set the list of FormattedText objects
        public List<FormattedText> List
        {
            get { return list; }
            set { list = value; }
        }       

        public ContentAlignment GetLineAligment()
        {
            if (list != null && list.Count > 0)
                return list[0].ContentAlignment;
            return ContentAlignment.MiddleLeft;
        }

        public float GetLineOffset()
        {
            if (list != null && list.Count > 0)
                return list[0].OffsetSize;
            return 0f;
        }
    }
}

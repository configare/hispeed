using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Enumerations;

namespace Telerik.WinControls.Primitives
{
    public interface IImageElement
    {
        #region Properties

        Image Image
        {
            get;
            set;
        }

        int ImageIndex
        {
            get;
            set;
        }

        string ImageKey
        {
            get;
            set;
        }

        #endregion
    }
}

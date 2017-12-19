using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public enum RadPropertyGridBeginEditModes
    {
        /// <summary>
        /// Editing begins when the cell receives focus.
        /// </summary>
        BeginEditOnClick = 0,

        /// <summary>
        /// Editing begins when a focused cell is clicked again.
        /// </summary>
        BeginEditOnSecondClick = 1,
     
        /// <summary>
        /// Editing begins only when the RadGridView.BeginEdit(System.Boolean) method is called.
        /// </summary>
        BeginEditProgrammatically = 4,    
    }
}

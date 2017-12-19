using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Design;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a DropDownList editor element.
    /// </summary>
    [RadToolboxItem(false)]
    public class BaseDropDownListEditorElement : RadDropDownListElement
    {
        #region Properties

        protected override Type ThemeEffectiveType
        {
            get { return typeof(RadDropDownListElement); }
        }

        #endregion

        #region Initialization

        public BaseDropDownListEditorElement()
        {
            this.PopupForm.FadeAnimationType = FadeAnimationType.None;
            this.DefaultSize = new System.Drawing.Size(150, 20);
            this.Alignment = ContentAlignment.MiddleCenter;
            this.StretchVertically = false;
        }

        #endregion

        #region Events

        public event KeyEventHandler HandleKeyDown;

        protected override void ProcessKeyDown(object sender, KeyEventArgs e)
        {
            if (HandleKeyDown != null)
            {
                HandleKeyDown(sender, e);
                if (e.Handled == true)
                {
                    return;
                }
            }
            base.ProcessKeyDown(sender, e);
        }

        public event KeyEventHandler HandleKeyUp;

        protected override void ProcessKeyUp(object sender, KeyEventArgs e)
        {
            if (HandleKeyUp != null)
            {
                HandleKeyUp(sender, e);
                if (e.Handled == true)
                {
                    return;
                }
            }
            base.ProcessKeyUp(sender, e);
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            availableSize.Width = Math.Min(availableSize.Width, this.DefaultSize.Width);
            SizeF desiredSize = base.MeasureOverride(availableSize);
            if (desiredSize.Width < availableSize.Width &&
                desiredSize.Width < this.DefaultSize.Width)
            {
                desiredSize.Width = availableSize.Width;
            }
            return desiredSize;
        }

        #endregion
    }
}

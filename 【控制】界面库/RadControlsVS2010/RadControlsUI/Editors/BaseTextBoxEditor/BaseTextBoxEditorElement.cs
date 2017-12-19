using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a text box editor element.
    /// </summary>  
    public class BaseTextBoxEditorElement : RadTextBoxElement
    {
        #region Initalization

        static BaseTextBoxEditorElement()
        {
            TextProperty.OverrideMetadata(typeof(BaseTextBoxEditorElement), new RadElementPropertyMetadata(string.Empty, ElementPropertyOptions.Cancelable));
        }

        public BaseTextBoxEditorElement()
        {
            this.TextBoxItem.RouteMessages = false;
            this.DefaultSize = new System.Drawing.Size(150, 20);
            this.Alignment = ContentAlignment.MiddleCenter;
        }

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadTextBoxElement);
            }
        }

        #endregion

        #region Properties

        public bool IsCaretAtFirstLine
        {
            get
            {
                int currentLine = (int)NativeMethods.SendMessage(this.TextBoxItem.HostedControl.Handle, NativeMethods.EM_LINEFROMCHAR, -1, 0);
                if (currentLine == this.TextBoxItem.GetLineFromCharIndex(0))
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsCaretAtLastLine
        {
            get
            {
                int currentLine = (int)NativeMethods.SendMessage(this.TextBoxItem.HostedControl.Handle, NativeMethods.EM_LINEFROMCHAR, -1, 0);
                if (currentLine == this.TextBoxItem.GetLineFromCharIndex(this.Text.Length - 1))
                {
                    return true;
                }
                return false;
            }
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

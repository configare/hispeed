using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a date time editor element used in RadDateTimeEditor
    /// </summary>
    public class BaseDateTimeEditorElement : RadDateTimePickerElement
    {
        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.TextBoxElement.TextBoxItem.RouteMessages = false;
            this.DefaultSize = new System.Drawing.Size(150, 20);
            this.Alignment = ContentAlignment.MiddleCenter;
        }

        protected override void DisposeManagedResources()
        {
            IDisposable disposable = this.DefaultBehavior as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
            base.DisposeManagedResources();
        }

        protected override Type ThemeEffectiveType
        {
            get { return typeof(RadDateTimePickerElement); }
        }

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

using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class RadApplicationMenuDropDownMenuElement : RadDropDownMenuElement
    {
        #region Fields

        private float? cachedVerticalClampMargin = null;
        private float? cachedHorizontalClampMargin = null;

        #endregion

        #region Methods

        protected override System.Drawing.SizeF ApplySizeConstraints(System.Drawing.SizeF desiredSize)
        {
            RadApplicationMenuDropDownElement dropDownElement = this.FindAncestor<RadApplicationMenuDropDownElement>();

            if (dropDownElement == null)
                return desiredSize;

            SizeF dropDownElementSize = dropDownElement.DesiredSize;
            SizeF clampedSize = base.ApplySizeConstraints(dropDownElement.DesiredSize.ToSize());

            float clampMargin = dropDownElementSize.Height - clampedSize.Height;
            float clampedWidth = 0;
            float clampedHeight = 0;

            if (!this.cachedVerticalClampMargin.HasValue || clampMargin > 0)
            {
                this.cachedVerticalClampMargin = clampMargin;
            }

            clampMargin = dropDownElementSize.Width - clampedSize.Width;

            if (!this.cachedHorizontalClampMargin.HasValue || clampMargin > 0)
            {
                this.cachedHorizontalClampMargin = clampMargin;
            }

            clampedWidth = desiredSize.Width - cachedHorizontalClampMargin.Value;
            clampedHeight = desiredSize.Height - cachedVerticalClampMargin.Value;
            return new SizeF(clampedWidth, clampedHeight);
        }

        #endregion

    }
}

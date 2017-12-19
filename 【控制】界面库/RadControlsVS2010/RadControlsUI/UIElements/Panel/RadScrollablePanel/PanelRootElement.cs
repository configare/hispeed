using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the root element
    ///  of a <see cref="Telerik.WinControls.UI.RadPanel"/> control.
    /// </summary>
    public class PanelRootElement : RootRadElement
    {
        #region Overrides

        protected override ValueUpdateResult SetValueCore(RadPropertyValue propVal, object propModifier, object newValue, ValueSource source)
        {
            if (propVal.Property == RadElement.ShapeProperty)
            {
                if (newValue != null && !(newValue is RoundRectShape))
                {
                    return ValueUpdateResult.Canceled;
                }
            }

            return base.SetValueCore(propVal, propModifier, newValue, source);
        }

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RootRadElement);
            }
        }

       
        #endregion
    }
}

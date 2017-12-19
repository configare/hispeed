using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a simple button within a RadPageViewElement.
    /// </summary>
    public class RadPageViewButtonElement : RadPageViewElementBase
    {
        #region Initialization

        static RadPageViewButtonElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadPageViewButtonElementStateManager(), typeof(RadPageViewButtonElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.ImageAlignment = ContentAlignment.MiddleCenter;
        }

        #endregion

        #region Overrides

        protected override void DoDoubleClick(EventArgs e)
        {
            //double click is not defined for a button, raise click instead
            base.DoClick(e);
        }

        #endregion
    }
}

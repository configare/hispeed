using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class RadPageViewContentAreaElement : RadPageViewElementBase
    {
        #region Fields

        private RadPageViewElement owner;

        #endregion

        #region Constructor/Initializers

        public RadPageViewContentAreaElement()
        {
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.MinSize = new Size(24, 24);
            this.Padding = new Padding(4);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the RadPageViewElement instance that owns this instance.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadPageViewElement Owner
        {
            get
            {
                return this.owner;
            }
            internal set
            {
                this.owner = value;
            }
        }

        #endregion

        #region Overrides

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (this.owner == null || this.ElementState != ElementState.Loaded)
            {
                return;
            }

            if (e.Property == PaddingProperty || e.Property == BoundsProperty)
            {
                this.owner.OnContentBoundsChanged();
            }
        }

        #endregion
    }
}

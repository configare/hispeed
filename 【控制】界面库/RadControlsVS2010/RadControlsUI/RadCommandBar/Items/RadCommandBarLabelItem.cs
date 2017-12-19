using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Telerik.WinControls.Styles;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a label in <see cref="CommandBarStripElement"/>.
    /// </summary>
    public class CommandBarLabel : RadCommandBarBaseItem
    {
        #region Static memnbers

        static CommandBarLabel()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ItemStateManagerFactory(), typeof(CommandBarLabel));
        }

        #endregion

        #region Fields

        private bool useVerticalText;
        private string savedText;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether the orientation of the text should be affected by its parent's orientation.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets whether the orientation of the text should be affected by its parent's orientation.")]
        [Obsolete("This property is obsolete. Use the InheritsParentOrientation property instead")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool UseVerticalText
        {
            get
            {
                return useVerticalText;
            }
            set
            {
                useVerticalText = value;
            }

        }

        #endregion
 
        #region Overrides

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.DrawText = true;
        }

        protected override void OnOrientationChanged(EventArgs e)
        {
            if (this.orientation == System.Windows.Forms.Orientation.Vertical)
            {
                this.SetDefaultValueOverride(TextImageRelationProperty, System.Windows.Forms.TextImageRelation.ImageAboveText);
            }
            else
            {
                this.SetDefaultValueOverride(TextImageRelationProperty, System.Windows.Forms.TextImageRelation.ImageBeforeText);
            }

            this.SetDefaultValueOverride(TextOrientationProperty, this.orientation);
            this.InvalidateMeasure(true);

            base.OnOrientationChanged(e);
        }

        #endregion
    }
}

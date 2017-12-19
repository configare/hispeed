using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Telerik.WinControls.Styles;
using System.ComponentModel;
using Telerik.WinControls.UI.Properties;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a button in <see cref="CommandBarStripElement"/>.
    /// </summary>
    [DefaultEvent("Click")]
    public class CommandBarButton : RadCommandBarBaseItem
    {
        #region Static members

        static CommandBarButton()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ItemStateManagerFactory(), typeof(CommandBarButton));
        }

        #endregion 

        #region Cstors

        public CommandBarButton()
        {
             this.Image = Resources.DefaultButton;
             
        }

        #endregion

        #region Properties
        #endregion

        #region Overrides

        protected override void CreateChildElements()
        {            
            base.CreateChildElements();
            this.DrawText = false;
            this.Image = Resources.DefaultButton;
        }

        protected override void  OnOrientationChanged(EventArgs e)
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

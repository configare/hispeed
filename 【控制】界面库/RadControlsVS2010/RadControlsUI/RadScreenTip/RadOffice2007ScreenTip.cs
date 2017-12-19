using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.ComponentModel;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using Telerik.WinControls.Themes.Design;

namespace Telerik.WinControls.UI
{
	[RadThemeDesignerData(typeof(RadOffice2007ScreenTipDesignTimeData))]
    [ToolboxItem(false)]
	public class RadOffice2007ScreenTip : RadScreenTip
	{
        private RadOffice2007ScreenTipElement tipElement;

		public RadOffice2007ScreenTip() 
		{
		}

		public override Type TemplateType
		{
			get
			{
				return typeof(RadOffice2007ScreenTipElement);
			}
			set 
			{ 
			}
		}



        /// <summary>
        /// Gets the instance of RadScreenTipElement wrapped by this control. RadScreenTipElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadScreenTip.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override RadScreenTipElement ScreenTipElement
        {
            get
            {
                return this.tipElement;
            }
        }

		public bool CaptionVisible
		{
			get
			{
				return this.tipElement.CaptionVisible;
			}
			set
			{
				this.tipElement.CaptionVisible = value;
			}
		}

		public bool FooterVisible
		{
			get
			{
				return this.tipElement.FooterVisible;
			}
			set
			{
				this.tipElement.FooterVisible = value;
			}
		}

		protected override void CreateChildItems(RadElement parent)
        {
            this.tipElement = new RadOffice2007ScreenTipElement();
            this.RootElement.Children.Add(this.tipElement);
        }
	}
}

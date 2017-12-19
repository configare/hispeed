using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Telerik.WinControls
{
	internal class RadScreenTipPlaceholder : RadScreenTip
	{
		private Type templateType;
        private RadScreenTipElement content;

		public RadScreenTipPlaceholder() 
		{
            this.RootElement.ApplyShapeToControl = true;
		}

		public override Type TemplateType
		{
			get
			{
				return templateType;
			}
			set
			{
				templateType = value;
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
                return content;
            }
        }

        /// <summary>
        /// Sets the screntip element to be wrapped by this control.
        /// </summary>
        /// <param name="element">An instance of <see cref="Telerik.WinControls.RadScreenTipElement">RadScreenTipElement</see></param>
        public void SetScreenTipElement(RadScreenTipElement element)
        {
            if (element == null)
            {
                if (content != null)
                {
                    this.RootElement.Children.Remove(content);
                    content = null;
                }
            }
            else if (content != element && !this.RootElement.Children.Contains(element))
            {
                if (content != null)
                {
                    this.RootElement.Children.Remove(content);
                }
                content = element;
                this.RootElement.Children.Add(content);
            }
        }
	}
}
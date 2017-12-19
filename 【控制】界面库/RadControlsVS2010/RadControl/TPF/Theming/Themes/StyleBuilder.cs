using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Telerik.WinControls
{
    public abstract class StyleBuilder
    {
        public abstract void BuildStyle(RadElement element);
        public abstract XmlBuilderData BuilderData { get;set; }
    }

    public abstract class StyleBuilderBase : StyleBuilder
    {
        private StyleSheet style = null;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public StyleSheet Style
        {
            get
            {
                if (this.style == null)
                {
                    this.style = this.CreateDefaultStyle();
                }

                return this.style;
            }
            set
            {
                this.style = value;
            }
        }

        protected abstract StyleSheet CreateDefaultStyle();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override XmlBuilderData BuilderData
        {
            get { return new XmlStyleSheet(this.Style); }
            set 
			{
				if (value != null)
				{
					this.Style = ((XmlStyleSheet)value).GetStyleSheet();
				}
			}
        }

        public override void BuildStyle(RadElement element)
        {
			element.Style = this.Style;
        }
    }    
}

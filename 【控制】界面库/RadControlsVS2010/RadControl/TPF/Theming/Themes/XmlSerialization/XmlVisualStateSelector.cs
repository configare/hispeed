using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls
{
    public class XmlVisualStateSelector : XmlSelectorBase
    {
        private string visualState;

        public XmlVisualStateSelector()
        {
        }

        public XmlVisualStateSelector(string visualState)
        {
            this.visualState = visualState;
        }

        /// <summary>
        /// Gets or sets value corresponding to the VisualState of the item that the selector targets
        /// </summary>        
        public string VisualState
        {
            get
            {
                return this.visualState;
            }
            set
            {
                this.visualState = value;
            }
        }

        protected override IElementSelector CreateInstance()
        {
            return new VisualStateSelector(this.VisualState);
        }
    }
}

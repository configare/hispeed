using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.Themes.Design;

namespace Telerik.WinControls.UI
{
	#if RIBBONBAR
	[ToolboxItem(false)]
    internal class RadSplitButton : RadDropDownButton
	#else
	[ToolboxItem(true)]
	[Description("Provides a menu-like interface within a button")]
    public class RadSplitButton : RadDropDownButton
	#endif
	{

		//private RadSplitButtonElement splitButtonElement;

		private static readonly object DefaultItemChangedEventKey;
		
		static RadSplitButton()
		{
			DefaultItemChangedEventKey = new object();
		}	

		public RadSplitButton()
		{
		}

        /// <summary>
        /// Gets the instance of RadSplitButtonElement wrapped by this control. RadSplitButtonElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadSplitButton.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new RadSplitButtonElement DropDownButtonElement
		{
			get { return (RadSplitButtonElement)base.DropDownButtonElement; }
		}

        protected override Size DefaultSize
        {
            get
            {
                return new Size(104, 24);
            }
        }

		/// <commentsfrom cref="RadSplitButtonElement.DefaultItem" filter=""/>
		[Browsable(false)]
		public RadItem DefaultItem
		{
			get
			{
				return this.DropDownButtonElement.DefaultItem;
			}
			set
			{
				this.DropDownButtonElement.DefaultItem = value;
			}
		}

        /// <summary>
        /// Create main button element that is specific for RadSplitButton.
        /// </summary>
        /// <returns>The element that encapsulates the funtionality of RadSplitButton</returns>
        protected override RadDropDownButtonElement CreateButtonElement()
        {
            RadSplitButtonElement res = new RadSplitButtonElement();
            res.DefaultItemChanged += new EventHandler(SplitButtonElement_DefaultItemChanged);
            return res;
        }

		private void SplitButtonElement_DefaultItemChanged(object sender, EventArgs e)
		{
			this.OnDefaultItemChanged(e);
		}

		/// <commentsfrom cref="RadSplitButtonElement.DefaultItemChanged" filter=""/>
		[Browsable(true),
		Category(RadDesignCategory.ActionCategory),
		Description("Occurs when the default item is changed.")]
		public event EventHandler DefaultItemChanged
		{
			add
			{
				this.Events.AddHandler(RadSplitButton.DefaultItemChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadSplitButton.DefaultItemChangedEventKey, value);
			}
		}

		/// <commentsfrom cref="RadSplitButtonElement.OnDefaultItemChanged" filter=""/>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDefaultItemChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadSplitButton.DefaultItemChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

        ///// <summary>
        ///// Creates the children items 
        ///// </summary>
        ///// <param name="parent"></param>
        //protected override void CreateChildItems(RadElement parent)
        //{
        //    // The base child items creation is customized by overriding CreateButtonElement()
        //    base.CreateChildItems(parent);
        //}

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            Type elementType = element.GetType();

            if (elementType == typeof(RadSplitButtonElement))
                return true;

            if (elementType == typeof(RadButtonElement))
                return true;

            return false;
        }
	}
}

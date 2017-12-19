using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
	//[ToolboxItem(false), ComVisible(false)]
	[Designer(DesignerConsts.RadSplitButtonElementDesignerString)]
	public class RadSplitButtonElement : RadDropDownButtonElement
	{
		private RadItem defaultItem;
        private LightVisualElement separator;
		private static readonly object DefaultItemChangedEventKey;

		static RadSplitButtonElement()
		{
			new Themes.ControlDefault.SplitButton().DeserializeTheme();
            DefaultItemChangedEventKey = new object();
		}

        /// <summary>
        /// Get or sets the item that is activated when the button portion of the 
		/// <see cref="Telerik.WinControls.UI.RadSplitButtonElement"> RadSplitButtonElement</see> is clicked or selected and Enter is pressed.
		/// </summary>
		[Browsable(false)]
		public RadItem DefaultItem
		{
		  get 
		  {			  
			  return this.defaultItem; 
		  }
		  set 
		  {
			  if (value != this.defaultItem)
			  {
				  this.defaultItem = value;
				  this.OnDefaultItemChanged(EventArgs.Empty);
			  }
		  }
		}

        /// <summary>
        /// Get or sets the item that is separating the action part and the arrow part of the button. 
        /// </summary>
        [Browsable(false)]
        public LightVisualElement ButtonSeparator
        {
            get
            {
                return separator;
            }
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.separator = new LightVisualElement();
            this.separator.SetDefaultValueOverride(RadElement.MinSizeProperty, new System.Drawing.Size(2, 2));
            this.separator.DrawText = false;
            this.separator.SetDefaultValueOverride(LightVisualElement.DrawFillProperty, false);
            this.separator.SetDefaultValueOverride(LightVisualElement.DrawBorderProperty, true);
            this.separator.SetValue(DropDownEditorLayoutPanel.IsButtonSeparatorProperty, true);

            this.layoutPanel.Children.Add(separator);
        }

		internal override void DoOnBubbleEvent(RadElement sender, RoutedEventArgs args)
		{
            if (args.RoutedEvent == RadElement.MouseDownEvent)
            {
                this.IsMouseDown = true;
            }

            if (args.RoutedEvent == RadElement.MouseUpEvent)
            {
                this.IsMouseDown = false;
            }

			if (args.RoutedEvent == RadElement.MouseDownEvent &&
				(sender == this.ArrowButton || (sender == this.ActionButton && this.defaultItem == null)))
			{
                if (this.menu == null)
                    return;

                if (this.Items.Count > 0 && !this.menu.IsVisible)
                {
                    this.ShowDropDown();
                }
                else
                {
                    this.menu.ClosePopup(RadPopupCloseReason.Mouse);
                }
			}
			if (args.RoutedEvent == RadElement.MouseClickedEvent)
			{
				if (sender == this.ActionButton && this.defaultItem != null)
				{
					this.defaultItem.PerformClick();
                    this.DropDownMenu.ClosePopup(RadPopupCloseReason.Mouse);
				    args.Canceled = true;
					return;
				}
			}
		}

		/// <summary>
		/// Occurs when the default item is changed.
		/// </summary>
		[Browsable(true),
		Category(RadDesignCategory.ActionCategory),
	    Description("Occurs when the default item is changed.")]
		public event EventHandler DefaultItemChanged
		{
			add
			{
				this.Events.AddHandler(RadSplitButtonElement.DefaultItemChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadSplitButtonElement.DefaultItemChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the DropDownItemClicked event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDefaultItemChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadSplitButtonElement.DefaultItemChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadDropDownButtonElement);
            }
        }
	}
}

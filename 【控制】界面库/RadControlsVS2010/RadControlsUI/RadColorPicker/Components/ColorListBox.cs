using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI.RadColorPicker
{
    /// <exclude/>
    [ToolboxItem(false), ComVisible(false)]
    public class ColorListBox : RadListControl
    {
        public override string ThemeClassName
        {
            get
            {
                return typeof(RadListControl).FullName;
            }
        }

        /// <summary>
        /// Fires when the selected color has changed
        /// </summary>
        public event ColorChangedEventHandler ColorChanged;

        protected override void OnSelectedValueChanged(object sender, int newIndex, object newValue, object oldValue)
        {
            base.OnSelectedValueChanged(sender, newIndex, newValue, oldValue);
            if (ColorChanged != null && this.SelectedItem != null)
                ColorChanged(this, new ColorChangedEventArgs(Color.FromName(((RadListDataItem)this.SelectedItem).Text)));
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);
            this.ListElement.ViewElement.ElementProvider = new ColorListBoxElementProvider(this.ListElement);
            this.ListElement.ItemHeight = 24;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
     /// <summary>Represents the method that will handle the DataBindingComplete event of a RadListView and RadDropDownList</summary>
    /// <filterpriority>2</filterpriority>
    public delegate void ListBindingCompleteEventHandler(object sender, ListBindingCompleteEventArgs e);

    /// <summary>Provides data for the ListBindingCompleteEventHandler event.</summary>
    /// <filterpriority>2</filterpriority>
    public class ListBindingCompleteEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the ListBindingCompleteEventArgs class.</summary>
        /// <param name="listChangedType">One of the <see cref="T:System.ComponentModel.ListChangedType"></see> values.</param>
        public ListBindingCompleteEventArgs(ListChangedType listChangedType)
        {
            this.listChangedType = listChangedType;
        }


        /// <summary>Gets a value specifying how the list changed.</summary>
        /// <returns>One of the <see cref="T:System.ComponentModel.ListChangedType"></see> values.</returns>
        /// <filterpriority>1</filterpriority>
        public ListChangedType ListChangedType
        {
            get
            {
                return this.listChangedType;
            }
        }


        private ListChangedType listChangedType;
    }
}

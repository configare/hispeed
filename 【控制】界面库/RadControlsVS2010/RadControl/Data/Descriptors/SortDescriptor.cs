using System;
using System.ComponentModel;
using Telerik.WinControls.Interfaces;
using System.Drawing.Design;

namespace Telerik.WinControls.Data
{
    public class SortDescriptor :
        INotifyPropertyChanged,
        INotifyPropertyChangingEx
    {
        #region Fields

        private string propertyName = String.Empty;
        private ListSortDirection direction = ListSortDirection.Ascending;
        private SortDescriptorCollection owner;
        private int propertyIndex;

        #endregion

        #region Constructors

        public SortDescriptor()
        {

        }

        public SortDescriptor(string propertyName, ListSortDirection direction)
        {
            this.propertyName = propertyName;
            this.direction = direction;
        }

        public SortDescriptor(string propertyName, ListSortDirection direction, SortDescriptorCollection owner)
            : this(propertyName, direction)
        {
            this.owner = owner;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string PropertyName
        {
            get { return this.propertyName; }
            set
            {
                if (this.OnPropertyChanging("PropertyName", this.propertyName, value))
                {
                    this.propertyName = value;
                    this.OnPropertyChanged("PropertyName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        [DefaultValue(ListSortDirection.Ascending)]
        public ListSortDirection Direction
        {
            get { return this.direction; }
            set
            {
                if (this.OnPropertyChanging("Direction", this.direction, value))
                {
                    this.direction = value;
                    this.OnPropertyChanged("Direction");
                }
            }
        }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SortDescriptorCollection Owner
        {
            get
            {
                return this.owner;
            }
            internal set
            {
                this.owner = value;
            }
        }

        #endregion

        #region Implementation

        internal int PropertyIndex
        {
            get { return propertyIndex; }
            set { propertyIndex = value; }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="e">A <see cref="PropertyChangedEventArgs"/> instance containing event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        #endregion

        #region INotifyPropertyChangingEx Members

        public event PropertyChangingEventHandlerEx PropertyChanging;

        /// <summary>
        /// Raises the <see cref="E:PropertyChanging"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>Returns [TRUE] If the events is not canceled, otherwise [FALSE].</returns>
        protected virtual bool OnPropertyChanging(string propertyName, object oldValue, object newValue)
        {
            PropertyChangingEventArgsEx e = new PropertyChangingEventArgsEx(propertyName, oldValue, newValue);
            this.OnPropertyChanging(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanging"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangingEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanging(PropertyChangingEventArgsEx e)
        {
            PropertyChangingEventHandlerEx handler = this.PropertyChanging;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion
    }
}

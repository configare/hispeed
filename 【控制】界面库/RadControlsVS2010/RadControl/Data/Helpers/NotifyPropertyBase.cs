using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Interfaces;

namespace Telerik.WinControls.Data
{
    [Serializable]
    public class NotifyPropertyBase : INotifyPropertyChangingEx, INotifyPropertyChanged
    {
        #region Fields

        private PropertyChangedEventArgs tempStore = null;
        private int suspendCount = 0;

        #endregion

        #region Properties

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool IsSuspended
        {
            get { return this.suspendCount != 0; }
        }

        #endregion

        #region Public methods

        public virtual bool SuspendNotifications()
        {
            return 0 == suspendCount++;
        }

        public virtual bool ResumeNotifications(bool notifyChanges)
        {
            if (suspendCount <= 0)
            {
                return false;
            }

            bool res = 0 == --suspendCount;

            if (res && tempStore != null && notifyChanges)
                SignalPropertyChanged();

            tempStore = null;

            return res;
        }

        public bool ResumeNotifications()
        {
            return ResumeNotifications(true);
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property of an object changes. 		
        /// </summary>
        [field: NonSerialized]
        public virtual event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region INotifyPropertyChangingEx Members

        /// <summary>
        /// Occurs before a property of an object changes. 		
        /// </summary>
        [field: NonSerialized]
        public virtual event PropertyChangingEventHandlerEx PropertyChanging;

        #endregion

        #region OnPropertyChanged

        private void SignalPropertyChanged()
        {
            this.ProcessPropertyChanged(tempStore);

            if (PropertyChanged != null)
            {
                PropertyChanged(this, tempStore);
            }

            tempStore = null;
        }

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        internal void CallOnPropertyChanged(PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="e">A <see cref="PropertyChangedEventArgs"/> instance containing event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            tempStore = tempStore == null ? e : new PropertyChangedEventArgs("this");

            if (this.IsSuspended)
            {
                return;
            }

            SignalPropertyChanged();
        }

        /// <summary>
        /// This method is called right befor the <see cref="PropertyChanged"/> event is fired.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void ProcessPropertyChanged(PropertyChangedEventArgs e)
        {
        }

        #endregion

        #region OnPropertyChanging

        /// <summary>
        /// Raises the PropertyChanging event
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="originalValue"></param>
        /// <param name="value">The value that is goint to be set to the property.</param>
        protected PropertyChangingEventArgsEx OnPropertyChanging(string propertyName, object originalValue, object value)
        {
            PropertyChangingEventArgsEx ea = new PropertyChangingEventArgsEx(propertyName, originalValue, value, false);
            this.OnPropertyChanging(ea);
            return ea;
        }

        /// <summary>
        /// Raises the PropertyChanging event
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>true if the event has been canceled, for more information see <see cref="CancelEventArgs.Cancel"/></returns>
        protected bool OnPropertyChanging(string propertyName)
        {
            PropertyChangingEventArgsEx ea = new PropertyChangingEventArgsEx(propertyName, null, null, false);
            this.OnPropertyChanging(ea);
            return ea.Cancel;
        }

        /// <summary>
        /// Raises the PropertyChanging event.
        /// <b>Note:</b> This method is called even when the notifications are suspended. 
        /// </summary>
        /// <param name="e">A <see cref="PropertyChangingEventArgsEx"/> instance containing event data.</param>
        protected virtual void OnPropertyChanging(PropertyChangingEventArgsEx e)
        {
            if (this.IsSuspended)
                return;

            this.ProcessPropertyChanging(e);

            if (PropertyChanging != null)
            {
                PropertyChanging(this, e);
            }
        }

        /// <summary>
        /// This method is called right before the <see cref="PropertyChanging"/> event is fired.
        /// Note: If <see cref="IsSuspended"/> is true, this method is not called.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void ProcessPropertyChanging(PropertyChangingEventArgsEx e)
        {
        }

        #endregion

        /// <summary>
        /// General method for setting the value of the field related to the property that is modified. 
        /// This method confirms that the old and new values are different, then fires the 
        /// <see cref="PropertyChanging"/> event, then sets the given value to the supplied field, 
        /// and fires the <see cref="PropertyChanged"/> event.
        /// <b>Note:</b> If the <see cref="PropertyChanging"/> event is canceled, the last two actions are
        /// not performed.
        /// </summary>
        /// <example>
        /// <code>
        /// public class MyNotificationsTest : NotifyPropertyBase
        /// {
        ///     private int myInt = 0;
        ///     private int myInt2 = 0; //
        /// 
        ///     public int AsInt
        ///     {
        ///        get 
        ///        { 
        ///           return this.myField; 
        ///        }
        ///        set
        ///        {
        ///           if (SetProperty("AsInt", ref this.myInt, value))
        ///           {
        ///              // perform additional actions when new value is set to myInt.
        ///           }
        ///        }
        ///     }
        /// 
        ///     public int AsInt2
        ///     {
        ///        get 
        ///        { 
        ///           return (float)this.myInt2; 
        ///        }
        ///        set
        ///        {
        ///           // The following property setter is the same as the previous one.
        ///           if (this.myInt2 != value) 
        ///           {
        ///               PropertyChangingEventArgs2 ea = new PropertyChangingEventArgs2("AsInt2", value);
        ///               OnPropertyChanging(ea);
        /// 
        ///               if (!ea.Cancel)
        ///               {
        ///                  this.myInt2 = (int)ea.Value;
        ///                  OnPropertyChanged("AsInt2");
        /// 
        ///                  // perform additional actions when new value is set to myInt2.
        ///               }
        ///            }
        ///        }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// The two setter implementations are identical. If you require to perform some actions before
        /// the <see cref="PropertyChanged"/> event is fired, you can use the second implementation, or,
        /// a better solution is to override the <see cref="ProcessPropertyChanged"/> method and place
        /// the code there.
        /// <typeparam name="T">The type of the field that is to be modified.</typeparam>
        /// <param name="propertyName">The name of the property, that will appear as propertyName in the <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> event args.</param>
        /// <param name="propertyField">The field, that is related to the property.</param>
        /// <param name="value">The value that is to be set to the field in case the <see cref="PropertyChanging"/> event is not being <see cref="CancelEventArgs.Cancel">Canceled</see>.</param>
        /// <returns>true if new value is being set</returns>
        protected virtual bool SetProperty<T>(string propertyName, ref T propertyField, T value)
        {
            if (object.Equals(propertyField, value)) return false;

            PropertyChangingEventArgsEx ea = OnPropertyChanging(propertyName, propertyField, value);

            if (ea.Cancel || object.Equals(propertyField, ea.NewValue))
                return false;

            propertyField = (T)ea.NewValue;

            this.OnPropertyChanged(propertyName);

            return true;
        }
    }
}

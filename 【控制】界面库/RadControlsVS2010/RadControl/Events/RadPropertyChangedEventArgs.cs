using System;

namespace Telerik.WinControls
{
    //[StructLayout(LayoutKind.Sequential)]
    public class RadPropertyChangedEventArgs : EventArgs
    {
        private RadProperty _property = null;
        private RadPropertyMetadata _metadata = null;

        private object _oldValue;
        private object _newValue;

        private bool _isASubPropertyChange = false;
        private bool _isOldValueDeferred = false;
        private bool _isNewValueDeferred = false;

        private ValueSource _oldValueSource = ValueSource.Unknown;
        private ValueSource _newValueSource = ValueSource.Unknown;

        public static new readonly RadPropertyChangedEventArgs Empty = new RadPropertyChangedEventArgs();

        private RadPropertyChangedEventArgs()
        {
        }

        public RadPropertyChangedEventArgs(RadProperty property, object oldValue, object newValue)
        {
            this._property = property;
            this._oldValue = oldValue;
            this._newValue = newValue;
        }

        internal RadPropertyChangedEventArgs(RadProperty property, RadPropertyMetadata metadata, object oldValue, object newValue)
            : this(property, oldValue, newValue)
        {
            this._metadata = metadata;
        }


        internal RadPropertyChangedEventArgs(RadProperty property, RadPropertyMetadata metadata, object value)
            : this(property, metadata, value, value)
        {
            this._isASubPropertyChange = true;
        }

        internal RadPropertyChangedEventArgs(RadProperty property, RadPropertyMetadata metadata, object oldValue, object newValue, bool isOldValueDeferred, bool isNewValueDeferred, ValueSource oldValueSource, ValueSource newValueSource)
            : this(property, metadata, oldValue, newValue)
        {
            this._isOldValueDeferred = isOldValueDeferred;
            this._isNewValueDeferred = isNewValueDeferred;
            this._oldValueSource = oldValueSource;
            this._newValueSource = newValueSource;
        }

        public RadProperty Property
        {
            get { return this._property; }
        }

        public object OldValue
        {
            get
            {
                if (this._isOldValueDeferred)
                {
                    this._oldValue = ((DeferredReference)this._oldValue).GetValue();
                    this._isOldValueDeferred = false;
                }

                return this._oldValue;
            }
        }

        public object NewValue
        {
            get
            {
                if (this._isNewValueDeferred)
                {
                    this._newValue = ((DeferredReference)this._newValue).GetValue();
                    this._isNewValueDeferred = false;
                }

                return this._newValue;
            }
        }

        /*originally marked as internal*/
        internal bool IsASubPropertyChange
        {
            get { return this._isASubPropertyChange; }

        }

        public RadPropertyMetadata Metadata
        {
            get { return this._metadata; }
        }

        internal ValueSource OldValueSource
        {
            get { return this._oldValueSource; }
        }

        internal ValueSource NewValueSource
        {
            get { return this._newValueSource; }
        }

        /*end  --- originally marked as internal*/

        public override bool Equals(object obj)
        {
            return this.Equals((RadPropertyChangedEventArgs)obj);
        }

        public bool Equals(RadPropertyChangedEventArgs args)
        {
            return
                (this._property == args._property) &&
                (this._metadata == args._metadata) &&
                (this._oldValue == args._oldValue) &&
                (this._newValue == args._newValue) &&
                (this._isASubPropertyChange == args._isASubPropertyChange) &&
                (this._isOldValueDeferred == args._isOldValueDeferred) &&
                (this._isNewValueDeferred == args._isNewValueDeferred) &&
                (this._oldValueSource == args._oldValueSource) &&
                (this._newValueSource == args._newValueSource);
        }

        public static bool operator ==(RadPropertyChangedEventArgs left, RadPropertyChangedEventArgs right)
        {
            return Object.Equals(left, right);
        }

        public static bool operator !=(RadPropertyChangedEventArgs left, RadPropertyChangedEventArgs right)
        {
            return !Object.Equals(left, right);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

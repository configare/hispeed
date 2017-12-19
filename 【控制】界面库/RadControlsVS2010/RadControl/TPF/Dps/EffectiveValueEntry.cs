using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Telerik.WinControls
{
    internal class ModifiedValue
    {
        // Methods
        public ModifiedValue()
        {
        }

        internal object AnimatedValue
        {
            get
            {
                return this._animatedValue;
            }
            set
            {
                this._animatedValue = value;
            }
        }

        internal object BaseValue
        {
            get
            {
                return this._baseValue;
            }
            set
            {
                this._baseValue = value;
            }
        }

        internal object CoercedValue
        {
            get
            {
                return this._coercedValue;
            }
            set
            {
                this._coercedValue = value;
            }
        }

        internal object ExpressionValue
        {
            get
            {
                return this._expressionValue;
            }
            set
            {
                this._expressionValue = value;
            }
        }

        // Fields
        private object _animatedValue;
        private object _baseValue;
        private object _coercedValue;
        private object _expressionValue;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EffectiveValueEntry
    {
        private short _propertyIndex;
        private object _value;
        private PrivateFlags _flags;

		internal bool IsTrackingDefaultValues;
		internal bool IsTrackingDesignerValues;

        internal void SetExpressionValue(object value, object baseValue)
        {
            this.ModifiedValue.ExpressionValue = value;
            this.IsExpression = true;
        }

        internal void SetAnimatedValue(object value, object baseValue)
        {
            this.ModifiedValue.AnimatedValue = value;
            this.IsAnimated = true;
        }

        internal void SetCoercedValue(object value, object baseValue)
        {
            this.ModifiedValue.CoercedValue = value;
            this.IsCoerced = true;
        }

        public int PropertyIndex
        {
            get
            {
                return this._propertyIndex;
            }
            set
            {
                this._propertyIndex = (short)value;
            }
        }

        internal object Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        internal ValueSource ValueSource
        {
            get
            {
                return (ValueSource)((byte)(this._flags & EffectiveValueEntry.PrivateFlags.ValueSourceMask));
            }
            set
            {
                this._flags = (EffectiveValueEntry.PrivateFlags)((byte)(((byte)(this._flags & (EffectiveValueEntry.PrivateFlags.IsAnimated | (EffectiveValueEntry.PrivateFlags.IsCoerced | (EffectiveValueEntry.PrivateFlags.IsDeferredReference | EffectiveValueEntry.PrivateFlags.IsExpression))))) | ((byte)value)));
            }
        }

        internal bool IsDeferredReference
        {
            get
            {
                return this.ReadPrivateFlag(EffectiveValueEntry.PrivateFlags.IsDeferredReference);
            }
            set
            {
                this.WritePrivateFlag(EffectiveValueEntry.PrivateFlags.IsDeferredReference, value);
            }
        }

        internal bool IsExpression
        {
            get
            {
                return this.ReadPrivateFlag(EffectiveValueEntry.PrivateFlags.IsExpression);
            }
            set
            {
                this.WritePrivateFlag(EffectiveValueEntry.PrivateFlags.IsExpression, value);
            }
        }

        internal bool IsAnimated
        {
            get
            {
                return this.ReadPrivateFlag(EffectiveValueEntry.PrivateFlags.IsAnimated);
            }
            set
            {
                this.WritePrivateFlag(EffectiveValueEntry.PrivateFlags.IsAnimated, value);
            }
        }

        internal bool IsCoerced
        {
            get
            {
                return this.ReadPrivateFlag(EffectiveValueEntry.PrivateFlags.IsCoerced);
            }
            set
            {
                this.WritePrivateFlag(EffectiveValueEntry.PrivateFlags.IsCoerced, value);
            }
        }

        internal object LocalValue
        {
            get
            {
                if (this.ValueSource != ValueSource.Local)
                {
                    return RadProperty.UnsetValue;
                }
                if (!this.HasModifiers)
                {
                    return this.Value;
                }
                return ((ModifiedValue)this.Value).BaseValue;
            }
            set
            {
                if (!this.HasModifiers)
                {
                    this.Value = value;
                }
                else
                {
                    ((ModifiedValue)this.Value).BaseValue = value;
                }
            }
        }

        internal bool HasModifiers
        {
            get
            {
                return (((byte)(this._flags & (EffectiveValueEntry.PrivateFlags.IsAnimated | (EffectiveValueEntry.PrivateFlags.IsCoerced | EffectiveValueEntry.PrivateFlags.IsExpression)))) != 0);
            }
        }

        internal ModifiedValue ModifiedValue
        {
            get
            {
                ModifiedValue value1 = null;
                if (this._value == null)
                {
                    this._value = value1 = new ModifiedValue();
                    return value1;
                }
                value1 = this._value as ModifiedValue;
                if (value1 == null)
                {
                    value1 = new ModifiedValue();
                    value1.BaseValue = this._value;
                    this._value = value1;
                }
                return value1;
            }
        }

        private void WritePrivateFlag(EffectiveValueEntry.PrivateFlags bit, bool value)
        {
            if (value)
            {
                this._flags = (EffectiveValueEntry.PrivateFlags)((byte)(this._flags | bit));
            }
            else
            {
                this._flags = (EffectiveValueEntry.PrivateFlags)((byte)(this._flags & ((EffectiveValueEntry.PrivateFlags)((byte)~bit))));
            }
        }

        private bool ReadPrivateFlag(EffectiveValueEntry.PrivateFlags bit)
        {
            return (((byte)(this._flags & bit)) != 0);
        }

        // Nested Types
        private enum PrivateFlags : byte
        {
            // Fields
            IsExpression = 16,
            IsAnimated = 32,
            IsCoerced = 64,
            IsDeferredReference = 128,
            ValueSourceMask = 15
        }
    }    
}

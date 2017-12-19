using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
	public class RadElementPropertyMetadata : RadPropertyMetadata
	{
		private ElementPropertyOptions _options = ElementPropertyOptions.None;

		protected override RadPropertyMetadata CreateInstance()
		{
			return new RadElementPropertyMetadata();
		}

		public RadElementPropertyMetadata()
		{ }

		public RadElementPropertyMetadata(object defaultValue)
			: base(defaultValue)
		{
		}

		public RadElementPropertyMetadata(object defaultValue, ElementPropertyOptions options)
			: base(defaultValue)
		{
			this._options = options;			
		}

		public RadElementPropertyMetadata(object defaultValue, ElementPropertyOptions options, PropertyChangedCallback propertyChangedCallback)
			: base(defaultValue, propertyChangedCallback)
		{
			this._options = options;			
		}

		public ElementPropertyOptions PropertyOptions
		{
			get
			{
				return _options;
			}
		}

        public bool Cancelable
        {
            get
            {
                return (_options & ElementPropertyOptions.Cancelable) == ElementPropertyOptions.Cancelable;
            }
            set
            {
                if (value)
                {
                    _options |= ElementPropertyOptions.Cancelable;
                }
                else
                {
                    _options &= ~ElementPropertyOptions.Cancelable;
                }
            }
        }

		public bool AffectsDisplay
		{
			get
			{
				return (_options & ElementPropertyOptions.AffectsDisplay) == ElementPropertyOptions.AffectsDisplay;
			}
			set
			{
				if (value)
				{
					_options |= ElementPropertyOptions.AffectsDisplay;
				}
				else
				{
					_options &= ~ElementPropertyOptions.AffectsDisplay;
				}
			}
		}

		public bool AffectsLayout
		{
			get
			{
				return (_options & ElementPropertyOptions.AffectsLayout) == ElementPropertyOptions.AffectsLayout;
			}
			set
			{
				if (value)
				{
					_options |= ElementPropertyOptions.AffectsLayout;
				}
				else
				{
					_options &= ~ElementPropertyOptions.AffectsLayout;
				}
			}
		}

		public bool AffectsMeasure
		{
			get
			{
				return (_options & ElementPropertyOptions.AffectsMeasure) == ElementPropertyOptions.AffectsMeasure;
			}
			set
			{
				if (value)
				{
					_options |= ElementPropertyOptions.AffectsMeasure;
				}
				else
				{
					_options &= ~ElementPropertyOptions.AffectsMeasure;
				}
			}
		}

		public bool AffectsArrange
		{
			get
			{
				return (_options & ElementPropertyOptions.AffectsArrange) == ElementPropertyOptions.AffectsArrange;
			}
			set
			{
				if (value)
				{
					_options |= ElementPropertyOptions.AffectsArrange;
				}
				else
				{
					_options &= ~ElementPropertyOptions.AffectsArrange;
				}
			}
		}

        public bool AffectsParentMeasure
        {
            get
            {
                return (_options & ElementPropertyOptions.AffectsParentMeasure) == ElementPropertyOptions.AffectsParentMeasure;
            }
            set
            {
                if (value)
                {
                    _options |= ElementPropertyOptions.AffectsParentMeasure;
                }
                else
                {
                    _options &= ~ElementPropertyOptions.AffectsParentMeasure;
                }
            }
        }

        public bool AffectsParentArrange
        {
            get
            {
                return (_options & ElementPropertyOptions.AffectsParentArrange) == ElementPropertyOptions.AffectsParentArrange;
            }
            set
            {
                if (value)
                {
                    _options |= ElementPropertyOptions.AffectsParentArrange;
                }
                else
                {
                    _options &= ~ElementPropertyOptions.AffectsParentArrange;
                }
            }
        }

		public bool InvalidatesLayout
		{
			get
			{
				return (_options & ElementPropertyOptions.InvalidatesLayout) == ElementPropertyOptions.InvalidatesLayout;
			}
			set
			{
				if (value)
				{
					_options |= ElementPropertyOptions.InvalidatesLayout;
				}
				else
				{
					_options &= ~ElementPropertyOptions.InvalidatesLayout;
				}
			}
		}

		public bool CanInheritValue
		{
			get
			{
				return (_options & ElementPropertyOptions.CanInheritValue) == ElementPropertyOptions.CanInheritValue;
			}
			set
			{
				if (value)
				{
					_options |= ElementPropertyOptions.CanInheritValue;
				}
				else
				{
					_options &= ~ElementPropertyOptions.CanInheritValue;
				}
			}
		}

		protected override void OnApply(RadProperty dp, Type targetType)
		{
			base.IsInherited = this.CanInheritValue;
			base.OnApply(dp, targetType);
		}

		protected override void Merge(RadPropertyMetadata baseMetadata, RadProperty dp)
		{
			base.Merge(baseMetadata, dp);
            //RadElementPropertyMetadata metadata = baseMetadata as RadElementPropertyMetadata;
            //if (metadata != null)
            //{
            //    this._options = metadata._options;
            //}
		}
	}
}
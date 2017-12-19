using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Telerik.WinControls.Collections;
using System.ComponentModel;


namespace Telerik.WinControls
{
    /// <summary>Represents a base class for the HierarchicalSelector class.
    /// Selectors in telerik presentation framework are very similar to CSS 
    /// selectors.</summary>
    public abstract class SelectorBase : IElementSelector
    {
        private Condition condition = null;
        private Condition unapplyCondition = null;
        private bool isActiveSelectorInStyleBuilder;
        private bool autoUnapply = false;
        public SelectorBase ExcludeSelector;
        private bool disableStyle;
        PropertyChangeBehaviorCollection createdPropertyChangeBehaviors = null;
        RoutedEventBehaviorCollection createdRoutedEventsBehaviors = null;
        private IElementSelector childSelector;

        /// <summary>Gets or sets the Condition upon which to apply the customization.</summary>
        public Condition Condition
        {
            get 
            {
                return condition;
            }
            set
            {
                this.condition = value;
            }
        }
        
        /// <summary>Gets or sets the condition upon which to un-apply the customization.</summary>
        public Condition UnapplyCondition
        {
            get
            {
                return unapplyCondition;
            }
            set
            {
                this.unapplyCondition = value;
            }
        }

        int IElementSelector.Key 
        {
            get
            {
                return this.GetKey();
            }
        }

        protected abstract int GetKey();

        public abstract bool Equals(IElementSelector elementSelector);

        /// <summary>Gets or sets a value indicating whether auto-un-apply is on or off.</summary>
        public bool AutoUnapply
        {
            get { return autoUnapply; }
            set { autoUnapply = value; }
        }

		/// <summary> Gets or sets a value indicating whether the current selector is the active one in style builder</summary>
		public bool IsActiveSelectorInStyleBuilder
		{
			get { return isActiveSelectorInStyleBuilder; }
			set { isActiveSelectorInStyleBuilder = value; }
		}

		public bool DisableStyle
		{
			get { return disableStyle; }
			set { disableStyle = value; }
		}

        public bool CanSelectIgnoringConditions(RadElement targetElement)
        {
            return this.CanSelectOverride(targetElement);
        }

        public virtual bool CanSelect(RadElement targetElement)
        {
            return this.CanSelectOverride(targetElement) && this.CanSelectCore(targetElement);
        }

		protected virtual bool CanSelectCore(RadElement onElement)
		{
			return !DisableStyle && 
				   (IsActiveSelectorInStyleBuilder || 
					this.Condition == null || 
					this.Condition.Evaluate(onElement));
		}

        protected virtual bool CanSelectOverride(RadElement element)
        {
            return false;
        }

        /// <summary>Retrieves a value indicating whether the customization should be 
        /// un-applied to the given element..</summary>
        public virtual bool ShouldUnapply(RadElement onElement)
        {
            if (AutoUnapply)
            {
                return this.Condition != null && 
                    (! this.Condition.Evaluate(onElement));
            }
            else
            {
                return this.UnapplyCondition != null &&
                     this.UnapplyCondition.Evaluate(onElement);
            }
        }

        /// <summary>Gets a value indicating whether the an apply condition is set.</summary>
        public virtual bool HasApplyCondition
        {
            get
            {
                return this.Condition != null;
            }
        }

        /// <summary>Retrieves a value indicating whether value is set for the element.</summary>
        public bool IsValueApplied(RadElement element)
        {
            object res = element.IsStyleSelectorValueSet[this.GetHashCode()];
            if (res == null)
                return false;

            return (bool)res;
        }

		public bool IsValueUnapplied(RadElement element)
		{
            object res = element.IsStyleSelectorValueSet[this.GetHashCode()];
			if (res == null)
				return false;

			return !(bool)res;
		}

        public void Apply(RadElement element, PropertySettingCollection propertySettings)
        {
			bool shouldApply = true;
			bool shouldUnapply = false;
			bool applyConditionEvaluated = false;
			bool unApplyConditionEvaluated = false;

			if (this.condition != null || this.unapplyCondition != null)
			{
				applyConditionEvaluated = CanSelect(element);

				shouldApply = applyConditionEvaluated &&
								   (!this.IsValueApplied(element) || this.IsValueUnapplied(element));
				if (!shouldApply)
				{
					unApplyConditionEvaluated = ShouldUnapply(element);
					shouldUnapply = ShouldUnapply(element) && this.IsValueApplied(element);
				}
			}

			if (shouldApply)
			{
				foreach(IPropertySetting setting in propertySettings)
                {
					setting.ApplyValue(element);
				}
			}
            //else if (shouldUnapply)
            //{
            //    PropertySettingGroup group = element.GetInitialStyle();
            //    if (group != null)
            //    {
            //        for (int i = 0; i < group.PropertySettings.Count; i++)
            //        {
            //            IPropertySetting setting = group.PropertySettings[i];
            //            setting.ApplyValue(element);
            //        }
            //    }
            //    else
            //    {
            //        for (int i = 0; i < propertySettings.Count; i++)
            //        {
            //            IPropertySetting setting = propertySettings[i];
            //            element.ResetValue(setting.Property, ValueResetFlags.Style);
            //        }
            //    }
            //}

			if (this.condition != null || this.unapplyCondition != null)
			{
				if (applyConditionEvaluated)
				{
					element.IsStyleSelectorValueSet[this.GetHashCode()] = true;
				}
				else if (unApplyConditionEvaluated)
				{
                    element.IsStyleSelectorValueSet[this.GetHashCode()] = false;
				}
				else
				{
                    element.IsStyleSelectorValueSet.Remove(this.GetHashCode());
				}
			}
        }

        public void Unregister(RadElement element, PropertySettingCollection propertySettings)
        {
            if (element.IsDisposed)
            {
                return;
            }

            foreach (IPropertySetting setting in propertySettings)
            {
                setting.UnregisterValue(element);
            }

            element.IsStyleSelectorValueSet.Remove(this.GetHashCode());
        }

        //Method supports old theming infrastructure and will be depricated
        public abstract LinkedList<RadElement> GetSelectedElements(RadElement element);

        public IElementSelector ChildSelector 
        { 
            get
            {
                return this.childSelector;
            }
            set
            {
                this.childSelector = value;
            }
        }

        //Method supports old theming infrastructure and will be depricated
        public virtual PropertyChangeBehaviorCollection GetBehaviors(PropertySettingGroup group)
        {
            if (this.createdPropertyChangeBehaviors == null)
            {
                if (this.condition != null)
                {
                    this.createdPropertyChangeBehaviors = new PropertyChangeBehaviorCollection(condition.AffectedProperties.Count);
                    for (int i = 0; i < condition.AffectedProperties.Count; i++)
                    {
                        RadProperty property = condition.AffectedProperties[i];
                        PropertyChangeStyleBehavior behavior =
                            new PropertyChangeStyleBehavior(property, this, group.PropertySettings);

                        this.createdPropertyChangeBehaviors.Add(behavior);
                    }
                }
                else
                {
                    this.createdPropertyChangeBehaviors = new PropertyChangeBehaviorCollection();
                }
            }

            return createdPropertyChangeBehaviors;
        }

        //Method supports old theming infrastructure and will be depricated
        void IElementSelector.AddConditionPropertiesToList(List<RadProperty> list)
        {
            if (this.condition != null)
            {
                list.AddRange(condition.AffectedProperties);
            }
        }

        public virtual RoutedEventBehaviorCollection GetRoutedEventBehaviors(PropertySettingGroup group)
        {
			if (this.createdRoutedEventsBehaviors == null)
			{
				this.createdRoutedEventsBehaviors = new RoutedEventBehaviorCollection();
				if (this.condition != null)
				{
					foreach (RaisedRoutedEvent routedEvent in Condition.AffectedEvents)
					{
						RoutedEventStyleBehavior behavior = new RoutedEventStyleBehavior(routedEvent, this, group.PropertySettings);
						this.createdRoutedEventsBehaviors.Add(behavior);
					}
				}
			}

			return this.createdRoutedEventsBehaviors;
		}

        XmlElementSelector IElementSelector.Serialize()
        {
            XmlElementSelector xmlselctor = this.CreateSerializableInstance();
            this.SerializeProperties(xmlselctor);
            return xmlselctor;
        }

        protected abstract XmlElementSelector CreateSerializableInstance();

        protected virtual void SerializeProperties(XmlElementSelector xmlselctor)
        {
            XmlSelectorBase instance = (XmlSelectorBase)xmlselctor;
            if (this.Condition != null)
            {
                instance.Condition = this.Condition.Serialize();
            }

            if (this.UnapplyCondition != null)
            {
                instance.UnapplyCondition = this.UnapplyCondition.Serialize();
            }

            if (this.ChildSelector != null)
            {
                instance.ChildSelector = this.ChildSelector.Serialize();
            }

            instance.AutoUnapply = this.AutoUnapply;
        }
    }
}

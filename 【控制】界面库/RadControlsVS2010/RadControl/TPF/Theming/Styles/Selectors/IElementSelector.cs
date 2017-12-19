using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls
{
    /// <summary>Exposes methods and properties required for a general selector. 
    /// Selectors in telerik presentation framework are like CSS selectors.</summary>
    public interface IElementSelector
    {
        /// <summary>
        /// Gets value indicating whether the selector applies to the specified element
        /// </summary>
        /// <param name="targetElement"></param>
        /// <returns></returns>
        bool CanSelect(RadElement targetElement);

        /// <summary>
        /// Gets value indicating whether the selector applies to the specified element, without checking conditions that apply to properties of the element.
        /// </summary>
        /// <param name="targetElement"></param>
        /// <returns></returns>
        bool CanSelectIgnoringConditions(RadElement targetElement);

        /// <summary>Retrieves an array of selected elements of the element given as an 
        /// argument.</summary>
        LinkedList<RadElement> GetSelectedElements(RadElement element);

        /// <summary>
        /// Method supports obsolete theming infrastructure
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        PropertyChangeBehaviorCollection GetBehaviors(PropertySettingGroup group);

        /// <summary>
        /// Method supports obsolete theming infrastructure
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        RoutedEventBehaviorCollection GetRoutedEventBehaviors(PropertySettingGroup group);

        /// <summary>Gets a value indicating whether a condition has been applied.</summary>
        bool HasApplyCondition { get;}

        /// <summary>Applies the property settings to the given element. Method supports obsolete theming infrastructure. </summary>
        void Apply(RadElement element, PropertySettingCollection propertySettings);

        //void Unapply(RadElement selected, PropertySettingCollection propertySettingCollection);
        /// <summary>Retrieves a serializable wrapper for the selector.</summary>
        XmlElementSelector Serialize();

        /// <summary>
        /// Gets or sets the child selector.
        /// </summary>
        IElementSelector ChildSelector { get; set; }
        
        /// <summary>
        /// Gets value indicating whether the selector Equals to the specified selector
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        bool Equals(IElementSelector selector);

        /// <summary>
        /// Selector Key
        /// </summary>
        int Key { get; }

        /// <summary>Unregisters the propertySettings for the given element. Method supports obsolete theming infrastructure</summary>
        void Unregister(RadElement element, PropertySettingCollection propertySettings);

        /// <summary>
        /// Method supports obsolete theming infrastructure.
        /// If HasApplyCondition returns true, this method should add the RadProperties that the selector depends, so style manager 
        /// can refresh afected element by the selector selector, when property changes
        /// </summary>
        /// <param name="list"></param>
        void AddConditionPropertiesToList(List<RadProperty> list);
    }
}
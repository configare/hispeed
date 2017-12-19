using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Specializes the PropertyChangeBehavior used internally by StlyeSheet system with the
    /// functionality to change element appearance/behavior defined in a StyleSheet when a 
    /// certain RoutedEvent bubbles or tunnels through an element.
    /// </summary>
    public class RoutedEventStyleBehavior : RoutedEventBehavior
    {
        IElementSelector selector;
        PropertySettingCollection propertySettings;
        /// <summary>
        /// Initializes a new instance of the RoutedEventStyleBehavior class using 
        /// the fired routed event, element selector, and property settings.
        /// </summary>
        /// <param name="raisedRoutedEvent"></param>
        /// <param name="selector"></param>
        /// <param name="propertySettings"></param>
        public RoutedEventStyleBehavior(RaisedRoutedEvent raisedRoutedEvent, IElementSelector selector, PropertySettingCollection propertySettings)
            : base(raisedRoutedEvent)
        {
            this.selector = selector;
            this.propertySettings = propertySettings;
        }

        public override void OnEventOccured(RadElement sender, RadElement element, RoutedEventArgs args)
        {
            this.selector.Apply(element, propertySettings);
        }
        /// <summary>
        /// Removes the behavior from the given element.
        /// </summary>
        /// <param name="fromElement"></param>
        public override void BehaviorRemoving(RadElement fromElement)
        {
            this.selector.Unregister(fromElement, propertySettings);
        }
    }
}

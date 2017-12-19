using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{

    /// <summary>
    /// Specielizes the PropertyChangeBehavior used internally by StlyeSheet system with the
    /// functionality to change element appearance/behavior defined in a StyleSheet when 
    /// the corresponding element property changes.
    /// </summary>
    public class PropertyChangeStyleBehavior : PropertyChangeBehavior
    {
        IElementSelector selector;
        PropertySettingCollection propertySettings;

        /// <summary>
        /// Initializes a new instance of the PropertyChangeStyleBehavior class.
        /// </summary>
        /// <param name="executeBehaviorPropertyTrigger"></param>        
        /// <param name="selector"></param>
        /// <param name="propertySettings"></param>
        public PropertyChangeStyleBehavior(RadProperty executeBehaviorPropertyTrigger, IElementSelector selector, PropertySettingCollection propertySettings)
            : base(executeBehaviorPropertyTrigger)
        {
            this.selector = selector;
            this.propertySettings = propertySettings;
        }

        public override void OnPropertyChange(RadElement element, RadPropertyChangedEventArgs e)
        {
            /*VisualElement visualElement = element as VisualElement;
            if (visualElement == null)
                return;*/

            selector.Apply(element, propertySettings);
        }

        public override void BehaviorRemoving(RadElement element)
        {
            selector.Unregister(element, propertySettings);
        }
    }
}

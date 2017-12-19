using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    

    public class RoutedEventBehavior
    {
        RaisedRoutedEvent raisedRoutedEvent;

        public RaisedRoutedEvent RaisedRoutedEvent
        {
            get { return raisedRoutedEvent; }
            set { raisedRoutedEvent = value; }
        }

        public RoutedEventBehavior(RaisedRoutedEvent raisedRoutedEvent)
        {
            this.raisedRoutedEvent = raisedRoutedEvent;
        }

        public virtual void OnEventOccured(RadElement sender, RadElement element, RoutedEventArgs args)
        {

        }

        public virtual void BehaviorRemoving(RadElement fromElement)
        {
        }
    }

    /// <summary>
    /// A collection of the RoutedEventBehavior objects. Used by the StyleSheet system.
    /// </summary>
    public class RoutedEventBehaviorCollection : List<RoutedEventBehavior>
    {
    }
}

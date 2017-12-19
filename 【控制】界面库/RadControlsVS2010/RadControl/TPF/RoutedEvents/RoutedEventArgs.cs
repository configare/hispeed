using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Defines the routing directions for an events.
    /// </summary>
    public enum RoutingDirection
    {
        /// <summary>
        /// Indicates a tunnel event.
        /// </summary>
        Tunnel = 0,
        /// <summary>
        /// Indicates a bubble event.
        /// </summary>
        Bubble
    }
    /// <summary>
    /// Represents event arguments for a routed event.
    /// </summary>
    public class RoutedEventArgs : EventArgs
    {
        private bool canceled = false;
        private RoutingDirection direction;
        private RoutedEvent routedEvent;
        private EventArgs originalEventArgs = EventArgs.Empty;

        /////The constructor is removed due to usage of the new constructor, that takes into account
        ///// the original CLR event args from the clr event
        /////
        //public RoutedEventArgs(RoutedEvent routedEvent)
        //{
        //    this.RoutedEvent = routedEvent;
        //}
        /// <summary>
        /// Initializes a new instance of the RoutedEventArgs class using EventsArgs to 
        /// initializes its base class and the RoutedEvent.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="routedEvent"></param>
        public RoutedEventArgs(EventArgs args, RoutedEvent routedEvent)//: this(routedEvent)
        {
            this.originalEventArgs = args;
            this.RoutedEvent = routedEvent;
        }

        /// <summary>
        /// Gets or sets the original EventArgs.
        /// </summary>
        public EventArgs OriginalEventArgs
        {
            get
            {
                return originalEventArgs;
            }
            set
            {
                originalEventArgs = value;
            }
        }
        
        /// <summary>
        ///Gets or sets a value indicating the RoutedEvent. 
        /// </summary>
        public RoutedEvent RoutedEvent
        {
            get 
            { 
                return routedEvent; 
            }
            set 
            { 
                routedEvent = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event is canceled.
        /// </summary>
        public bool Canceled
        {
            get 
            { 
                return canceled; 
            }
            set 
            { 
                canceled = value; 
            }
        }
 
        /// <summary>
        /// Gets or sets a value indicating the routing direction for the event.
        /// </summary>
        public RoutingDirection Direction
        {
            get 
            { 
                return direction; 
            }
            set 
            { 
                direction = value; 
            }
        }        
    }
}

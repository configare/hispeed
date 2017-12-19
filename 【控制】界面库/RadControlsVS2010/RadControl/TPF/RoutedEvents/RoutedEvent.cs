using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Telerik.WinControls
{
    public enum EventBehaviorSenderType
    {
        AnySender = 0,
        ElementType,
        ElementName
    }

    /// <summary>
    /// Represents a routed event. Routed events can be tunnel or bubble event 
    /// according to the routed direction of the event.
    /// </summary>
    public class RoutedEvent
    {
        private string eventName;
        private Type ownerType;
        
        internal RoutedEvent(string eventName, Type ownerType)
        {
            this.eventName = eventName;
            this.ownerType = ownerType;
        }
        
        /// <summary>
        /// Gets or sets the event name.
        /// </summary>
        public string EventName
        {
            get { return eventName; }
            set { eventName = value; }
        }

        /// <summary>
        /// Gets the owner's type.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Type OwnerType
        {
            get
            {
                return this.ownerType;
            }
            internal set
            {
                ownerType = value;
            }
        }
    }
    /// <summary>
    /// Represents a raised routed event.
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(EventBehaviorSenderType))]
    //[XmlInclude(typeof(RoutedEvent))]
    [XmlInclude(typeof(RoutingDirection))]
    public class RaisedRoutedEvent
    {
        private RoutedEvent routedEvent;
        private string sender;
        private EventBehaviorSenderType senderType;
        private RoutingDirection direction;
        /// <summary>
        /// Initializes a new instance of the RaisedRoutedEvent class.
        /// </summary>
        public RaisedRoutedEvent()
        {
        }
        /// <summary>
        /// Initializes a new instance of the RaisedRoutedEvent class using
        /// routed event, event sender, sender's type, and routing direction (tunnel 
        /// or bubble).
        /// </summary>
        /// <param name="routedEvent"></param>
        /// <param name="sender"></param>
        /// <param name="senderType"></param>
        /// <param name="direction"></param>
        public RaisedRoutedEvent(RoutedEvent routedEvent, string sender, EventBehaviorSenderType senderType, RoutingDirection direction)
        {
            this.routedEvent = routedEvent;
            this.sender = sender;
            this.senderType = senderType;
            this.direction = direction;
        }
        /// <summary>
        /// Gets or sets a value indicating the routed event.
        /// </summary>
        [XmlIgnore]
        public RoutedEvent RoutedEvent
        {
            get
            {
                if (!string.IsNullOrEmpty( this.RoutedEventFullName))
                {
                    routedEvent = XmlRoutedEventCondition.GetRoutedEvent(this.RoutedEventFullName);
                }
                return routedEvent;
            }
            set
            {
                routedEvent = value;
                this.RoutedEventFullName = routedEvent.OwnerType.FullName + "." + routedEvent.EventName;
            }
        }

        private string routedEventFullName;
        /// <summary>
        /// Gets or sets a string value indicating the routed event name.
        /// </summary>
        [XmlAttribute]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string RoutedEventFullName
        {
            get
            {
                return this.routedEventFullName;
            }
            set
            {
                this.routedEventFullName = value;
            }
        }
        /// <summary>
        /// Gets or sets the sender's type.
        /// </summary>
        [XmlAttribute]
        public EventBehaviorSenderType SenderType
        {
            get { return senderType; }
            set { senderType = value; }
        }
        /// <summary>
        /// Gets or sets the sender.
        /// </summary>
        [XmlAttribute]
        public string Sender
        {
            get { return sender; }
            set { sender = value; }
        }
        /// <summary>
        /// Gets or sets the routing direction - tunnel or bubble.
        /// </summary>
        [XmlAttribute]
        public RoutingDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        /// <summary>
        /// Compares the instance with the other event arguments and the sender of the event.
        /// </summary>
        /// <param name="senderElement"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public bool IsSameEvent(RadElement senderElement, RoutedEventArgs eventArgs)
        {
            bool res = false;
            if (string.Compare(this.RoutedEvent.EventName, eventArgs.RoutedEvent.EventName, true) == 0)
            {
                switch (this.SenderType)
                {
                    case EventBehaviorSenderType.AnySender:
                        res = true;
                        break;
                    case EventBehaviorSenderType.ElementName:
                        if (this.Direction == eventArgs.Direction && string.Compare(senderElement.Name, this.sender, true) == 0)
                            res = true;
                        break;
                    case EventBehaviorSenderType.ElementType:
                        if (this.Direction == eventArgs.Direction && senderElement.GetType().Name == this.sender)
                            res = true;
                        break;
                }
            }

            return res;
        }

        /// <summary>
        /// Compares the instance with another event passed as a parameter.
        /// </summary>
        /// <param name="targetEvent"></param>
        /// <returns></returns>
        public bool IsSameEvent(RaisedRoutedEvent targetEvent)
        {
            bool res = false;
            if (string.Compare(this.RoutedEvent.EventName, targetEvent.RoutedEvent.EventName, true) == 0)
            {
                if (this.direction == targetEvent.direction)
                {
                    res = this.Sender == targetEvent.Sender;
                }
            }

            return res;
        }
    }
}

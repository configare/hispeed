using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// RoutedEventCondition evaluates if a routed event is currently 
    /// tunneling/bubbling through an Element.
    /// </summary>
    public class RoutedEventCondition : Condition
    {
        private RoutedEvent routedEvent;
        private string sender;
        private EventBehaviorSenderType senderType;
        private UnaryOperator unaryOperator = UnaryOperator.None;
        private RoutingDirection direction;
        /// <summary>
        /// Gets or sets the direction. Events could be tunneling or bubbling.
        /// </summary>
        public RoutingDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        /// <summary>
        /// Gets or sets the unary operator for the condition.
        /// </summary>
        public UnaryOperator UnaryOperator
        {
            get { return unaryOperator; }
            set { unaryOperator = value; }
        }
        /// <summary>
        /// Gets or sets the sender's type.
        /// </summary>
        public EventBehaviorSenderType SenderType
        {
            get { return senderType; }
            set { senderType = value; }
        }
        /// <summary>
        /// Gets or sets the sender of the event.
        /// </summary>
        public string Sender
        {
            get { return sender; }
            set { sender = value; }
        }
        /// <summary>
        /// Gets or sets the routed event.
        /// </summary>
        public RoutedEvent RoutedEvent
        {
            get { return routedEvent; }
            set { routedEvent = value; }
        }
        /// <summary>
        /// Initializes a new instance of the RoutedEventCondtion class.
        /// </summary>
        /// <param name="routedEvent"></param>
        /// <param name="sender"></param>
        /// <param name="senderType"></param>
        /// <param name="direction"></param>
        public RoutedEventCondition(RoutedEvent routedEvent, string sender, EventBehaviorSenderType senderType, RoutingDirection direction)
        {
            this.routedEvent = routedEvent;
            this.sender = sender;
            this.senderType = senderType;
            this.direction = direction;
        }
        /// <summary>
        /// Initializes a new instance of the RoutedEventCondition class from 
        /// routed event, sender, sender's type, and unary operator used in the condition. 
        /// </summary>
        /// <param name="routedEvent"></param>
        /// <param name="sender"></param>
        /// <param name="senderType"></param>
        /// <param name="direction"></param>
        /// <param name="unaryOperator"></param>
        public RoutedEventCondition(RoutedEvent routedEvent, string sender, EventBehaviorSenderType senderType, RoutingDirection direction, UnaryOperator unaryOperator)
        {
            this.routedEvent = routedEvent;
            this.sender = sender;
            this.senderType = senderType;
            this.direction = direction;
            this.unaryOperator = unaryOperator;
        }
        /// <summary>
        /// Evaluates the 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool Evaluate(RadElement target)
        {
            if (target == null)
            {
                return false;
            }

            switch (this.UnaryOperator)
            {
                case UnaryOperator.None:
                    return target.IsEventInProcess(new RaisedRoutedEvent(this.RoutedEvent, this.sender, this.senderType, this.Direction));
                case UnaryOperator.NotOperator:
                    return !target.IsEventInProcess(new RaisedRoutedEvent(this.RoutedEvent, this.sender, this.senderType, this.Direction));
            }

            return false;
        }

        protected override void FillAffectedEvents(List<RaisedRoutedEvent> inList)
        {
            RaisedRoutedEvent routedEvent = new RaisedRoutedEvent(this.RoutedEvent, this.sender, this.senderType, this.Direction);
            inList.Add(routedEvent);
        }
        /// <summary>
        /// Creates a serializable instance of the the RoutedEventCondition.
        /// </summary>
        /// <returns></returns>
        public override XmlCondition CreateSerializableInstance()
        {
            return new XmlRoutedEventCondition(this);
        }
    }
}

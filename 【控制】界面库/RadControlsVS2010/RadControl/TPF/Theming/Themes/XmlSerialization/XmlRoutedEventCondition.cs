using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Telerik.WinControls
{
    [Serializable]
    public class XmlRoutedEventCondition : XmlCondition
    {
        private string routedEvent;
        private string sender;
        private EventBehaviorSenderType senderType;
        private UnaryOperator unaryOperator = UnaryOperator.None;
        private RoutingDirection direction;

        [XmlAttribute]
        public RoutingDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        [XmlAttribute]
        public UnaryOperator UnaryOperator
        {
            get { return unaryOperator; }
            set { unaryOperator = value; }
        }

        [XmlAttribute]
        public EventBehaviorSenderType SenderType
        {
            get { return senderType; }
            set { senderType = value; }
        }

        [XmlAttribute]
        public string Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        [XmlAttribute]
        public string RoutedEvent
        {
            get { return routedEvent; }
            set { routedEvent = value; }
        }

        public XmlRoutedEventCondition()
        {
        }

        public XmlRoutedEventCondition(RoutedEventCondition condition)
        {
            this.Sender = condition.Sender;
            this.SenderType = condition.SenderType;
            this.UnaryOperator = condition.UnaryOperator;
            this.Direction = condition.Direction;
            this.RoutedEvent = condition.RoutedEvent.OwnerType.FullName + "." + condition.RoutedEvent.EventName;
        }

        protected override void DeserializeProperties(Condition selector)
        {
            //
        }

        internal static RoutedEvent GetRoutedEvent(string routedEvent)
        {
            if (!string.IsNullOrEmpty(routedEvent))
            {
                string[] propertyParts = routedEvent.Split('.');
                string eventName;
                string className;
                if (propertyParts.Length > 1)
                {
                    eventName = propertyParts[propertyParts.Length - 1];
                    className = string.Join(".", propertyParts, 0, propertyParts.Length - 1);
                }
                else
                {
                    throw new Exception("Invalid property parts");
                }

                return RadElement.GetRegisterRoutedEvent(eventName, className);
            }

            return null;
        }

        protected override Condition CreateInstance()
        {
            return new RoutedEventCondition(GetRoutedEvent(this.RoutedEvent), sender, senderType, direction, unaryOperator);
        }
    }
}

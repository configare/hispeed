using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Telerik.WinControls
{
    /// <summary>
    ///Defines a base abstract class that describes a condition which checks when to apply 
    ///a style rule. SimpleCondition evaluates when a property of an Element equals a
    ///certain value. RoutedEventCondition evaluates if a routed event is currently 
    ///tunneling/bubbling through an Element. ComplexCondition evaluates two conditions 
    ///related with a binary operator.
    /// </summary>
    public abstract class Condition
    { 
        /// <summary>
        /// Retrieves a value indicating whether to apply a style rule.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public abstract bool Evaluate(RadElement target);
        /// <summary>
        /// Retrieves a list of the affected properties of the current condition.
        /// </summary>
        public List<RadProperty> AffectedProperties
        {
            get
            {
                List<RadProperty> res = new List<RadProperty>();
                this.FillAffectedProperties(res);
                return res;
            }
        }
        /// <summary>
        /// Retrieves a list of the affected events of the current condition.
        /// </summary>
        public List<RaisedRoutedEvent> AffectedEvents
        {
            get
            {
                List<RaisedRoutedEvent> res = new List<RaisedRoutedEvent>();
                this.FillAffectedEvents(res);
                return res;
            }
        }

        protected virtual void FillAffectedProperties(List<RadProperty> inList)
        { 
        }

        protected virtual void FillAffectedEvents(List<RaisedRoutedEvent> inList)
        {
        }
        /// <summary>
        /// Serializes the condition by creating a serializabe instance of the XmlCondition 
        /// class.  
        /// </summary>
        /// <returns></returns>
        public XmlCondition Serialize()
        {
            XmlCondition res = this.CreateSerializableInstance();
            this.SerializeProperties(res);

            return res;
        }

        protected virtual void SerializeProperties(XmlCondition res)
        {            
        }
        /// <summary>
        /// Creates an instance of the XmlCondition class which is a serializable 
        /// condition.         
        /// </summary>
        /// <returns></returns>
        public abstract XmlCondition CreateSerializableInstance();
    }
}

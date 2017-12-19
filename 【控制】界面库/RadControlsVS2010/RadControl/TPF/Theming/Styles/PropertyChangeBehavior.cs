using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// 
    /// </summary>
    public class PropertyChangeBehavior
    {
        RadProperty _property;

        public PropertyChangeBehavior(RadProperty executeBehaviorPropertyTrigger)
        {
            this._property = executeBehaviorPropertyTrigger;
         }

        public RadProperty Property
        {
            get
            {
                return _property;
            }
            set
            {
                _property = value;
            }
        }

        public virtual void OnPropertyChange(RadElement element, RadPropertyChangedEventArgs e)
        {
        }

        public virtual void BehaviorRemoving(RadElement element)
        {
        }
    }
}

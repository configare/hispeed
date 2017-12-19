using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{

    /// <summary>
    /// Represents a collection of PropertyChangeBahavior instances.
    /// See also RadElement.AddBehavior
    /// </summary>
    public class PropertyChangeBehaviorCollection : List<PropertyChangeBehavior>
    {
        public PropertyChangeBehaviorCollection()
        { }

        public PropertyChangeBehaviorCollection(int capacity): base(capacity)
        { 
        }
    }
}
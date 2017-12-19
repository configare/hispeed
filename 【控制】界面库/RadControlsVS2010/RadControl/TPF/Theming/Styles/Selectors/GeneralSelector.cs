using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls
{
    ///// <summary>
    ///// Represents a general selector. General selectors are used to apply customization
    ///// to all elements. Behavior is very similar to that of CSS universal selectors.
    ///// </summary>
    //public class GeneralSelector : SelectorBase
    //{
    //    /// <summary>Retrieves all elements in the selected elements hierarchy.</summary>
    //    public override LinkedList<RadElement> GetSelectedElements(RadElement element)
    //    {
    //        RadElementReadonlyList children = new RadElementReadonlyList(element.ChildrenHierarchy);
    //        LinkedList<RadElement> res = new LinkedList<RadElement>(children);

    //        res.AddFirst(element);

    //        return res;
    //    }

    //    protected override XmlElementSelector CreateSerializableInstance()
    //    {
    //        return new XmlGeneralSelector();
    //    }
    //}
}

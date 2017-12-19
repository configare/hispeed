using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// This interface gives the ability to create reusable providers for VisualElements
    /// that are in some relation with logical data objects.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public interface IVisualElementProvider
    {
        /// <summary>
        /// Create element using the pased data
        /// </summary>
        /// <param name="data">Logical data that will be used to initialize the element.</param>
        /// <returns>The newly created element if everything is OK; null on error.</returns>
        VisualElement CreateElement(object data);

        /// <summary>
        /// Cleans up when an element that is created with CreateElement() is no longer necessary.
        /// </summary>
        /// <param name="element"></param>
        void CleanupElement(VisualElement element);

        /// <summary>
        /// Initialize already created element with logical data (if possible).
        /// </summary>
        /// <param name="element">the element to  be initilaized</param>
        /// <param name="data">with this data the given element should be initialized</param>
        /// <returns>false if the element cannot be initialized with the given data</returns>
        bool SetElementData(VisualElement element, object data);

        /// <summary>
        /// Check if an element can be initialized with some logical data.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="data"></param>
        /// <returns>true if the lement can be initialized with the data.</returns>
        bool IsElementCompatible(VisualElement element, object data);
    }
}

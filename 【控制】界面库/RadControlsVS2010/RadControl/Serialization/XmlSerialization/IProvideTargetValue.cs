using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.XmlSerialization
{
    public interface IProvideTargetValue
    {
        // Properties
        object TargetObject { get; }
        object TargetProperty { get; }
        string SourceValue { get; }
    }
}

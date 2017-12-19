using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Primitives
{
    public class TextPrimitiveFactory
    {
        static public ITextPrimitive CreateTextPrimitiveImp(bool htmlEnabled)
        {
            if (htmlEnabled)
            {
                return new TextPrimitiveHtmlImpl();
            }
            else
            {
                return new TextPrimitiveImpl();
            }
        }
    }
}

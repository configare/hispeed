using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// <Argument name="AreaStatArgs" description="NDVI阈值低端值" editoruiprovider="assembly:class" />
    /// </summary>
    public interface IArgumentEditorUI
    {
        object GetArgumentValue();
        void SetChangeHandler(Action<object> handler);
        object ParseArgumentValue(XElement ele);
    }
}

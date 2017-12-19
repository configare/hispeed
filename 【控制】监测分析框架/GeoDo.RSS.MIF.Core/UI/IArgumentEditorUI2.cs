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
    public interface IArgumentEditorUI2
    {
        /// <summary>
        /// 如果为true，则在调用注册的值改变事件的方法时候，会调用
        /// 参数的值改变事件。
        /// </summary>
        bool IsExcuteArgumentValueChangedEvent{set;get;}
        void InitControl(IExtractPanel panel,ArgumentBase arg);
        object GetArgumentValue();
        /// <summary>
        /// 注册的值改变事件
        /// </summary>
        /// <param name="handler"></param>
        void SetChangeHandler(Action<object> handler);
        object ParseArgumentValue(XElement ele);
    }
}

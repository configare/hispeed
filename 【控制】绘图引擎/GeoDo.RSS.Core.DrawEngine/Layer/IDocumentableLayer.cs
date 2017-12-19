using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    /// <summary>
    /// 标记某个层是否可被序列化到视图文档（例如：Gxd文件）
    /// </summary>
    public interface IDocumentableLayer
    {
        bool IsCanDocumentable { get; set; }
    }
}

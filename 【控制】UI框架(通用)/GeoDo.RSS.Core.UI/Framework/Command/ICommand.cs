using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.UI
{
    public interface ICommand
    {
        int Id { get; }
        //命令名称
        string Name { get; }
        //命令显示名称
        string Text { get; }
        //命令鼠标提示
        string ToolTip { get; }
        //命令图标
        Bitmap Bitmap { get; }
        //标识显示名称或图标是否被修改
        bool IsChanged { get; set; }
        //可用状态
        bool Enable { get; }
        //可见状态
        bool Visible { get; }
        //命令可以作用的业务上下文Id(1,2,4,8,16,...)
        int OpertionContextIds { get; }
        //应用会话到命令(根据会话的状态改变命令的状态)
        void Apply(ISmartSession session);
        //执行命令
        void Execute();
        void Execute(string argument);
        void Execute(string argument,params string[] args);
    }
}

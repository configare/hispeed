using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    /// <summary>
    /// 命令环境,主要指菜单和工具栏构成的环境
    /// </summary>
    public interface ICommandEnvironment
    {
        ICommand Get(int id);
        ICommand Find(string name);
        ICommand[] Search(string keyword);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.UI
{
    public interface IUIFrameworkHelper
    {
        void InsertTab(int idx,object tab);
        int IndexOf(string tabText);
        bool IsExist(string tabText);
        void ActiveTab(string tabText);
        void Remove(string tabText);
        void ActiveDefaultTab();
        void SetVisible(string groupName, bool visible);
        void Insert(string groupName, int index);
        void SetLockBesideX(string groupName, bool locked);
        Image GetImage(string resName);
        void SetTabChangedEvent(Action<object> tabChanged);
        object GetActiveTab();
    }
}

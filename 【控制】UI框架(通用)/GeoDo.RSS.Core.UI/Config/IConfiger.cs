using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public interface IConfiger
    {
        object GetConfigItemValue(string key);
        void BeginUpdate();
        void UpdateConfigItem(string key, object value);
        void EndUpdate();
    }
}

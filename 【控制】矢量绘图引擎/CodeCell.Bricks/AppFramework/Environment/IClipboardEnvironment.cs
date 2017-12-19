using System;
using System.Collections.Generic;
using System.Text;

namespace CodeCell.Bricks.AppFramework
{
    public interface IClipboardEnvironment
    {
        bool IsEmpty { get;}
        int Count { get;}
        IEnumerable<object> Datas { get;}
        void Clear();
        object First { get;}
        object[] GetDatas();
        void PutData(object obj);
        void PutData(object[] objects);
    }
}

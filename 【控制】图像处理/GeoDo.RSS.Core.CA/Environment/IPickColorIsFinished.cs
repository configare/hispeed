using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.CA
{
    public interface IPickColorIsFinished
    {
        void Picking(Color color);
        void PickColorFinished(Color color);
    }
}

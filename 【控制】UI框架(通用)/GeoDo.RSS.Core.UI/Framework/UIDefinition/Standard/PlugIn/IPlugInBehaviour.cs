using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public interface IPlugInBehaviour
    {
    }

    public interface IZoomBehaviour : IPlugInBehaviour
    {
        void ToHome();//1:1,Full Global
        void To(object envelope);
    }

    public interface IAcceptFileBehaviour : IPlugInBehaviour
    {
        void Accept(string fname);
        void Accept(object memorydata);
    }
}

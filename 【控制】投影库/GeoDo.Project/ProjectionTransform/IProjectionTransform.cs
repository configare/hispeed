using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Project
{
    public interface IProjectionTransform:IDisposable
    {
        void Transform(double[] xs,double[] ys);
        void InverTransform(double[] xs,double[] ys);
    }
}

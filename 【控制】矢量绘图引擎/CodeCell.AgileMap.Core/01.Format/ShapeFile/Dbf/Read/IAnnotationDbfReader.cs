using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IAnnotationDbfReader:IDisposable
    {
        LabelLocation[] GetLabelLocation(int oid);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface ILabelLocationService
    {
        LabelLocation[] LabelLocations { get; }
        void Update(Shape geometry);
        void UpdateLocations();
    }
}

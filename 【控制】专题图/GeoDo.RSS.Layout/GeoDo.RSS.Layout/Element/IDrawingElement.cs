using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout
{
    public interface IDrawingElement : IElement, ISizableElement
    {
        string RefLayerName { get; set; }
        string DataFrameId { get; set; }
        void SetDataFrame(IDataFrame dataFrame);
        void Update();
        bool IsSimpleLegend { get; set; }
        bool IsShowBorder { get; set; }
    }
}

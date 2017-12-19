using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public interface IBandOperator
    {
        int Width { get; }
        int Height { get; }
        enumDataType DataType { get; }
        void Read(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize);
    }
}

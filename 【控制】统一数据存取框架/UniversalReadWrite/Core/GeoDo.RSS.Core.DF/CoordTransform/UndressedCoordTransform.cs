using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    /// <summary>
    /// 没有坐标系统的栅格文件坐标转换对象
    /// 栅格的左下角为(0,0),向上为正方向(+,Y),向下为负方向(-,X)
    /// </summary>
    public class UndressedCoordTransform:ICoordTransform
    {
        private int _width ;
        private int _height ;

        public UndressedCoordTransform(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public double[] GTs
        {
            get { return null; }
        }

        public void Raster2DataCoord(int row, int col, out double coordX, out double coordY)
        {
            coordX = col;
            coordY = _height - row;
        }

        public void Raster2DataCoord(int row, int col, double[] coord)
        {
            coord[0] = col;
            coord[1] = _height - row;
        }

        public void Dispose()
        {
        }
    }
}

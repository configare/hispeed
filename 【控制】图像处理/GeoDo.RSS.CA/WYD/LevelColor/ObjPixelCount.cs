using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.CA
{
    public class ObjPixelCount
    {
        private const int CANSHU = 256;
        private const int INIT = 0;
        private int[] _pixelCountRGB = new int[CANSHU];
        private int[] _pixelCountBlue = new int[CANSHU];
        private int[] _pixelCountGreen = new int[CANSHU];
        private int[] _pixelCountRed = new int[CANSHU];

        public ObjPixelCount()
        {
            
        }
       
        public int[] PixelCountRGB
        {
            get { return _pixelCountRGB; }
            set { _pixelCountRGB = value; }
        }

        /// <summary>
        /// 统计图像中每一阶蓝色像元的数量
        /// </summary>
        public int[] PixelCountBlue
        {
            get { return _pixelCountBlue; }
            set { _pixelCountBlue = value; }
        }

        /// <summary>
        /// 统计图像中每一阶绿色像元的数量
        /// </summary>
        public int[] PixelCountGreen
        {
            get { return _pixelCountGreen; }
            set { _pixelCountGreen = value; }
        }

        /// <summary>
        /// 统计图像每一阶红色像元的数量
        /// </summary>
        public int[] PixelCountRed
        {
            get { return _pixelCountRed; }
            set { _pixelCountRed = value; }
        }

        public void ClearPixelCount()
        {
            for (int i = 0; i < CANSHU; i++)
            {
                _pixelCountRGB[i] = INIT;
                _pixelCountGreen[i] = INIT;
                _pixelCountBlue[i] = INIT;
                _pixelCountRed[i] = INIT;
            }
        }
    }
}

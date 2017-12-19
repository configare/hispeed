using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout
{
    public interface ILayoutCoordinateTransform
    {
        void Layout2Pixel(ref float x,ref float y);
        void Pixel2Layout(ref float x, ref float y);
        void Layout2Screen(ref float x, ref float y);
        void Screen2Layout(ref float x, ref float y);
        void Layout2Screen(ref float v);
        float Pixel2Centimeter(int pixels);
        float Pixel2Centimeter(int pixels, float dpi);
        int Centimeter2Pixel(float centimeters);
        int Centimeter2Pixel(float centimeters, float dpi);
    }
}

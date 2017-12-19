using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    public interface IRasterDictionaryTemplate<T>:IDisposable
    {
        string GetPixelName(T pixelValue);
        string GetPixelName(double geoX, double geoY);
        T GetCode(double geoX, double geoY);
        string GetPixelName(int row, int col);
        Dictionary<T, string> CodeNameParis { get; }
        int[] GetAOI(string name, double minX, double maxX, double minY, double maxY, Size outSize);
        int[] GetAOIByKey(T name, double minX, double maxX, double minY, double maxY, Size outSize);
    }
}

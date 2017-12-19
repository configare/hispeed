using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class MathHelper
    {
        public static bool MethodOfLeastSquares(List<Samples> samples, out double a, out double b, out double fEigenValue)
        {
            a = 0.0;
            b = 0.0;
            fEigenValue = 0;
            double valueCount = samples.Count;
            if (valueCount == 0)
                return false;
            double sumX = 0.0;
            double sumY = 0.0;
            double sumXSquares = 0.0;
            double sumXY = 0.0;
            int validSampleSize;//有效样本空间
            validSampleSize = 0;
            for (int i = 0; i < valueCount; i++)
            {
                validSampleSize++;
                sumX += samples[i].Ndvi;
                sumY += samples[i].Lst;
                sumXY += samples[i].Ndvi * samples[i].Lst;
                sumXSquares += samples[i].Ndvi * samples[i].Ndvi;
            }
            b = (validSampleSize * sumXY - sumX * sumY) / (validSampleSize * sumXSquares - sumX * sumX);
            a = (sumY / validSampleSize) - b * (sumX / validSampleSize);

            double avgX = sumX / validSampleSize;
            double avgY = sumY / validSampleSize;

            double fDifX = 0.0; //样本X－X差值
            double fDifY = 0.0; //样本Y－Y差值
            double fLXX = 0.0;  //样本X－X差值的平方和
            double fLXY = 0.0;	 //（样本X－X差值）* （样本Y－Y差值）之和
            double fLYY = 0.0;	 //样本Y－Y差值的平方和 总体平方和

            for (int i = 0; i < validSampleSize; i++)
            {
                fDifX = samples[i].Ndvi - avgX;
                fDifY = samples[i].Lst - avgY;
                fLXX = fLXX + fDifX * fDifX;
                fLYY = fLYY + fDifY * fDifY;
                fLXY = fLXY + fDifX * fDifY;
            }

            if (fLXX == 0)
                return false;
            fEigenValue = fLXY / Math.Sqrt(fLXX * fLYY);
            return true;
        }
    }
}

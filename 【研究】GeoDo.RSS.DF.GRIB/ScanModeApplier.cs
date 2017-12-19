using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GRIB
{
    public static class ScanModeApplier
    {
        public static void ApplyScanMode(int scanMode, int width, ref float[] datas)
        {
            if (datas == null || datas.Length == 0)
                return;
            int num = width / 2;
            int dataLength = datas.Length;
            float num2 = 0;
            switch (scanMode)
            {
                case 0:
                case 64:
                    return;
                case 128:
                case 192:
                    for (int i = 0; i < dataLength; i += width)
                    {
                        for (int j = 0; j < num; j++)
                        {
                            num2 = datas[i + j];
                            datas[i + j] = datas[i + width - j - 1];
                            datas[i + width - j - 1] = num2;
                        }
                    }
                    break;
            }
            int num3 = 0;
            float num4 = 0;
            for (int k = 0; k < dataLength; k += width)
            {
                num3 = k / width;
                if (num3 % 2 == 1)
                {
                    for (int l = 0; l < num; l++)
                    {
                        num4 = datas[k + l];
                        datas[k + l] = datas[k + width - l - 1];
                        datas[k + width - l - 1] = num4;
                    }
                }
            }
        }
    }
}

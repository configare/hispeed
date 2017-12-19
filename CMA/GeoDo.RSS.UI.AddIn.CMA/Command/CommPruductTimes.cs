using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Core;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.CMA
{
    public class CommPruductTimes
    {
        /// <summary>
        /// 当期区域像素点频次统计（基于判识二值数据进行计算）
        /// </summary>
        /// <param name="productIdentify">产品标识</param>
        /// <param name="subProIdentify">子产品标识</param>
        /// <param name="invaild">无效值</param>
        public static void TimesStatAnalysisByPixel(string productIdentify, string subProIdentify, Int16 invaild)
        {
            IRasterOperator<Int16> roper = new RasterOperator<Int16>();
            RasterIdentify identify = GetIdentify(productIdentify, subProIdentify);
            IInterestedRaster<Int16> timeResult = null;
            string[] files = GetFiles();
            //频次统计
            timeResult = roper.Times(files, identify, (dstValue, srcValue) =>
            {
                if (srcValue != invaild)
                    return (Int16)(dstValue++);
                else
                    return dstValue;
            });
            if (timeResult == null)
                return;

        }

        private static string[] GetFiles()
        {
            string[] files = null;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Multiselect = true;
                dlg.Filter = "NSMC Raster(*.dat)|*.dat";
                if (dlg.ShowDialog() == DialogResult.OK)
                    files = dlg.FileNames;
            }
            return files;
        }

        private static RasterIdentify GetIdentify(string productIdentify, string subProIdentify)
        {
            RasterIdentify identify = new RasterIdentify();
            identify.ProductIdentify = productIdentify;
            identify.SubProductIdentify = subProIdentify;
            identify.ThemeIdentify = "CMA";
            return identify;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.CMA
{
    public class CommProductCompute
    {
        public CommProductCompute()
        {

        }

        /// <summary>
        /// 差值运算
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aoi">感兴趣区域</param>
        /// <param name="fristFileName">被减数文件</param>
        /// <param name="secondFileName">减数文件</param>
        /// <param name="rasterIdentify">产品标识</param>
        /// <returns></returns>
        public static IExtractResult ChaZhi<T>(int[] aoi, string fristFileName, string secondFileName, RasterIdentify rasterIdentify)
        {
            IRasterOperator<float> rasterOperator = new RasterOperator<float>();
            IInterestedRaster<float> resultRaster = rasterOperator.Compare(aoi, fristFileName, secondFileName,
                (beiJianShu, jianShu) =>
                {
                    return beiJianShu - jianShu;
                }, rasterIdentify
                );
            return resultRaster;
        }

        public static IExtractResult CommMaxFile(RasterIdentify rasterIdentify)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Multiselect = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (dlg.FileNames.Count() < 2)
                        return null;
                    return ComputeMaxFile(dlg.FileNames, rasterIdentify);
                }
                else
                {
                    return null;
                }
            }
        }

        public static IExtractResult CommMinFile(RasterIdentify rasterIdentify)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Multiselect = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (dlg.FileNames.Count() < 2)
                        return null;
                    return ComputeMinFile(dlg.FileNames, rasterIdentify);
                }
                else
                {
                    return null;
                }
            }
        }

        public static IExtractResult ComputeMaxFile(string[] fnames, RasterIdentify rasterIdentify)
        {
            IRasterOperator<float> rasterOperator = new RasterOperator<float>();
            IInterestedRaster<float> resultRaster = rasterOperator.Times(fnames, rasterIdentify,
                (value, valuei) =>
                {
                    if (valuei > value)
                        value = valuei;
                    return value;
                }
                );
            return resultRaster;
        }

        public static IExtractResult ComputeMinFile(string[] fnames, RasterIdentify rasterIdentify)
        {
            IRasterOperator<float> rasterOperator = new RasterOperator<float>();
            IInterestedRaster<float> resultRaster = rasterOperator.Times(fnames, rasterIdentify,
                (value, valuei) =>
                {
                    if (valuei < value)
                        value = valuei;
                    return value;
                }
                );
            return resultRaster;
        }

        //public static IExtractResult ComputeAvgFile(string[] fnames,RasterIdentify rasterIdentify)
        //{
        //    IRasterOperator<float> rasterOperator = new RasterOperator<float>();
        //    int i = 1;
        //    float v = 0;
        //    IInterestedRaster<float> resultRaster = rasterOperator.Times(fnames, rasterIdentify,
        //        (value, valuei) =>
        //        {
        //            float f=((v += valuei) - v) / i;
        //            i++;
        //            return f;
        //        }
        //        );
        //    return resultRaster;
        //}
    }
}

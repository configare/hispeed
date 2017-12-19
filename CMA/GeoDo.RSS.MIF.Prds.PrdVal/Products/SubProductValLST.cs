#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-6-15 19:55:45
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.MEM;
using System.Text.RegularExpressions;
using GeoDo.Project;
using GeoDo.RasterProject;
using GeoDo.FileProject;

namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    /// <summary>
    /// 类名：SubProductValLST
    /// 属性描述：
    /// 创建者：lxj   创建日期：2014-6-15 19:55:45
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProductValLST: CmaMonitoringSubProduct
    {
        private IArgumentProvider _curArguments = null;
        private IContextMessage _contextMessage;
        private static Regex[] DataReg = new Regex[]
        {
            new Regex (@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})", RegexOptions.Compiled),
            new Regex (@"A(?<year>\d{4})(?<day>\d{3})", RegexOptions.Compiled),
        };
        
        public SubProductValLST(SubProductDef subProductDef)
            : base(subProductDef)
        { 

        }
        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            _curArguments = _argumentProvider;
            if (_curArguments == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName") == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName").ToString() == "SateValAlgorithm")
            {
                return SateValAlgorithm(progressTracker);
            }
            return null;
        }
        private IExtractResult SateValAlgorithm(Action<int, string> progressTracker)
        {
            ValArguments args = _argumentProvider.GetArg("SateArgs") as ValArguments;
            if (args == null)
                return null;
            string toValFile = args.FileNamesToVal[0];
            string forValFile = args.FileNamesForVal[0];
            string invalid = args.Invalid;
            //先判断时间是否一样
            string toValDate = "0000";
            string forValDate = "0000";
            try
            {
                foreach (Regex dt in DataReg)
                {
                    Match m1 = dt.Match(Path.GetFileName(toValFile));
                    Match m2 = dt.Match(Path.GetFileName(forValFile));
                    if (m1.Success)
                        toValDate = m1.Value;
                    if (m2.Success)
                        forValDate = m2.Value;

                }
                if (toValDate == "0000" || forValDate == "0000")
                    MessageBox.Show("数据文件时间为空，默认两文件时间相符");
                else
                {
                    int year = Convert.ToInt32(toValDate.Substring(0, 4));
                    int month = Convert.ToInt32(toValDate.Substring(4, 2));
                    int day = Convert.ToInt32(toValDate.Substring(6, 2));
                    DateTime datetime = new DateTime(year, month, day);
                    string days = Convert.ToString(datetime.DayOfYear);
                     
                    if (days != forValDate.Substring(1))
                    {
                        MessageBox.Show("提示：对比数据时间不同");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("异常信息：" + ex.Message);
            }
            //再看两个数据范围是否相同
            IRasterDataProvider forValRaster = GeoDataDriver.Open(forValFile) as IRasterDataProvider;
            IRasterDataProvider toValRaster = GeoDataDriver.Open(toValFile) as IRasterDataProvider;
            if (forValRaster.CoordEnvelope.MaxX != toValRaster.CoordEnvelope.MaxX
                || forValRaster.CoordEnvelope.MinX != toValRaster.CoordEnvelope.MinX ||
               forValRaster.CoordEnvelope.MaxY != toValRaster.CoordEnvelope.MaxY ||
               forValRaster.CoordEnvelope.MinY != toValRaster.CoordEnvelope.MinY || 
               toValRaster.Height != forValRaster.Height || toValRaster.Width != toValRaster.Width)
            {
                MessageBox.Show("两个数据空间范围不一致");
                //这里也可修改为读取两数据的交差范围，做直方图
                return null;
            }
            #region//读取数据
            Int16[] toValbuffer = new Int16[toValRaster.Height * toValRaster.Width];
            Int16[] forValbuffer = new Int16[toValRaster.Height * toValRaster.Width];
            Int16[] toVal = new Int16[toValRaster.Height * toValRaster.Width];
            Int16[] forVal = new Int16[toValRaster.Height * toValRaster.Width];
            IRasterBand toValBand = toValRaster.GetRasterBand(1);
            IRasterBand forValBand = forValRaster.GetRasterBand(1);
            unsafe
            {
                fixed (Int16* pointer = toValbuffer)
                {
                    IntPtr ptr = new IntPtr(pointer);
                    toValBand.Read(0, 0, toValRaster.Width, toValRaster.Height, ptr, toValRaster.DataType,
                        toValRaster.Width, toValRaster.Height);
                    for (int i = 0; i < toValRaster.Height * toValRaster.Width; i++)
                    {
                        toVal[i] = toValbuffer[i];
                    }
                }
                fixed (Int16* pointer = forValbuffer)
                {
                    IntPtr ptr = new IntPtr(pointer);
                    forValBand.Read(0, 0, forValRaster.Width, forValRaster.Height, ptr, forValRaster.DataType,
                        forValRaster.Width, forValRaster.Height);
                    for (int i = 0; i < forValRaster.Height * forValRaster.Width; i++)
                    {
                        forVal[i] = forValbuffer[i];
                    }
                }
            }
          #endregion
            IExtractResultArray array = new ExtractResultArray("统计表格");
            if (args.CreatHistogram)
            {
                List<string[]> listRowHist = new List<string[]>();
                string histfilename = "";
                IStatResult fresult = null;
                int maxcol = 10;
                if (!String.IsNullOrEmpty(args.MaxColumns))
                    maxcol = Convert.ToInt32(args.MaxColumns);
                IStaticComputer<Int16> computer = new StaticComputerInt16();
                listRowHist = computer.ComputeDeviation(toVal, forVal, GetInvalidArray(args.Invalid), GetInvalidArray(args.ForInvalid), maxcol);
                string sentitle = "统计日期：" + DateTime.Now.ToShortDateString();
                string[] columns = new string[] { "偏差值", "累加值" };
                fresult = new StatResult(sentitle, columns, listRowHist.ToArray());
                string title = "陆表温度产品偏差直方图";
                histfilename = StatResultToFile(new string[] { toValFile }, fresult, "VAL", "HIST", title, null, 1, true, 1);
                IFileExtractResult res = new FileExtractResult("HIST", histfilename);
                array.Add(res);
            }
            if (args.CreatRMSE)
            {
                IStaticComputer<Int16> computer = new StaticComputerInt16();
                double rmse = computer.ComputeRMSE(toVal, forVal, GetInvalidArray(args.Invalid), GetInvalidArray(args.ForInvalid));
                MessageBox.Show("数据均方根误差是：" + Convert.ToString(rmse));
            }
            #region 不用这种方式出栅格数据的散点图
            //if (args.CreatScatter)
            //{
            //    List<string[]> listRow = new List<string[]>();    //散点图
            //    for (int i = 0; i < toValRaster.Height * toValRaster.Width; i++)
            //    {
            //        //剔除无效值
            //        if (!String.IsNullOrEmpty(args.invalid))
            //        {
            //            if (args.invalid.Contains(Convert.ToString(toVal[i])) || args.invalid.Contains(Convert.ToString(forVal[i])))
            //            {
            //            }
            //            else
            //            {
            //                string[] row = new string[] { Convert.ToString(toVal[i]), Convert.ToString(forVal[i]) };
            //                listRow.Add(row);

            //            }
            //        }
            //        else
            //        {
            //            string[] row = new string[] { Convert.ToString(toVal[i]), Convert.ToString(forVal[i]) };
            //            listRow.Add(row);
            //        }
            //    }
            //    string[][] rows = listRow.ToArray();
            //    IStatResult result = new StatResult("统计时间:", new string[] { "待验证云参数", "MOD06云产品" }, rows);
            //    string title = "陆表温度数据与MODIS LST对比";
            //    string filename = "";
            //    try
            //    {
            //        using (StatResultToChartInExcelFile excelControl = new StatResultToChartInExcelFile())
            //        {
            //            excelControl.Init(masExcelDrawStatType.xlXYScatter);
            //            excelControl.Add("数据对比", result, true, 0, false, result.Columns[0], result.Columns[1
            //                ]);
            //            filename = StatResultToFile(new string[] { toValFile }, result, "VAL", "SCAT", title, null, 1, true, 1);
            //            if (!filename.ToUpper().EndsWith(".XLSX"))
            //                filename += ".XLSX";
            //            excelControl.SaveFile(filename);
            //            IFileExtractResult res = new FileExtractResult("SCAT", filename);
            //            array.Add(res);
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}
            #endregion
            return array;
        }

        private Int16[] GetInvalidArray(string invalidString)
        {
            if (string.IsNullOrEmpty(invalidString))
                return null;
            string[] stringArray = invalidString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (stringArray.Length < 1)
                return null;
            List<Int16> list = new List<short>();
            Int16 value;
            foreach (string item in stringArray)
            {
                if (Int16.TryParse(item, out value))
                    list.Add(value);
            }
            return list.ToArray();
        }
    }
}
